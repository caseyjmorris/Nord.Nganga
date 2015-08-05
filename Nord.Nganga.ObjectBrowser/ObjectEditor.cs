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
    private IContainer components;

    public ObjectControl ObjectControl
    {
      get { return this.pObjectControl; }
    }

    public object DataSource
    {
      set
      {
        try
        {

          this.pObjectControl = new ObjectControl(value);
          this.Controls.Add(this.pObjectControl);
          this.ClientSize = new System.Drawing.Size(this.pObjectControl.Width, this.pObjectControl.Height);
          this.SizeChanged += this.ObjectEditor_SizeChanged;
          this.Text = value.ToString();
          this.vScrollBar1.ValueChanged += this.vScrollBar1_ValueChanged;
          this.vScrollBar1.VisibleChanged += this.vScrollBar1_VisibleChanged;
          this.vScrollBar1.Visible = (this.ClientSize.Height < this.pObjectControl.Height);
        }
        catch (Exception e)
        {
          this.exceptionVisitor?.Invoke(e);
        }
    }
      get { return this.pObjectControl.DataSource; }
    }

    private readonly Action<Exception> exceptionVisitor;
    public ObjectEditor(Action<Exception> exceptionVisitor)
    {
      this.exceptionVisitor = exceptionVisitor;
      this.InitializeComponent();
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.components?.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new Container();
      this.vScrollBar1 = new VScrollBar();
      this.mainMenu1 = new MainMenu(this.components);
      this.SuspendLayout();
      // 
      // vScrollBar1
      // 
      this.vScrollBar1.Dock = DockStyle.Right;
      this.vScrollBar1.Location = new System.Drawing.Point(765, 0);
      this.vScrollBar1.Name = "vScrollBar1";
      this.vScrollBar1.Size = new System.Drawing.Size(16, 299);
      this.vScrollBar1.TabIndex = 0;
      this.vScrollBar1.Visible = false;
      // 
      // ObjectEditor
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(781, 299);
      this.Controls.Add(this.vScrollBar1);
      this.Menu = this.mainMenu1;
      this.Name = "ObjectEditor";
      this.Text = "ObjectEditor";
      this.Load += new System.EventHandler(this.ObjectEditor_Load);
      this.ResumeLayout(false);
    }

    #endregion

    private bool clientAreaResizeInProcess = false;

    private void ObjectEditor_SizeChanged(object sender, EventArgs e)
    {
      if (!this.clientAreaResizeInProcess)
      {
        this.clientAreaResizeInProcess = true;

        this.AdjustSizes();

        this.clientAreaResizeInProcess = false;
      }
    }

    private void vScrollBar1_ValueChanged(object sender, EventArgs e)
    {
      this.pObjectControl.Top = -this.vScrollBar1.Value;
    }

    private bool adjustingSize = false;

    private void AdjustSizes()
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

    private void vScrollBar1_VisibleChanged(object sender, EventArgs e)
    {
      this.AdjustSizes();
    }

    private void ObjectEditor_Load(object sender, EventArgs e)
    {
    }

    private void mnuFileExit_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void mnuViewRefresh_Click(object sender, EventArgs e)
    {
      this.pObjectControl.RefreshView();
    }
  }
}