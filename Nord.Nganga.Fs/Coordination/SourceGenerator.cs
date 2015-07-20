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
    private readonly WebApiSettingsPackage settingsPackage;

    public SourceGenerator(WebApiSettingsPackage settingsPackage)
    {
      this.settingsPackage = settingsPackage;
    }

    public string GenerateController(Type controllerType )
    {
      var stringTemplate = TemplateFactory.GetTemplate(TemplateFactory.Context.Controller, "controller", false);

      var endpointMapper = new EndpointMapper(this.settingsPackage);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(endpointMapper,
        new EndpointFilter(new ViewModelMapper()));
      
      var model = controllerCoordinatedInfoMapper.GetControllerCoordinatedInformationViewModel(controllerType);
      return this.ProcessModel(stringTemplate, model);
    }

    public string GenerateResource(Type controllerType)
    {
      var stringTemplate = TemplateFactory.GetTemplate(TemplateFactory.Context.Resource, "resourceFile", false);

      var endpointMapper = new EndpointMapper(this.settingsPackage);
      var resourceCoordMapper = new ResourceCoordinationMapper(endpointMapper);

      var model = resourceCoordMapper.GetResourceCoordinationInformationViewModel(controllerType);
      //model.UseCustomCache = true;
      //model.CustomCacheFactory = "test";
      //model.ServiceName = "service";

      return this.ProcessModel(stringTemplate, model);
    }

    public string GenerateView(Type controllerType)
    {
      var stringTemplate = TemplateFactory.GetTemplate(TemplateFactory.Context.View, "view", false);
      var vcMapper = new ViewCoordinationMapper(this.settingsPackage);

      var model = vcMapper.GetViewCoordinatedInformationCollection(controllerType);
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
