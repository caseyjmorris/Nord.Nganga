using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nord.Nganga.ProjectTemplate.Wizard
{
  public partial class UserInputForm : Form
  {
    private readonly Dictionary<string, string> replacementsDictionary;
    public UserInputForm(Dictionary<string, string> replacementsDictionary)
    {
      this.InitializeComponent();
      this.replacementsDictionary = replacementsDictionary ?? new Dictionary<string, string>();
    }

    private void ngAppName_TextChanged(object sender, EventArgs e)
    {
      this.SetReplacementValue((Control)sender);
    }

    private void SetReplacementValue(Control c)
    {
      if (c.Tag == null)
      {
        throw new ApplicationException(string.Format("The wizard UI control '{0}' has no tag value!", c.Name));
      }
      this.replacementsDictionary[c.Tag.ToString()] = c.Text;
    }

    private void webApiRoot_TextChanged(object sender, EventArgs e)
    {
      this.SetReplacementValue((Control)sender);
    }
  }
}
