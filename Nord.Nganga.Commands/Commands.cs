using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

    private static string FindCommonPrefix(IEnumerable<string> strings, char splitToken = '.')
    {
      var stringNodeSets = strings.Select(s => s.Split(splitToken)).ToList();

      var minNodeCount = (from string[] nodes in stringNodeSets
        let l = nodes.Length
        select l).Min();

      var prefixNodes = new Dictionary<int, string>();
      for (var i = 0; i < minNodeCount; i++)
      {
        var nodes = GetNodes(stringNodeSets, i);
        if (nodes.Distinct().Count() == 1)
        {
          prefixNodes[prefixNodes.Count] = nodes.First();
        }
      }

      var result = string.Join(splitToken.ToString(), prefixNodes.OrderBy(n => n.Key).Select(n => n.Value).ToList());
      return result;
    }

    private static IEnumerable<string> RemoveCommonPrefixNodes(IEnumerable<string> strings, char splitToken = '.')
    {
      var input = strings.ToList();
      var commonPrefix = FindCommonPrefix(input, splitToken) + splitToken;
      return input.Select(s => s.Replace(commonPrefix, string.Empty)).ToList();
    }

    private static List<string> GetNodes(IEnumerable<string[]> stringNodeSets, int index)
    {
      return
        (from stringNodeSet in stringNodeSets where index <= stringNodeSet.Length select stringNodeSet[index]).ToList();
    }

    private static string InsensitiveSettingsName(string name)
    {
      return name.ToUpperInvariant().Replace(SettingsPackage.ToUpperInvariant(), string.Empty);
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

    public static ListControllerNamesResult ListControllerNames(
      string assyFileName,
      string filter = null,
      bool resourceOnly = false)
    {
      if (!File.Exists(assyFileName))
      {
        return new ListControllerNamesResult
        {
          MessageText = $"The specified assembly file name {assyFileName} does not exist."
        };
      }

      var logs = new List<string>();
      var result = new ListControllerNamesResult();
      try
      {
        var typenames = CoordinationExecutor.GetControllerList(assyFileName, logs, resourceOnly)
          .Where(n => filter == null || n.ToLowerInvariant().Contains(filter.ToLowerInvariant()));

        result.TypeNames = RemoveCommonPrefixNodes(typenames);
      }
      catch (Exception e)
      {
        result.MessageText = $"{e}{Environment.NewLine}";
      }

      result.MessageText =
        $"{result.MessageText}{(logs.Any() ? string.Join(Environment.NewLine, logs) : string.Empty)}";

      return result;
    }

    public static GenerateResult Generate(
      string assyFileName,
      string controllerName,
      string vsProjectPath,
      bool resourceOnly = false)
    {
      if (!File.Exists(assyFileName))
      {
        return new GenerateResult
        {
          MessageText = $"The specified assembly file name {assyFileName} does not exist."
        };
      }

      if (!Directory.Exists(vsProjectPath))
      {
        return new GenerateResult
        {
          MessageText = $"The specified project path {vsProjectPath} does not exist."
        };
      }

      var logs = new List<string>();

      var result = new GenerateResult();
      try
      {
        result.CoordinationResult = CoordinationExecutor.Coordinate(assyFileName, controllerName, vsProjectPath, logs,
          resourceOnly);
      }
      catch (Exception e)
      {
        result.MessageText = $"{e}{Environment.NewLine}";
      }

      result.MessageText =
        $"{result.MessageText}{(logs.Any() ? string.Join(Environment.NewLine, logs) : string.Empty)}";

      return result;
    }
  }
}