namespace Nord.Nganga.Models.Configuration
{
  public class WebApiSettingsPackage : IConfigurationPackage
  {
    public void SetPropertiesToDefault()
    {
      this.HttpGetAttributeName = "HttpGetAttribute";
      this.HttpPostAttributeName = "HttpPostAttribute";
      this.ApiControllerName = "ApiController";
      this.AuthorizeAttributeName = "AuthorizeAttribute";
    }

    public string HttpPostAttributeName { get; set; }
    public string HttpGetAttributeName { get; set; }
    public string ApiControllerName { get; set; }
    public string AuthorizeAttributeName { get; set; }
  }
}