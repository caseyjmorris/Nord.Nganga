using System;
using System.Collections;
using System.Reflection;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for ObjectControl.
  /// </summary>
  public class ObjectControl : System.Windows.Forms.UserControl
  {
    private static Hashtable controlMap = new Hashtable();

    private int stackHeight = 0;
    private object pObject;
    public object DataSource => this.pObject;

    public Hashtable PropertyControls { get; } = new Hashtable();

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private readonly System.ComponentModel.Container components = null;

    public ObjectControl(object anObject)
    {
      // This call is required by the Windows.Forms Form Designer.
      this.InitializeComponent();

      this.LoadElements(anObject);
    }

    private void LoadElements(object anObject)
    {
      var width = 0;
      if (controlMap.ContainsKey(anObject.GetHashCode())) return;
      System.Diagnostics.Debug.WriteLine(anObject.GetType().FullName + " " + anObject.GetHashCode());
      controlMap.Add(anObject.GetHashCode(), anObject);
      this.pObject = anObject;
      var oType = this.pObject.GetType();
      var piList = oType.GetProperties();
      var tabIndex = 0;
      foreach (var pi in piList)
      {
        // we can only open a property control on properties marked as CanRead
        if (!pi.CanRead) continue;
        System.Diagnostics.Debug.WriteLine("	" + pi.PropertyType + " " + pi.Name);

        var pc = new PropertyControl(anObject, pi) {Top = this.stackHeight};
        this.stackHeight += pc.Size.Height + 2;
        if (pc.ValueControl.TabStop)
        {
          pc.TabIndex = tabIndex++;
        }
        this.Controls.Add(pc);
        this.PropertyControls.Add(pc.PropertyBinding.PropertyName, pc);
        width = Math.Max(pc.Width, width);
      }
      this.Height = this.stackHeight;
      this.Width = width;

      this.ConformLabelWidths();

      this.ConformControlWidths();

      this.SizeChanged += this.ObjectControl_SizeChanged;

      controlMap = new Hashtable();
    }

    public int MaxControlSize
    {
      get
      {
        var m = 0;
        foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
        {
          m = Math.Max(aPropertyControl.ValueControl.Width, m);
        }
        return m;
      }
    }

    public int MaxLabelSize
    {
      get
      {
        var m = 0;
        foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
        {
          m = Math.Max(aPropertyControl.LabelControl.Width, m);
        }
        return m;
      }
    }

    public void ConformLabelWidths()
    {
      var width = this.MaxLabelSize;
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        aPropertyControl.LabelControl.AutoSize = false;
        aPropertyControl.LabelControl.Width = width;
        aPropertyControl.Refresh();
      }
    }

    public void ConformControlWidths()
    {
      var width = this.MaxControlSize;
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        aPropertyControl.ValueControl.Width = width;
        aPropertyControl.Refresh();
      }
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.components != null)
        {
          this.components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      // 
      // ObjectControl
      // 
      this.Name = "ObjectControl";
      this.Load += new System.EventHandler(this.ObjectControl_Load);
    }

    #endregion

    public void RefreshView()
    {
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        try
        {
          aPropertyControl.Value = aPropertyControl.PropertyBinding.PropertyValue;
        }
        catch
        {
        }
      }
    }


    private void ObjectControl_Load(object sender, EventArgs e)
    {
    }

    private void ObjectControl_SizeChanged(object sender, EventArgs e)
    {
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        aPropertyControl.Width = this.Width;
        aPropertyControl.Refresh();
      }
    }
  }
}