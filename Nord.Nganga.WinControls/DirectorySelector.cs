using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace Nord.Nganga.WinControls
{
  public partial class DirectorySelector : UserControl
  {

    public event EventHandler<SelectionChangedEventArgs<string>> SelectionChanged;
    public event EventHandler<EventArgs> HistoryChanged;
    public StringCollection History { get { return this.selectedPath.History; } set { this.selectedPath.History = value; } }

    [Description("The Folder description"),
      Category("Values"),
      DefaultValue(""),
      Browsable(true)]
    public string Description { get; set; }

    public DirectorySelector()
    {
      this.InitializeComponent();

      this.selectedPath.SelectionChanged += this.selectedPath_SelectionChanged;
      this.selectedPath.HistoryChanged += this.selectedPath_HistoryChanged;
    }

    void selectedPath_HistoryChanged(object sender, EventArgs e)
    {
      if (this.HistoryChanged != null)
      {
        this.HistoryChanged(sender, e);
      }
    }

    void selectedPath_SelectionChanged(object sender, SelectionChangedEventArgs<string> e)
    {
      this.SelectionChanged?.Invoke(sender, e);
    }

    public string SelectedPath
    {
      get
      {
        return this.selectedPath.Text;
      }
      set
      {
        this.selectedPath.Text = value;
      }
    }

    private void browse_Click(object sender, EventArgs e)
    {
      var d = new FolderBrowserDialog()
      {
        SelectedPath = this.SelectedPath,
        Description = this.Description,
        ShowNewFolderButton = true
      };

      var r = d.ShowDialog();
      if (r != DialogResult.OK && r != DialogResult.Yes) return;
      this.SelectedPath = d.SelectedPath;
    }
  }
}
