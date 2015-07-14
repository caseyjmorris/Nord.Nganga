namespace Nord.Nganga.ProjectTemplate.Wizard
{
  partial class UserInputForm
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
      this.label1 = new System.Windows.Forms.Label();
      this.ngAppName = new System.Windows.Forms.TextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.webApiRoot = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(24, 51);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(110, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Ng Application Name:";
      // 
      // ngAppName
      // 
      this.ngAppName.Location = new System.Drawing.Point(140, 48);
      this.ngAppName.Name = "ngAppName";
      this.ngAppName.Size = new System.Drawing.Size(100, 20);
      this.ngAppName.TabIndex = 1;
      this.ngAppName.Tag = "$ngAppName$";
      this.ngAppName.TextChanged += new System.EventHandler(this.ngAppName_TextChanged);
      // 
      // button1
      // 
      this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button1.Location = new System.Drawing.Point(372, 202);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 2;
      this.button1.Text = "OK";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // webApiRoot
      // 
      this.webApiRoot.Location = new System.Drawing.Point(140, 86);
      this.webApiRoot.Name = "webApiRoot";
      this.webApiRoot.Size = new System.Drawing.Size(100, 20);
      this.webApiRoot.TabIndex = 4;
      this.webApiRoot.Tag = "$webAPIRoot$";
      this.webApiRoot.TextChanged += new System.EventHandler(this.webApiRoot_TextChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(24, 89);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(79, 13);
      this.label2.TabIndex = 3;
      this.label2.Text = "Web API Root:";
      // 
      // UserInputForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(493, 267);
      this.Controls.Add(this.webApiRoot);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.ngAppName);
      this.Controls.Add(this.label1);
      this.Name = "UserInputForm";
      this.Text = "Nganga - New Client Project Wizard";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox ngAppName;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox webApiRoot;
    private System.Windows.Forms.Label label2;
  }
}