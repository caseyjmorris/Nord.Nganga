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
      this.coordinationResultAcceptor(NgangaLog.Instance.Log, this.ResultRouter);
    }

    private void toolStripButton2_Click(object sender, EventArgs e)
    {
      (new AppDomainAssemblyListBrowser()).Show();
    }

    private void ResultRouter(IEnumerable<CoordinationResult> coordinationResults)
    {
      // this invoke is required to ensure that the CoordinationResultBrowser instance 
      // is created on the same thread, and in the same app domain 
      // as the one hosting this form... 
      this.Invoke(new Action<IEnumerable<CoordinationResult>>(RouteResult), coordinationResults);

      // creating the browser on the INVOKING thread would have caused to to be owned by the 
      // thread (and associated app domain) hosting the assembly selection 
      // and therefore the coordination browsers would have closed when the assy selector was closed
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