using System;
using System.Collections.Generic;
using System.IO;

namespace Nord.Nganga.Fs.Coordination
{
  [Serializable]
  public class CoordinationResult
  {
    public string SourceAssemblyLocation { get; set; }

    public string ControllerTypeName { get; set; }

    public string ControllerBody { get; set; }
    public string ControllerPath { get; set; }

    public string ViewBody { get; set; }
    public string ViewPath { get; set; }

    public string ResourceBody { get; set; }
    public string ResourcePath { get; set; }

    public string NgControllersPath { get; set; }
    public string NgViewsPath { get; set; }
    public string NgResourcesPath { get; set; }
    public string VsProjectName { get; set; }
    public string VsProjectPath { get; set; }

    public string VsProjectFileName
    {
      get
      {
        var path = this.VsProjectPath ?? string.Empty;
        var name = this.VsProjectName ?? string.Empty;
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(path))
        {
          return Path.Combine(path, name);
        }
        return name;
      }
    }
  }
}