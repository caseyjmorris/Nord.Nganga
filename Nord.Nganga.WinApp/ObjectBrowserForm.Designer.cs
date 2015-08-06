namespace Nord.Nganga.WinApp
{
  partial class ObjectBrowserForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing)
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
    private void InitializeComponent ()
    {
      this.objectBrowser1 = new Nord.Nganga.WinApp.ObjectBrowser();
      this.SuspendLayout();
      // 
      // objectBrowser1
      // 
      this.objectBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.objectBrowser1.Location = new System.Drawing.Point(0, 0);
      this.objectBrowser1.Name = "objectBrowser1";
      this.objectBrowser1.Size = new System.Drawing.Size(651, 511);
      this.objectBrowser1.TabIndex = 0;
      // 
      // ObjectBrowserForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(651, 511);
      this.Controls.Add(this.objectBrowser1);
      this.Name = "ObjectBrowserForm";
      this.Text = "ObjectBrowserForm";
      this.Load += new System.EventHandler(this.ObjectBrowserForm_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private ObjectBrowser objectBrowser1;
  }
}