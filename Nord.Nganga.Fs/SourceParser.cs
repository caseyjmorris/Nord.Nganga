using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.StringTemplate;
using Nord.Nganga.Annotations;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs
{
  public static class SourceParser
  {
    public static GeneratorParseResult ParseFile(TemplateContext templateContext, string source)
    {
      var pathSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();

      var openComment = TemplateFactory.GetTemplate(templateContext, "openComment", pathSettings).Render();
      var closeComment = TemplateFactory.GetTemplate(templateContext, "closeComment", pathSettings).Render();

      var hs = TemplateFactory.GetTemplate(TemplateContext.Master, "headerStart", pathSettings);
      hs.Add("model", new {openComment = string.Empty, closeComment = string.Empty});
      var headerStart = hs.Resolve(); // get the HEADER START text WITHOUT open or close tokens

      var he = TemplateFactory.GetTemplate(TemplateContext.Master, "headerEnd", pathSettings);
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

      // var headerRegex = new Regex(@"^\s*Output signature:\s+(?<md5>[A-Fa-f0-9]{32})\s*$", RegexOptions.Compiled);
      //var declaredHeaderMd5 = (from line in lines
      //  select headerRegex.Matches(line)
      //  into matches
      //  where matches.Count != 0
      //  select matches[0].Groups["md5"].Value).FirstOrDefault();

      var declaredHeaderMd5 = GetKeyValue("Output signature:", "[A-Fa-f0-9]{32}", lines);

      if (declaredHeaderMd5 == null)
      {
        return new GeneratorParseResult {Success = false};
      }

      var body = source.Replace(header, string.Empty);

      var calculatedBodyMd5 = body.CalculateMd5Hash();

      var versionPattern = @"[0-9]+.[0-9]+.[0-9]+";

      var bodyTemplateVersion = GetKeyValue("Body Template version:", versionPattern, lines);
      //new Regex(@"^\s*Body Template version:\s+(?<bodyVersion>[0-9]+'.'[0-9]+'.'[0-9]+)\s*$", RegexOptions.Compiled);

      var masterTemplateVersion = GetKeyValue("Master Template version:", versionPattern, lines);
      // new Regex(@"^\s*Master Template version:\s+(?<masterVersion>[0-9]+'.'[0-9]+'.'[0-9]+)\s*$", RegexOptions.Compiled);

      var result = new GeneratorParseResult
      {
        Success = true,
        TemplateContext = templateContext,
        Input = source,
        Header = header,
        Body = body,
        CalculatedBodyMd5 = calculatedBodyMd5,
        DeclaredHeaderMd5 = declaredHeaderMd5,
        DeclaredHeaderBodyTemplateVersion = bodyTemplateVersion,
        DeclaredHeaderMasterTemplateVersion = masterTemplateVersion
      };

      return result;
    }

    private static string GetKeyValue(string label, string pattern, IEnumerable<string> lines)
    {
      var regex = new Regex(@"^\s*" + label + @"\s+(?<value>" + pattern + @")\s*$", RegexOptions.Compiled);
      var value = (from line in lines
        select regex.Matches(line)
        into matches
        where matches.Count != 0
        select matches[0].Groups["value"].Value).FirstOrDefault();
      return value;
    }
  }
}