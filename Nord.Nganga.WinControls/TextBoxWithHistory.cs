using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;

namespace Nord.Nganga.WinControls
{
  public partial class TextBoxWithHistory : UserControl
  {
    public event EventHandler<SelectionChangedEventArgs<string>> SelectionChanged;
    public event EventHandler<EventArgs> HistoryChanged;

    public Action<StringCollection> StringCollectionVisitor { get; set; }

    private StringCollection history;

    public StringCollection History
    {
      set
      {
        this.history = value ?? new StringCollection();
        this.comboBox1.DataSource = (from string s in this.history select s).ToList();
      }
      get { return this.history; }
    }

    public TextBoxWithHistory()
    {
      this.InitializeComponent();
      this.history = new StringCollection();
    }

    public new string Text
    {
      get { return this.comboBox1.SelectedItem == null ? this.comboBox1.Text : this.comboBox1.SelectedItem as string; }
      set
      {
        this.ConditionalAdd(value);
        this.comboBox1.Text = value;
        this.comboBox1.SelectedItem = value;
        if (this.SelectionChanged != null)
        {
          this.SelectionChanged(this, new SelectionChangedEventArgs<string> {SelectedValue = value});
        }
      }
    }

    private void comboBox1_Leave(object sender, EventArgs e)
    {
      this.HandleManualEntry();
    }

    private void HandleManualEntry()
    {
      var currentText = this.Text;

      this.ConditionalAdd(currentText);

      this.SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<string> {SelectedValue = currentText});
    }

    private void ConditionalAdd(string currentText)
    {
      if (string.IsNullOrEmpty(currentText))
        return;
      if ((from string s in this.history select s)
        .Any(i => i.Equals(currentText, StringComparison.InvariantCultureIgnoreCase))) return;

      this.history.Add(currentText);

      this.comboBox1.DataSource = null;
      this.comboBox1.DataSource = this.history;

      this.HistoryChanged?.Invoke(this, new EventArgs());
    }

    private void comboBox1_DropDownClosed(object sender, EventArgs e)
    {
      this.SelectionChanged?.Invoke(this, new SelectionChangedEventArgs<string> {SelectedValue = this.Text});
    }

    private void comboBox1_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        this.HandleManualEntry();
      }
    }

    private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      this.deleteToolStripMenuItem.DropDownItems.Clear();
      (from string s in this.history select s).ToList().ForEach(i =>
      {
        var mi = new ToolStripMenuItem(i);
        mi.Click += this.mi_Click;
        this.deleteToolStripMenuItem.DropDownItems.Add(mi);
      });
    }

    private void mi_Click(object sender, EventArgs e)
    {
      this.history.Remove(((ToolStripMenuItem) sender).Text);
    }

    private void comboBox1_Format(object sender, ListControlConvertEventArgs e)
    {
      var cb = (ComboBox) sender;
      var fv = e.ListItem.ToString().ReduceToFit(cb, 3);
      e.Value = fv;
    }
  }
}