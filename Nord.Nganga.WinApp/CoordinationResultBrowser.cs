using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    private static IEnumerable<object>  ToDataSource(CoordinationResult cr)
    {
      var propertyInfoCollection = typeof(CoordinationResult)
      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .Where(p => p.CanRead && p.PropertyType == typeof(string));
      return propertyInfoCollection.Select(p => new KeyValuePair<string, string>(p.Name, (string)p.GetValue(cr))).Cast<object>().ToList();
    }
    private void Form1_Load(object sender, EventArgs e)
    {
      this.Text = string.Format(
          "{0} - [{1}] - Coordination Result Browser",
          typeof(CoordinationResultBrowser).Assembly.GetName().Name,
          typeof(CoordinationResultBrowser).Assembly.GetName().Version);

      this.dataGridView1.DataSource = ToDataSource(this.coordinationResult).ToList();

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

    private void dataGridView1_DoubleClick(object sender, EventArgs e)
    {
      var q = (from DataGridViewRow r 
      in this.dataGridView1.SelectedRows 
      select (KeyValuePair<string,string>)r.DataBoundItem).ToList();
      if (!q.Any()) return;
      var kvp = q.First();
      var value = kvp.Value ;

      Action<string> updateAcceptor = s =>
      {
        var propertyInfoCollection = typeof (CoordinationResult)
          .GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(p => p.CanRead && p.PropertyType == typeof (string) && p.Name == kvp.Key).ToList();
        if (!propertyInfoCollection.Any()) return;
        var propertyInfo = propertyInfoCollection.First();
        propertyInfo.SetValue(this.coordinationResult, s);
        this.dataGridView1.DataSource = ToDataSource(this.coordinationResult);
      };

      (new SourceBrowser(kvp.Key, () => value, updateAcceptor)).Show();

    }
  }
}