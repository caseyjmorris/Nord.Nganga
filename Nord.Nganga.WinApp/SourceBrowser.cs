using System;
using System.Windows.Forms;

namespace Nord.Nganga.WinApp
{
  public partial class SourceBrowser : Form
  {
    private readonly Func<string> sourceProvider;
    private readonly Action<string> sourceVisitor;
    private string originalSource ;
    private readonly string name;
    public SourceBrowser(string name, Func<string> sourceProvider, Action<string> sourceVisitor)
    {
      this.sourceProvider = sourceProvider;
      this.sourceVisitor = sourceVisitor;
      this.name = name;
      this.InitializeComponent();
    }

    private void SourceBrowser_Load(object sender, EventArgs e)
    {
      this.Text = string.Format(
        "{0} - [{1}] - Source Browser({2})",
        typeof(CoordinationResultBrowser).Assembly.GetName().Name,
        typeof(CoordinationResultBrowser).Assembly.GetName().Version,
        this.name);

      this.richTextBox1.Text = this.sourceProvider();
      this.originalSource = this.richTextBox1.Text;

      this.fontSelector1.Bind(this.richTextBox1, Settings1.Default.SourceBrowserFontFamilyName, Settings1.Default.SourceBrowserFontSize);
      this.richTextBox1.FontChanged += richTextBox1_FontChanged;
    }

    void richTextBox1_FontChanged(object sender, EventArgs e)
    {
      Settings1.Default.SourceBrowserFontFamilyName = this.richTextBox1.Font.FontFamily.Name;
      Settings1.Default.SourceBrowserFontSize = this.richTextBox1.Font.Size;
    }

    private void SourceBrowser_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (string.Equals(this.richTextBox1.Text,this.originalSource,StringComparison.InvariantCultureIgnoreCase)) return;

      if (MessageBox.Show("Save shanges?", "Confirm Change", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
          DialogResult.Yes)
      {
        this.sourceVisitor(this.richTextBox1.Text);
      }
    }
  }
}
