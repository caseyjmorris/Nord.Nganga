using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nord.Nganga.Core.Reflection;

namespace Nord.Nganga.WinControls
{
  public partial class AssemblySelector : FileSelector
  {
    public delegate void LogHandlerDelegate(string formatProvider, params object[] parms);

    public new event EventHandler<EventArgs> SelectionChanged ;
    public Assembly SelectedAssembly { get; private set; }

    public LogHandlerDelegate LogHandler { get; set; }
    public bool LogFusionResolutionEvents { get; set; }

    public AssemblySelector()
    {
      this.InitializeComponent();
      this.DialogFilter = "Assemblies|*.dll";
      base.SelectionChanged += this.AssemblySelector_SelectionChanged;
      this.LogFusionResolutionEvents = false;
    }

    void AssemblySelector_SelectionChanged(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(this.SelectedFile)) return;

      Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly> resolveEventVisitor =
        (resolveEventArgs, dirInfo, fileInfo, resolvedAssy) =>
        {
          if (this.LogHandler != null)
          {

            this.LogHandler(
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
          }
        };

      var assyTypes = DependentTypeResolver.GetTypesFrom(
        this.SelectedFile,
        this.LogFusionResolutionEvents ? resolveEventVisitor : null);

      this.SelectedAssembly = assyTypes[0].Assembly;

      if (this.SelectionChanged != null)
      {
        this.SelectionChanged(this, new EventArgs());
      }

    }
  }
}
