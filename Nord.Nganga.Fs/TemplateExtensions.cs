using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.StringTemplate;

namespace Nord.Nganga.Fs
{
  public static class TemplateExtensions
  {
      public static string Resolve(this Template template)
    {
      var sb = new StringBuilder();
      var aiw = new AutoIndentWriter(new StringWriter(sb));
      var s = template.Write(aiw);
      return sb.ToString();
    }
  }
}
