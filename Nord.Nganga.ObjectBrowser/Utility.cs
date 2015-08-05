using System.Reflection;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for Utility.
  /// </summary>
  class Utility
  {
    public Utility ()
    {
      //
      // TODO: Add constructor logic here
      //
    }
    private static void DumpObject (object o)
    {
      var t = o.GetType();

      System.Diagnostics.Debug.WriteLine("Dumping instance of " + t.FullName);
      System.Diagnostics.Debug.WriteLine("Value is " + o.ToString());
      System.Diagnostics.Debug.WriteLine("Properties are:");
      foreach (var pi in t.GetProperties())
      {
        try
        {
          System.Diagnostics.Debug.WriteLine("    " + pi.Name + " = " + pi.GetValue(o, null).ToString());
        }
        catch (System.Exception ex)
        {
          System.Diagnostics.Debug.WriteLine("    " + pi.Name + " : GetValueException = " + ex.ToString());
        }
      }
    }
  }
}
