using System.Web.Http;
using Microsoft.AspNet.Identity;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.TestConsumer.Models;

namespace Nord.Nganga.TestConsumer.Controllers.Api
{
  [AngularRouteIdParameter("programId", IsNullable = true)]
  [Authorize(
    Roles =
      ApplicationRoles.FinanceUser + "," + ApplicationRoles.PatientAssistanceAssociate + "," +
      ApplicationRoles.FinanceManager + ApplicationRoles.PatientAssistanceManager)]
  public class ProgramProfileController : BaseApiController
  {
    [HttpGet]
    public SponsorViewModel GetProgramProfile(int id)
    {
      return new SponsorViewModel();
    }

    [Authorize(Roles = ApplicationRoles.PatientAssistanceManager + "," + ApplicationRoles.FinanceManager)]
    [HttpPost]
    [JavaScriptOnPostComplete("$state.go('map.program.single.profile', {programId: result.id});")]
    public SponsorViewModel SaveChangesToProgramProfile(SponsorViewModel model)
    {
      return new SponsorViewModel();
    }
  }
}