namespace Nord.Nganga.WinApp
{
  partial class CoordinationForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CoordinationForm));
      this.assemblySelector1 = new Nord.Nganga.WinControls.AssemblySelector();
      this.directorySelector1 = new Nord.Nganga.WinControls.DirectorySelector();
      this.typeSelector1 = new Nord.Nganga.WinControls.TypeSelector();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.loadedAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.logFusionEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.resourceOnly = new System.Windows.Forms.CheckBox();
      this.allTypes = new System.Windows.Forms.CheckBox();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.menuStrip1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.SuspendLayout();
      // 
      // assemblySelector1
      // 
      this.assemblySelector1.DialogFilter = "Assemblies|*.dll";
      this.assemblySelector1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.assemblySelector1.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("assemblySelector1.History")));
      this.assemblySelector1.Location = new System.Drawing.Point(153, 3);
      this.assemblySelector1.LogFusionResolutionEvents = false;
      this.assemblySelector1.LogHandler = null;
      this.assemblySelector1.Name = "assemblySelector1";
      this.assemblySelector1.SelectedFile = "";
      this.assemblySelector1.Size = new System.Drawing.Size(752, 29);
      this.assemblySelector1.TabIndex = 0;
      // 
      // directorySelector1
      // 
      this.directorySelector1.Description = null;
      this.directorySelector1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.directorySelector1.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("directorySelector1.History")));
      this.directorySelector1.Location = new System.Drawing.Point(153, 38);
      this.directorySelector1.Name = "directorySelector1";
      this.directorySelector1.SelectedPath = "";
      this.directorySelector1.Size = new System.Drawing.Size(752, 29);
      this.directorySelector1.TabIndex = 1;
      // 
      // typeSelector1
      // 
      this.typeSelector1.BaseApiControllerName = null;
      this.typeSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.typeSelector1.Location = new System.Drawing.Point(3, 3);
      this.typeSelector1.Name = "typeSelector1";
      this.typeSelector1.SelectedType = null;
      this.typeSelector1.Size = new System.Drawing.Size(674, 23);
      this.typeSelector1.SourceAssembly = null;
      this.typeSelector1.TabIndex = 2;
      // 
      // button1
      // 
      this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button1.Location = new System.Drawing.Point(808, 3);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(71, 29);
      this.button1.TabIndex = 3;
      this.button1.Text = "Ok";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button2.Location = new System.Drawing.Point(31, 3);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(76, 29);
      this.button2.TabIndex = 4;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label5.Location = new System.Drawing.Point(3, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(144, 35);
      this.label5.TabIndex = 5;
      this.label5.Text = "ApiController Assembly";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label1.Location = new System.Drawing.Point(3, 35);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(144, 35);
      this.label1.TabIndex = 6;
      this.label1.Text = "VS Project Directory";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label2.Location = new System.Drawing.Point(3, 70);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(144, 35);
      this.label2.TabIndex = 7;
      this.label2.Text = "Web Controller Type";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem,
            this.optionsToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(908, 24);
      this.menuStrip1.TabIndex = 8;
      this.menuStrip1.Text = "menuStrip1";
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
      this.loadedAssembliesToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
      this.loadedAssembliesToolStripMenuItem.Text = "Loaded Assemblies";
      this.loadedAssembliesToolStripMenuItem.Click += new System.EventHandler(this.loadedAssembliesToolStripMenuItem_Click);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.logFusionEventsToolStripMenuItem});
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.optionsToolStripMenuItem.Text = "Options";
      // 
      // logFusionEventsToolStripMenuItem
      // 
      this.logFusionEventsToolStripMenuItem.Name = "logFusionEventsToolStripMenuItem";
      this.logFusionEventsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
      this.logFusionEventsToolStripMenuItem.Text = "Log Fusion Events";
      this.logFusionEventsToolStripMenuItem.Click += new System.EventHandler(this.logFusionEventsToolStripMenuItem_Click);
      // 
      // resourceOnly
      // 
      this.resourceOnly.AutoSize = true;
      this.resourceOnly.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resourceOnly.Location = new System.Drawing.Point(694, 3);
      this.resourceOnly.Name = "resourceOnly";
      this.resourceOnly.Size = new System.Drawing.Size(108, 29);
      this.resourceOnly.TabIndex = 9;
      this.resourceOnly.Text = "Resource Only";
      this.resourceOnly.UseVisualStyleBackColor = true;
      this.resourceOnly.CheckedChanged += new System.EventHandler(this.resourceOnly_CheckedChanged);
      // 
      // allTypes
      // 
      this.allTypes.AutoSize = true;
      this.allTypes.Dock = System.Windows.Forms.DockStyle.Fill;
      this.allTypes.Location = new System.Drawing.Point(683, 3);
      this.allTypes.Name = "allTypes";
      this.allTypes.Size = new System.Drawing.Size(66, 23);
      this.allTypes.TabIndex = 10;
      this.allTypes.Text = "All";
      this.allTypes.UseVisualStyleBackColor = true;
      this.allTypes.CheckedChanged += new System.EventHandler(this.allTypes_CheckedChanged);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.assemblySelector1, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.directorySelector1, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 4;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(908, 105);
      this.tableLayoutPanel1.TabIndex = 11;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 2;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 90.55745F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.442549F));
      this.tableLayoutPanel2.Controls.Add(this.typeSelector1, 0, 0);
      this.tableLayoutPanel2.Controls.Add(this.allTypes, 1, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(153, 73);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(752, 29);
      this.tableLayoutPanel2.TabIndex = 8;
      // 
      // splitContainer1
      // 
      this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer1.Location = new System.Drawing.Point(0, 24);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
      this.splitContainer1.Panel1MinSize = 105;
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel3);
      this.splitContainer1.Size = new System.Drawing.Size(908, 205);
      this.splitContainer1.SplitterDistance = 105;
      this.splitContainer1.TabIndex = 12;
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 6;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25.61983F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 74.38017F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 581F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 114F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
      this.tableLayoutPanel3.Controls.Add(this.button1, 4, 0);
      this.tableLayoutPanel3.Controls.Add(this.resourceOnly, 3, 0);
      this.tableLayoutPanel3.Controls.Add(this.button2, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.Size = new System.Drawing.Size(908, 96);
      this.tableLayoutPanel3.TabIndex = 10;
      // 
      // CoordinationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(908, 229);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "CoordinationForm";
      this.Text = "NgangaGenParmsForm";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CoordinationForm_FormClosing);
      this.Load += new System.EventHandler(this.NgangaGenParmsForm_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tableLayoutPanel2.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private WinControls.AssemblySelector assemblySelector1;
    private WinControls.DirectorySelector directorySelector1;
    private WinControls.TypeSelector typeSelector1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem logFusionEventsToolStripMenuItem;
    private System.Windows.Forms.CheckBox resourceOnly;
    private System.Windows.Forms.CheckBox allTypes;
    private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem loadedAssembliesToolStripMenuItem;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
  }
}