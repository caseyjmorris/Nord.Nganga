using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using Antlr4.StringTemplate;
using Nord.Nganga.Annotations;
using Nord.Nganga.Annotations.Attributes;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.StEngine
{
  public static class TemplateFactory
  {
    public static void AssertTemplateMinimumVersions(
      SystemPathSettingsPackage settings,
      Type controllerType,
      TemplateContext context)
    {
      var assy = controllerType.Assembly;
      var mva = assy
        .GetCustomAttributes(typeof(MinimumTemplateVersionAttribute), false)
        .Select(a => (MinimumTemplateVersionAttribute) a)
        .FirstOrDefault(a => a.TemplateContext == context);

      if (mva == null)
      {
        return;
      }
      AssertTemplateMinimumVersion(settings, mva);
    }

    private static void AssertTemplateMinimumVersion(
      SystemPathSettingsPackage settings,
      MinimumTemplateVersionAttribute mtva)
    {
      var versionTemplate = GetTemplate(settings, mtva.TemplateContext, name: "templateVersion");
      if (versionTemplate == null)
      {
        throw new InvalidOperationException($"The {mtva.TemplateContext} template does not declare a templateVersion!");
      }
      var version = versionTemplate.Render();
      var cr = CompareVersionStrings(version, mtva.MinimumVersion);
      if (cr >= 1)
      {
        throw new ApplicationException(
          $"The current nganga {mtva.TemplateContext} template version {version} is less than the assembly specified minimum {mtva.MinimumVersion}!");
      }
    }

    public static int CompareVersionStrings(string one, string two)
    {
      var a1 = one.Split('.').Select(t => t.Trim()).ToArray();
      var a2 = two.Split('.').Select(t => t.Trim()).ToArray();
      if (a1.Length != a2.Length)
      {
        throw new InvalidOperationException(
          $"Cannot compare version strings {one} and {two} - length/node count mismatch!");
      }

      for (var i = 0; i < a1.Length; i++)
      {
        var i1 = int.Parse(a1[i]);
        var i2 = int.Parse(a2[i]);
        var d = i2 - i1;
        if (d == 0)
        {
          continue;
        }
        return d;
      }

      return 0; // total match 
    }

    public static Template GetTemplate(
      SystemPathSettingsPackage settings,
      TemplateContext templateContext,
      string name = "body",
      bool debug = false)
    {
      var filename = Path.Combine(settings.TemplatesDirectory, templateContext.ToString() + ".stg");
      if (debug)
      {
        Console.WriteLine(filename + ":");
        Console.WriteLine(File.ReadAllText(filename));
        Console.WriteLine("---");
      }
      TemplateGroup group = new TemplateGroupFile(filename);
      var template = group.GetInstanceOf(name);
      return template;
    }
  }
}