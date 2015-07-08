using System.Windows.Forms;

namespace Nord.AngularUiGen.WinApp
{
  public partial class ResourceOnlyParmsDialog : Form
  {
    public ResourceOnlyParmsDialog()
    {
      this.InitializeComponent();
    }

    public string AppName
    {
      get { return this.appName.Text; }
    }

    public string CacheName
    {
      get { return this.cacheName.Text; }
    }

    public bool UseCache
    {
      get
      {
        return this.chkCache.Checked ;
      }
    }
  }
}
