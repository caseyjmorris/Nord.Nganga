using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.Commands
{
  [Serializable]
  public class GenerateResult
  {
    public string MessageText { get; set; }

    public CoordinationResult CoordinationResult { get; set; }
  }
}