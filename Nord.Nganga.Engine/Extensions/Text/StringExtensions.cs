using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nord.Nganga.Engine.Extensions.Text
{
  public static class StringExtensions
  {
    private static HashSet<char> numerals = new HashSet<char>(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '9', });

    // todo   override this with an implementation that will look for multiple instances 
    // todo   of an assembly and select the one with the newest assy version info

    /// <summary>
    /// this is a "safe" file search - deals with auth exceptions thrown by framework enumerate operations 
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string SearchDirectory(this string rootPath, string target)
    {
      var files = Directory.GetFiles(rootPath, target, SearchOption.TopDirectoryOnly);

      if (files.Any())
        return files.First();

      var dirs = Directory.GetDirectories(rootPath, "*", SearchOption.TopDirectoryOnly);
      foreach (var dir in dirs)
      {
        var di = new DirectoryInfo(dir);
        var ignore = di.Attributes.HasFlag(FileAttributes.System) || di.Attributes.HasFlag(FileAttributes.Hidden);
        if (ignore)
          continue;

        try
        {
          var f = dir.SearchDirectory(target);
          if (!string.IsNullOrEmpty(f))
            return f;
        }
        catch
        {
        }
      }
      return null;
    }

    public static string ToCamelCase(this string val)
    {
      if (string.IsNullOrEmpty(val))
      {
        throw new ArgumentException("val");
      }
      var charArr = val.ToCharArray();
      charArr[0] = char.ToLowerInvariant(charArr[0]);
      return new string(charArr);
    }

    public static string ToSpaced(this string val)
    {
      if (string.IsNullOrWhiteSpace(val))
      {
        throw new ArgumentException("val");
      }

      var cArr = val.ToCharArray();

      var sb = new StringBuilder();

      sb.Append(cArr[0]);

      var previousWasNumeral = numerals.Contains(cArr[0]);

      for (var i = 1; i < cArr.Length; i++)
      {
        var currentIsNumeral = numerals.Contains(cArr[i]);
        var lc = char.ToLowerInvariant(cArr[i]);
        if (lc != cArr[i] || (currentIsNumeral ^ previousWasNumeral))
        {
          sb.Append(' ');
        }
        sb.Append(lc);
        previousWasNumeral = currentIsNumeral;
      }

      return sb.ToString();
    }
  }
}