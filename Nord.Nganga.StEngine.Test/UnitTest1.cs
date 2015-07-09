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
      t.Add("model", model);

      var s = t.Render();
      Console.WriteLine(s);
    }
  }
}
