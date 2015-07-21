using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Fs.VsIntegration;
using Nord.Nganga.Models;

namespace Nord.Nganga.WinApp
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


    public void Reset()
    {
      this.vsIntegrationDictionary.Clear();
    }

    private void LogKeys(LogHandler logHandler)
    {
      this.vsIntegrationDictionary.Keys.ToList().ForEach(fileName => logHandler("{0}{1}", '\t', fileName));
    }


    public void IntegrateFiles(string projectPath, AssemblyOptionsModel optionsModel, LogHandler logHandler)
    {
      var vsProjectFile = Path.Combine(projectPath, optionsModel.CsProjectPath);

      if (string.IsNullOrEmpty(vsProjectFile) || !File.Exists(vsProjectFile)) return;

      logHandler("VS integration target is {0}", vsProjectFile);

      this.LogKeys(logHandler);

      if (!Settings1.Default.AutoVSIntegration)
      {
        logHandler("Integration disabled.");
        return;
      }
      try
      {
        logHandler("Integration starting.");

        this.csProjEditor.AddFileToCsProj(
          vsProjectFile, this.vsIntegrationDictionary.Keys.ToList(),
          (p) => logHandler("{0}{1}", '\t', p));

        logHandler("Integration complete.");
      }
      catch (Exception ie)
      {
        logHandler("VS Integration failed due to {0}", ie.Message);
        logHandler(string.Format("{0}", ie));
      }
    }

    public void SaveResult(CoordinationResult coordinationResult, string projectPath, AssemblyOptionsModel optionsModel, LogHandler logHandler)
    {
      this.SaveFile(
        () => optionsModel.NgViewsPath,
        () => coordinationResult.ViewPath,
        () => coordinationResult.ViewBody,
        projectPath,
        logHandler);

      this.SaveFile(
        () => optionsModel.NgControllersPath,
        () => coordinationResult.ControllerPath,
        () => coordinationResult.ControllerBody,
        projectPath,
        logHandler);

      this.SaveFile(
      () => optionsModel.NgResourcesPath,
      () => coordinationResult.ResourcePath,
        () => coordinationResult.ResourceBody,
        projectPath,
        logHandler);
    }
    private void CreatePathTree(string path)
    {
      var dir = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(dir)) return;
      Directory.CreateDirectory(dir);
    }

    public  void SaveFile(
      Func<string> rootProvider, 
      Func<string> nameProvider, 
      Func<string> dataProvider, 
      string projectPath,
      LogHandler logHandler )
    {
      var relativeName = nameProvider();
      var targetFileName = Path.Combine(projectPath,Path.Combine(rootProvider(), relativeName));
      this.CreatePathTree(targetFileName);
      File.WriteAllText(targetFileName, dataProvider());
      logHandler("{0} written to disk.", targetFileName);
      this.vsIntegrationDictionary[targetFileName] = relativeName;
    }

  }
}
