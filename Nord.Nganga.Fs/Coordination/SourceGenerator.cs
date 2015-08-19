using System;
using System.CodeDom;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.StringTemplate;
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
      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(
        endpointMapper,
        new EndpointFilter(new ViewModelMapper()));
      var model = controllerCoordinatedInfoMapper.GetControllerCoordinatedInformationViewModel(controllerType);
      this.modelVisitor?.Invoke(model);

      return this.ProcessModel(controllerType, TemplateFactory.Context.Controller, model);
    }

    public string GenerateResource(Type controllerType)
    {
      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var resourceCoordMapper = new ResourceCoordinationMapper(endpointMapper);
      var model = resourceCoordMapper.GetResourceCoordinationInformationViewModel(controllerType);
      this.modelVisitor?.Invoke(model);
      return this.ProcessModel(controllerType, TemplateFactory.Context.Resource, model);
    }

    public string GenerateView(Type controllerType)
    {
      var vcMapper = new ViewCoordinationMapper(this.webApiSettings);
      var model = vcMapper.GetViewCoordinatedInformationCollection(controllerType);
      this.modelVisitor?.Invoke(model);
      return this.ProcessModel(controllerType, TemplateFactory.Context.View, model);
    }


    private string ProcessModel(Type controllerType, TemplateFactory.Context context, object model)
    {
      var body = this.GetBody(context, model);
      var header = this.GetHeader(context, controllerType, body);
      var masterTemplate = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.Master, "file");
      masterTemplate.Add("header", header);
      masterTemplate.Add("body", body);
      var source = masterTemplate.Resolve();
      if (!this.Validate(source, body, context))
      {
        throw new Exception($"Failed to validate generated {context} source for {controllerType.Name}.");
      }
      return source;
    }

    private bool Validate(string source, string body, TemplateFactory.Context context)
    {
      var pr = SourceParser.ParseFile(context, source);
      if (!pr.Success) 
      {
        return false;
      }
      var md5 = body.CalculateMd5Hash();
      var isValid = md5 == pr.CalculatedBodyMd5 && pr.CalculatedBodyMd5 == pr.DeclaredHeaderMd5;

      return isValid;
    }

    private string GetBody(TemplateFactory.Context context, object model)
    {
      var template = TemplateFactory.GetTemplate(this.pathSettings, context);
      template.Add("model", model);
      var body = template.Resolve();
      return body;
    }

    private string GetHeader(TemplateFactory.Context context, Type controllerType, string source)
    {
      var a = controllerType.Assembly;

      var template = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.Master, "header");
      var openComment = TemplateFactory.GetTemplate(this.pathSettings, context, "openComment").Render();
      var closeComment = TemplateFactory.GetTemplate(this.pathSettings, context, "closeComment").Render();
      var bodyVersionTemplate = TemplateFactory.GetTemplate(this.pathSettings, context, "templateVersion");
      template.Add("model", new
      {
        genDate = DateTime.Now.ToShortDateString(),
        genTime = DateTime.Now.ToShortTimeString(),
        bodyTemplateVersion = bodyVersionTemplate.Render(),
        context = context.ToString(),
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