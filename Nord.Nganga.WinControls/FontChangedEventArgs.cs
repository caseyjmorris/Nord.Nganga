using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nord.Nganga.WinControls
{
  public class FontChangedEventArgs:EventArgs 
  {
    public Font NewFont { get; set; }
  }
}
