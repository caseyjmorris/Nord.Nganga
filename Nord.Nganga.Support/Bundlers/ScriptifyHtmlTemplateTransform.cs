using System;
using System.Text;
using System.Web;
using System.Web.Optimization;

namespace Nord.Nganga.Support.Bundlers
{
  /// <summary>
  /// This template turns a series of HTML templates into JavaScript strings and loads them into the Angular $templateCache.
  /// </summary>
  public class ScriptifyHtmlTemplateTransform : IBundleTransform
  {
    private readonly string moduleName;


    /// <summary>
    /// Instantiate a new <see cref="ScriptifyHtmlTemplateTransform"/>.
    /// </summary>
    /// <param name="moduleName">The Angular module name</param>
    public ScriptifyHtmlTemplateTransform(string moduleName)
    {
      this.moduleName = HttpUtility.JavaScriptStringEncode(moduleName);
    }


    public void Process(BundleContext context, BundleResponse response)
    {
      var sb =
        new StringBuilder(
          $"angular.module('{this.moduleName}').run(['$templateCache', function($templateCache){Environment.NewLine}  {{{Environment.NewLine}    ");

      foreach (var file in response.Files)
      {
        var content = file.ApplyTransforms();

        sb.Append(
          $"$templateCache.put('{file.VirtualFile.VirtualPath}', '{HttpUtility.JavaScriptStringEncode(content)}');{Environment.NewLine}    ");
      }

      sb.Remove(sb.Length - 3, 2);

      sb.Append(@"}]);");

      response.Files = new BundleFile[] {};

      response.Content = sb.ToString();

      response.ContentType = "text/javascript";
    }
  }
}