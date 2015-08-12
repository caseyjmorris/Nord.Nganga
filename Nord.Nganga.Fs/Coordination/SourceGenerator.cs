using System;
using System.IO;
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
      var template = TemplateFactory.GetTemplate(this.pathSettings, context, "file");
      template.Add("header", header);
      template.Add("body", body);
      return template.Resolve();
    }

    private string GetBody(TemplateFactory.Context context, object model)
    {
      var template = TemplateFactory.GetTemplate(this.pathSettings, context);
      template.Add("model", model);
      return template.Resolve();
    }

    private string GetHeader(TemplateFactory.Context context, Type controllerType, string source)
    {
      var template = TemplateFactory.GetTemplate(this.pathSettings, context, "header");
      template.Add("model", new
      {
        genDate = DateTime.Now.ToShortDateString(),
        genTime = DateTime.Now.ToShortTimeString(),
        controllerTypeName = controllerType.FullName,
        templatesDirectory = this.pathSettings.TemplatesDirectory,
        md5Checksum = source.CalculateMd5Hash()
      });
      return template.Resolve();
    }
  }

}