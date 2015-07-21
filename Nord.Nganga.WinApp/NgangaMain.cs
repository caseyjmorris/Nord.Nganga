using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Fs.Naming;
using Nord.Nganga.Fs.VsIntegration;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.WinApp.Properties;
using Nord.Nganga.WinControls;

namespace Nord.Nganga.WinApp
{
  public partial class NgangaMain : Form
  {
    private readonly Dictionary<Type, CoordinationResult> coordinationResults =
      new Dictionary<Type, CoordinationResult>();

    private VsIntegrator vsIntegrator ;

    public NgangaMain()
    {
      this.InitializeComponent();

      this.controllerTypeSelector.BaseApiControllerName = Settings1.Default.BaseApiControllerName;
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
      this.Text = string.Format("{0} - [{1}]", typeof(NgangaMain).Assembly.GetName().Name, typeof(NgangaMain).Assembly.GetName().Version);

      this.logFusionResolutionEventsToolStripMenuItem.Checked = Settings1.Default.LogFusionResolutionEvents;

      this.autoVSIntegration.Checked = Settings1.Default.AutoVSIntegration;

      this.assemblySelector.SelectionChanged += this.assemblySelector_SelectionChanged;
      this.assemblySelector.LogHandler = this.Log;
      
      this.assemblySelector.History = Settings1.Default.AssemblyFileNameHistory;
      this.assemblySelector.HistoryChanged += this.assemblySelector_HistoryChanged;
      this.assemblySelector.SelectedFile = Settings1.Default.SelectedAssemblyFileName;

      this.vsProjectPathSelector.SelectionChanged += this.VsProjectFileSelectorSelectionChanged;
      this.vsProjectPathSelector.History = Settings1.Default.VSFileHistory;
      this.vsProjectPathSelector.SelectedPath = Settings1.Default.SelectedVSPath;
      this.vsProjectPathSelector.HistoryChanged += this.VsProjectFileSelectorHistoryChanged;
    }

    void VsProjectFileSelectorHistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.VSFileHistory = this.vsProjectPathSelector.History;
    }

    void VsProjectFileSelectorSelectionChanged(object sender, EventArgs e)
    {
      Settings1.Default.SelectedVSPath = this.vsProjectPathSelector.SelectedPath;

      this.InitVsIntegrator(this.vsProjectPathSelector.SelectedPath, this.selectedAssemblyOptionsModel);
    }

    private void assemblySelector_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.AssemblyFileNameHistory = this.assemblySelector.History;
    }

    private void assemblySelector_SelectionChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.assemblySelector.SelectedFile) || this.assemblySelector.SelectedAssembly == null ) return;
      
      Settings1.Default.SelectedAssemblyFileName = this.assemblySelector.SelectedFile;
      this.selectedAssemblyOptionsModel = new AssemblyOptionsModel(this.assemblySelector.SelectedAssembly );
      this.InitVsIntegrator(this.vsProjectPathSelector.SelectedPath, this.selectedAssemblyOptionsModel);
      this.controllerTypeSelector.SourceAssembly = this.assemblySelector.SelectedAssembly;
      this.resourcesDir.Text = this.selectedAssemblyOptionsModel.NgResourcesPath;
      this.viewsDir.Text = this.selectedAssemblyOptionsModel.NgViewsPath;
      this.controllersDir.Text = this.selectedAssemblyOptionsModel.NgControllersPath;
      this.vsProjectFileName.Text = this.selectedAssemblyOptionsModel.CsProjectName;
      
    }

    private void InitVsIntegrator(string path, AssemblyOptionsModel model)
    {
      if (string.IsNullOrEmpty(path) || model == null) return;

      this.vsIntegrator = new VsIntegrator(path, model,this.Log);
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

      this.vsIntegrator.SaveResult(this.coordinationResults[targetType] );
      if (this.autoVSIntegration.Checked && this.vsIntegrator.IntegrateFiles())
      {
        this.Log("{0}", Resources._The_generated_files_have_been_saved_to_the_output_paths);
      }
    }

    private void allToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.vsIntegrator.Reset();
      
      foreach (var target in this.controllerTypeSelector.TypeList)
      {
        this.GenerateTarget(target);
        if (!this.coordinationResults.ContainsKey(target)) continue;
        this.vsIntegrator.SaveResult(this.coordinationResults[target]);
      }
      if (this.autoVSIntegration.Checked)
      {
        this.vsIntegrator.IntegrateFiles();
      }
    }

    private void resourceOnlyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.GenerateResource(this.controllerTypeSelector.SelectedType);
    }

    private void GenerateResource(Type controllerType)
    {
      var wasp = new WebApiSettingsPackage();
      wasp.SetPropertiesToDefault();

      this.Log("Generating resource for {0}", controllerType.Name);
      var resourceGenerator = new SourceGenerator(wasp);

      this.resourceRTB.Text = resourceGenerator.GenerateResource(
        controllerType);

      this.saveResourceOnlyToolStripMenuItem.Enabled = true;
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
      if (this.selectedAssemblyOptionsModel == null || this.controllerTypeSelector.SelectedType == null) return;

      this.vsIntegrator.SaveFile(
        () => this.selectedAssemblyOptionsModel.NgResourcesPath,
        () => new NameSuggester().SuggestResourceFileName(this.controllerTypeSelector.SelectedType),
        () => this.resourceRTB.Text);
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