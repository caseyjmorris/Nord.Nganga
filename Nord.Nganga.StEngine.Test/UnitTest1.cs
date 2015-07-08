using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nord.AngularUiGen.Mappers.Resources;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.StEngine.Test
{
  [TestClass]
  public class UnitTest1
  {

    [TestMethod]
    public void TestMethod3()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.View, "t1");
      t.Add("model", new { people = new[] { new { lastName = "Zhao", sex = "female" }, new { lastName = "Morris", sex = "male" } } });
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

      var subjectType =
        typeof (
          NORD.RareCare.Presentation.Web.Controllers.Api.Programs.ProgramPeriods.AssociatesAndProviders.
            SponsorsController);

      var model = resourceCoordMapper.GetResourceCoordinationInformationViewModel(subjectType);
      //model.GetEndpoints = model.GetEndpoints.ToList();
      //model.PostEndpoints = model.PostEndpoints.ToList();
      t.Add("model", model);
      t.Add("getEndpoints", model.GetEndpoints.ToList());
      t.Add("postEndpoints", model.PostEndpoints.ToList());
      t.Add("hasGetAndPost", model.PostEndpoints.Any() && model.GetEndpoints.Any());

      var s = t.Render();
      Console.WriteLine(s);
    }
  }
}
