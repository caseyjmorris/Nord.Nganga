﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Nord.Nganga.Core;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.WinControls;

namespace Nord.Nganga.WinApp
{
  public partial class CoordinationForm : Form
  {
    private readonly Action<IEnumerable<CoordinationResult>> resultVisitor;
    private readonly StringFormatProviderVisitor logHandler;

    public CoordinationForm(StringFormatProviderVisitor logHandler,
      Action<IEnumerable<CoordinationResult>> resultVisitor)
    {
      this.InitializeComponent();
      this.resultVisitor = resultVisitor;
      this.logHandler = logHandler;
      DependentTypeResolver.ResolveEventVisitor = DependentTypeResolver.CreateResolveEventLogger(this.logHandler);
    }

    private AssemblyOptionsModel AssemblyOptionsModel { get; set; }

    private Assembly SelectedAssembly => this.assemblySelector1.SelectedAssembly;

    private Type SelectedType => this.typeSelector1.SelectedType;

    private void NgangaGenParmsForm_Load(object sender, EventArgs e)
    {
      this.SetId("Assembly Selection");
      this.logHandler($"{this.Text} ready.");
      this.Top = Settings1.Default.MainTop;

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
      this.logHandler($"{this.Text} ready.");
    }

    private void directorySelector1_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.VsProjectPathHistory = this.directorySelector1.History;
    }

    private void assemblySelector1_HistoryChanged(object sender, EventArgs e)
    {
      Settings1.Default.AssemblyFileNameHistory = this.assemblySelector1.History;
    }

    private void typeSelector1_SelectionChanged(object sender, EventArgs e)
    {
      this.SetOkButtonState();
    }

    private void SetOkButtonState()
    {
      this.button1.Enabled = false;
      if (string.IsNullOrEmpty(this.directorySelector1.SelectedPath)) return;
      if (this.typeSelector1.SelectedType == null && !this.allTypes.Checked) return;
      this.button1.Enabled = true;
    }

    private void AssertDirectorySelection()
    {
      if (this.assemblySelector1.SelectedAssembly == null) return;
      if (string.IsNullOrEmpty(this.directorySelector1.SelectedPath)) return;

      if (!File.Exists(Path.Combine(this.directorySelector1.SelectedPath, this.AssemblyOptionsModel.CsProjectName)))
      {
        MessageBox.Show(
          $"The project {this.AssemblyOptionsModel.CsProjectName} was not found in {this.directorySelector1.SelectedPath}.  Please verify this is the correct path?",
          @"Verify Directory Selection",
          MessageBoxButtons.OK,
          MessageBoxIcon.Warning);
      }
    }

    private void directorySelector1_SelectionChanged(object sender, SelectionChangedEventArgs<string> e)
    {
      this.SetOkButtonState();
      this.AssertDirectorySelection();
    }

    private void assemblySelector1_SelectionChanged(object sender, EventArgs e)
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
        this.HandleException(ex);
        this.DialogResult = DialogResult.Abort;
      }
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.Coordinate();
    }

    private void Coordinate()
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
    }

    private void VisitModel(object model)
    {
      try
      {
        var obf = new ObjectBrowserForm {DataSource = model};
        obf.Show();
      }
      catch (Exception e)
      {
        this.logHandler($"Error - Model bind failed for object browser: {e.Message} at {e.StackTrace}");
      }
    }

    private void Coordinate(Type controllerType)
    {
      try
      {
        if (controllerType == null) return;

        var modelVisitor = this.browseModelToolStripMenuItem.Checked ? (Action<object>) this.VisitModel : null;

        var wasp = ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();
        var fileSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
        var coordinationResult = new List<CoordinationResult>
        {
          this.resourceOnly.Checked
            ? (new GenerationCoordinator(wasp, fileSettings, modelVisitor)).CoordinateResourceGeneration(
              controllerType,
              this.directorySelector1.SelectedPath)
            : (new GenerationCoordinator(wasp, fileSettings, modelVisitor)).CoordinateUiGeneration(controllerType,
              this.directorySelector1.SelectedPath)
        };

        if (coordinationResult.Any(cr => cr.ControllerTemplateRegression))
        {
          this.logHandler?.Invoke($"Controller template regression detected!");
        }

        if (coordinationResult.Any(cr => cr.ResourceTemplateRegression))
        {
          this.logHandler?.Invoke($"Resource template regression detected!");
        }

        if (coordinationResult.Any(cr => cr.ViewTemplateRegression))
        {
          this.logHandler?.Invoke($"View template regression detected!");
        }

        this.resultVisitor(coordinationResult);
      }
      catch (Exception ex)
      {
        this.HandleException(ex);
        this.DialogResult = DialogResult.Abort;
      }
    }

    private void HandleException(Exception ex)
    {
      var iex = ex;
      while (iex != null)
      {
        this.logHandler(iex.ToString());
        iex = ex.InnerException;
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
      Settings1.Default.LogFusionResolutionEvents = this.logFusionEventsToolStripMenuItem.Checked;
    }

    private void loadedAssembliesToolStripMenuItem_Click(object sender, EventArgs e)
    {
      (new AppDomainAssemblyListBrowser()).Show();
    }

    private void label3_Click(object sender, EventArgs e)
    {
      this.Coordinate();
      this.Close();
    }
  }
}