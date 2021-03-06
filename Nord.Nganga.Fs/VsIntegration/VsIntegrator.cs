﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using EnvDTE;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.StEngine;

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

    private static bool AssertArgumentQuality(
      CoordinationResult coordinationResult,
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
      SaveFile(
        integrationDictionary,
        coordinationResult.View,
        logHandler,
        forceOverwrite);
      SaveFile(
        integrationDictionary,
        coordinationResult.Controller,
        logHandler,
        forceOverwrite);
      SaveFile
        (
          integrationDictionary,
          coordinationResult.Resource,
          logHandler,
          forceOverwrite);
    }

    private static void SaveFile(
      Dictionary<string, string> integrationDictionary,
      GenerationResult result,
      StringFormatProviderVisitor logHandler,
      bool forceOverwrite)
    {
      if (result == null)
        return;

      CreatePathTree(result.AbsoluteFileNameName);
      if (File.Exists(result.AbsoluteFileNameName))
      {
        if ((!result.PreviousParseSuccess ||
             (result.PreviousParseSuccess && result.PreviousDeclaredMd5 != result.PreviousCalculatedMd5)) && !forceOverwrite)
        {
          var msgPreamble = $"WARNING: {Environment.NewLine}" + (
            result.PreviousParseSuccess
              ? $"  The declared {result.PreviousDeclaredMd5} and calculated {result.PreviousCalculatedMd5} MD5 expressions do not match for the existing file: {result.AbsoluteFileNameName}."
              : $"  Parse of the existing file: {result.AbsoluteFileNameName} failed.  It is uncertain if this file was previously generated by Nganga."
            ) + $"{Environment.NewLine}";

          var msgClose = $"  Force was not specified.{Environment.NewLine}" +
                         $"  The file was not overwritten and the generated results were not saved.{Environment.NewLine}" +
                         $"  Use the force option to overwrite this file.{Environment.NewLine}";

          var msg = $"{msgPreamble}{Environment.NewLine}{msgClose}";

          logHandler(msg);
          return;
        }
        if (result.PreviousParseSuccess && result.Md5 == result.PreviousCalculatedMd5)
        {
          logHandler("Generated output for {0} is unchanged from previous version, use FORCE to overwrite!",
            result.AbsoluteFileNameName);
          return;
        }
      }

      File.WriteAllText(result.AbsoluteFileNameName, result.Text);
      logHandler("{0} written to disk.", result.AbsoluteFileNameName);
      integrationDictionary[result.AbsoluteFileNameName] = result.RelativeFileName;
    }

    private static void CreatePathTree(string path)
    {
      var dir = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(dir)) return;
      Directory.CreateDirectory(dir);
    }

    private static void CompareStrings(
      KeyValuePair<string, string> data1,
      KeyValuePair<string, string> data2,
      StringFormatProviderVisitor logHandler,
      DTE dteCompareInstance)
    {
      var dte = CreateIsolatedDTEInstance(); // GetCurrentDTEInstance();
      if (dte == null)
      {
        return;
      }
      var tempFolder = Path.GetTempPath();

      var tempFile1 = Path.Combine(tempFolder, Guid.NewGuid().ToString());
      File.WriteAllText(tempFile1, data1.Value);

      var tempFile2 = Path.Combine(tempFolder, Guid.NewGuid().ToString());
      File.WriteAllText(tempFile2, data2.Value);
      logHandler($"Invoking VS diff between {tempFile1} and {tempFile2}");
      dteCompareInstance?.ExecuteCommand("Tools.DiffFiles",
        $"\"{tempFile1}\" \"{tempFile2}\" \"{data1.Key}\" \"{data2.Key}\"");
    }

    [DllImport("ole32.dll")]
    private static extern void CreateBindCtx(int reserved, out IBindCtx ppbc);

    [DllImport("ole32.dll")]
    private static extern void GetRunningObjectTable(int reserved,
      out IRunningObjectTable prot);

    // ReSharper disable once InconsistentNaming
    public static DTE CreateIsolatedDTEInstance(string progId = "VisualStudio.DTE.14.0")
    {
      var visualStudioType = Type.GetTypeFromProgID(progId);
      var dte = Activator.CreateInstance(visualStudioType) as DTE;
      return dte;
    }

    // ReSharper disable once InconsistentNaming
    public static DTE GetCurrentDTEInstance()
    {
      //rot entry for visual studio running under current process.
      string rotEntry = $"!VisualStudio.DTE";
      IRunningObjectTable rot;
      GetRunningObjectTable(0, out rot);
      IEnumMoniker enumMoniker;
      rot.EnumRunning(out enumMoniker);
      enumMoniker.Reset();
      IntPtr fetched = IntPtr.Zero;
      IMoniker[] moniker = new IMoniker[1];
      while (enumMoniker.Next(1, moniker, fetched) == 0)
      {
        IBindCtx bindCtx;
        CreateBindCtx(0, out bindCtx);
        string displayName;
        moniker[0].GetDisplayName(bindCtx, null, out displayName);
        if (displayName.StartsWith(rotEntry))
        {
          object comObject;
          rot.GetObject(moniker[0], out comObject);
          return (DTE) comObject;
        }
      }
      return null;
    }
  }
}