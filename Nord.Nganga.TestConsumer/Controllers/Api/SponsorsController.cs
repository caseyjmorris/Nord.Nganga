using System;
using System.Web.Http;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.TestConsumer.Models;

namespace Nord.Nganga.TestConsumer.Controllers.Api
{
  [AngularRouteIdParameter("programPeriodId")]
  public class SponsorsController : BaseApiController
  {
    public SponsorsController() : base()
    {
    }

    [HttpGet]
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