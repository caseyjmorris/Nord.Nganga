using System;

namespace Nord.AngularUiGen.Annotations
{
  public static class StringExtensions
  {
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
  }
}
