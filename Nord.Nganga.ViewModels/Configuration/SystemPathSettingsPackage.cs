using System;
using System.IO;

namespace Nord.Nganga.Models.Configuration
{
  /// <summary>
  /// Controls locations of Nganga files
  /// </summary>
  public class SystemPathSettingsPackage : IConfigurationPackage
  {
    /// <summary>
    /// Location of Nganga templates
    /// </summary>
    public string TemplatesDirectory { get; set; }

    public void SetPropertiesToDefault()
    {
      this.TemplatesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Github",
        @"Nord.Nganga\Nord.Nganga.StEngine\templates");
    }
  }
}