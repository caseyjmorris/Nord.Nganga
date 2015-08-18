using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EnvDTE;
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

    private static string vsiErr = "VS integration error.";

    public static IEnumerable<string> ListControllerNames(
      string assyFileName,
      string filter = null,
      bool resourceOnly = false,
      bool verbose = false)
    {
      if (!File.Exists(assyFileName))
      {
        throw new Exception(
          $"The specified assembly file name {assyFileName} does not exist.  {vsiErr}!");
      }

      var logs = new List<string>();
      IEnumerable<string> results = null;
      try
      {
        var typenames = CoordinationExecutor.GetControllerList(assyFileName, logs, resourceOnly)
          .Where(n => filter == null || n.ToLowerInvariant().Contains(filter.ToLowerInvariant()));

        results = RemoveCommonPrefixNodes(typenames);
      }
      catch (Exception e)
      {
        logs.Add(e.ToString());
      }

      DumpLogs(logs, verbose);

      return results;
    }

    public static CoordinationResult Generate(
      string assyFileName,
      string controllerName,
      string vsProjectPath,
      bool resourceOnly = false,
      bool verbose = false)
    {
      if (!File.Exists(assyFileName))
      {
        throw new Exception(
          $"The specified assembly file name {assyFileName} does not exist.  {vsiErr}!");
      }

      if (!Directory.Exists(vsProjectPath))
      {
        throw new Exception(
          $"The specified project path {vsProjectPath} does not exist.  {vsiErr}");
      }

      var logs = new List<string>();

      CoordinationResult results = null;
      try
      {
        results = CoordinationExecutor.Coordinate(assyFileName, controllerName, vsProjectPath, logs, resourceOnly);
      }
      catch (Exception e)
      {
        logs.Add(e.ToString());
      }

      DumpLogs(logs, verbose);

      return results;
    }


    public static bool IntegrateResults(
      CoordinationResult coordinationResult,
      bool force = false,
      bool verbose = false,
      DTE dte = null)
    {
      var logs = new List<string>();
      var result = VsIntegrator.Save(
        coordinationResult,
        true,
        (provider, parms) => logs.Add(string.Format(provider, parms)),
        force,
        dte);
      DumpLogs(logs, verbose);


      return result;
    }

    private static void DumpLogs(IEnumerable<string> logs, bool verbose = false)
    {
      if (!verbose) return;
      foreach (var log in logs)
      {
        Console.WriteLine(log);
      }
    }
  }
}