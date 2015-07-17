using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Antlr4.StringTemplate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nord.Nganga.Annotations;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Mappers;
using Nord.Nganga.Mappers.Controllers;
using Nord.Nganga.Mappers.Resources;
using Nord.Nganga.Mappers.Views;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.Models.ViewModels;
using Nord.Nganga.TestConsumer.Controllers.Api;
using Nord.Nganga.TestConsumer.Models;

namespace Nord.Nganga.StEngine.Test
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestAssyOptions()
    {
      var subjectType = typeof(Nganga.TestConsumer.Controllers.Api.SponsorsController);
      var ao = new AssemblyOptionsModel(subjectType);
      Console.WriteLine(ao.CsProjectPath);
      Console.WriteLine(ao.NgControllersPath);
      Console.WriteLine(ao.NgResourcesPath);
      Console.WriteLine(ao.NgViewsPath);
      Console.WriteLine(ao.GetOption(CasingOptionContext.Field));
    }

    [TestMethod]
    public void ExcerciseViewCoordinator()
    {
      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();

      var coord = new ViewCoordinationMapper(new ViewModelMapper(), new EndpointFilter(new ViewModelMapper()),
        new EndpointMapper(wasp));

      var coordinated = coord.GetViewCoordinatedInformationCollection(typeof(SponsorsController));
    }


    [TestMethod]
    public void TestControllerGeneration()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.Controller, "controller", false);

      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();
      var endpointMapper = new EndpointMapper(wasp);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(endpointMapper,
        new EndpointFilter(new ViewModelMapper()));

      var subjectType = typeof(Nganga.TestConsumer.Controllers.Api.SponsorsController);

      var model = controllerCoordinatedInfoMapper.GetControllerCoordinatedInformationViewModel(subjectType);
      t.Add("model", model);

      var sb = new StringBuilder();

      var aiw = new AutoIndentWriter(new StringWriter(sb));

      var s = t.Write(aiw);

      //var s = sb.ToString(); //t.Render();
      Console.WriteLine(sb.ToString());
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

      var sb = new StringBuilder();

      var aiw = new AutoIndentWriter(new StringWriter(sb));

      t.Write(aiw);

      var s = sb.ToString();
      Console.WriteLine(s);
    }

    [TestMethod]
    public void TestViewCoordination()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.View, "view", false);

      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();
      var endpointMapper = new EndpointMapper(wasp);
      var viewModelMapper = new ViewModelMapper();
      var endPointFilter = new EndpointFilter(viewModelMapper);
      var vcMapper = new ViewCoordinationMapper(viewModelMapper, endPointFilter, endpointMapper);

      var subjectType = typeof(Nganga.TestConsumer.Controllers.Api.SponsorsController);

      var model = vcMapper.GetViewCoordinatedInformationCollection(subjectType);

      t.Add("model", model);

      var sb = new StringBuilder();

      var aiw = new AutoIndentWriter(new StringWriter(sb));

      t.Write(aiw);

      var s = sb.ToString();
      Console.WriteLine(s);
    }
  }
}