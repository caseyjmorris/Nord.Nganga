﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Nord.Nganga.Fs.Coordination
{
  [Serializable]
  public class CoordinationResult
  {
    public string SourceAssemblyLocation { get; set; }

    public string ControllerTypeName { get; set; }

    public GenerationResult View { get; set; }
    public string ViewFileName => this.View?.AbsoluteFileNameName;
    public string ViewText => this.View?.Text;

    public GenerationResult Resource { get; set; }
    public string ResourceFileName => this.Resource?.AbsoluteFileNameName;
    public string ResourceText => this.Resource?.Text;

    public GenerationResult Controller { get; set; }
    public string ControllerFileName => this.Controller?.AbsoluteFileNameName;
    public string ControllerText => this.Controller?.Text;

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