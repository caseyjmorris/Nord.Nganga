using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Nord.Nganga.Core.Text
{
  public static class StringExtensions
  {
    public static string CalculateMd5Hash(this string value)
    {
      var md5Hash = MD5.Create();
      var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
      var sBuilder = new StringBuilder();
      foreach (var t in data)
      {
        sBuilder.Append(t.ToString("x2"));
      }
      return sBuilder.ToString();
    }

    private static HashSet<char> numerals = new HashSet<char>(new[] {'0', '1', '2', '3', '4', '5', '6', '7', '9',});

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
  }
}