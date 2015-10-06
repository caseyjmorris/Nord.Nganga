using System;
using System.Reflection;
using Nord.Nganga.Annotations;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Mappers;
using Nord.Nganga.Mappers.Controllers;
using Nord.Nganga.Mappers.Resources;
using Nord.Nganga.Mappers.Views;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs.Coordination
{
  public class SourceGenerator
  {
    private readonly WebApiSettingsPackage webApiSettings;
    private readonly SystemPathSettingsPackage pathSettings;
    private readonly Action<object> modelVisitor;

    public SourceGenerator(WebApiSettingsPackage settingsPackage, SystemPathSettingsPackage pathSettings,
      Action<object> modelVisitor = null)
    {
      this.webApiSettings = settingsPackage;
      this.pathSettings = pathSettings;
      this.modelVisitor = modelVisitor;
    }

    public string GenerateController(Type controllerType)
    {
      TemplateFactory.AssertTemplateMinimumVersions(this.pathSettings, controllerType, TemplateContext.Controller);
      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(
        endpointMapper,
        new EndpointFilter(new ViewModelMapper()));
      var model = controllerCoordinatedInfoMapper.GetControllerCoordinatedInformationViewModel(controllerType);
      this.modelVisitor?.Invoke(model);

      return this.ProcessModel(controllerType, TemplateContext.Controller, model);
    }

    public string GenerateResource(Type controllerType)
    {
      TemplateFactory.AssertTemplateMinimumVersions(this.pathSettings, controllerType, TemplateContext.Resource);
      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var resourceCoordMapper = new ResourceCoordinationMapper(endpointMapper);
      var model = resourceCoordMapper.GetResourceCoordinationInformationViewModel(controllerType);
      this.modelVisitor?.Invoke(model);
      return this.ProcessModel(controllerType, TemplateContext.Resource, model);
    }

    public string GenerateView(Type controllerType)
    {
      TemplateFactory.AssertTemplateMinimumVersions(this.pathSettings, controllerType, TemplateContext.View);
      var vcMapper = new ViewCoordinationMapper(this.webApiSettings);
      var model = vcMapper.GetViewCoordinatedInformationCollection(controllerType);
      this.modelVisitor?.Invoke(model);
      return this.ProcessModel(controllerType, TemplateContext.View, model);
    }


    private string ProcessModel(Type controllerType, TemplateContext templateContext, object model)
    {
      var body = this.GetBody(templateContext, model);
      var header = this.GetHeader(templateContext, controllerType, body);
      var masterTemplate = TemplateFactory.GetTemplate(this.pathSettings, TemplateContext.Master, "file");
      masterTemplate.Add("header", header);
      masterTemplate.Add("body", body);
      var source = masterTemplate.Resolve();
      if (!this.Validate(source, body, templateContext))
      {
        throw new Exception($"Failed to validate generated {templateContext} source for {controllerType.Name}.");
      }
      return source;
    }

    private bool Validate(string source, string body, TemplateContext templateContext)
    {
      var pr = SourceParser.ParseFile(templateContext, source);
      if (!pr.Success) 
      {
        return false;
      }
      var md5 = body.CalculateMd5Hash();
      var isValid = md5 == pr.CalculatedBodyMd5 && pr.CalculatedBodyMd5 == pr.DeclaredHeaderMd5;

      return isValid;
    }

    private string GetBody(TemplateContext templateContext, object model)
    {
      var template = TemplateFactory.GetTemplate(this.pathSettings, templateContext);
      template.Add("model", model);
      var body = template.Resolve();
      return body;
    }

    private string GetHeader(TemplateContext templateContext, Type controllerType, string source)
    {
      var a = controllerType.Assembly;

      var template = TemplateFactory.GetTemplate(this.pathSettings, TemplateContext.Master, "header");
      var openComment = TemplateFactory.GetTemplate(this.pathSettings, templateContext, "openComment").Render();
      var closeComment = TemplateFactory.GetTemplate(this.pathSettings, templateContext, "closeComment").Render();
      var bodyVersionTemplate = TemplateFactory.GetTemplate(this.pathSettings, templateContext, "templateVersion");
      template.Add("model", new
      {
        genDate = DateTime.Now.ToShortDateString(),
        genTime = DateTime.Now.ToShortTimeString(),
        bodyTemplateVersion = bodyVersionTemplate.Render(),
        context = templateContext.ToString(),
        controllerTypeName = controllerType.FullName,
        templatesDirectory = this.pathSettings.TemplatesDirectory,
        md5Checksum = source.CalculateMd5Hash(),
        openComment,
        closeComment,
        copyright = this.GetCustomAttributeValue<AssemblyCopyrightAttribute>(a)?.Copyright,
        company = this.GetCustomAttributeValue<AssemblyCompanyAttribute>(a)?.Company
      });
      var header = template.Resolve();
      return header;
    }

    private TAttribType GetCustomAttributeValue<TAttribType>(Assembly a)
    {
      var attribs = a.GetCustomAttributes(typeof(TAttribType), false);

      if (attribs.Length > 0)
      {
        return (TAttribType) attribs[0];
      }
      return default(TAttribType);
    }
  }
}