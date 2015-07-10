namespace Nord.Nganga.WinControls
{
  partial class FileSelector
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.button1 = new System.Windows.Forms.Button();
      this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
      this.selectedFile = new Nord.Nganga.WinControls.TextBoxWithHistory();
      this.tableLayoutPanel4.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.button1.Location = new System.Drawing.Point(559, 3);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 21);
      this.button1.TabIndex = 27;
      this.button1.Text = "Browse";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.browse_Click);
      // 
      // tableLayoutPanel4
      // 
      this.tableLayoutPanel4.ColumnCount = 2;
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel4.Controls.Add(this.button1, 1, 0);
      this.tableLayoutPanel4.Controls.Add(this.selectedFile, 0, 0);
      this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel4.Name = "tableLayoutPanel4";
      this.tableLayoutPanel4.RowCount = 1;
      this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel4.Size = new System.Drawing.Size(637, 27);
      this.tableLayoutPanel4.TabIndex = 30;
      // 
      // selectedPath
      // 
      this.selectedFile.Dock = System.Windows.Forms.DockStyle.Fill;
      this.selectedFile.Location = new System.Drawing.Point(3, 3);
      this.selectedFile.Name = "selectedFile";
      this.selectedFile.Size = new System.Drawing.Size(550, 21);
      this.selectedFile.TabIndex = 28;
      // 
      // FileSelector
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel4);
      this.Name = "FileSelector";
      this.Size = new System.Drawing.Size(637, 27);
      this.tableLayoutPanel4.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
    private TextBoxWithHistory selectedFile;
  }
}
