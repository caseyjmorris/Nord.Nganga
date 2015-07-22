using System;
using System.Windows.Forms;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Fs.VsIntegration;

namespace Nord.Nganga.WinApp
{
  public partial class CoordinationResultBrowser : Form
  {
    private readonly CoordinationResult coordinationResult;
    public CoordinationResultBrowser(CoordinationResult coordinationResult)
    {
      this.coordinationResult = coordinationResult;
      this.InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {

      this.Text = string.Format(
          "{0} - [{1}] - Coordination Result Browser",
          typeof(CoordinationResultBrowser).Assembly.GetName().Name,
          typeof(CoordinationResultBrowser).Assembly.GetName().Version);

      this.viewRTB.Text = this.coordinationResult.ViewBody;
      this.contorllerRTB.Text = this.coordinationResult.ControllerBody;
      this.resourceRTB.Text = this.coordinationResult.ResourceBody;
      this.apiControllerAssyLocation.Text = this.coordinationResult.SourceAssemblyLocation;
      this.apiControllerTypeName.Text = this.coordinationResult.ControllerTypeName;
      this.resourcesDir.Text = this.coordinationResult.NgResourcesPath;
      this.viewsDir.Text = this.coordinationResult.NgViewsPath;
      this.controllersDir.Text = this.coordinationResult.NgControllersPath;
      this.vsProjectFileName.Text = this.coordinationResult.VsProjectName;
      this.vsProjectPath.Text = this.coordinationResult.VsProjectPath;

      foreach (var ex in this.coordinationResult.Exceptions)
      {
        var ie = ex;
        while (ie != null)
        {
          Log("{0}", ie.Message);
          ie = ex.InnerException;
        }
      }
      this.autoVSIntegration.Checked = Settings1.Default.AutoVSIntegration;
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      VsIntegrator.Save(this.coordinationResult, this.autoVSIntegration.Checked, Log);
    }

    private void tabControl1_Selected(object sender, TabControlEventArgs e)
    {
      var tabControl = sender as TabControl;
      if (tabControl == null) return;

      var selectedPage = tabControl.SelectedTab;
      if (selectedPage == null) return;

      var rtb = selectedPage.Controls[0] as RichTextBox;
      if (rtb == null) return;
      var txt = rtb.Text;
      if (string.IsNullOrEmpty(txt)) return;

      Clipboard.SetText(rtb.Text);
    }

    private void autoVSIntegration_Click(object sender, EventArgs e)
    {
      Settings1.Default.AutoVSIntegration = this.autoVSIntegration.Checked;
    }

    private static void Log(string formatProvider, params object[] parms)
    {
      NgangaLog.Instance.Log(formatProvider, parms);
    }
  }
}