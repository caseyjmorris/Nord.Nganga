using System;
using System.Linq;
using System.Text.RegularExpressions;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs
{
  public class SourceParser
  {


    private readonly SystemPathSettingsPackage pathSettings;

    public SourceParser()
    {
      this.pathSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
    }

    public GeneratorParseResult ParseFile(TemplateFactory.Context context, string source)
    {
      var openComment = TemplateFactory.GetTemplate(this.pathSettings, context, "openComment").Render();
      var closeComment = TemplateFactory.GetTemplate(this.pathSettings, context, "closeComment").Render();

      var hs = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.Master, "headerStart");
      hs.Add("model", new {openComment, closeComment});
      var headerStart = hs.Resolve();

      var he = TemplateFactory.GetTemplate(this.pathSettings, TemplateFactory.Context.Master, "headerEnd");
      he.Add("model", new {openComment, closeComment});
      var headerEnd = he.Resolve();

      if (string.IsNullOrEmpty(headerStart) || string.IsNullOrEmpty(headerEnd) || !source.Contains(headerStart) ||
          !source.Contains(headerEnd))
      {
        return new GeneratorParseResult {Success = false};
      }
      var start = source.IndexOf(headerStart, StringComparison.Ordinal);
      var end = source.IndexOf(headerEnd, StringComparison.Ordinal) + headerEnd.Length;

      if (start < 0 || end > source.Length)
      {
        return new GeneratorParseResult {Success = false};
      }

      var headerRegex = new Regex(@"^\s*Output signature:\s+(?<md5>[A-Fa-f0-9]{32})\s*$" ,RegexOptions.Compiled);

      var header = source.Substring(start, end);
      var lines = header.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
      var declaredHeaderMd5 = (from line in lines select headerRegex.Matches(line) into matches where matches.Count != 0 select matches[0].Groups["md5"].Value).FirstOrDefault();

      if (declaredHeaderMd5 == null)
        {
          return new GeneratorParseResult {Success = false};
        } 

      var body = source.Substring(end + Environment.NewLine.Length);
      var calculatedBodyMd5 = body.CalculateMd5Hash();

      var result = new GeneratorParseResult
      {
        Success = true,
        Context = context,
        Input = source,
        Header = header,
        Body = body,
        CalculatedBodyMd5 = calculatedBodyMd5,
        DeclaredHeaderMd5 = declaredHeaderMd5
      };

      return result;
    }
  }
}