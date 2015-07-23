namespace Nord.Nganga.WinApp
{
  partial class NgangaLog
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
      this.rtbLog = new System.Windows.Forms.RichTextBox();
      this.fontSelector1 = new Nord.Nganga.WinControls.FontSelector();
      this.SuspendLayout();
      // 
      // rtbLog
      // 
      this.rtbLog.ContextMenuStrip = this.fontSelector1;
      this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtbLog.Location = new System.Drawing.Point(0, 0);
      this.rtbLog.Name = "rtbLog";
      this.rtbLog.Size = new System.Drawing.Size(737, 538);
      this.rtbLog.TabIndex = 0;
      this.rtbLog.Text = "";
      this.rtbLog.TextChanged += new System.EventHandler(this.rtbLog_TextChanged);
      // 
      // fontSelector1
      // 
      this.fontSelector1.Name = "FontSelector";
      this.fontSelector1.Size = new System.Drawing.Size(61, 4);
      // 
      // NgangaLog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(737, 538);
      this.Controls.Add(this.rtbLog);
      this.Name = "NgangaLog";
      this.Text = "NgangaLog";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NgangaLog_FormClosing);
      this.Load += new System.EventHandler(this.NgangaLog_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.RichTextBox rtbLog;
    private WinControls.FontSelector fontSelector1;
  }
}