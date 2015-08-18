using System;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.StringTemplate;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs
{
  public static class SourceParser
  {
    public static GeneratorParseResult ParseFile(TemplateFactory.Context context, string source)
    {
      var pathSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();

      var openComment = TemplateFactory.GetTemplate(pathSettings, context, "openComment").Render();
      var closeComment = TemplateFactory.GetTemplate(pathSettings, context, "closeComment").Render();

      var hs = TemplateFactory.GetTemplate(pathSettings, TemplateFactory.Context.Master, "headerStart");
      hs.Add("model", new {openComment = string.Empty, closeComment = string.Empty});
      var headerStart = hs.Resolve(); // get the HEADER START text WITHOUT open or close tokens

      var he = TemplateFactory.GetTemplate(pathSettings, TemplateFactory.Context.Master, "headerEnd");
      he.Add("model", new {openComment = string.Empty, closeComment = string.Empty});
      var headerEnd = he.Resolve(); // get the HEADER END text WITHOUT open or close tokens

      if (string.IsNullOrEmpty(headerStart) || string.IsNullOrEmpty(headerEnd))
      {
        return new GeneratorParseResult {Success = false};
      }
      // now build whitespace agnostic expressions to match 
      // OPEN COMMENT + HEADERSTART 
      //  and 
      // HEADER END + CLOSE COMMENT 

      if (closeComment.StartsWith(@"*"))
      {
        closeComment = @"\" + closeComment;
      }
      var openPattern = "\\s*" + openComment + "\\s*" + headerStart + "\\s+";
      var closePattern = "\\s*" + headerEnd + "\\s*" + closeComment + "\\s*";

      var headerStartRegex = new Regex(openPattern, RegexOptions.Compiled);
      var headerEndRegex = new Regex(closePattern, RegexOptions.Compiled);

      var headerStartMatch = headerStartRegex.Match(source);
      var headerEndMatch = headerEndRegex.Match(source);

      if (!headerStartMatch.Success || !headerEndMatch.Success)
      {
        return new GeneratorParseResult {Success = false};
      }
      var matchedEnd = headerEndMatch.Value;

      var headerEndPosition = source.IndexOf(matchedEnd, StringComparison.Ordinal) + matchedEnd.Length;
      if (headerEndPosition > source.Length)
      {
        return new GeneratorParseResult {Success = false};
      }

      var header = source.Substring(0, headerEndPosition);
      var lines = header.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);

      var headerRegex = new Regex(@"^\s*Output signature:\s+(?<md5>[A-Fa-f0-9]{32})\s*$", RegexOptions.Compiled);
      var declaredHeaderMd5 = (from line in lines
        select headerRegex.Matches(line)
        into matches
        where matches.Count != 0
        select matches[0].Groups["md5"].Value).FirstOrDefault();

      if (declaredHeaderMd5 == null)
      {
        return new GeneratorParseResult {Success = false};
      }

      var body = source.Replace(header, string.Empty);

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