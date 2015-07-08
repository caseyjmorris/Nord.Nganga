using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Nord.AngularUiGen.Engine.Extensions.Reflection;

namespace Nord.AngularUiGen.WinControls
{
  public partial class TypeSelector : UserControl
  {
    public event EventHandler<EventArgs> SelectionChanged;
    public IEnumerable<TypeSelectorFilter> Filters { private get; set; }

    private Assembly _sourceAssembly;

    public Assembly SourceAssembly
    {
      get { return this._sourceAssembly; }
      set
      {
        this._sourceAssembly = value;
        if (this._sourceAssembly == null)
        {
          this.TypeList = new List<Type>();
          return;
        }

        var assertWebApi = this.Filters.Any(f => f.IsActive && f.FilterDescription.Contains("Api"));
        var assertModule = this.Filters.Any(f => f.IsActive && f.FilterDescription.Contains("Module"));
        var assertRoute = this.Filters.Any(f => f.IsActive && f.FilterDescription.Contains("Route"));

        var types = this._sourceAssembly.FindWebApiControllers(
          assertWebApi,
          assertModule,
          assertRoute).ToList();

        this.TypeList = types;

        if (!this.TypeList.Any())
        {
          MessageBox.Show(
          "No types match the selected filter criteria", 
          "No Match", 
          MessageBoxButtons.OK,
          MessageBoxIcon.Information);
        }
      }
    }

    private List<Type> _typeList;
    public List<Type> TypeList
    {
      get { return this._typeList; }
      private set
      {
        this._typeList = value ?? new List<Type>();
        this.comboBox1.DataSource = this._typeList;

        if (this._typeList.Count == 1)
        {
          this.SelectedType = this._typeList.First();
        }
        else
        {
          this.comboBox1.Text = string.Empty;
        }
      }
    }

    public TypeSelector()
    {
      this.InitializeComponent();
      this.comboBox1.DisplayMember = "FullName";
      this.Filters = new List<TypeSelectorFilter>();
    }

    public Type SelectedType
    {
      get
      {
        return (Type) this.comboBox1.SelectedItem;
      }
      set { this.comboBox1.SelectedItem = value; }
    }

    private void comboBox1_DropDownClosed(object sender, EventArgs e)
    {
      if (this.SelectionChanged != null)
      {
        this.SelectionChanged(this, new EventArgs());
      }
    }

    private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.contextMenuStrip1.Items.Count == 0)
      {
        this.Filters.ToList().ForEach(filter =>
        {
          var filterCheckbox = new ToolStripMenuItem
          {
            Text = filter.FilterDescription,
            Checked = filter.IsActive,
            CheckOnClick = true
          };
          filterCheckbox.CheckStateChanged += this.filterCheckbox_CheckStateChanged;
          filterCheckbox.Tag = filter;
          this.contextMenuStrip1.Items.Add(filterCheckbox);
        });
      }
    }

    void filterCheckbox_CheckStateChanged(object sender, EventArgs e)
    {
      var menuItem = sender as ToolStripMenuItem;
      if (menuItem == null) return;
      var filter = menuItem.Tag as TypeSelectorFilter;
      if (filter == null) return;
      filter.IsActive = menuItem.Checked;
      this.SourceAssembly = this._sourceAssembly;
    }
  }
}
