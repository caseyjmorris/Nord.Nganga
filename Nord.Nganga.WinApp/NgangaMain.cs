using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.WinApp
{
  public partial class NgangaMain : Form
  {
    private readonly Func<StringFormatProviderVisitor, IEnumerable<CoordinationResult>> coordinationResultProvider;

    public NgangaMain(Func<StringFormatProviderVisitor, IEnumerable<CoordinationResult>> coordinationResultProvider)
    {
      this.coordinationResultProvider = coordinationResultProvider;
      this.InitializeComponent();
    }

    private void openAssemblyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var coordinationResults = this.coordinationResultProvider(NgangaLog.Instance.Log).ToList();
      if (!coordinationResults.Any())
      {
        return;
      }

      if (coordinationResults.Count() == 1)
      {
        (new CoordinationResultBrowser(coordinationResults.First())).Show();
      }
      else
      {
        (new CoordinationResultCollectionBrowser(coordinationResults)).Show();
      }
    }

    private void loadedAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      (new AppDomainAssemblyListBrowser()).Show();
    }

    private void NgangaMain_Load(object sender, EventArgs e)
    {
      this.Text = string.Format(
        "{0} - [{1}] - Main",
        typeof(CoordinationResultBrowser).Assembly.GetName().Name,
        typeof(CoordinationResultBrowser).Assembly.GetName().Version);
    }

    private void quitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }
  }
}