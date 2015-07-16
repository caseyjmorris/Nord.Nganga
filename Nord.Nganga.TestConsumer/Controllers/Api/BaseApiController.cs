using System.Web;
using System.Web.Http;
using Nord.Nganga.Annotations.Attributes.Angular;

namespace Nord.Nganga.TestConsumer.Controllers.Api
{
  
  [AngularModuleName("rareCareNgApp")]
  public class BaseApiController : ApiController
  {
  

    public BaseApiController()
    {
      
    }
  }
}