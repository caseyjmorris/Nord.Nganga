using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
      NgangaLog.Instance.Log($"{this.Text} ready.");
      this.Top = Settings1.Default.MainTop;
      this.Left = Settings1.Default.MainLeft;
      this.TrackLog();
    }

    private void TrackLog()
    {
      ((Form) NgangaLog.Instance).Top = this.Top + this.Height;
      ((Form) NgangaLog.Instance).Left = this.Left;
    }

    private void toolStripButton1_Click(object sender, EventArgs e)
    {
      this.toolStripButton1.Enabled = false;
      this.coordinationResultAcceptor(NgangaLog.Instance.Log, this.ResultRouter);
      this.toolStripButton1.Enabled = true;
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
      this.Invoke(new Action<IEnumerable<CoordinationResult>>(this.RouteResult), coordinationResults);

      // creating the browser on the INVOKING thread would have caused to to be owned by the 
      // thread (and associated app domain) hosting the assembly selection 
      // and therefore the coordination browsers would have closed when the assy selector was closed
    }

    private void RouteResult(IEnumerable<CoordinationResult> coordinationResults)
    {
      var results = coordinationResults.ToList();

      if (!results.Any())
      {
        return;
      }

      //if (this.InvokeRequired)
      //{
      //  this.Invoke(new Action<IEnumerable<CoordinationResult>>(this.RouteResult), coordinationResults);
      //  return;
      //}

      if (results.Count() == 1)
      {
        (new CoordinationResultBrowser(results.First())).Show();
      }
      else
      {
        (new CoordinationResultCollectionBrowser(results)).Show();
      }
    }

    private void NgangaMain_Move(object sender, EventArgs e)
    {
      Settings1.Default.MainTop = this.Top;
      Settings1.Default.MainLeft = this.Left;
      this.TrackLog();
    }
  }
}