using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Mappers;
using Nord.Nganga.Mappers.Controllers;
using Nord.Nganga.Mappers.Resources;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.StEngine.Test
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestControllerGeneration()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.Resource, "resourceFile", false);

      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();
      var endpointMapper = new EndpointMapper(wasp);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(endpointMapper, new ViewModelMapper());

      var subjectType = typeof(Nganga.TestConsumer.Controllers.Api.SponsorsController);

      var model = controllerCoordinatedInfoMapper.GetControllerCoordinatedInformationViewModel(subjectType);
      t.Add("model", model);

      var s = t.Render();
      Console.WriteLine(s);
    }

    [TestMethod]
    public void TestResourceCoordination()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.Resource, "resourceFile", false);

      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();
      var endpointMapper = new EndpointMapper(wasp);
      var resourceCoordMapper = new ResourceCoordinationMapper(endpointMapper);

      var subjectType = typeof(Nganga.TestConsumer.Controllers.Api.SponsorsController);

      var model = resourceCoordMapper.GetResourceCoordinationInformationViewModel(subjectType);
      model.UseCustomCache = true;
      model.CustomCacheFactory = "test";
      model.ServiceName = "service";
      t.Add("model", model);

      var s = t.Render();
      Console.WriteLine(s);
    }
  }
}