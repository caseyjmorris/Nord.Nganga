using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.WinControls;

namespace Nord.Nganga.WinApp
{
  public partial class NgangaMain : Form
  {
    public NgangaMain()
    {
      this.InitializeComponent();

      this.assemblySelector.DialogFilter = "Assemblies|*.dll";
      this.assemblySelector.SelectionChanged += this.assemblySelector_SelectionChanged;
      this.typeSelector1.BaseApiControllerName = Settings1.Default.BaseApiControllerName;
      this.typeSelector1.SelectionChanged += this.typeSelector1_SelectionChanged;
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
    }

    void typeSelector1_SelectionChanged(object sender, EventArgs e)
    {
      this.button1.Enabled = this.typeSelector1.SelectedType != null;
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
      this.typeSelector1.SourceAssembly = assy;
    }

    private void typeSelector1_Load(object sender, EventArgs e)
    {

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

    private void button1_Click(object sender, EventArgs e)
    {

      this.GenerateTarget(this.typeSelector1.SelectedType);
    }
    private readonly Dictionary<Type, CoordinationResult> coordinationResults =
  new Dictionary<Type, CoordinationResult>();
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
  }
}
