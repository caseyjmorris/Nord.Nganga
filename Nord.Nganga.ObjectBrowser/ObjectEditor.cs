using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for ObjectEditor.
  /// </summary>

  public class ObjectEditor : Form
  {
    private ObjectControl pObjectControl;
    private VScrollBar vScrollBar1;
    private MainMenu mainMenu1;
    private MenuItem mnuFile;
    private MenuItem mnuFileExit;
    private MenuItem mnuView;
    private MenuItem mnuViewRefresh;
    private MenuItem menuItem1;
    private MenuItem menuItem2;
    private IContainer components;

    public ObjectControl ObjectControl
    {
      get
      {
        return this.pObjectControl;
      }
    }

    public object DataSource
    {
      set
      {
        this.pObjectControl = new ObjectControl(value);
        this.Controls.Add(this.pObjectControl);
        this.ClientSize = new System.Drawing.Size(this.pObjectControl.Width, this.pObjectControl.Height);
        this.SizeChanged += new EventHandler(this.ObjectEditor_SizeChanged);
        this.Text = value.ToString();
        this.vScrollBar1.ValueChanged += new EventHandler(this.vScrollBar1_ValueChanged);
        this.vScrollBar1.VisibleChanged += new EventHandler(this.vScrollBar1_VisibleChanged);
        this.vScrollBar1.Visible = (this.ClientSize.Height < this.pObjectControl.Height);
      }
      get
      {
        return this.pObjectControl.DataSource;
      }
    }

    public ObjectEditor ()
    {
      //
      // Required for Windows Form Designer support
      //
      this.InitializeComponent();

      //
      // TODO: Add any constructor code after InitializeComponent call
      //
      LicenseManager.Validate(typeof(ObjectEditor));
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose (bool disposing)
    {
      if (disposing)
      {
        if (this.components != null)
        {
          this.components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.components = new System.ComponentModel.Container();
      this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
      this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
      this.mnuFile = new System.Windows.Forms.MenuItem();
      this.mnuFileExit = new System.Windows.Forms.MenuItem();
      this.mnuView = new System.Windows.Forms.MenuItem();
      this.mnuViewRefresh = new System.Windows.Forms.MenuItem();
      this.menuItem1 = new System.Windows.Forms.MenuItem();
      this.menuItem2 = new System.Windows.Forms.MenuItem();
      this.SuspendLayout();
      // 
      // vScrollBar1
      // 
      this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
      this.vScrollBar1.Location = new System.Drawing.Point(276, 0);
      this.vScrollBar1.Name = "vScrollBar1";
      this.vScrollBar1.Size = new System.Drawing.Size(16, 273);
      this.vScrollBar1.TabIndex = 0;
      this.vScrollBar1.Visible = false;
      // 
      // mainMenu1
      // 
      this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFile,
            this.mnuView,
            this.menuItem1});
      // 
      // mnuFile
      // 
      this.mnuFile.Index = 0;
      this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuFileExit});
      this.mnuFile.Text = "File";
      // 
      // mnuFileExit
      // 
      this.mnuFileExit.Index = 0;
      this.mnuFileExit.Text = "Exit";
      this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
      // 
      // mnuView
      // 
      this.mnuView.Index = 1;
      this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuViewRefresh});
      this.mnuView.Text = "View";
      this.mnuView.Visible = false;
      // 
      // mnuViewRefresh
      // 
      this.mnuViewRefresh.Index = 0;
      this.mnuViewRefresh.Text = "Refresh";
      this.mnuViewRefresh.Click += new System.EventHandler(this.mnuViewRefresh_Click);
      // 
      // menuItem1
      // 
      this.menuItem1.Index = 2;
      this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
      this.menuItem1.Text = "Help";
      // 
      // menuItem2
      // 
      this.menuItem2.Index = 0;
      this.menuItem2.Text = "About";
      // 
      // ObjectEditor
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(292, 273);
      this.Controls.Add(this.vScrollBar1);
      this.Menu = this.mainMenu1;
      this.Name = "ObjectEditor";
      this.Text = "ObjectEditor";
      this.Load += new System.EventHandler(this.ObjectEditor_Load);
      this.ResumeLayout(false);

    }
    #endregion

    private bool clientAreaResizeInProcess = false;

    private void ObjectEditor_SizeChanged (object sender, EventArgs e)
    {
      if (!this.clientAreaResizeInProcess)
      {
        this.clientAreaResizeInProcess = true;

        this.AdjustSizes();

        this.clientAreaResizeInProcess = false;
      }
    }

    private void vScrollBar1_ValueChanged (object sender, EventArgs e)
    {
      this.pObjectControl.Top = -this.vScrollBar1.Value;
    }

    private bool adjustingSize = false;
    private void AdjustSizes ()
    {
      if (!this.adjustingSize)
      {
        this.adjustingSize = true;
        this.vScrollBar1.Visible = (this.ClientSize.Height < this.pObjectControl.Height);

        if (this.vScrollBar1.Visible)
        {
          this.pObjectControl.Width = this.ClientSize.Width - (this.vScrollBar1.Width + 2);
          this.vScrollBar1.Height = this.ClientSize.Height;
          this.vScrollBar1.Maximum = (this.pObjectControl.Height - this.ClientSize.Height) + 10;
        }
        else
        {
          this.pObjectControl.Top = 0;
          this.pObjectControl.Width = this.ClientSize.Width;
        }
        this.adjustingSize = false;
      }
    }

    private void vScrollBar1_VisibleChanged (object sender, EventArgs e)
    {
      this.AdjustSizes();
    }

    private void ObjectEditor_Load (object sender, EventArgs e)
    {

    }

    private void mnuFileExit_Click (object sender, EventArgs e)
    {
      this.Close();
    }

    private void mnuViewRefresh_Click (object sender, EventArgs e)
    {
      this.pObjectControl.RefreshView();
    }

  }
}
