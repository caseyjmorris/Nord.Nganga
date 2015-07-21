﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models;

namespace Nord.Nganga.Fs.VsIntegration
{
  public delegate void LogHandler(string formatProvider, params object[] parms);

  public class VsIntegrator
  {

    private readonly CsProjEditor csProjEditor = new CsProjEditor();
    private readonly Dictionary<string, string> vsIntegrationDictionary = new Dictionary<string, string>();

    private readonly Dictionary<string, string> saveFileFiltersDictionary = new Dictionary<string, string>
    {
      {".html","HTML | *.html"},
      {".js","JavaScript| *.js"}
    };

    private readonly string vsProjectFileName;
    private readonly LogHandler logHandler;
    private readonly AssemblyOptionsModel optionsModel;
    private readonly string vsProjectPath;
    public VsIntegrator(string projectPath, AssemblyOptionsModel optionsModel, LogHandler logHandler)
    {
      if (logHandler == null)
      {
        throw new ArgumentNullException("logHandler");
      }

      if (optionsModel == null)
      {
        logHandler("Assembly attribute ProjectStructure cannot be null.");
        return;
      }

      if (string.IsNullOrEmpty(optionsModel.CsProjectName))
      {
        logHandler("Assembly attribute ProjectStructure.CsProjectName cannot be null.");
        return;
      }
      
      var vsProjectFile = Path.Combine(projectPath, optionsModel.CsProjectName);

      if (string.IsNullOrEmpty(vsProjectFile) || !File.Exists(vsProjectFile))
      {
        logHandler("Cannot find " + vsProjectFile + ".  Are you sure the specified path is correct?");
        return;
      }

      this.logHandler = logHandler;
      this.optionsModel = optionsModel;
      this.vsProjectPath = projectPath;
      this.vsProjectFileName = vsProjectFile;
    }

    public void Reset()
    {
      this.vsIntegrationDictionary.Clear();
    }
    

    public bool IntegrateFiles()
    {
      this.logHandler("VS integration target is {0}", this.vsProjectFileName);
      
      this.vsIntegrationDictionary.Values.ToList().ForEach(fileName => this.logHandler("{0}{1}", '\t', fileName));

      try
      {
        this.logHandler("Integration starting.");

        this.csProjEditor.AddFileToCsProj(
          this.vsProjectFileName, this.vsIntegrationDictionary.Values.ToList(),
          (p) => this.logHandler("{0}{1}", '\t', p));

        this.logHandler("Integration complete.");
      }
      catch (Exception ie)
      {
        this.logHandler("VS Integration failed due to {0}", ie.Message);
        this.logHandler(string.Format("{0}", ie));
        return false;
      }
      return true;
    }

    public void SaveResult(CoordinationResult coordinationResult)
    {
      this.SaveFile(
        () => this.optionsModel.NgViewsPath,
        () => coordinationResult.ViewPath,
        () => coordinationResult.ViewBody);

      this.SaveFile(
        () => this.optionsModel.NgControllersPath,
        () => coordinationResult.ControllerPath,
        () => coordinationResult.ControllerBody);

      this.SaveFile(
      () => this.optionsModel.NgResourcesPath,
      () => coordinationResult.ResourcePath,
        () => coordinationResult.ResourceBody);
    }
    private static void CreatePathTree(string path)
    {
      var dir = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(dir)) return;
      Directory.CreateDirectory(dir);
    }

    public  void SaveFile(
      Func<string> rootProvider, 
      Func<string> nameProvider, 
      Func<string> dataProvider)
    {
      var relativeName = Path.Combine(rootProvider(), nameProvider());
      var targetFileName = Path.Combine(this.vsProjectPath, relativeName);
      CreatePathTree(targetFileName);
      File.WriteAllText(targetFileName, dataProvider());
      this.logHandler("{0} written to disk.", targetFileName);
      this.vsIntegrationDictionary[targetFileName] = relativeName;
    }

  }
}
