using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nord.Nganga.WinApp
{
  public partial class NgangaLog : Form
  {
    private static readonly Lazy<NgangaLog> LaxyLog = new Lazy<NgangaLog>();
    public static NgangaLog Instance
    {
      get { return LaxyLog.Value; }
    }

    public NgangaLog()
    {
      this.InitializeComponent();
    }

    private void NgangaLog_Load(object sender, EventArgs e)
    {
      this.Text = string.Format(
        "{0} - [{1}] - Log",
        typeof(CoordinationResultBrowser).Assembly.GetName().Name,
        typeof(CoordinationResultBrowser).Assembly.GetName().Version);
        this.fontSelector1.Bind(this.rtbLog);
    }

    public void Log(string formatProvider, params object[] parms)
    {
      this.rtbLog.Select(0, 0);
      try
      {
        this.rtbLog.SelectedText = string.Format("{0}{1} - {2}", Environment.NewLine,
          DateTime.Now.ToString("hh:mm:ss.fff"), string.Format(formatProvider, parms));
          this.Show();
      }
      catch (Exception ex)
      {
        this.rtbLog.SelectedText = ex.Message;
        this.rtbLog.Select(0, 0);
        this.rtbLog.SelectedText = "Failing format provider:" + '"' + formatProvider + '"';
      }
    }

    private void rtbLog_TextChanged(object sender, EventArgs e)
    {

    }
  }
}
