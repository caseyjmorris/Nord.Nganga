using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nord.Nganga.Models.Configuration
{
  /// <summary>
  /// Retrieve configuration files from the file system or create defaults.
  /// </summary>
  public static class ConfigurationFactory
  {
    private static readonly string FileLocation =
      Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "NORD",
        "Nganga"
        );

    private static readonly HashSet<char> IllegalFileNameCharacters = new HashSet<char>(Path.GetInvalidPathChars());

    private static string GetPath(Type t)
    {
      var typeName = t.Name;

      var noIllegalArr = typeName.ToCharArray().Where(ch => !IllegalFileNameCharacters.Contains(ch)).ToArray();

      var fileName = new string(noIllegalArr) + ".json";

      return Path.Combine(FileLocation, fileName);
    }

    public static void UpdateSettings<T>(T settings) where T : IConfigurationPackage
    {
      Directory.CreateDirectory(FileLocation);

      var path = GetPath(typeof(T));

      var serialized = JsonConvert.SerializeObject(settings, new StringEnumConverter());

      File.WriteAllText(path, serialized);
    }

    public static T GetConfiguration<T>() where T : IConfigurationPackage, new()
    {
      Directory.CreateDirectory(FileLocation);

      var path = GetPath(typeof(T));

      if (File.Exists(path))
      {
        var contents = File.ReadAllText(path);

        try
        {
          return JsonConvert.DeserializeObject<T>(contents);
        }
        catch
        {
          Trace.WriteLine(string.Format("Couldn't read configuration file at {0}... setting defaults", path));
        }
      }

      var defSettings = new T();

      defSettings.SetPropertiesToDefault();

      UpdateSettings(defSettings);

      return defSettings;
    }
  }
}