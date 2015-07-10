using System.IO;
using System.Linq;
namespace Nord.Nganga.WinApp
{
  public static class DirectoryExtensions
  {
    public static void ClearDirectory(this DirectoryInfo di)
    {
      // todo   add a where before the foreach to filter out system files that are likely to throw on an attempt to delete them....
      di.GetFiles().ToList().ForEach(fi => fi.Delete());
    }
  }
}
