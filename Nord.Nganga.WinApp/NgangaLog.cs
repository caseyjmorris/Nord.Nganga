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

    public int LogSize => this.rtbLog.TextLength;

    public NgangaLog()
    {
      this.InitializeComponent();
    }

    private void NgangaLog_Load(object sender, EventArgs e)
    {
         this.SetId("Log");
        this.fontSelector1.Bind(this.rtbLog, Settings1.Default.LogFontFamilyName, Settings1.Default.LogFontSize);
        this.rtbLog.FontChanged += this.rtbLog_FontChanged;
    }

    void rtbLog_FontChanged(object sender, EventArgs e)
    {
      Settings1.Default.LogFontFamilyName = this.rtbLog.Font.FontFamily.Name;
      Settings1.Default.LogFontSize = this.rtbLog.Font.Size;
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
      Application.DoEvents();
      System.Threading.Thread.Yield();
    }

    private void rtbLog_TextChanged(object sender, EventArgs e)
    {

    }

    private void NgangaLog_FormClosing(object sender, FormClosingEventArgs e)
    {
      Settings1.Default.Save();
    }
  }
}
