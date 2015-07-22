using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nord.Nganga.WinApp
{
  public partial class AppDomainAssemblyListBrowser : Form
  {
    public AppDomainAssemblyListBrowser()
    {
      InitializeComponent();
    }

    private void AppDomainAssemblyListBrowser_Load(object sender, EventArgs e)
    {
      this.dataGridView1.DataSource = AppDomain.CurrentDomain.GetAssemblies()
        .Select(a => a.GetName())
        .OrderBy(n => n.Name)
        .Select(n=> new {AssemblyName=n.Name, AssemblyVersion=n.Version})
        .ToList();
    }
  }
}
