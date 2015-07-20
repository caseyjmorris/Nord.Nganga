namespace Nord.Nganga.WinApp
{
  partial class NgangaMain
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NgangaMain));
      this.rtbLog = new System.Windows.Forms.RichTextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.typeSelector1 = new Nord.Nganga.WinControls.TypeSelector();
      this.assemblySelector = new Nord.Nganga.WinControls.FileSelector();
      this.SuspendLayout();
      // 
      // rtbLog
      // 
      this.rtbLog.Location = new System.Drawing.Point(27, 123);
      this.rtbLog.Name = "rtbLog";
      this.rtbLog.Size = new System.Drawing.Size(554, 287);
      this.rtbLog.TabIndex = 9;
      this.rtbLog.Text = "";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(588, 69);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(166, 23);
      this.button1.TabIndex = 10;
      this.button1.Text = "Generate Selected Type";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // typeSelector1
      // 
      this.typeSelector1.BaseApiControllerName = null;
      this.typeSelector1.Location = new System.Drawing.Point(27, 69);
      this.typeSelector1.Name = "typeSelector1";
      this.typeSelector1.SelectedType = null;
      this.typeSelector1.Size = new System.Drawing.Size(554, 25);
      this.typeSelector1.SourceAssembly = null;
      this.typeSelector1.TabIndex = 1;
      this.typeSelector1.Load += new System.EventHandler(this.typeSelector1_Load);
      // 
      // assemblySelector
      // 
      this.assemblySelector.DialogFilter = "All Files | *.*";
      this.assemblySelector.History = ((System.Collections.Specialized.StringCollection)(resources.GetObject("assemblySelector.History")));
      this.assemblySelector.Location = new System.Drawing.Point(27, 26);
      this.assemblySelector.Name = "assemblySelector";
      this.assemblySelector.SelectedFile = "";
      this.assemblySelector.Size = new System.Drawing.Size(637, 27);
      this.assemblySelector.TabIndex = 0;
      // 
      // NgangaMain
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(780, 528);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.rtbLog);
      this.Controls.Add(this.typeSelector1);
      this.Controls.Add(this.assemblySelector);
      this.Name = "NgangaMain";
      this.Text = "NgangaMain";
      this.ResumeLayout(false);

    }

    #endregion

    private WinControls.FileSelector assemblySelector;
    private WinControls.TypeSelector typeSelector1;
    private System.Windows.Forms.RichTextBox rtbLog;
    private System.Windows.Forms.Button button1;
  }
}