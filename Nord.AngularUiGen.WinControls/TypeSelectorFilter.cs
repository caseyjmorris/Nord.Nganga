using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nord.AngularUiGen.WinControls
{
  public class TypeSelectorFilter
  {
    public Func<Type, bool> FilterProvider { get; set; }
    public string FilterDescription { get; set; }
    public bool IsActive { get; set; }
  }
}
