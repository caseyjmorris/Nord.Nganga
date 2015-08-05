using System;
using System.IO;
using System.Text;
using Antlr4.StringTemplate;
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
    private Action<object> modelVisitor;

    public SourceGenerator(WebApiSettingsPackage settingsPackage, SystemPathSettingsPackage pathSettings,
      Action<object> modelVisitor = null)
    {
      this.webApiSettings = settingsPackage;
      this.pathSettings = pathSettings;
      this.modelVisitor = modelVisitor;
    }

    public string GenerateController(Type controllerType)
    {
      var stringTemplate = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.Controller,
        "controller", false);

      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(endpointMapper,
        new EndpointFilter(new ViewModelMapper()));

      var model = controllerCoordinatedInfoMapper.GetControllerCoordinatedInformationViewModel(controllerType);
      this.modelVisitor?.Invoke(model);

      return this.ProcessModel(stringTemplate, model);
    }

    public string GenerateResource(Type controllerType)
    {
      var stringTemplate = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.Resource,
        "resourceFile", false);

      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var resourceCoordMapper = new ResourceCoordinationMapper(endpointMapper);

      var model = resourceCoordMapper.GetResourceCoordinationInformationViewModel(controllerType);
      this.modelVisitor?.Invoke(model);
      return this.ProcessModel(stringTemplate, model);
    }

    public string GenerateView(Type controllerType)
    {
      var stringTemplate = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.View, "view", false);
      var vcMapper = new ViewCoordinationMapper(this.webApiSettings);

      var model = vcMapper.GetViewCoordinatedInformationCollection(controllerType);
      this.modelVisitor?.Invoke(model);
      return this.ProcessModel(stringTemplate, model);
    }

    private string ProcessModel(Template template, object model)
    {
      template.Add("model", model);

      var sb = new StringBuilder();

      var aiw = new AutoIndentWriter(new StringWriter(sb));

      var s = template.Write(aiw);

      return sb.ToString();
    }
  }
}