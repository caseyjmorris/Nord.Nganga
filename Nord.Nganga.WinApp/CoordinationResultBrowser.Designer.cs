using Nord.Nganga.WinControls;

namespace Nord.Nganga.WinApp
{
  partial class CoordinationResultBrowser
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
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.openAssemblyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
      this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveResourceOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.loadedAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.autoVSIntegration = new System.Windows.Forms.ToolStripMenuItem();
      this.logFusionResolutionEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(947, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openAssemblyToolStripMenuItem,
            this.toolStripMenuItem4,
            this.saveToolStripMenuItem,
            this.saveResourceOnlyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // openAssemblyToolStripMenuItem
      // 
      this.openAssemblyToolStripMenuItem.Name = "openAssemblyToolStripMenuItem";
      this.openAssemblyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
      // 
      // toolStripMenuItem4
      // 
      this.toolStripMenuItem4.Name = "toolStripMenuItem4";
      this.toolStripMenuItem4.Size = new System.Drawing.Size(169, 6);
      // 
      // saveToolStripMenuItem
      // 
      this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
      this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
      this.saveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
      this.saveToolStripMenuItem.Text = "Save";
      this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
      // 
      // saveResourceOnlyToolStripMenuItem
      // 
      this.saveResourceOnlyToolStripMenuItem.Enabled = false;
      this.saveResourceOnlyToolStripMenuItem.Name = "saveResourceOnlyToolStripMenuItem";
      this.saveResourceOnlyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
      this.saveResourceOnlyToolStripMenuItem.Text = "Save resource only";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 6);
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // viewToolStripMenuItem
      // 
      this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadedAssembliesToolStripMenuItem});
      this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
      this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
      this.viewToolStripMenuItem.Text = "View";
      // 
      // loadedAssembliesToolStripMenuItem
      // 
      this.loadedAssembliesToolStripMenuItem.Name = "loadedAssembliesToolStripMenuItem";
      this.loadedAssembliesToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoVSIntegration,
            this.logFusionResolutionEventsToolStripMenuItem});
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.optionsToolStripMenuItem.Text = "Options";
      // 
      // autoVSIntegration
      // 
      this.autoVSIntegration.Checked = true;
      this.autoVSIntegration.CheckOnClick = true;
      this.autoVSIntegration.CheckState = System.Windows.Forms.CheckState.Checked;
      this.autoVSIntegration.Name = "autoVSIntegration";
      this.autoVSIntegration.Size = new System.Drawing.Size(247, 22);
      this.autoVSIntegration.Text = "Automatic VS Project Integration";
      this.autoVSIntegration.Click += new System.EventHandler(this.autoVSIntegration_Click);
      // 
      // logFusionResolutionEventsToolStripMenuItem
      // 
      this.logFusionResolutionEventsToolStripMenuItem.CheckOnClick = true;
      this.logFusionResolutionEventsToolStripMenuItem.Name = "logFusionResolutionEventsToolStripMenuItem";
      this.logFusionResolutionEventsToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
      this.logFusionResolutionEventsToolStripMenuItem.Text = "Log Fusion Resolution Events";
      // 
      // dataGridView1
      // 
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 24);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new System.Drawing.Size(947, 672);
      this.dataGridView1.TabIndex = 1;
      this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
      // 
      // CoordinationResultBrowser
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(947, 696);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "CoordinationResultBrowser";
      this.Text = "Nganga";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem saveResourceOnlyToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem autoVSIntegration;
    private System.Windows.Forms.ToolStripMenuItem logFusionResolutionEventsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem loadedAssembliesToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
    private System.Windows.Forms.ToolStripMenuItem openAssemblyToolStripMenuItem;
    private System.Windows.Forms.DataGridView dataGridView1;
  }
}

