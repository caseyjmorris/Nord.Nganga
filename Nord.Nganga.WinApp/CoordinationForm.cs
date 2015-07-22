﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.WinControls;

namespace Nord.Nganga.WinApp
{
  public partial class CoordinationForm : Form
  {
    private Action<CoordinationResult> resultVisitor;
    public CoordinationForm(Action<CoordinationResult> resultVisitor)
    {
      this.InitializeComponent();
      this.resultVisitor = resultVisitor;
    }

    private AssemblyOptionsModel AssemblyOptionsModel { get; set; }

    private Assembly SelectedAssembly
    {
      get { return this.assemblySelector1.SelectedAssembly; }
    }

    private Type SelectedType
    {
      get { return this.typeSelector1.SelectedType; }
    }
    private void NgangaGenParmsForm_Load(object sender, EventArgs e)
    {

      this.Text = string.Format(
        "{0} - [{1}] - Client Assembly Selection", 
        typeof(CoordinationResultBrowser).Assembly.GetName().Name,
        typeof(CoordinationResultBrowser).Assembly.GetName().Version);

      this.logFusionEventsToolStripMenuItem.Checked = Settings1.Default.LogFusionResolutionEvents;

      this.typeSelector1.BaseApiControllerName = Settings1.Default.BaseApiControllerName;
      this.typeSelector1.Filters = new List<TypeSelectorFilter>
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
      this.assemblySelector1.SelectionChanged += this.assemblySelector1_SelectionChanged;
      this.directorySelector1.SelectionChanged += this.directorySelector1_SelectionChanged;
      this.typeSelector1.SelectionChanged += this.typeSelector1_SelectionChanged;
      this.assemblySelector1.SelectedFile = Settings1.Default.SelectedAssemblyFileName;
      this.assemblySelector1.History = Settings1.Default.AssemblyFileNameHistory;
      this.assemblySelector1.HistoryChanged += this.assemblySelector1_HistoryChanged;
      this.directorySelector1.History = Settings1.Default.VsProjectPathHistory;
      this.directorySelector1.HistoryChanged += this.directorySelector1_HistoryChanged;

      this.SetOkButtonState();
    }

    void directorySelector1_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.VsProjectPathHistory = this.directorySelector1.History;
    }

    void assemblySelector1_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.AssemblyFileNameHistory = this.assemblySelector1.History;
    }

    void typeSelector1_SelectionChanged(object sender, EventArgs e)
    {
      this.SetOkButtonState();
    }

    void SetOkButtonState()
    {
      this.button1.Enabled = false;
      if (string.IsNullOrEmpty(this.directorySelector1.SelectedPath)) return;
      if (this.typeSelector1.SelectedType == null && !this.allTypes.Checked) return;
      this.button1.Enabled = true;
    }

    void AssertDirectorySelection()
    {
      if (this.assemblySelector1.SelectedAssembly == null) return;
      if (string.IsNullOrEmpty(this.directorySelector1.SelectedPath)) return;

      if (!File.Exists(Path.Combine(this.directorySelector1.SelectedPath, this.AssemblyOptionsModel.CsProjectName)))
      {
        MessageBox.Show(
          "The project " + this.AssemblyOptionsModel.CsProjectName + " was not found in " +
          this.directorySelector1.SelectedPath + ".  Please verify this is the correct path?",
          "Verify Directory Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }
    void directorySelector1_SelectionChanged(object sender, SelectionChangedEventArgs<string> e)
    {
      this.SetOkButtonState();
      this.AssertDirectorySelection();
    }

    void assemblySelector1_SelectionChanged(object sender, EventArgs e)
    {
      this.AssertDirectorySelection();
      try
      {
        this.typeSelector1.SourceAssembly = this.SelectedAssembly;
        if (this.SelectedAssembly == null) return;
        Settings1.Default.SelectedAssemblyFileName = this.assemblySelector1.SelectedFile;

        this.AssemblyOptionsModel = new AssemblyOptionsModel(this.SelectedAssembly);
      }
      catch (Exception ex)
      {
        var result = new CoordinationResult();
        result.Exceptions.Add(ex);
        this.resultVisitor(result);
        this.DialogResult = DialogResult.Abort;
        this.Close();
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (this.typeSelector1.SelectedType != null && !this.allTypes.Checked)
      {
        this.Coordinate(this.SelectedType);
      }
      else
      {
        foreach (var t in this.typeSelector1.TypeList)
        {
          this.Coordinate(t);
        }
      }
      this.Close();
    }

    private void Coordinate(Type controllerType)
    {
      try
      {
        if (controllerType == null) return;

        var wasp = new WebApiSettingsPackage();
        wasp.SetPropertiesToDefault();
        this.resultVisitor(this.resourceOnly.Checked
          ? (new GenerationCoordinator(wasp)).CoordinateResourceGeneration(controllerType, this.directorySelector1.SelectedPath)
          : (new GenerationCoordinator(wasp)).CoordinateUiGeneration(controllerType, this.directorySelector1.SelectedPath));
      }
      catch (Exception ex)
      {
        var result = new CoordinationResult();
        result.Exceptions.Add(ex);
        this.resultVisitor(result);
        this.DialogResult = DialogResult.Abort;
      }
    }

    
    private void button2_Click(object sender, EventArgs e)
    {
    
      this.Close();
    }

    private void CoordinationForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      Settings1.Default.Save();
    }

    private void resourceOnly_CheckedChanged(object sender, EventArgs e)
    {

    }

    private void allTypes_CheckedChanged(object sender, EventArgs e)
    {
      this.SetOkButtonState();
    }

    private void logFusionEventsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Settings1.Default.LogFusionResolutionEvents = this.logFusionEventsToolStripMenuItem.Checked ;
    }

    private void loadedAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      (new AppDomainAssemblyListBrowser()).Show();
    }

  }
}
