using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs.VsIntegration
{
  public static class VsIntegrator
  {
    private static readonly SourceParser SourceParser = new SourceParser();

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

    private static void Save(
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
          TemplateFactory.Context.View,
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
          TemplateFactory.Context.Controller,
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
            TemplateFactory.Context.Resource,
            logHandler,
            forceOverwrite);
      }
    }

    private static void SaveFile(
      string vsProjectPath,
      Dictionary<string, string> integrationDictionary,
      Func<string> rootProvider,
      Func<string> nameProvider,
      Func<string> dataProvider,
      TemplateFactory.Context context,
      StringFormatProviderVisitor logHandler,
      bool forceOverwrite
      )
    {
      var relativeName = Path.Combine(rootProvider(), nameProvider());
      var targetFileName = Path.Combine(vsProjectPath, relativeName);
      CreatePathTree(targetFileName);
      if (File.Exists(targetFileName))
      {
        var source = File.ReadAllText(targetFileName);
        var parseResult = SourceParser.ParseFile(context, source);

        if ((!parseResult.Success ||
             (parseResult.Success && parseResult.CalculatedBodyMd5 != parseResult.DeclaredHeaderMd5)) && !forceOverwrite)
        {
          var msgPreamble = $"WARNING: {Environment.NewLine}" + (
            parseResult.Success
              ? $"  The declared {parseResult.DeclaredHeaderMd5} and calculated {parseResult.CalculatedBodyMd5} MD5 expressions do not match for the existing file: {targetFileName}."
              : $"  Parse of the existing file: {targetFileName} failed.  It is uncertain if this file was previously generated by Nganga."
            ) + $"{Environment.NewLine}";

          var msgClose = $"  Force was not specified.{Environment.NewLine}" +
                         $"  The file was not overwritten and the generated results were not saved.{Environment.NewLine}" +
                         $"  Use the force option to overwrite this file.{Environment.NewLine}";

          var msg = $"{msgPreamble}{Environment.NewLine}{msgClose}";

          logHandler(msg);
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