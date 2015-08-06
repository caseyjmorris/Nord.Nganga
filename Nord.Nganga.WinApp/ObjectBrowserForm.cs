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
  public partial class ObjectBrowserForm : Form
  {
    public ObjectBrowserForm ()
    {
      this.InitializeComponent();
    }

    private void ObjectBrowserForm_Load (object sender, EventArgs e)
    {

    }

    public object DataSource
    {
      set
      {
        this.objectBrowser1.DataSource = value; 
        this.SetId($"Object Browser - [{value.GetType().Name}]");
      }
    }
  }
}
