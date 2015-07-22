using System;
using System.Collections.Generic;
using System.IO;

namespace Nord.Nganga.Fs.Coordination
{
  [Serializable]
  public class CoordinationResult
  {
    public CoordinationResult()
    {
      this.Exceptions = new List<Exception>();
    }
    public List<Exception> Exceptions { get; private set; }

    public string SourceAssemblyLocation { get;set; }
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
      get { return Path.Combine(this.VsProjectPath, this.VsProjectName); }
    }

  }
}