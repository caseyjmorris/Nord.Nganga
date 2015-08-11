using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.Fs.VsIntegration
{
  public static class VsIntegrator
  {
    public static bool Save(
      IEnumerable<CoordinationResult> coordinationResults,
      bool integrate,
      StringFormatProviderVisitor logHandler,
      bool forceOverwrite)
    {
      var list = coordinationResults.ToList();

      var integrationDictionary = new Dictionary<string, string>();

      foreach (var cr in list)
      {
        Save(cr, integrationDictionary, logHandler, forceOverwrite);
      }

      return !integrate || Integrate(list.First().VsProjectFileName, integrationDictionary, logHandler);
    }

    public static bool Save(
      CoordinationResult coordinationResult,
      bool integrate,
      StringFormatProviderVisitor logHandler,
      bool forceOverwrite)
    {
      if (!AssertArgumentQuality(coordinationResult, logHandler)) return false;

      var integrationDictionary = new Dictionary<string, string>();

      Save(coordinationResult, integrationDictionary, logHandler, forceOverwrite);

      return !integrate || Integrate(coordinationResult.VsProjectFileName, integrationDictionary, logHandler);
    }

    private static bool Integrate(
      string vsProjectFileName,
      Dictionary<string, string> integrationDictionary,
      StringFormatProviderVisitor logHandler)
    {
      logHandler("VS integration target is {0}", vsProjectFileName);

      try
      {
        logHandler("Integration starting.");

        var csProjEditor = new CsProjEditor();

        csProjEditor.AddFileToCsProj(
          vsProjectFileName, integrationDictionary.Values.ToList(),
          p => logHandler("{0}{1}", '\t', p));

        logHandler("Integration complete.");
      }
      catch (Exception ie)
      {
        logHandler("VS Integration failed due to {0}", ie.Message);
        logHandler($"{ie}");
        return false;
      }
      return true;
    }

    private static bool AssertArgumentQuality(CoordinationResult coordinationResult,
      StringFormatProviderVisitor logHandler)
    {
      if (logHandler == null)
      {
        throw new ArgumentNullException(nameof(logHandler));
      }

      if (string.IsNullOrEmpty(coordinationResult.VsProjectPath) ||
          string.IsNullOrEmpty(coordinationResult.VsProjectName) ||
          !File.Exists(coordinationResult.VsProjectFileName))
      {
        logHandler("Cannot find " + coordinationResult.VsProjectFileName +
                   ".  Are you sure the specified path is correct?");
        return false;
      }
      return true;
    }

    public static void Save(
      CoordinationResult coordinationResult,
      Dictionary<string, string> integrationDictionary,
      StringFormatProviderVisitor logHandler,
      bool forceOverwrite)
    {
      if (!string.IsNullOrEmpty(coordinationResult.ViewBody))
      {
        SaveFile(
          coordinationResult.VsProjectPath,
          integrationDictionary,
          () => coordinationResult.NgViewsPath,
          () => coordinationResult.ViewPath,
          () => coordinationResult.ViewBody,
          () => coordinationResult.ViewChangesWillBeLostMarker,
          logHandler,
          forceOverwrite);
      }

      if (!string.IsNullOrEmpty(coordinationResult.ControllerBody))
      {
        SaveFile(
          coordinationResult.VsProjectPath,
          integrationDictionary,
          () => coordinationResult.NgControllersPath,
          () => coordinationResult.ControllerPath,
          () => coordinationResult.ControllerBody,
          () => coordinationResult.ControllerChangesWillBeLostMarker,
          logHandler,
          forceOverwrite);
      }
      if (!string.IsNullOrEmpty(coordinationResult.ResourceBody))
      {
        SaveFile
          (coordinationResult.VsProjectPath,
            integrationDictionary,
            () => coordinationResult.NgResourcesPath,
            () => coordinationResult.ResourcePath,
            () => coordinationResult.ResourceBody,
            () => coordinationResult.ResourceChangesWillBeLostMarker,
            logHandler,
            forceOverwrite);
      }
    }

    public static void SaveFile(
      string vsProjectPath,
      Dictionary<string, string> integrationDictionary,
      Func<string> rootProvider,
      Func<string> nameProvider,
      Func<string> dataProvider,
      Func<string> markerProvider,
      StringFormatProviderVisitor logHandler,
      bool forceOverwrite
      )
    {
      var relativeName = Path.Combine(rootProvider(), nameProvider());
      var targetFileName = Path.Combine(vsProjectPath, relativeName);
      CreatePathTree(targetFileName);
      if (File.Exists(targetFileName))
      {
        var existingData = File.ReadAllText(targetFileName);
        var markerFound = existingData.Contains(markerProvider());
        if (!markerFound && !forceOverwrite)
        {
          logHandler(
            $"WARNING: {Environment.NewLine}" +
            $"  The lost changes marker was not found in the existing file: {targetFileName}.{Environment.NewLine}" +
            $"  Force was not specified.{Environment.NewLine}" +
            $"  The file was not overwritten and the generated results were not saved.{Environment.NewLine}" +
            $"  Use the force option to overwrite this file.{Environment.NewLine}");
          return;
        }
      }
      var data = dataProvider();
      File.WriteAllText(targetFileName, data);
      logHandler("{0} written to disk.", targetFileName);
      integrationDictionary[targetFileName] = relativeName;
    }

    private static void CreatePathTree(string path)
    {
      var dir = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(dir)) return;
      Directory.CreateDirectory(dir);
    }
  }
}