﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Fs.Naming;
using Nord.Nganga.Fs.VsIntegration;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.WinApp.Properties;
using Nord.Nganga.WinControls;

namespace Nord.Nganga.WinApp
{
  public partial class Form1 : Form
  {
    private readonly Dictionary<Type, CoordinationResult> coordinationResults =
      new Dictionary<Type, CoordinationResult>();

    private readonly VsIntegrator vsIntegrator = new VsIntegrator();

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
      Settings1.Default.SelectedVSFile = this.vsProjectFileSelector.SelectedFile;
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
            "{0}{1}On behalf of:{3}" +
            "{0}{1}Base dir:{4}" +
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
        Settings1.Default.LogFusionResolutionEvents ? resolveEventVisitor : null);

      var assy = assyTypes[0].Assembly;
      this.selectedAssemblyOptionsModel = new AssemblyOptionsModel(assy);
      this.resourceDirSelector.SelectedPath = this.selectedAssemblyOptionsModel.NgResourcesPath;
      this.viewDirSelector.SelectedPath = this.selectedAssemblyOptionsModel.NgViewsPath;
      this.controllersDirSelector.SelectedPath = this.selectedAssemblyOptionsModel.NgControllersPath;
      this.vsProjectFileSelector.SelectedFile = this.selectedAssemblyOptionsModel.CsProjectPath;
      this.controllerTypeSelector.SourceAssembly = assy;
      //var types = assy.FindWebApiControllers().ToList();
      //controllerTypeSelector.TypeList = types;
    }

    private AssemblyOptionsModel selectedAssemblyOptionsModel; 

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
      if (!this.coordinationResults.ContainsKey(targetType)) return;

      this.viewRTB.Text = this.coordinationResults[targetType].ViewBody;
      this.contorllerRTB.Text = this.coordinationResults[targetType].ControllerBody;
      this.resourceRTB.Text = this.coordinationResults[targetType].ResourceBody;
    }

    private void GenerateTarget(Type targetControllerType)
    {
      try
      {
        //TODO FIX THIS !!!! 
        var wasp = new WebApiSettingsPackage();
        wasp.SetPropertiesToDefault();

        this.Log("Generating {0}.", targetControllerType.Name);
        var coordinator = new GenerationCoordinator(wasp);
        this.coordinationResults[targetControllerType] = coordinator.CoordinateUiGeneration(targetControllerType);
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
      if (!this.coordinationResults.ContainsKey(targetType))
      {
        MessageBox.Show(
          Resources._The_selected_controller_has_not_been_generated,
          Resources._Sequence_Error,
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);
        return;
      }

      this.vsIntegrator.Reset();

      this.vsIntegrator.SaveResult(this.coordinationResults[targetType], this.selectedAssemblyOptionsModel, this.Log);

      this.vsIntegrator.IntegrateFiles(this.selectedAssemblyOptionsModel, this.Log);

      this.Log("{0}", Resources._The_generated_files_have_been_saved_to_the_output_paths);
    }


    private readonly Dictionary<string, string> saveFileFiltersDictionary = new Dictionary<string, string>
    {
      {".html","HTML | *.html"},
      {".js","JavaScript| *.js"}
    };


    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      this.vsIntegrator.SaveCustomMap();
    }

    private void allToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.vsIntegrator.Reset();
      
      foreach (var target in this.controllerTypeSelector.TypeList)
      {
        this.GenerateTarget(target);
        if (!this.coordinationResults.ContainsKey(target)) continue;
        this.vsIntegrator.SaveResult(this.coordinationResults[target], this.selectedAssemblyOptionsModel, this.Log);
      }
      this.vsIntegrator.IntegrateFiles(this.selectedAssemblyOptionsModel, this.Log);
    }



    private void resourceOnlyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.GenerateResource(this.controllerTypeSelector.SelectedType);
    }

    private void GenerateResource(Type controllerType)
    {

      //TODO FIX THIS !!!! 
      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();

      this.Log("Generating resource for {0}", controllerType.Name);
      var resourceGenerator = new SourceGenerator(wasp);

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
      this.vsIntegrator.SaveFile(
        () => this.resourceDirSelector.SelectedPath,
        () => new NameSuggester().SuggestResourceFileName(this.controllerTypeSelector.SelectedType),
        () => this.resourceRTB.Text,
         this.selectedAssemblyOptionsModel, 
         this.Log);
    }

    private void tabControl1_Selected(object sender, TabControlEventArgs e)
    {
      var tabControl = sender as TabControl;
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
      this.rtbLog.Select(0, 0);
      try
      {
        this.rtbLog.SelectedText = string.Format("{0}{1} - {2}", Environment.NewLine, DateTime.Now.ToString("hh:mm:ss.fff"), string.Format(formatProvider, parms));
      }
      catch (Exception ex)
      {
        this.rtbLog.SelectedText = ex.Message;
        this.rtbLog.Select(0, 0);
        this.rtbLog.SelectedText = "Failing format provider:" + '"' + formatProvider + '"';
      }
    }
  }
}