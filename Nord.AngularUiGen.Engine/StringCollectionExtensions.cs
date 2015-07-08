using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Nord.AngularUiGen.Engine.Extensions.Text;

namespace Nord.AngularUiGen.Engine
{
  public static class StringCollectionExtensions
  {
    /// <summary>
    /// scans the collection looking for pre or fully qualified names (i.e. names not requiring further qualification) 
    /// gathers any remaining names requiring qualification 
    /// 
    /// using the root of the current directory aS the starting point
    /// searches for the un/partially qualified names
    /// returns a union of the prequalified and qualified names
    /// </summary>
    /// <param name="targets"></param>
    /// <returns></returns>
    public static StringCollection AsFullyQualifiedFileNames(this System.Collections.Specialized.StringCollection targets)
    {
      var tc = (from string s in targets select s).ToList();

      var preQualified = tc.Where(File.Exists);
      var unQualified = tc.Where(f => !preQualified.Contains(f));

      var searchRoot = Path.GetPathRoot(Environment.CurrentDirectory);
      var qualified = (
        from string targetAssyName
          in unQualified
        select searchRoot.SearchDirectory(targetAssyName))
        .Where(resolvedName => !string.IsNullOrEmpty(resolvedName)).ToList();

      var sc = new StringCollection();
      sc.AddRange(preQualified.Union(qualified).ToArray());
      return sc;
    }
  }
}
