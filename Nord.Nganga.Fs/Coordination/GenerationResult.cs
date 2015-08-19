using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nord.Nganga.Fs.Naming;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs.Coordination
{
  [Serializable]
  public class GenerationResult
  {
    public string NgPath { get; set; }

    public string RelativeFileName { get; set; }
    public string AbsoluteFileNameName { get; set; }

    public bool ParseSuccess { get; set; }

    public string Text { get; set; }
    public string Header { get; set; }
    public string Body { get; set; }
    public string Md5 { get; set; }


    public bool PreviousParseSuccess { get; set; }

    public bool PreviousVersionExists { get; set; }
    public string PreviousText { get; set; }
    public string PreviousHeader { get; set; }
    public string PreviousBody { get; set; }
    public string PreviousDeclaredMd5 { get; set; }
    public string PreviousCalculatedMd5 { get; set; }

    public bool MergeOrDiffRecommended => this.PreviousVersionExists && (!this.PreviousParseSuccess || (this.PreviousDeclaredMd5 != this.PreviousCalculatedMd5));
    public bool GenerationIsRedundantNoSaveRequired => !this.MergeOrDiffRecommended && this.Md5 == this.PreviousCalculatedMd5;

    public GenerationResult(
      Func<string, GeneratorParseResult> parser,
      Func<string> sourceProvider,
      Func<string> nameProvider,
      string vsProjectPath,
      string ngPath)
    {
      this.NgPath = ngPath;
      this.Text = sourceProvider();
      this.RelativeFileName = Path.Combine(this.NgPath, nameProvider());
      this.AbsoluteFileNameName = Path.Combine(vsProjectPath, this.RelativeFileName);

      var current = parser(this.Text);
      this.ParseSuccess = current.Success;
      if (current.Success)
      {
        this.Body = current.Body;
        this.Header = current.Header;
        this.Md5 = current.DeclaredHeaderMd5;
      }

      this.PreviousVersionExists = File.Exists(this.AbsoluteFileNameName);

      if (!this.PreviousVersionExists)
      {
        return;
      }

      this.PreviousText = File.ReadAllText(this.AbsoluteFileNameName).TrimEnd( Environment.NewLine.ToCharArray());
      var diskVersion = parser(this.PreviousText);
      this.PreviousParseSuccess = diskVersion.Success;

      if (!diskVersion.Success)
      {
        return;
      }
      this.PreviousBody = diskVersion.Body;
      this.PreviousHeader = diskVersion.Header;
      this.PreviousDeclaredMd5 = diskVersion.DeclaredHeaderMd5;
      this.PreviousCalculatedMd5 = diskVersion.CalculatedBodyMd5;
    }
  }
}