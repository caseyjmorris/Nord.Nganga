using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Fs.VsIntegration;

namespace Nord.Nganga.WinApp
{
  public partial class CoordinationResultCollectionBrowser : Form
  {
    private readonly IEnumerable<CoordinationResult> coordinationResultsColllection;
    public CoordinationResultCollectionBrowser(IEnumerable<CoordinationResult> coordinationResultsColllection)
    {
      this.coordinationResultsColllection = coordinationResultsColllection;
      this.InitializeComponent();
      this.dataGridView1.DataSource = this.coordinationResultsColllection.ToList();
    }

    private void CoordinationResultCollectionBrowser_Load(object sender, EventArgs e)
    {
         this.SetId("Coordination Result Collection Browser");
      
    }

    private void dataGridView1_DoubleClick(object sender, EventArgs e)
    {
      foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
      {
        (new CoordinationResultBrowser((CoordinationResult)row.DataBoundItem)).Show();
      }
    }

    private void autoIntegrateToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Settings1.Default.AutoVSIntegration = this.autoIntegrateToolStripMenuItem.Checked;
    }

    private void allToolStripMenuItem_Click(object sender, EventArgs e)
    {
      foreach (var c in this.coordinationResultsColllection)
      {
        VsIntegrator.Save(c, this.autoIntegrateToolStripMenuItem.Checked, NgangaLog.Instance.Log);
      }
    }

    private void CoordinationResultCollectionBrowser_FormClosing(object sender, FormClosingEventArgs e)
    {
      Settings1.Default.Save();
    }
  }
}
