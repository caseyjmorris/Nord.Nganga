using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Elmah.Contrib.WebApi;
using Newtonsoft.Json.Serialization;
using Nord.Nganga.ProjectTemplate.App_Start;
using Unity.WebApi;

namespace Nord.Nganga.ProjectTemplate
{
  public static class WebApiConfig
  {
    public const string UrlPrefix = "api";
    public const string UrlPrefixRelative = "~" + UrlPrefix;

    public static void Register(HttpConfiguration config)
    {
      config.Services.Add(typeof(IExceptionLogger), new ElmahExceptionLogger());

      GlobalConfiguration.Configuration.DependencyResolver =
        new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

      var settings = config.Formatters.JsonFormatter.SerializerSettings;

      settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

      config.Formatters.Remove(config.Formatters.XmlFormatter);

      config.Routes.MapHttpRoute(
        name: "API custom actions",
        routeTemplate: UrlPrefix + "/{controller}/{action}/{id}",
        defaults: new {id = RouteParameter.Optional});
    }
  }
}