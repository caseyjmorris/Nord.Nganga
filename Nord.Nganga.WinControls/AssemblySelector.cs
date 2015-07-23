using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nord.Nganga.Core;
using Nord.Nganga.Core.Reflection;

namespace Nord.Nganga.WinControls
{
  public partial class AssemblySelector : FileSelector
  {

    public new event EventHandler<EventArgs> SelectionChanged ;
    public Assembly SelectedAssembly { get; private set; }

    public StringFormatProviderVisitor LogHandler { get; set; }
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

      var assyTypes = DependentTypeResolver.GetTypesFrom(
        this.SelectedFile,
        this.LogFusionResolutionEvents ? 
        DependentTypeResolver.CreateResolveEventLogger(this.LogHandler) : 
        null);

      this.SelectedAssembly = assyTypes[0].Assembly;

      if (this.SelectionChanged != null)
      {
        this.SelectionChanged(this, new EventArgs());
      }

    }
  }
}
