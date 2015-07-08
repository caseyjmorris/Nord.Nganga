using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nord.Nganga.Models.Configuration
{
  /// <summary>
  /// Retrieve configuration files from the file system or create defaults.
  /// </summary>
  public class ConfigurationFactory
  {
    private static readonly string FileLocation =
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static T GetConfiguration<T>(string filename) where T : IConfigurationPackage
    {
      var path = Path.Combine(FileLocation, filename);

      var text = File.ReadAllText(path);

      throw new NotImplementedException();
    }
  }
}