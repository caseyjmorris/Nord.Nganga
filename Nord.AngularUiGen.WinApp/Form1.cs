using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Nord.AngularUiGen.Engine.Coordination;
using Nord.AngularUiGen.Engine.CsProj;
using Nord.AngularUiGen.Engine.Extensions.Text;
using Nord.AngularUiGen.Engine.Html;
using Nord.AngularUiGen.Engine.JavaScript;
using Nord.AngularUiGen.Engine.Reflection;
using Nord.AngularUiGen.Engine.Support;
using Nord.AngularUiGen.Engine.Io;
using Nord.AngularUiGen.WinApp.Properties;
using Nord.AngularUiGen.WinControls;

namespace Nord.AngularUiGen.WinApp
{
  public partial class Form1 : Form
  {
    private readonly Dictionary<Type, CoordinationResult> _coordinationResults =
      new Dictionary<Type, CoordinationResult>();

    private readonly CsProjEditor _csProjEditor = new CsProjEditor();
    private Dictionary<string, string> _customOutputMappingDictionary = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _vsIntegrationDictionary = new Dictionary<string, string>();

    public Form1()
    {
      this.InitializeComponent();

      this.controllerTypeSelector.Filters = new List<TypeSelectorFilter>
      {
        new TypeSelectorFilter {FilterDescription = "Api Controller", FilterProvider = t => true, IsActive = true},
        new TypeSelectorFilter
        {
          FilterDescription = "Angular Route Attribute",
          FilterProvider = t => true,
          IsActive = true
        },
        new TypeSelectorFilter
        {
          FilterDescription = "Angular Module Attribute",
          FilterProvider = t => true,
          IsActive = true
        },
      };
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.logFusionResolutionEventsToolStripMenuItem.Checked = Settings1.Default.LogFusionResolutionEvents;

      this.autoVSIntegration.Checked = Settings1.Default.AutoVSIntegration;
      this.useCustomOutputMappingToolStripMenuItem.Checked = Settings1.Default.UseCustomOutputMapping;
      if (Settings1.Default.UseCustomOutputMapping)
      {
        this._customOutputMappingDictionary = (from string mapEntry in Settings1.Default.CustomOutputMap
          let toks = mapEntry.Split('=')
          let key = toks[0]
          let value = toks[1]
          select new {key,value }).ToDictionary(r=> r.key, r=> r.value);
      }

      this.assemblySelector.DialogFilter = "Assemblies|*.dll";
      this.assemblySelector.SelectionChanged += this.assemblySelector_SelectionChanged;
      this.assemblySelector.History = Settings1.Default.AssemblyFileNameHistory;
      this.assemblySelector.HistoryChanged += this.assemblySelector_HistoryChanged;
      this.assemblySelector.SelectedFile = Settings1.Default.SelectedAssemblyFileName;

      this.resourceDirSelector.SelectedPath = Settings1.Default.SelectedResourceDir;
      this.resourceDirSelector.SelectionChanged += this.resourceDirSelector_SelectionChanged;
      this.resourceDirSelector.HistoryChanged += this.resourceDirSelector_HistoryChanged;

      this.controllersDirSelector.SelectedPath = Settings1.Default.SelectedControllerDir;
      this.controllersDirSelector.SelectionChanged += this.controllersDirSelector_SelectionChanged;
      this.controllersDirSelector.HistoryChanged += this.controllersDirSelector_HistoryChanged;

      this.viewDirSelector.SelectedPath = Settings1.Default.SelectedViewDir;
      this.viewDirSelector.SelectionChanged += this.viewDirSelector_SelectionChanged;
      this.viewDirSelector.HistoryChanged += this.viewDirSelector_HistoryChanged;

      this.vsProjectFileSelector.DialogFilter = "VS Project|*.csproj";
      this.vsProjectFileSelector.SelectionChanged += this.VsProjectFileSelectorSelectionChanged;
      this.vsProjectFileSelector.History = Settings1.Default.VSFileHistory;
      this.vsProjectFileSelector.SelectedFile = Settings1.Default.SelectedVSFile;
      this.vsProjectFileSelector.HistoryChanged += this.VsProjectFileSelectorHistoryChanged;
    }

    void VsProjectFileSelectorHistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.VSFileHistory = this.vsProjectFileSelector.History;
    }

    void VsProjectFileSelectorSelectionChanged(object sender, EventArgs e)
    {
      Settings1.Default.SelectedVSFile = this.vsProjectFileSelector.SelectedFile ;
    }

    private void AssertOutputDirsUnique()
    {
      var x = new List<string>
      {
        this.viewDirSelector.SelectedPath, this.controllersDirSelector.SelectedPath, this.resourceDirSelector.SelectedPath
      };
      var duplciatesExist = x.Where(path => !string.IsNullOrEmpty(path)).GroupBy(path => path).Any(g => g.Count() > 1);
      if (duplciatesExist)
      {
        this.Log(
          "{1} - {0}",
          Resources._All_three_output_paths_must_be_unique,
          Resources._Duplicate_paths);
      }

    }

    private void viewDirSelector_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.ViewDirHistory = this.viewDirSelector.History;
    }

    private void viewDirSelector_SelectionChanged(object sender, SelectionChangedEventArgs<string> e)
    {
      if (string.IsNullOrEmpty(e.SelectedValue)) return;
      this.AssertOutputDirsUnique();
      Settings1.Default.SelectedViewDir = e.SelectedValue;
    }

    private void controllersDirSelector_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.ControllerDirHistory = this.controllersDirSelector.History;
    }

    private void controllersDirSelector_SelectionChanged(object sender, SelectionChangedEventArgs<string> e)
    {
      if (string.IsNullOrEmpty(e.SelectedValue)) return;
      this.AssertOutputDirsUnique();
      Settings1.Default.SelectedControllerDir = e.SelectedValue;
    }

    private void resourceDirSelector_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.ResourcePathHistory = this.resourceDirSelector.History;
    }

    private void resourceDirSelector_SelectionChanged(object sender, SelectionChangedEventArgs<string> e)
    {
      if (string.IsNullOrEmpty(e.SelectedValue)) return;
      this.AssertOutputDirsUnique();
      Settings1.Default.SelectedResourceDir = e.SelectedValue;
    }

    private void assemblySelector_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.AssemblyFileNameHistory = this.assemblySelector.History;
    }

    private void assemblySelector_SelectionChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.assemblySelector.SelectedFile)) return;
      Settings1.Default.SelectedAssemblyFileName = this.assemblySelector.SelectedFile;

      Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly> resolveEventVisitor = (resolveEventArgs, dirInfo, fileInfo, resolvedAssy) =>
      {
        this.Log(
            "RESOLVE - for:{2}" +
            "{0}{1}On behalf of:{3}"+
            "{0}{1}Base dir:{4}"+
            "{0}{1}Module:{5}" +
            "{0}{1}Result Assy:{6}", 
            '\n', 
            '\t',
            resolveEventArgs.Name, 
            resolveEventArgs.RequestingAssembly.FullName, 
            dirInfo.FullName,
            fileInfo.FullName,
            resolvedAssy == null ? "- RESOLVE FAILED!" : resolvedAssy.Location);
      };

      var assyTypes = DependentTypeResolver.GetTypesFrom(
        this.assemblySelector.SelectedFile, 
        Settings1.Default.LogFusionResolutionEvents ? resolveEventVisitor : null );

      var assy = assyTypes[0].Assembly;
      this.controllerTypeSelector.SourceAssembly = assy;
      //var types = assy.FindWebApiControllers().ToList();
      //controllerTypeSelector.TypeList = types;
    }
    
    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Application.Exit();
    }

    private void selectedToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.viewRTB.Clear();
      this.contorllerRTB.Clear();
      this.resourceRTB.Clear();
      var targetType = this.controllerTypeSelector.SelectedType;
      this.GenerateTarget(targetType);
      if (!this._coordinationResults.ContainsKey(targetType)) return;

      this.viewRTB.Text = this._coordinationResults[targetType].ViewBody;
      this.contorllerRTB.Text = this._coordinationResults[targetType].ControllerBody;
      this.resourceRTB.Text = this._coordinationResults[targetType].ResourceBody;
    }

    private void GenerateTarget(Type targetControllerType)
    {
      try
      {
        this.Log("Generating {0}.",targetControllerType.Name);
        var coordinator = new GenerationCoordinator(
          new HtmlGenerator(new OccurrenceCounter<string>()),
          new ResourceGenerator(),
          new ControllerGenerator(), new NameSuggester());
        this._coordinationResults[targetControllerType] = coordinator.CoordinateUiGeneration(targetControllerType);
        this.Log("{0} - generated.", targetControllerType.Name);
      }
      catch (Exception generationException)
      {
        this.Log("VS Integration failed due to {0}", generationException.Message);
        this.Log("{0}", generationException.ToString());
      }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var targetType = this.controllerTypeSelector.SelectedType;
      if (!this._coordinationResults.ContainsKey(targetType))
      {
        MessageBox.Show(
          Resources._The_selected_controller_has_not_been_generated,
          Resources._Sequence_Error,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      this._vsIntegrationDictionary.Clear();

      this.SaveResult(this._coordinationResults[targetType]);

      this.IntegrateFiles();

      this.Log("{0}", Resources._The_generated_files_have_been_saved_to_the_output_paths);
    }

    private void SaveResult(CoordinationResult coordinationResult)
    {
      this.SaveFile(() => this.viewDirSelector.SelectedPath, () => coordinationResult.ViewPath, () => coordinationResult.ViewBody);
      this.SaveFile(() => this.controllersDirSelector.SelectedPath, () => coordinationResult.ControllerPath,
        () => coordinationResult.ControllerBody);
      this.SaveFile(() => this.resourceDirSelector.SelectedPath, () => coordinationResult.ResourcePath,
        () => coordinationResult.ResourceBody);
    }

    private void SaveFile(Func<string> rootProvider, Func<string> nameProvider, Func<string> dataProvider)
    {
      var relativeName = nameProvider();
      var targetFileName = Path.Combine(rootProvider(),relativeName);
      this.CreatePathTree(targetFileName);
      if (!Settings1.Default.UseCustomOutputMapping)
      {
        File.WriteAllText(targetFileName, dataProvider());
        this.Log("{0} written to disk.", targetFileName);
        this._vsIntegrationDictionary[targetFileName] = relativeName;
        return;
      }

      // attempt to find a file with the target name under the VS integraiton directory 
      var searchRoot = Path.GetDirectoryName(Path.GetDirectoryName(this.vsProjectFileSelector.SelectedFile));

      var f = searchRoot.SearchDirectory(Path.GetFileName(targetFileName));
      if (f == null)
      {
        var d = new SaveFileDialog();
        if (!this._customOutputMappingDictionary.ContainsKey(targetFileName))
        {
          d.FileName = targetFileName;
          d.Filter = this._saveFileFiltersDictionary[Path.GetExtension(targetFileName)];
          var dr = d.ShowDialog();
          if (dr == DialogResult.OK)
          {
            this._customOutputMappingDictionary[targetFileName] = d.FileName;
          }
          else
          {
            return;
          }
        }
      }
      else
      {
        this._customOutputMappingDictionary[targetFileName] = f;
      }
      File.WriteAllText(this._customOutputMappingDictionary[targetFileName], dataProvider());
      this.Log("{0} written to disk.", this._customOutputMappingDictionary[targetFileName]);
      this._vsIntegrationDictionary[targetFileName] = relativeName;
    }

    private readonly Dictionary<string,string> _saveFileFiltersDictionary = new Dictionary<string, string>
    {
      {".html","HTML | *.html"},
      {".js","JavaScript| *.js"}
    }; 
    private void CreatePathTree(string path)
    {
      var dir = Path.GetDirectoryName(path);
      if (string.IsNullOrEmpty(dir)) return;
      Directory.CreateDirectory(dir);
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      var sc = new StringCollection();
      foreach (var kvp in this._customOutputMappingDictionary)
      {
        sc.Add(kvp.Key + "=" + kvp.Value);
      }
      Settings1.Default.CustomOutputMap = sc;
      Settings1.Default.Save();
    }

    private void allToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this._vsIntegrationDictionary.Clear();
      foreach (var target in this.controllerTypeSelector.TypeList)
      {
        this.GenerateTarget(target);
        if (!this._coordinationResults.ContainsKey(target)) continue;
        this.SaveResult(this._coordinationResults[target]);
      }
      this.IntegrateFiles();
    }

    private void IntegrateFiles()
    {
      var vsProjectFile = this.vsProjectFileSelector.SelectedFile;
      if (string.IsNullOrEmpty(vsProjectFile) || !File.Exists(vsProjectFile)) return;

      this.Log("VS integration target is {0}", vsProjectFile);

      this._vsIntegrationDictionary.Keys.ToList().ForEach(fileName => this.Log("{0}{1}", '\t', fileName));
      if (!Settings1.Default.AutoVSIntegration)
      {
        this.Log("Integration disabled.");
        return;
      }
      try
      {
        this.Log("Integration starting.");

        this._csProjEditor.AddFileToCsProj(
          vsProjectFile, this._vsIntegrationDictionary.Keys.ToList(),
          (p)=> this.Log("{0}{1}",'\t',p));

        this.Log("Integration complete.");
      }
      catch (Exception ie)
      {
        this.Log("VS Integration failed due to {0}",ie.Message);
        this.Log(string.Format("{0}", ie));
      }
    }

    private void resourceOnlyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.GenerateResource(this.controllerTypeSelector.SelectedType);
    }

    private void GenerateResource(Type controllerType)
    {
      this.Log("Generating resource for {0}", controllerType.Name);
      var resourceGenerator = new ResourceGenerator();
      this.resourceRTB.Text = resourceGenerator.GenerateResource(
        controllerType);


      this.saveResourceOnlyToolStripMenuItem.Enabled = true;
    }

    private void generateToolStripMenuItem_Click(object sender, EventArgs e)
    {
    }

    private void generateToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
    {
      this.selectedToolStripMenuItem.Enabled = this.controllerTypeSelector.SelectedType != null;

      this.selectedToolStripMenuItem.Text = string.Format(
        "{0}", this.controllerTypeSelector.SelectedType == null
          ? "<none selected>"
          : this.controllerTypeSelector.SelectedType.Name);
    }

    private void saveResourceOnlyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.SaveFile(() => this.resourceDirSelector.SelectedPath,
        () => new NameSuggester().SuggestResourceFileName(this.controllerTypeSelector.SelectedType),
        () => this.resourceRTB.Text);
    }

    private void tabControl1_Selected(object sender, TabControlEventArgs e)
    {
      var tabControl = sender as TabControl ;
      if (tabControl == null) return;

      var selectedPage = tabControl.SelectedTab;
      if (selectedPage == null) return;

      var rtb = selectedPage.Controls[0] as RichTextBox;
      if (rtb == null) return;
      var txt = rtb.Text;
      if (string.IsNullOrEmpty(txt)) return;

      Clipboard.SetText(rtb.Text);
    }

    private void useCustomOutputMappingToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Settings1.Default.UseCustomOutputMapping = this.useCustomOutputMappingToolStripMenuItem.Checked;
    }

    private void autoVSIntegration_Click(object sender, EventArgs e)
    {
      Settings1.Default.AutoVSIntegration = this.autoVSIntegration.Checked;
    }

    private void logFusionResolutionEventsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Settings1.Default.LogFusionResolutionEvents = this.logFusionResolutionEventsToolStripMenuItem.Checked;
    }

    private void Log(string formatProvider, params object[] parms)
    {
      this.rtbLog.Select(0,0);
      try
      {
        this.rtbLog.SelectedText = string.Format("{0}{1} - {2}", Environment.NewLine, DateTime.Now.ToString("hh:mm:ss.fff"), string.Format(formatProvider, parms));
      }
      catch (Exception ex)
      {
        this.rtbLog.SelectedText = ex.Message;
        this.rtbLog.Select(0, 0);
        this.rtbLog.SelectedText = "Failing format provider:"+ '"' + formatProvider + '"';
      }
    }
  }
}