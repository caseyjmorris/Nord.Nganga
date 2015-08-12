using System;
using System.Web.Http;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.TestConsumer.Models;

namespace Nord.Nganga.TestConsumer.Controllers.Api
{
  [AngularRouteIdParameter("programPeriodId")]
  [InjectJavaScript("alert('Hello world!');", ControllerPosition = InjectJavaScriptAttribute.Position.Beginning)]
  [InjectJavaScript("alert('Isn\\'t this really annoying?');",
    ControllerPosition = InjectJavaScriptAttribute.Position.End)]
  public class SponsorsController : BaseApiController
  {
    public SponsorsController() : base()
    {
    }

    [HttpGet]
    [InjectHtmlInView("<p>Hello world!</p>")]
    public SponsorProgramPeriodDetailCollectionViewModel GetProgramPeriodSponsors(int id)
    {
      return new SponsorProgramPeriodDetailCollectionViewModel();
    }

    [HttpPost]
    public SponsorProgramPeriodDetailCollectionViewModel SaveChangesToProgramPeriodSponsors(
      SponsorProgramPeriodDetailCollectionViewModel model)
    {
      return new SponsorProgramPeriodDetailCollectionViewModel();
    }
  }
}