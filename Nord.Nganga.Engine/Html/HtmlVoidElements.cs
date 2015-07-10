using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Nord.Nganga.Engine.Html
{
  /// <summary>
  /// Contains HTML void elements.  
  /// </summary>
  /// <remarks>
  /// c.f. http://www.w3.org/html/wg/drafts/html/master/syntax.html#void-elements:  " Void elements only have a start tag; end tags must not be specified for void elements."
  /// </remarks>
  public static class HtmlVoidElements
  {
    /// <summary>
    /// A list of all W3C-specified void elements.
    /// </summary>
    public static ICollection<string> Elements =
      new HashSet<string>(new[]
      {
        "area", "base", "br", "col", "embed", "hr", "img", "input", "keygen", "link", "menuitem", "meta", "param",
        "source", "track", "wbr",
      });

    /// <summary>
    /// A regex that will match closing tags for void elements.
    /// </summary>
    public static Regex InvalidClosingTagRegex = new Regex(@"<\/(" + string.Join("|", Elements) + @")>",
      RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase);
  }
}