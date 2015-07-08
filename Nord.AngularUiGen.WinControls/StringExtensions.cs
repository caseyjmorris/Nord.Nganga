using System;
using System.Drawing;

namespace Nord.AngularUiGen.WinControls
{
  public static class StringExtensions
  {
    public static SizeF Measure(this System.Windows.Forms.Control control, Func<string> textProvider)
    {
      var g = control.CreateGraphics();
      var text = textProvider();
      var s =g.MeasureString(text, control.Font);
      return s;
    }

    public static string ReduceToFit(this string originalText, System.Windows.Forms.Control control, int preserveNLeadingCharacters = 0)
    {
      if(string.IsNullOrEmpty(originalText))
        return originalText;
      
      var prefix = preserveNLeadingCharacters == 0 ? string.Empty : originalText.Substring(0, preserveNLeadingCharacters);
      var text = originalText.Substring(preserveNLeadingCharacters);
      var text1 = text;
      var size = control.Measure(()=>text1);
      while (size.Width > (control.Width- preserveNLeadingCharacters))
      {
        text = text.Substring(1);
        var text2 = text;
        size = control.Measure(() => text2);
      }

      if (!originalText.Equals(prefix + text, StringComparison.InvariantCultureIgnoreCase))
      {
        return prefix + "..." + text;
      }
      return originalText;

    }
  }
}
