﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nord.Nganga.Annotations;
using Nord.Nganga.Annotations.Attributes;
using Nord.Nganga.Annotations.Attributes.Html;

namespace Nord.Nganga.Models
{
  public class AssemblyOptionsModel
  {
    private readonly Dictionary<CasingOptionContext, CasingOption> textCaseOptions;

    public string NgControllersPath { get; set; }
    public string NgViewsPath { get; set; }
    public string NgResourcesPath { get; set; }
    public string CsProjectPath { get; set; }

    public CasingOption GetOption(CasingOptionContext context)
    {
      return this.textCaseOptions.ContainsKey(context) ? this.textCaseOptions[context] : this.textCaseOptions[CasingOptionContext.Default];
    }

    public AssemblyOptionsModel(Type type)
    {
      var assy = type.Assembly;

      this.textCaseOptions = new Dictionary<CasingOptionContext, CasingOption>
      {
          {CasingOptionContext.Default,CasingOption.Sentence}
      };

      var tcpaList = assy.GetCustomAttributes(typeof(TextCasePreferencesAttribute), true);

      if (tcpaList.Any())
      {
        foreach (TextCasePreferencesAttribute tcpa in tcpaList)
        {
          this.textCaseOptions[tcpa.Context] = tcpa.Option;
        }
      }

      var psa = assy.GetCustomAttributes(typeof(ProjectStructureAttribute), true).Select(p => (ProjectStructureAttribute)p).FirstOrDefault();
      if (psa == null) return;
      this.NgControllersPath = psa.NgControllersPath;
      this.NgViewsPath = psa.NgViewsPath;
      this.NgResourcesPath = psa.NgResourcesPath;
      this.CsProjectPath = psa.CsProjectPath;
    }
  }
}
