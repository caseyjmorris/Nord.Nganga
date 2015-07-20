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
    private readonly Dictionary<string, string> customOutputMappingDictionary = new Dictionary<string, string>();
    private readonly Dictionary<string, string> vsIntegrationDictionary = new Dictionary<string, string>();

    private readonly Dictionary<string, string> saveFileFiltersDictionary = new Dictionary<string, string>
    {
      {".html","HTML | *.html"},
      {".js","JavaScript| *.js"}
    };

    public VsIntegrator()
    {
      if (Settings1.Default.UseCustomOutputMapping)
      {
        this.customOutputMappingDictionary = (from string mapEntry in Settings1.Default.CustomOutputMap
                                              let toks = mapEntry.Split('=')
                                              let key = toks[0]
                                              let value = toks[1]
                                              select new { key, value }).ToDictionary(r => r.key, r => r.value);
      }
    }

    public void Reset()
    {
      this.vsIntegrationDictionary.Clear();
    }

    public void SaveCustomMap()
    {
      var sc = new StringCollection();
      foreach (var kvp in this.customOutputMappingDictionary)
      {
        sc.Add(kvp.Key + "=" + kvp.Value);
      }
      Settings1.Default.CustomOutputMap = sc;
      Settings1.Default.Save();
    }

    public void LogKeys(LogHandler logHandler)
    {
      this.vsIntegrationDictionary.Keys.ToList().ForEach(fileName => logHandler("{0}{1}", '\t', fileName));
    }


    public void IntegrateFiles(AssemblyOptionsModel optionsModel, LogHandler logHandler)
    {
      var vsProjectFile = optionsModel.CsProjectPath;
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

    public void SaveResult(CoordinationResult coordinationResult, AssemblyOptionsModel optionsModel, LogHandler logHandler)
    {
      this.SaveFile(
        () => optionsModel.NgViewsPath,
        () => coordinationResult.ViewPath,
        () => coordinationResult.ViewBody,
        optionsModel,
        logHandler);

      this.SaveFile(
        () => optionsModel.NgControllersPath,
        () => coordinationResult.ControllerPath,
        () => coordinationResult.ControllerBody,
        optionsModel,
        logHandler);

      this.SaveFile(
      () => optionsModel.NgResourcesPath,
      () => coordinationResult.ResourcePath,
        () => coordinationResult.ResourceBody,
        optionsModel,
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
      AssemblyOptionsModel optionsModel,
      LogHandler logHandler )
    {
      var relativeName = nameProvider();
      var targetFileName = Path.Combine(rootProvider(), relativeName);
      this.CreatePathTree(targetFileName);
      if (!Settings1.Default.UseCustomOutputMapping)
      {
        File.WriteAllText(targetFileName, dataProvider());
        logHandler("{0} written to disk.", targetFileName );
        this.vsIntegrationDictionary[targetFileName] = relativeName;
        return;
      }

      // attempt to find a file with the target name under the VS integraiton directory 
      var searchRoot = Path.GetDirectoryName(Path.GetDirectoryName(optionsModel.CsProjectPath));

      var f = searchRoot.SearchDirectory(Path.GetFileName(targetFileName));
      if (f == null)
      {
        var d = new SaveFileDialog();
        if (!this.customOutputMappingDictionary.ContainsKey(targetFileName))
        {
          d.FileName = targetFileName;
          d.Filter = this.saveFileFiltersDictionary[Path.GetExtension(targetFileName)];
          var dr = d.ShowDialog();
          if (dr == DialogResult.OK)
          {
            this.customOutputMappingDictionary[targetFileName] = d.FileName;
          }
          else
          {
            return;
          }
        }
      }
      else
      {
        this.customOutputMappingDictionary[targetFileName] = f;
      }
      File.WriteAllText(this.customOutputMappingDictionary[targetFileName], dataProvider());
      logHandler("{0} written to disk.", this.customOutputMappingDictionary[targetFileName]);
      this.vsIntegrationDictionary[targetFileName] = relativeName;
    }

  }
}
