using System.Diagnostics;
using System.Windows.Forms;

namespace Nord.Nganga.WinApp
{
  public static class FormExtensions
  {
    public static void SetId(this Form form, string text)
    {
      var stackTrace = new StackTrace();
      var m = stackTrace.GetFrame(1).GetMethod();
      var t = m.DeclaringType;
      if (t == null) return;
      var a = t.Assembly;
      var n = a.GetName();
      form.Text = $"{n.Name} - [{n.Version}] - {text}";
    }
  }
}