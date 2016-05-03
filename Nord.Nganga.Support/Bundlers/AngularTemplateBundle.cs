using System.Web.Optimization;

namespace Nord.Nganga.Support.Bundlers
{
  /// <summary>
  /// A bundle which takes Angular template HTML files and joins them into a single JavaScript file where they are read into strings and read into the Angular $templateCache.
  /// </summary>
  public class AngularTemplateBundle : Bundle
  {
    /// <summary>
    /// Instantiates a new <see cref="AngularTemplateBundle"/>
    /// </summary>
    /// <param name="moduleName">The name of the Angular module in whose run block the code should run.</param>
    /// <param name="virtualPath">Virtual file location.</param>
    /// <param name="minify">Enable JavaScript minification</param>
    public AngularTemplateBundle(string moduleName, string virtualPath, bool minify)
      : base(
        virtualPath,
        minify
          ? new IBundleTransform[] {new ScriptifyHtmlTemplateTransform(moduleName), new JsMinify(),}
          : new IBundleTransform[] {new ScriptifyHtmlTemplateTransform(moduleName)})
    {
    }
  }
}