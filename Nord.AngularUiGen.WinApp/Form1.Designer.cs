using Nord.AngularUiGen.WinControls;

namespace Nord.AngularUiGen.WinApp
{
  partial class Form1
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveResourceOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
      this.selectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
      this.resourceOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.useCustomOutputMappingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.autoVSIntegration = new System.Windows.Forms.ToolStripMenuItem();
      this.controllerPage = new System.Windows.Forms.TabPage();
      this.contorllerRTB = new System.Windows.Forms.RichTextBox();
      this.viewPage = new System.Windows.Forms.TabPage();
      this.viewRTB = new System.Windows.Forms.RichTextBox();
      this.resourcePage = new System.Windows.Forms.TabPage();
      this.resourceRTB = new System.Windows.Forms.RichTextBox();
      this.sourcePage = new System.Windows.Forms.TabPage();
      this.masterTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
      this.label4 = new System.Windows.Forms.Label();
      this.outputControlGroupTabPanel = new System.Windows.Forms.TableLayoutPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.resourceDirSelector = new Nord.AngularUiGen.WinControls.DirectorySelector();
      this.label2 = new System.Windows.Forms.Label();
      this.viewDirSelector = new Nord.AngularUiGen.WinControls.DirectorySelector();
      this.vsProjectFileSelector = new Nord.AngularUiGen.WinControls.FileSelector();
      this.label8 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.controllersDirSelector = new Nord.AngularUiGen.WinControls.DirectorySelector();
      this.messageLabel = new System.Windows.Forms.Label();
      this.rtbLog = new System.Windows.Forms.RichTextBox();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.controllerTypeSelector = new Nord.AngularUiGen.WinControls.TypeSelector();
      this.assemblySelector = new Nord.AngularUiGen.WinControls.FileSelector();
      this.label7 = new System.Windows.Forms.Label();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.logFusionResolutionEventsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip1.SuspendLayout();
      this.controllerPage.SuspendLayout();
      this.viewPage.SuspendLayout();
      this.resourcePage.SuspendLayout();
      this.sourcePage.SuspendLayout();
      this.masterTableLayoutPanel.SuspendLayout();
      this.outputControlGroupTabPanel.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem,
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
            this.saveToolStripMenuItem,
            this.saveResourceOnlyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
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
      this.saveResourceOnlyToolStripMenuItem.Click += new System.EventHandler(this.saveResourceOnlyToolStripMenuItem_Click);
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
      // generateToolStripMenuItem
      // 
      this.generateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allToolStripMenuItem,
            this.toolStripMenuItem2,
            this.selectedToolStripMenuItem,
            this.toolStripMenuItem3,
            this.resourceOnlyToolStripMenuItem});
      this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
      this.generateToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
      this.generateToolStripMenuItem.Text = "Generate";
      this.generateToolStripMenuItem.DropDownOpening += new System.EventHandler(this.generateToolStripMenuItem_DropDownOpening);
      this.generateToolStripMenuItem.Click += new System.EventHandler(this.generateToolStripMenuItem_Click);
      // 
      // allToolStripMenuItem
      // 
      this.allToolStripMenuItem.Name = "allToolStripMenuItem";
      this.allToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
      this.allToolStripMenuItem.Text = "All";
      this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(147, 6);
      // 
      // selectedToolStripMenuItem
      // 
      this.selectedToolStripMenuItem.Name = "selectedToolStripMenuItem";
      this.selectedToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
      this.selectedToolStripMenuItem.Text = "Selected";
      this.selectedToolStripMenuItem.Click += new System.EventHandler(this.selectedToolStripMenuItem_Click);
      // 
      // toolStripMenuItem3
      // 
      this.toolStripMenuItem3.Name = "toolStripMenuItem3";
      this.toolStripMenuItem3.Size = new System.Drawing.Size(147, 6);
      // 
      // resourceOnlyToolStripMenuItem
      // 
      this.resourceOnlyToolStripMenuItem.Name = "resourceOnlyToolStripMenuItem";
      this.resourceOnlyToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
      this.resourceOnlyToolStripMenuItem.Text = "Resource Only";
      this.resourceOnlyToolStripMenuItem.Click += new System.EventHandler(this.resourceOnlyToolStripMenuItem_Click);
      // 
      // optionsToolStripMenuItem
      // 
      this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.useCustomOutputMappingToolStripMenuItem,
            this.autoVSIntegration,
            this.logFusionResolutionEventsToolStripMenuItem});
      this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
      this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.optionsToolStripMenuItem.Text = "Options";
      // 
      // useCustomOutputMappingToolStripMenuItem
      // 
      this.useCustomOutputMappingToolStripMenuItem.CheckOnClick = true;
      this.useCustomOutputMappingToolStripMenuItem.Name = "useCustomOutputMappingToolStripMenuItem";
      this.useCustomOutputMappingToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
      this.useCustomOutputMappingToolStripMenuItem.Text = "Use Custom Output Mapping";
      this.useCustomOutputMappingToolStripMenuItem.Click += new System.EventHandler(this.useCustomOutputMappingToolStripMenuItem_Click);
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
      // controllerPage
      // 
      this.controllerPage.Controls.Add(this.contorllerRTB);
      this.controllerPage.Location = new System.Drawing.Point(4, 22);
      this.controllerPage.Name = "controllerPage";
      this.controllerPage.Size = new System.Drawing.Size(939, 646);
      this.controllerPage.TabIndex = 3;
      this.controllerPage.Text = "Controller";
      this.controllerPage.UseVisualStyleBackColor = true;
      // 
      // contorllerRTB
      // 
      this.contorllerRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.contorllerRTB.Font = new System.Drawing.Font("Consolas", 8.25F);
      this.contorllerRTB.Location = new System.Drawing.Point(0, 0);
      this.contorllerRTB.Name = "contorllerRTB";
      this.contorllerRTB.Size = new System.Drawing.Size(939, 646);
      this.contorllerRTB.TabIndex = 0;
      this.contorllerRTB.Text = "";
      // 
      // viewPage
      // 
      this.viewPage.Controls.Add(this.viewRTB);
      this.viewPage.Location = new System.Drawing.Point(4, 22);
      this.viewPage.Name = "viewPage";
      this.viewPage.Size = new System.Drawing.Size(939, 646);
      this.viewPage.TabIndex = 2;
      this.viewPage.Text = "View";
      this.viewPage.UseVisualStyleBackColor = true;
      // 
      // viewRTB
      // 
      this.viewRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewRTB.Font = new System.Drawing.Font("Consolas", 8.25F);
      this.viewRTB.Location = new System.Drawing.Point(0, 0);
      this.viewRTB.Name = "viewRTB";
      this.viewRTB.Size = new System.Drawing.Size(939, 646);
      this.viewRTB.TabIndex = 0;
      this.viewRTB.Text = "";
      // 
      // resourcePage
      // 
      this.resourcePage.Controls.Add(this.resourceRTB);
      this.resourcePage.Location = new System.Drawing.Point(4, 22);
      this.resourcePage.Name = "resourcePage";
      this.resourcePage.Padding = new System.Windows.Forms.Padding(3);
      this.resourcePage.Size = new System.Drawing.Size(939, 646);
      this.resourcePage.TabIndex = 1;
      this.resourcePage.Text = "Resource";
      this.resourcePage.UseVisualStyleBackColor = true;
      // 
      // resourceRTB
      // 
      this.resourceRTB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resourceRTB.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.resourceRTB.Location = new System.Drawing.Point(3, 3);
      this.resourceRTB.Name = "resourceRTB";
      this.resourceRTB.Size = new System.Drawing.Size(933, 640);
      this.resourceRTB.TabIndex = 0;
      this.resourceRTB.Text = "";
      // 
      // sourcePage
      // 
      this.sourcePage.Controls.Add(this.masterTableLayoutPanel);
      this.sourcePage.Location = new System.Drawing.Point(4, 22);
      this.sourcePage.Name = "sourcePage";
      this.sourcePage.Padding = new System.Windows.Forms.Padding(3);
      this.sourcePage.Size = new System.Drawing.Size(939, 646);
      this.sourcePage.TabIndex = 0;
      this.sourcePage.Text = "Source";
      this.sourcePage.UseVisualStyleBackColor = true;
      // 
      // masterTableLayoutPanel
      // 
      this.masterTableLayoutPanel.ColumnCount = 1;
      this.masterTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.masterTableLayoutPanel.Controls.Add(this.label4, 0, 2);
      this.masterTableLayoutPanel.Controls.Add(this.outputControlGroupTabPanel, 0, 3);
      this.masterTableLayoutPanel.Controls.Add(this.tableLayoutPanel3, 0, 1);
      this.masterTableLayoutPanel.Controls.Add(this.label7, 0, 0);
      this.masterTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.masterTableLayoutPanel.Location = new System.Drawing.Point(3, 3);
      this.masterTableLayoutPanel.Name = "masterTableLayoutPanel";
      this.masterTableLayoutPanel.RowCount = 4;
      this.masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.517241F));
      this.masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.75F));
      this.masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.65625F));
      this.masterTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 72.96875F));
      this.masterTableLayoutPanel.Size = new System.Drawing.Size(933, 640);
      this.masterTableLayoutPanel.TabIndex = 1;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label4.Location = new System.Drawing.Point(3, 123);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(927, 49);
      this.label4.TabIndex = 1;
      this.label4.Text = "Output Paths";
      this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // outputControlGroupTabPanel
      // 
      this.outputControlGroupTabPanel.ColumnCount = 2;
      this.outputControlGroupTabPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
      this.outputControlGroupTabPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.33333F));
      this.outputControlGroupTabPanel.Controls.Add(this.label1, 0, 0);
      this.outputControlGroupTabPanel.Controls.Add(this.resourceDirSelector, 1, 0);
      this.outputControlGroupTabPanel.Controls.Add(this.label2, 0, 1);
      this.outputControlGroupTabPanel.Controls.Add(this.viewDirSelector, 1, 1);
      this.outputControlGroupTabPanel.Controls.Add(this.vsProjectFileSelector, 1, 3);
      this.outputControlGroupTabPanel.Controls.Add(this.label8, 0, 3);
      this.outputControlGroupTabPanel.Controls.Add(this.label3, 0, 2);
      this.outputControlGroupTabPanel.Controls.Add(this.controllersDirSelector, 1, 2);
      this.outputControlGroupTabPanel.Controls.Add(this.messageLabel, 0, 4);
      this.outputControlGroupTabPanel.Controls.Add(this.rtbLog, 1, 4);
      this.outputControlGroupTabPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.outputControlGroupTabPanel.Location = new System.Drawing.Point(3, 175);
      this.outputControlGroupTabPanel.Name = "outputControlGroupTabPanel";
      this.outputControlGroupTabPanel.RowCount = 5;
      this.outputControlGroupTabPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.523809F));
      this.outputControlGroupTabPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.74026F));
      this.outputControlGroupTabPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.30736F));
      this.outputControlGroupTabPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.874459F));
      this.outputControlGroupTabPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63.20346F));
      this.outputControlGroupTabPanel.Size = new System.Drawing.Size(927, 462);
      this.outputControlGroupTabPanel.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label1.Location = new System.Drawing.Point(3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(148, 43);
      this.label1.TabIndex = 0;
      this.label1.Text = "Resources Directory";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // resourceDirSelector
      // 
      this.resourceDirSelector.Description = "Resource Output Directory";
      this.resourceDirSelector.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resourceDirSelector.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("resourceDirSelector.History")));
      this.resourceDirSelector.Location = new System.Drawing.Point(157, 3);
      this.resourceDirSelector.Name = "resourceDirSelector";
      this.resourceDirSelector.SelectedPath = "C:\\Users\\mlloyd\\Desktop";
      this.resourceDirSelector.Size = new System.Drawing.Size(767, 37);
      this.resourceDirSelector.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label2.Location = new System.Drawing.Point(3, 43);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(148, 44);
      this.label2.TabIndex = 2;
      this.label2.Text = "View Directory";
      this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // viewDirSelector
      // 
      this.viewDirSelector.Description = "View Output Directory";
      this.viewDirSelector.Dock = System.Windows.Forms.DockStyle.Fill;
      this.viewDirSelector.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("viewDirSelector.History")));
      this.viewDirSelector.Location = new System.Drawing.Point(157, 46);
      this.viewDirSelector.Name = "viewDirSelector";
      this.viewDirSelector.SelectedPath = "C:\\Users\\mlloyd\\Desktop";
      this.viewDirSelector.Size = new System.Drawing.Size(767, 38);
      this.viewDirSelector.TabIndex = 3;
      // 
      // vsProjectFileSelector
      // 
      this.vsProjectFileSelector.DialogFilter = "All Files | *.*";
      this.vsProjectFileSelector.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("vsProjectFileSelector.History")));
      this.vsProjectFileSelector.Location = new System.Drawing.Point(157, 132);
      this.vsProjectFileSelector.Name = "vsProjectFileSelector";
      this.vsProjectFileSelector.SelectedFile = "";
      this.vsProjectFileSelector.Size = new System.Drawing.Size(767, 34);
      this.vsProjectFileSelector.TabIndex = 6;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label8.Location = new System.Drawing.Point(3, 129);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(148, 40);
      this.label8.TabIndex = 5;
      this.label8.Text = "Visual Studio Project File";
      this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label3.Location = new System.Drawing.Point(3, 87);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(148, 42);
      this.label3.TabIndex = 4;
      this.label3.Text = "Controllers Directory";
      this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // controllersDirSelector
      // 
      this.controllersDirSelector.Description = "Controller Output Directory";
      this.controllersDirSelector.Dock = System.Windows.Forms.DockStyle.Fill;
      this.controllersDirSelector.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("controllersDirSelector.History")));
      this.controllersDirSelector.Location = new System.Drawing.Point(157, 90);
      this.controllersDirSelector.Name = "controllersDirSelector";
      this.controllersDirSelector.SelectedPath = "C:\\Users\\mlloyd\\Desktop";
      this.controllersDirSelector.Size = new System.Drawing.Size(767, 36);
      this.controllersDirSelector.TabIndex = 5;
      // 
      // messageLabel
      // 
      this.messageLabel.AutoSize = true;
      this.messageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.messageLabel.Location = new System.Drawing.Point(3, 169);
      this.messageLabel.Name = "messageLabel";
      this.messageLabel.Size = new System.Drawing.Size(148, 293);
      this.messageLabel.TabIndex = 7;
      this.messageLabel.Text = "Messages";
      this.messageLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // rtbLog
      // 
      this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtbLog.Location = new System.Drawing.Point(157, 172);
      this.rtbLog.Name = "rtbLog";
      this.rtbLog.Size = new System.Drawing.Size(767, 287);
      this.rtbLog.TabIndex = 8;
      this.rtbLog.Text = "";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 2;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.62125F));
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.37875F));
      this.tableLayoutPanel3.Controls.Add(this.label5, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.label6, 0, 1);
      this.tableLayoutPanel3.Controls.Add(this.controllerTypeSelector, 1, 1);
      this.tableLayoutPanel3.Controls.Add(this.assemblySelector, 1, 0);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 38);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 2;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46.34146F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53.65854F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel3.Size = new System.Drawing.Size(927, 82);
      this.tableLayoutPanel3.TabIndex = 2;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label5.Location = new System.Drawing.Point(3, 0);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(148, 37);
      this.label5.TabIndex = 0;
      this.label5.Text = "ApiController Assembly";
      this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label6.Location = new System.Drawing.Point(3, 37);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(148, 45);
      this.label6.TabIndex = 2;
      this.label6.Text = "ApiController Type";
      this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // controllerTypeSelector
      // 
      this.controllerTypeSelector.Dock = System.Windows.Forms.DockStyle.Fill;
      this.controllerTypeSelector.Location = new System.Drawing.Point(157, 40);
      this.controllerTypeSelector.Name = "controllerTypeSelector";
      this.controllerTypeSelector.SelectedType = null;
      this.controllerTypeSelector.Size = new System.Drawing.Size(767, 39);
      this.controllerTypeSelector.SourceAssembly = null;
      this.controllerTypeSelector.TabIndex = 3;
      // 
      // assemblySelector
      // 
      this.assemblySelector.DialogFilter = "All Files | *.*";
      this.assemblySelector.Dock = System.Windows.Forms.DockStyle.Fill;
      this.assemblySelector.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("assemblySelector.History")));
      this.assemblySelector.Location = new System.Drawing.Point(157, 3);
      this.assemblySelector.Name = "assemblySelector";
      this.assemblySelector.SelectedFile = "";
      this.assemblySelector.Size = new System.Drawing.Size(767, 31);
      this.assemblySelector.TabIndex = 4;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
      this.label7.Location = new System.Drawing.Point(3, 0);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(927, 35);
      this.label7.TabIndex = 3;
      this.label7.Text = "Input";
      this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.sourcePage);
      this.tabControl1.Controls.Add(this.resourcePage);
      this.tabControl1.Controls.Add(this.viewPage);
      this.tabControl1.Controls.Add(this.controllerPage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 24);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(947, 672);
      this.tabControl1.TabIndex = 1;
      this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
      // 
      // logFusionResolutionEventsToolStripMenuItem
      // 
      this.logFusionResolutionEventsToolStripMenuItem.CheckOnClick = true;
      this.logFusionResolutionEventsToolStripMenuItem.Name = "logFusionResolutionEventsToolStripMenuItem";
      this.logFusionResolutionEventsToolStripMenuItem.Size = new System.Drawing.Size(247, 22);
      this.logFusionResolutionEventsToolStripMenuItem.Text = "Log Fusion Resolution Events";
      this.logFusionResolutionEventsToolStripMenuItem.Click += new System.EventHandler(this.logFusionResolutionEventsToolStripMenuItem_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(947, 696);
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "Form1";
      this.Text = "NORD ng-generator";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.controllerPage.ResumeLayout(false);
      this.viewPage.ResumeLayout(false);
      this.resourcePage.ResumeLayout(false);
      this.sourcePage.ResumeLayout(false);
      this.masterTableLayoutPanel.ResumeLayout(false);
      this.masterTableLayoutPanel.PerformLayout();
      this.outputControlGroupTabPanel.ResumeLayout(false);
      this.outputControlGroupTabPanel.PerformLayout();
      this.tableLayoutPanel3.ResumeLayout(false);
      this.tableLayoutPanel3.PerformLayout();
      this.tabControl1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem selectedToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    private System.Windows.Forms.ToolStripMenuItem resourceOnlyToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
    private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
    private System.Windows.Forms.ToolStripMenuItem saveResourceOnlyToolStripMenuItem;
    private System.Windows.Forms.TabPage controllerPage;
    private System.Windows.Forms.RichTextBox contorllerRTB;
    private System.Windows.Forms.TabPage viewPage;
    private System.Windows.Forms.RichTextBox viewRTB;
    private System.Windows.Forms.TabPage resourcePage;
    private System.Windows.Forms.RichTextBox resourceRTB;
    private System.Windows.Forms.TabPage sourcePage;
    private System.Windows.Forms.TableLayoutPanel masterTableLayoutPanel;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TableLayoutPanel outputControlGroupTabPanel;
    private System.Windows.Forms.Label label1;
    private DirectorySelector resourceDirSelector;
    private System.Windows.Forms.Label label2;
    private DirectorySelector viewDirSelector;
    private FileSelector vsProjectFileSelector;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label3;
    private DirectorySelector controllersDirSelector;
    private System.Windows.Forms.Label messageLabel;
    private System.Windows.Forms.RichTextBox rtbLog;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private TypeSelector controllerTypeSelector;
    private FileSelector assemblySelector;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem useCustomOutputMappingToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem autoVSIntegration;
    private System.Windows.Forms.ToolStripMenuItem logFusionResolutionEventsToolStripMenuItem;
  }
}

