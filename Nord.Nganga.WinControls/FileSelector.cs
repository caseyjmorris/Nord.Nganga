using System;
using System.Collections.Specialized;
using System.Windows.Forms;

namespace Nord.Nganga.WinControls
{
  public partial class FileSelector : UserControl
  {
    public string DialogFilter { get; set; }
    public event EventHandler<EventArgs> SelectionChanged;
    public event EventHandler<EventArgs> HistoryChanged;

    public StringCollection History
    {
      get { return this.selectedFile.History; }
      set { this.selectedFile.History = value; }
    }

    public FileSelector()
    {
      this.InitializeComponent();
      this.DialogFilter = "All Files | *.*";

      this.selectedFile.SelectionChanged += this.selectedFile_SelectionChanged;
      this.selectedFile.HistoryChanged += this.selectedFile_HistoryChanged;
    }

    private void selectedFile_HistoryChanged(object sender, EventArgs e)
    {
      this.HistoryChanged?.Invoke(sender, e);
    }

    private void selectedFile_SelectionChanged(object sender, EventArgs e)
    {
      this.SelectionChanged?.Invoke(sender, e);
    }

    public string SelectedFile
    {
      get { return this.selectedFile.Text; }
      set { this.selectedFile.Text = value; }
    }

    private void browse_Click(object sender, EventArgs e)
    {
      var d = new OpenFileDialog {Filter = this.DialogFilter};

      var r = d.ShowDialog();

      if (r != DialogResult.OK && r != DialogResult.Yes) return;

      this.SelectedFile = d.FileName;
    }
  }
}