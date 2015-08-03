using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.WinApp
{
  public partial class NgangaMain : Form
  {
    private readonly Action<StringFormatProviderVisitor, Action<IEnumerable<CoordinationResult>>>
      coordinationResultAcceptor;

    public NgangaMain(
      Action<StringFormatProviderVisitor, Action<IEnumerable<CoordinationResult>>> coordinationResultAcceptor)
    {
      this.coordinationResultAcceptor = coordinationResultAcceptor;
      this.InitializeComponent();
    }

    private void NgangaMain_Load(object sender, EventArgs e)
    {
      this.SetId("Main");
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      this.coordinationResultAcceptor(NgangaLog.Instance.Log, ResultRouter);
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      (new AppDomainAssemblyListBrowser()).Show();
    }

    private void ResultRouter(IEnumerable<CoordinationResult> coordinationResults)
    {
     this.Invoke(new Action<IEnumerable<CoordinationResult>>(RouteResult), coordinationResults);
    }

    private static void RouteResult(IEnumerable<CoordinationResult> coordinationResults)
    {
            var results = coordinationResults.ToList();

      if (!results.Any())
      {
        return;
      }

      if (results.Count() == 1)
      {
       (new CoordinationResultBrowser(results.First())).Show();
      }
      else
      {
        (new CoordinationResultCollectionBrowser(results)).Show();
      }
    }
  }
}