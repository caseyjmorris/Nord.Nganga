using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Nord.AngularUiGen.WinControls
{
  public partial class WaitDialog : Form
  {
    private readonly Control _parent;

    public WaitDialog(Control parent, string msg)
    {
      this.InitializeComponent();
      this._parent = parent;
      this.label1.Text = msg;
      var vCtr = parent.Top + (parent.Height / 2);
      var hCtr = parent.Left + (parent.Width / 2);
      this.Top = vCtr - (this.Height / 2);
      this.Left = hCtr - (this.Width / 2);
      var bw = new BackgroundWorker { WorkerSupportsCancellation = true };

      bw.DoWork += this.bw_DoWork;
      bw.RunWorkerAsync();
      this.SetParentState(false);
    }

    void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action<object, DoWorkEventArgs>(this.bw_DoWork), sender, e);
        return;
      }
      this.ShowDialog();
    }

    public void Dismiss()
    {
      if (this.InvokeRequired)
      {
        this.Invoke(new Action(this.Dismiss));
        return;
      }

      this.Close();
    }

    private void SetParentState(bool newState)
    {
      if (this._parent.InvokeRequired)
      {
        this._parent.Invoke(new Action<bool>(this.SetParentState), newState);
        return;
      }
      this._parent.Enabled = newState;
    }

    private void WaitDialog_FormClosed(object sender, FormClosedEventArgs e)
    {
      this.SetParentState(true);
    }
  }
}
