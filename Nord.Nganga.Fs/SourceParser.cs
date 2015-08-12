using System;
using System.Text.RegularExpressions;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs
{
  public class SourceParser
  {
    private readonly Regex headerRegex = new Regex(@"^\s*Output signature:\s+(?<md5>[A-Fa-f0-9]{32})\s*$",
      RegexOptions.Compiled);

    private readonly SystemPathSettingsPackage pathSettings;

    public SourceParser()
    {
      this.pathSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
    }

    public GeneratorParseResult ParseFile(TemplateFactory.Context context, string source)
    {
      var headerStartTemplate = TemplateFactory.GetTemplate(this.pathSettings, context, "headerStart");
      var headerStart = headerStartTemplate.Resolve();

      var headerEndTemplate = TemplateFactory.GetTemplate(this.pathSettings, context, "headerEnd");
      var headerEnd = headerEndTemplate.Resolve();

      if (!source.Contains(headerStart) || !source.Contains(headerEnd))
      {
        return new GeneratorParseResult {Success = false};
      }
      var start = source.IndexOf(headerStart, StringComparison.Ordinal);
      var end = source.IndexOf(headerEnd, StringComparison.Ordinal) + headerEnd.Length;

      if (start < 0 || end > source.Length)
      {
        return new GeneratorParseResult {Success = false};
      }

      var header = source.Substring(start, end);
      var body = source.Substring(end + 1);
      var calculatedBodyMd5 = body.CalculateMd5Hash();
      var matches = this.headerRegex.Matches(header);
      if (matches.Count == 0)
      {
        return new GeneratorParseResult {Success = false};
      }

      var result = new GeneratorParseResult
      {
        Success = true,
        Context = context,
        Input = source,
        Header = header,
        Body = body,
        CalculatedBodyMd5 = calculatedBodyMd5,
        DeclaredHeaderMd5 = matches[0].Groups["md5"].Value
      };

      return result;
    }
  }
}