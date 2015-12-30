using System;
using System.IO;
using System.Text;
using Antlr4.StringTemplate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nord.Nganga.Annotations;
using Nord.Nganga.Mappers;
using Nord.Nganga.Mappers.Controllers;
using Nord.Nganga.Mappers.Resources;
using Nord.Nganga.Mappers.Views;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.TestConsumer.Controllers.Api;

namespace Nord.Nganga.StEngine.Test
{
  [TestClass]
  public class UnitTest1
  {
    private readonly SystemPathSettingsPackage settings =
      ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();

    private readonly WebApiSettingsPackage webApiSettings =
      ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();

    [TestMethod]
    public void TestVersionCompareMethod()
    {
      var v1 = "1.0.0";
      var v2 = "1.1.0";

      Console.WriteLine(TemplateFactory.CompareVersionStrings(v1, v2));
      Console.WriteLine(TemplateFactory.CompareVersionStrings(v2, v1));
      Console.WriteLine(TemplateFactory.CompareVersionStrings(v1, v1));
    }


    [TestMethod]
    public void TestAssyOptions()
    {
      var subjectType = typeof (SponsorsController);
      var ao = new AssemblyOptionsModel(subjectType);
      Console.WriteLine(ao.CsProjectName);
      Console.WriteLine(ao.NgControllersPath);
      Console.WriteLine(ao.NgResourcesPath);
      Console.WriteLine(ao.NgViewsPath);
      Console.WriteLine(ao.GetOption(CasingOptionContext.Field));
    }

    [TestMethod]
    public void ExcerciseViewCoordinator()
    {
      var coord = new ViewCoordinationMapper(new ViewModelMapper(), new EndpointFilter(new ViewModelMapper()),
        new EndpointMapper(this.webApiSettings), this.webApiSettings);

      var coordinated = coord.GetViewCoordinatedInformationCollection(typeof (SponsorsController));
    }


    [TestMethod]
    public void TestControllerGeneration()
    {
      var t = TemplateFactory.GetTemplate(TemplateContext.Controller, settingsPackage: this.settings);

      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var controllerCoordinatedInfoMapper = new ControllerCoordinationMapper(endpointMapper,
        new EndpointFilter(new ViewModelMapper()));

      var subjectType = typeof (SponsorsController);

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
      var t = TemplateFactory.GetTemplate(TemplateContext.Resource, settingsPackage: this.settings);

      var endpointMapper = new EndpointMapper(this.webApiSettings);
      var resourceCoordMapper = new ResourceCoordinationMapper(endpointMapper);

      var subjectType = typeof (SponsorsController);

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
      var t = TemplateFactory.GetTemplate(TemplateContext.View, settingsPackage: this.settings);

      var vcMapper = new ViewCoordinationMapper(this.webApiSettings);

      var subjectType = typeof (SponsorsController);

      var model = vcMapper.GetViewCoordinatedInformationCollection(subjectType);

      t.Add("model", model);

      var sb = new StringBuilder();

      var aiw = new AutoIndentWriter(new StringWriter(sb));

      t.Write(aiw);

      var s = sb.ToString();
      Console.WriteLine(s);
    }

    [TestMethod]
    public void TestViewCoordination2()
    {
      var t = TemplateFactory.GetTemplate(TemplateContext.View, settingsPackage: this.settings);

      var vcMapper = new ViewCoordinationMapper(this.webApiSettings);

      var subjectType = typeof (ProgramProfileController);

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