using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Nord.Nganga.Fs;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Fs.VsIntegration;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.Commands
{
  /// <summary>
  /// These are commands intended to be easy to invoke from PowerShell.
  /// </summary>
  public static class Commands
  {
    private const string SettingsPackage = "SettingsPackage";

    private const string Controller = "Controller";

    private static string InsensitiveSettingsName(string name)
    {
      return name.ToUpperInvariant().Replace(SettingsPackage.ToUpperInvariant(), string.Empty);
    }

    private static IEnumerable<Type> GetSettingsTypes()
    {
      var asm = Assembly.GetAssembly(typeof (IConfigurationPackage));

      return asm.GetTypes().Where(t => typeof (IConfigurationPackage).IsAssignableFrom(t)).ToList();
    }

    public static IEnumerable<string> ListOptionTypes()
    {
      return GetSettingsTypes().Select(t => t.Name.Replace(SettingsPackage, string.Empty)).ToList();
    }

    public static IConfigurationPackage GetOptions(string name)
    {
      var nameInsens = InsensitiveSettingsName(name);

      var type = GetSettingsTypes().SingleOrDefault(st => InsensitiveSettingsName(st.Name) == nameInsens);

      if (type == null)
      {
        throw new KeyNotFoundException($"Setting type {name} not recognized.");
      }

      var method = typeof (ConfigurationFactory).GetMethod("GetConfiguration").MakeGenericMethod(type);

      return (IConfigurationPackage) method.Invoke(null, new object[0]);
    }

    public static void SetOptions(IConfigurationPackage pkg)
    {
      var method = typeof (ConfigurationFactory).GetMethod("UpdateSettings").MakeGenericMethod(pkg.GetType());

      method.Invoke(null, new object[] {pkg});
    }

    public static IEnumerable<string> GetEligibleWebApiControllers(string assemblyFileLocation, bool resourceOnly,
      bool verbose)
    {
      return verbose
        ? CoordinationExecutor.GetControllerList(assemblyFileLocation, resourceOnly, Console.Write)
        : CoordinationExecutor.GetControllerList(assemblyFileLocation, resourceOnly, (provider, parms) => { });
    }

    public static CoordinationResult GenerateCode(string assemblyLocation, string controllerName, string vsProjectPath,
      bool verbose)
    {
      return verbose
        ? CoordinationExecutor.Coordinate(assemblyLocation, controllerName, vsProjectPath, resourceOnly: false,
          logHandler: Console.Write)
        : CoordinationExecutor.Coordinate(assemblyLocation, controllerName, vsProjectPath, resourceOnly: false,
          logHandler: (x, y) => { });
    }

    public static CoordinationResult GenerateResource(string assemblyLocation, string controllerName,
      string vsProjectPath, bool verbose)
    {
      return verbose
        ? CoordinationExecutor.Coordinate(assemblyLocation, controllerName, vsProjectPath, resourceOnly: false,
          logHandler: Console.Write)
        : CoordinationExecutor.Coordinate(assemblyLocation, controllerName, vsProjectPath, resourceOnly: false,
          logHandler: (x, y) => { });
    }

    public static bool WriteUiGenerationResult(CoordinationResult coordinationResult, bool force)
    {
      var dict = new Dictionary<string, string>
      {
        {coordinationResult.ControllerPath, coordinationResult.ControllerBody},
        {coordinationResult.ResourcePath, coordinationResult.ResourceBody},
        {coordinationResult.ViewPath, coordinationResult.ViewBody}
      };

      var noSkips = true;

      foreach (var member in dict)
      {
        if (!force && File.Exists(member.Key))
        {
          noSkips = false;
          continue;
        }
        File.WriteAllText(member.Key, member.Value);
      }

      return noSkips;
    }

    public static bool WriteResourceGenerationResult(CoordinationResult coordinationResult, bool force)
    {
      if (!force && File.Exists(coordinationResult.ResourcePath))
      {
        return false;
      }
      File.WriteAllText(coordinationResult.ResourcePath, coordinationResult.ResourceBody);

      return true;
    }

    public static void EditCsProj(string csprojPath, params string[] newFiles)
    {
      var csprojEditor = new CsProjEditor();

      csprojEditor.AddFileToCsProj(csprojPath, newFiles, x => { });
    }
  }
}