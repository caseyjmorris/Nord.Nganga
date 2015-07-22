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
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.logFusionEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.resourceOnly = new System.Windows.Forms.CheckBox();
      this.allTypes = new System.Windows.Forms.CheckBox();
      this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.loadedAssembliesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // assemblySelector1
      // 
      this.assemblySelector1.DialogFilter = "Assemblies|*.dll";
      this.assemblySelector1.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("assemblySelector1.History")));
      this.assemblySelector1.Location = new System.Drawing.Point(150, 26);
      this.assemblySelector1.LogFusionResolutionEvents = false;
      this.assemblySelector1.LogHandler = null;
      this.assemblySelector1.Name = "assemblySelector1";
      this.assemblySelector1.SelectedFile = "";
      this.assemblySelector1.Size = new System.Drawing.Size(594, 27);
      this.assemblySelector1.TabIndex = 0;
      // 
      // directorySelector1
      // 
      this.directorySelector1.Description = null;
      this.directorySelector1.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("directorySelector1.History")));
      this.directorySelector1.Location = new System.Drawing.Point(151, 61);
      this.directorySelector1.Name = "directorySelector1";
      this.directorySelector1.SelectedPath = "";
      this.directorySelector1.Size = new System.Drawing.Size(593, 27);
      this.directorySelector1.TabIndex = 1;
      // 
      // typeSelector1
      // 
      this.typeSelector1.BaseApiControllerName = null;
      this.typeSelector1.Location = new System.Drawing.Point(151, 94);
      this.typeSelector1.Name = "typeSelector1";
      this.typeSelector1.SelectedType = null;
      this.typeSelector1.Size = new System.Drawing.Size(509, 25);
      this.typeSelector1.SourceAssembly = null;
      this.typeSelector1.TabIndex = 2;
      // 
      // button1
      // 
      this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button1.Location = new System.Drawing.Point(668, 135);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "Ok";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button2.Location = new System.Drawing.Point(12, 135);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 4;
      this.button2.Text = "Cancel";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(35, 32);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(113, 13);
      this.label5.TabIndex = 5;
      this.label5.Text = "ApiController Assembly";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(44, 68);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(102, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "VS Project Directory";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(40, 98);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(104, 13);
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
      this.menuStrip1.Size = new System.Drawing.Size(756, 24);
      this.menuStrip1.TabIndex = 8;
      this.menuStrip1.Text = "menuStrip1";
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
      this.resourceOnly.Location = new System.Drawing.Point(564, 139);
      this.resourceOnly.Name = "resourceOnly";
      this.resourceOnly.Size = new System.Drawing.Size(96, 17);
      this.resourceOnly.TabIndex = 9;
      this.resourceOnly.Text = "Resource Only";
      this.resourceOnly.UseVisualStyleBackColor = true;
      this.resourceOnly.CheckedChanged += new System.EventHandler(this.resourceOnly_CheckedChanged);
      // 
      // allTypes
      // 
      this.allTypes.AutoSize = true;
      this.allTypes.Location = new System.Drawing.Point(669, 97);
      this.allTypes.Name = "allTypes";
      this.allTypes.Size = new System.Drawing.Size(37, 17);
      this.allTypes.TabIndex = 10;
      this.allTypes.Text = "All";
      this.allTypes.UseVisualStyleBackColor = true;
      this.allTypes.CheckedChanged += new System.EventHandler(this.allTypes_CheckedChanged);
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
      // CoordinationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(756, 170);
      this.Controls.Add(this.allTypes);
      this.Controls.Add(this.resourceOnly);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.typeSelector1);
      this.Controls.Add(this.directorySelector1);
      this.Controls.Add(this.assemblySelector1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "CoordinationForm";
      this.Text = "NgangaGenParmsForm";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CoordinationForm_FormClosing);
      this.Load += new System.EventHandler(this.NgangaGenParmsForm_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
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
  }
}