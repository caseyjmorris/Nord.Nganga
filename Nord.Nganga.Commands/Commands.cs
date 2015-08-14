using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Fs.Coordination;
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

    private static string StripControllerName(string name)
    {
      return Regex.Replace(name, $"{Controller}$", string.Empty).ToUpperInvariant();
    }

    private static IEnumerable<Type> GetSettingsTypes()
    {
      var asm = Assembly.GetAssembly(typeof(IConfigurationPackage));

      return asm.GetTypes().Where(t => typeof(IConfigurationPackage).IsAssignableFrom(t)).ToList();
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

      var method = typeof(ConfigurationFactory).GetMethod("GetConfiguration").MakeGenericMethod(type);

      return (IConfigurationPackage) method.Invoke(null, new object[0]);
    }

    public static void SetOptions(IConfigurationPackage pkg)
    {
      var method = typeof(ConfigurationFactory).GetMethod("UpdateSettings").MakeGenericMethod(pkg.GetType());

      method.Invoke(null, new object[] {pkg});
    }

    public static IEnumerable<Type> GetEligibleWebApiControllers(Assembly asm, bool resourceOnly)
    {
      return asm.FindWebApiControllers("ApiController", true, true, assertAngularRouteIdParmAttribute: !resourceOnly);
    }

    public static Type ResolveController(Assembly asm, string controllerName, bool resourceOnly)
    {
      var scrubbed = StripControllerName(controllerName);

      var noNs = !scrubbed.Contains(".");

      var ctrl = GetEligibleWebApiControllers(asm, resourceOnly);

      var matching = (noNs
        ? ctrl.Where(c => StripControllerName(c.Name).StartsWith(scrubbed))
        : ctrl.Where(c => StripControllerName(c.FullName).EndsWith(scrubbed)))
        .ToList();

      if (matching.Count > 1)
      {
        throw new InvalidOperationException(
          $"Ambiguous match:  {controllerName} could refer to the following controllers in {asm.FullName}:  {string.Join(", ", matching.Select(c => c.FullName))}");
      }
      if (matching.Count == 0)
      {
        throw new KeyNotFoundException(
          $"No eligible controller matching {controllerName} could be found in {asm.FullName}");
      }

      return matching.Single();
    }

    public static CoordinationResult GenerateCode(Type controller, string vsProjectPath)
    {
      var coordinator = new GenerationCoordinator(ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>(),
        ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>());

      return coordinator.CoordinateUiGeneration(controller, vsProjectPath);
    }

    public static CoordinationResult GenerateResource(Type controller, string vsProjectPath)
    {
      var coordinator = new GenerationCoordinator(ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>(),
        ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>());

      return coordinator.CoordinateResourceGeneration(controller, vsProjectPath);
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
  }
}