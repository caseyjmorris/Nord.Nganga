using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nord.Nganga.Annotations;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs.Coordination
{
  public class GeneratorParseResult
  {
    public bool Success { get; set; }
    public string Input { get; set; }
    public TemplateContext TemplateContext { get; set; }
    public string Header { get; set; }
    public string DeclaredHeaderMd5 { get; set; }
    public string Body { get; set; }
    public string CalculatedBodyMd5 { get; set; }
  }
}