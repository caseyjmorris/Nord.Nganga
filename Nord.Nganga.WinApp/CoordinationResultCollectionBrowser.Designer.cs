namespace Nord.Nganga.WinApp
{
  partial class CoordinationResultCollectionBrowser
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
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
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.autoIntegrateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.forceOverwriteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 25);
      this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new System.Drawing.Size(1335, 353);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.optionsToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
      this.menuStrip1.Size = new System.Drawing.Size(1335, 25);
      this.menuStrip1.TabIndex = 1;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // saveToolStripMenuItem
      // 
      this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem});
      this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      this.saveToolStripMenuItem.Size = new System.Drawing.Size(37, 19);
      this.saveToolStripMenuItem.Text = "File";
      // 
      // allToolStripMenuItem
      // 
      this.allToolStripMenuItem.Name = "allToolStripMenuItem";
      this.allToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
      this.allToolStripMenuItem.Text = "Save";
      this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoIntegrateToolStripMenuItem,
            this.forceOverwriteToolStripMenuItem});
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 19);
      this.optionsToolStripMenuItem.Text = "Options";
      // 
      // autoIntegrateToolStripMenuItem
      // 
      this.autoIntegrateToolStripMenuItem.Checked = true;
      this.autoIntegrateToolStripMenuItem.CheckOnClick = true;
      this.autoIntegrateToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
      this.autoIntegrateToolStripMenuItem.Name = "autoIntegrateToolStripMenuItem";
      this.autoIntegrateToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.autoIntegrateToolStripMenuItem.Text = "Auto Integrate";
      this.autoIntegrateToolStripMenuItem.Click += new System.EventHandler(this.autoIntegrateToolStripMenuItem_Click);
      // 
      // forceOverwriteToolStripMenuItem
      // 
      this.forceOverwriteToolStripMenuItem.CheckOnClick = true;
      this.forceOverwriteToolStripMenuItem.Name = "forceOverwriteToolStripMenuItem";
      this.forceOverwriteToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.forceOverwriteToolStripMenuItem.Text = "Force Overwrite";
      // 
      // CoordinationResultCollectionBrowser
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1335, 378);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.Name = "CoordinationResultCollectionBrowser";
      this.Text = "CoordinationResultCollectionBrowser";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CoordinationResultCollectionBrowser_FormClosing);
      this.Load += new System.EventHandler(this.CoordinationResultCollectionBrowser_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem autoIntegrateToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem forceOverwriteToolStripMenuItem;
  }
}