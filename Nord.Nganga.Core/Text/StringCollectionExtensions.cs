using System.Collections.Specialized;
using System.Data;

namespace Nord.Nganga.Core.Text
{
  public static class StringCollectionExtensions
  {
    public static DataTable AsDataTable(this StringCollection value, string columnName = "Text")
    {
      var dt = new DataTable("StringCollection");
      var dc = new DataColumn(columnName, typeof(string));
      dt.Columns.Add(dc);
      foreach (var s in value)
      {
        var r = dt.NewRow();
        r[columnName] = s;
        dt.Rows.Add(r);
      }

      return dt;
    }
  }
}