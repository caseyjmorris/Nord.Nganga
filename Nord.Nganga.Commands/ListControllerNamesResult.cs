using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nord.Nganga.Commands
{
  [Serializable]
  public class ListControllerNamesResult
  {
    public string MessageText { get; set; }

    public IEnumerable<string> TypeNames { get; set; }
  }
}