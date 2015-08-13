using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.Commands
{
  /// <summary>
  /// These are commands intended to be easy to invoke from PowerShell.
  /// </summary>
  public static class Commands
  {
    private const string SettingsPackage = "SettingsPackage";

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

    public static IEnumerable<Type> GetEligibleWebApiControllers(Assembly asm, bool resourceOnly)
    {
      return asm.FindWebApiControllers("ApiController", true, true, assertAngularRouteIdParmAttribute: !resourceOnly);
    }
  }
}