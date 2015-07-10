using System.Collections.Generic;
using System.Text;

namespace Nord.Nganga.Engine.Support
{
  public static class AncestorAppendStringFormatter
  {
    public static string GetAncestorAppendString(IList<string> ancestors)
    {
      //this is pretty simple but encapsulating it allows us to change the rules easily later

      var sb = new StringBuilder();

      for (var i = 1; i < ancestors.Count; i++)
      {
        sb.Append(ancestors[i]).Append('_');
      }

      return sb.ToString();
    }
  }
}