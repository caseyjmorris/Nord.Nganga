using System;
using System.Text;

namespace Nord.Nganga.Core.Text
{
  public static class StringBuilderExtensions
  {
    public static StringBuilder Indent(this StringBuilder stringBuilder, int by)
    {
      for (var i = 0; i < by; i++)
      {
        stringBuilder.Append("  ");
      }
      return stringBuilder;
    }

    public static StringBuilder IndentIf(this StringBuilder stringBuilder, int by, bool condition)
    {
      if (condition)
      {
        stringBuilder.Indent(by);
      }
      return stringBuilder;
    }

    public static StringBuilder AppendIf(this StringBuilder stringBuilder, string value, bool condition)
    {
      if (condition)
      {
        stringBuilder.Append(value);
      }
      return stringBuilder;
    }

    public static StringBuilder AppendIf(this StringBuilder stringBuilder, Func<string> stringProducing,
      Func<bool> conditionProducing)
    {
      if (conditionProducing())
      {
        stringBuilder.Append(stringProducing());
      }
      return stringBuilder;
    }

    public static StringBuilder AppendLineIf(this StringBuilder stringBuilder, string value, bool condition)
    {
      if (condition)
      {
        stringBuilder.AppendLine(value);
      }

      return stringBuilder;
    }

    public static StringBuilder AppendFormatIf(this StringBuilder stringBuilder, string value, bool condition,
      params object[] formatItems)
    {
      if (condition)
      {
        stringBuilder.AppendFormat(value, formatItems);
      }

      return stringBuilder;
    }

    public static StringBuilder SkipLines(this StringBuilder stringBuilder, int times = 1)
    {
      for (var i = 1; i <= times; i++)
      {
        stringBuilder.Append("\r\n");
      }

      return stringBuilder;
    }

    public static StringBuilder SkipLineAndIndent(this StringBuilder stringBuilder, int by)
    {
      stringBuilder.SkipLines();
      stringBuilder.Indent(by);
      return stringBuilder;
    }

    public static StringBuilder SkipLinesAndIndent(this StringBuilder stringBuilder, int by, int lines)
    {
      stringBuilder.SkipLines(lines);
      stringBuilder.Indent(by);
      return stringBuilder;
    }
  }
}