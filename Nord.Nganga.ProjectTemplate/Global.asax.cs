using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using log4net;

namespace Nord.Nganga.ProjectTemplate
{
  public class Global : System.Web.HttpApplication
  {
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.IgnoreRoute("favicon.ico");

      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
        new[] { "NORD.RareCare.Web.Referrer.Controllers" }
        );
    }

    protected void Application_Start(object sender, EventArgs e)
    {
      GlobalConfiguration.Configure(WebApiConfig.Register);

      log4net.Config.XmlConfigurator.Configure();

      AreaRegistration.RegisterAllAreas();

      RegisterRoutes(RouteTable.Routes);

      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    protected void Session_Start(object sender, EventArgs e)
    {

    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {

    }

    protected void Application_Error(object sender, EventArgs e)
    {
      var exception = this.Server.GetLastError();

      if (this.Request != null && this.Request.Url != null)
      {
        MDC.Set("Url", this.Request.Url.ToString());
      }

      Log.Error("Web Application Exception", exception);
    }

    protected void Session_End(object sender, EventArgs e)
    {

    }

    protected void Application_End(object sender, EventArgs e)
    {

    }
  }
}