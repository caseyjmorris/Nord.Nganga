using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using EnvDTE;
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

    private static IEnumerable<object> ToDataSource(CoordinationResult cr)
    {
      var propertyInfoCollection = typeof(CoordinationResult)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(p => p.CanRead && p.PropertyType == typeof(string));
      return propertyInfoCollection.Select(p => new KeyValuePair<string, string>(p.Name, (string) p.GetValue(cr)))
        .OrderBy(k => k.Key)
        .Cast<object>()
        .ToList();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.SetId("Coordination Result Browser");

      this.dataGridView1.DataSource = ToDataSource(this.coordinationResult).ToList();

      this.autoVSIntegration.Checked = Settings1.Default.AutoVSIntegration;
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      // this invoke is required to ensure that the close 
      // is invoked on the same thread, and in the same app domain 
      // as the one hosting this form... 
      this.Invoke(new Action(this.Close));
    }

    private DTE dte;

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      VsIntegrator.Save(
        this.coordinationResult,
        this.autoVSIntegration.Checked,
        Log,
        this.forceOverwriteToolStripMenuItem.Checked,
        this.dte);
      if (this.dte != null)
      {
        this.dte.MainWindow.Visible = true;
      }
    }

    private void autoVSIntegration_Click(object sender, EventArgs e)
    {
      Settings1.Default.AutoVSIntegration = this.autoVSIntegration.Checked;
    }

    private static void Log(string formatProvider, params object[] parms)
    {
      NgangaLog.Instance.Log(formatProvider, parms);
    }

    private void dataGridView1_DoubleClick(object sender, EventArgs e)
    {
      var q = (from DataGridViewRow r
        in this.dataGridView1.SelectedRows
        select (KeyValuePair<string, string>) r.DataBoundItem).ToList();
      if (!q.Any()) return;
      var kvp = q.First();
      var value = kvp.Value;

      Action<string> updateAcceptor = s =>
      {
        var propertyInfoCollection = typeof(CoordinationResult)
          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(p => p.CanRead && p.PropertyType == typeof(string) && p.Name == kvp.Key).ToList();
        if (!propertyInfoCollection.Any()) return;
        var propertyInfo = propertyInfoCollection.First();
        propertyInfo.SetValue(this.coordinationResult, s);
        this.dataGridView1.DataSource = ToDataSource(this.coordinationResult);
      };

      (new SourceBrowser(kvp.Key, () => value, updateAcceptor)).Show();
    }

    private void enableVSDiffToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.ManageDteInstance(this.enableVSDiffToolStripMenuItem.Checked);
    }

    private void ManageDteInstance(bool create)
    {
      if (this.dte == null && create)
      {
        this.Enabled = false;
        var wait = new WaitDialog($"Starting Visual Studio instance{Environment.NewLine}Please wait...");
        wait.Show();
        this.dte = VsIntegrator.CreateIsolatedDTEInstance(Settings1.Default.DTEProgId);
        wait.Close();
        this.Enabled = true;
        return;
      }
      if (this.dte != null && !create)
      {
        this.dte.ExecuteCommand("File.Close");
      }
    }
  }
}