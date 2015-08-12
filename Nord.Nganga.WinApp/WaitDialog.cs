using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nord.Nganga.WinApp
{
  public partial class WaitDialog : Form
  {
    public WaitDialog (string msg = "Please wait...")
    {
      this.InitializeComponent();
      this.label1.Text = msg;
    }
  }
}
