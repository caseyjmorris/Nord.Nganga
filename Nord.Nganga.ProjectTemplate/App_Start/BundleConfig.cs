using System.Web.Optimization;

namespace Nord.Nganga.ProjectTemplate
{
  public class BundleConfig
  {
    // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    public static void RegisterBundles(BundleCollection bundles)
    {
      // Set EnableOptimizations to false for debugging. For more information,
      // visit http://go.microsoft.com/fwlink/?LinkId=301862
#if DEBUG
      BundleTable.EnableOptimizations = false;
#else
        BundleTable.EnableOptimizations = true;
#endif


      var jquery = new ScriptBundle("~/bundles/jquery")
        .Include("~/client/lib/jq/jquery-2.1.4.js");
#if DEBUG
      jquery.Transforms.Clear();
#endif
      bundles.Add(jquery);

      var bootStrapJs = new ScriptBundle("~/bundles/bootstrap")
        .Include(
          "~/client/lib/bootstrap/js/bootstrap.js");

#if DEBUG
      bootStrapJs.Transforms.Clear();
#endif

      bundles.Add(bootStrapJs);

      bundles.Add(new StyleBundle("~/Content/appCss")
        .Include(
          "~/client/lib/bootstrap/css/bootstrap.css",
          "~/Client/lib/ng/plugin/nganga/css/Site.csss",
          "~/Client/lib/ng/plugin/tablesort/tablesort.css",
          "~/Client/lib/ng/plugin/toaster/toaster.css"));

      var ngBundle = new ScriptBundle("~/bundles/angular")
        .Include(
          "~/client/lib/stacktrace.js",
          "~/client/lib/spin.js",
          "~/client/lib/lodash.js",
          "~/client/lib/ng/core/angular.js",
          "~/client/lib/ng/core/angular-animate.js",
          "~/client/lib/ng/core/angular-cookies.js",
          "~/client/lib/ng/core/angular-loader.js",
          "~/client/lib/ng/core/angular-resource.js",
          "~/client/lib/ng/core/angular-sanitize.js",
          "~/client/lib/ng/core/angular-touch.js",
          "~/client/lib/ng/plugin/tablesort/angular-tablesort.js",
          "~~/client/lib/ng/plugin/toaster/toaster.js",
          "~/client/lib/ng/plugin/nganga/js/_ngangaModuleDeclaration.js",
          "~/client/lib/ng/plugin/nganga/js/exceptionHandling.js",
          "~/client/lib/ng/plugin/nganga/js/httpHandling.js")
        .IncludeDirectory("~/client/lib/ng/plugin/ng-ui", "*.js", false)
        .IncludeDirectory("~/client/app/js/svc", "*.js", true)
        .IncludeDirectory("~/client/lib/ng/plugin/nganga/ui", "*.js", true);

#if DEBUG
      ngBundle.Transforms.Clear();
#endif
      bundles.Add(ngBundle);
    }
  }
}