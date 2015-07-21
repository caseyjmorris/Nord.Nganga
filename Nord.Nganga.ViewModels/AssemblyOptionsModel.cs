using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nord.Nganga.Annotations;
using Nord.Nganga.Annotations.Attributes;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.Core.Reflection;

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

    public AssemblyOptionsModel()
    {
      this.textCaseOptions = new Dictionary<CasingOptionContext, CasingOption>
      {
          {CasingOptionContext.Default,CasingOption.Sentence}
      };
    }

    public AssemblyOptionsModel(Type type):this()
    {
      var assy = type.Assembly;
      this.InitFromAssy(assy);
    }

    public AssemblyOptionsModel(Assembly assy)
      : this()
    {
      this.InitFromAssy(assy);
    }

    private void InitFromAssy(Assembly assy)
    {
      var tcpaList = DependentTypeResolver.GetCustomAssemblyAttributes<TextCasePreferencesAttribute>(assy);
      
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
