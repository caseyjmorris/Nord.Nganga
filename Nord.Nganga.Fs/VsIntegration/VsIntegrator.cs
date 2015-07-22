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
      StringFormatProviderVisitor logHandler)
    {
      var list = coordinationResults.ToList();

      var integrationDictionary = new Dictionary<string, string>();

      foreach (var cr in list)
      {
        Save(cr, integrationDictionary, logHandler);
      }

      return !integrate || Integrate(list.First().VsProjectFileName, integrationDictionary, logHandler);
    }

    public static bool Save(CoordinationResult coordinationResult, bool integrate, StringFormatProviderVisitor logHandler)
    {
      if (!AssertArgumentQuality(coordinationResult, logHandler)) return false;

      var integrationDictionary = new Dictionary<string, string>();

      Save(coordinationResult, integrationDictionary, logHandler);

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
          (p) => logHandler("{0}{1}", '\t', p));

        logHandler("Integration complete.");
      }
      catch (Exception ie)
      {
        logHandler("VS Integration failed due to {0}", ie.Message);
        logHandler(string.Format("{0}", ie));
        return false;
      }
      return true;
    }

    private static bool AssertArgumentQuality(CoordinationResult coordinationResult, StringFormatProviderVisitor logHandler)
    {
      if (logHandler == null)
      {
        throw new ArgumentNullException("logHandler");
      }

      if (string.IsNullOrEmpty(coordinationResult.VsProjectPath) ||
      string.IsNullOrEmpty(coordinationResult.VsProjectName) ||
      !File.Exists(coordinationResult.VsProjectFileName))
      {
        logHandler("Cannot find " + coordinationResult.VsProjectFileName + ".  Are you sure the specified path is correct?");
        return false;
      }
      return true;
    }

    public static void Save(
    CoordinationResult coordinationResult,
    Dictionary<string, string> integrationDictionary,
    StringFormatProviderVisitor logHandler)
    {
      if (!string.IsNullOrEmpty(coordinationResult.ViewBody))
      {
        SaveFile(
          coordinationResult.VsProjectPath,
          integrationDictionary,
          () => coordinationResult.NgViewsPath,
          () => coordinationResult.ViewPath,
          () => coordinationResult.ViewBody,
          logHandler);
      }

      if (!string.IsNullOrEmpty(coordinationResult.ControllerBody))
      {
        SaveFile(
          coordinationResult.VsProjectPath,
          integrationDictionary,
          () => coordinationResult.NgControllersPath,
          () => coordinationResult.ControllerPath,
          () => coordinationResult.ControllerBody,
          logHandler);
      }
      if (!string.IsNullOrEmpty(coordinationResult.ResourceBody))
      {
        SaveFile
        (coordinationResult.VsProjectPath,
        integrationDictionary,
        () => coordinationResult.NgResourcesPath,
        () => coordinationResult.ResourcePath,
        () => coordinationResult.ResourceBody,
        logHandler)
        ;
      }
    }

    public static void SaveFile(
    string vsProjectPath,
    Dictionary<string, string> integrationDictionary,
  Func<string> rootProvider,
  Func<string> nameProvider,
  Func<string> dataProvider,
   StringFormatProviderVisitor logHandler
  )
    {
      var relativeName = Path.Combine(rootProvider(), nameProvider());
      var targetFileName = Path.Combine(vsProjectPath, relativeName);
      CreatePathTree(targetFileName);
      File.WriteAllText(targetFileName, dataProvider());
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
