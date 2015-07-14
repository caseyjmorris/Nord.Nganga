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
        .Include("~/Scripts/libraries/jquery-2.1.1.js")
        .Include("~/Scripts/libraries/modernizr-2.6.2.js")
        .Include("~/Scripts/libraries/Blob.js");
#if DEBUG
      jquery.Transforms.Clear();
#endif
      bundles.Add(jquery);

      var bootStrapJs = new ScriptBundle("~/bundles/bootstrap")
        .Include(
          "~/Scripts/libraries/bootstrap.js",
          "~/Scripts/libraries/respond.js");

#if DEBUG
      bootStrapJs.Transforms.Clear();
#endif

      bundles.Add(bootStrapJs);

      bundles.Add(new StyleBundle("~/Content/referrerPortalCSS")
        .Include(
          "~/Content/bootstrap.css",
          "~/Content/ng-grid.min.css",
          "~/Content/site.css",
          "~/Content/tablesort.css",
          "~/Content/toaster.css"));

      var ngBundle = new ScriptBundle("~/bundles/angular")
        .Include(
          "~/Scripts/libraries/stacktrace.js",
          "~/Scripts/libraries/spin.js",
          "~/Scripts/libraries/lodash.js",
          "~/Scripts/libraries/angular.js",
          "~/Scripts/libraries/angular-animate.js",
          "~/Scripts/libraries/angular-cookies.js",
          "~/Scripts/libraries/angular-loader.js",
          "~/Scripts/libraries/angular-resource.js",
          "~/Scripts/libraries/angular-sanitize.js",
          "~/Scripts/libraries/angular-touch.js",
          "~/Scripts/libraries/angular-ui/ui-utils.js",
          "~/Scripts/libraries/angular-ui/ui-bootstrap.js",
          "~/Scripts/libraries/angular-ui-router.js",
          "~/Scripts/libraries/angular-ui/ui-bootstrap-tpls.js",
          "~/Scripts/libraries/ng-grid.js",
          "~/Scripts/libraries/angular-tablesort.js",
          "~/Scripts/libraries/ng-file-upload.js",
          "~/Scripts/libraries/toaster.js",
          "~/Scripts/referrerportal.js",
          "~/Scripts/referrerPortalExceptionHandling.js",
          "~/Scripts/ui/*.js")
        .IncludeDirectory("~/Scripts/services/", "*.js", true)
        .IncludeDirectory("~/Scripts/controllers/", "*.js", true);

#if DEBUG
      ngBundle.Transforms.Clear();
#endif
      bundles.Add(ngBundle);
    }
  }
}