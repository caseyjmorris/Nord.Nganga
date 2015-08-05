using System;
using System.Collections;
using System.Reflection;
using OBS.Nord.Nganga.ObjectBrowser;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for ObjectControl.
  /// </summary>
  public class ObjectControl : System.Windows.Forms.UserControl
  {
    private static System.Collections.Hashtable controlMap = new Hashtable();

    private int StackHeight = 0;
    private object pObject;
    public object DataSource
    {
      get
      {
        return this.pObject;
      }
    }

    private System.Collections.Hashtable pPropertyControls = new Hashtable();
    public System.Collections.Hashtable PropertyControls
    {
      get
      {
        return this.pPropertyControls;
      }
    }

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.Container components = null;

    public ObjectControl (object anObject)
    {
      // This call is required by the Windows.Forms Form Designer.
      this.InitializeComponent();

      this.LoadElements(anObject);
    }

    private void LoadElements (object anObject)
    {
      PropertyControl pc;
      int width = 0;
      if (!controlMap.ContainsKey(anObject.GetHashCode()))
      {
        System.Diagnostics.Debug.WriteLine(anObject.GetType().FullName + " " + anObject.GetHashCode());
        controlMap.Add(anObject.GetHashCode(), anObject);
        this.pObject = anObject;
        System.Type oType = this.pObject.GetType();
        PropertyInfo[] piList = oType.GetProperties();
        int tabIndex = 0;
        foreach (PropertyInfo pi in piList)
        {
          // we can only open a property control on properties marked as CanRead
          if (pi.CanRead)
          {
            System.Diagnostics.Debug.WriteLine("	" + pi.PropertyType + " " + pi.Name);
            try
            {
              //PropertyBinding pb = new PropertyBinding( anObject, pi ) ; 
              //PropertyControl pc = new PropertyControl( pb  ) ; 
              pc = new PropertyControl(anObject, pi);
              pc.Top = this.StackHeight;
              this.StackHeight += pc.Size.Height + 2;
              if (pc.ValueControl.TabStop)
              {
                pc.TabIndex = tabIndex++;
              }
              this.Controls.Add(pc);
              this.PropertyControls.Add(pc.PropertyBinding.PropertyName, pc);
              width = System.Math.Max(pc.Width, width);
            }
            catch (Exception exx)
            {
              string stop = "stop";
            }
          }
        }
        this.Height = this.StackHeight;
        this.Width = width;

        this.ConformLabelWidths();

        this.ConformControlWidths();

        this.SizeChanged += new EventHandler(this.ObjectControl_SizeChanged);

        controlMap = new Hashtable();
      }
    }

    public int MaxControlSize
    {
      get
      {
        int m = 0;
        foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
        {
          m = System.Math.Max(aPropertyControl.ValueControl.Width, m);
        }
        return m;
      }
    }

    public int MaxLabelSize
    {
      get
      {
        int m = 0;
        foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
        {
          m = System.Math.Max(aPropertyControl.LabelControl.Width, m);
        }
        return m;
      }
    }

    public void ConformLabelWidths ()
    {
      int width = this.MaxLabelSize;
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        aPropertyControl.LabelControl.AutoSize = false;
        aPropertyControl.LabelControl.Width = width;
        aPropertyControl.Refresh();
      }
    }

    public void ConformControlWidths ()
    {
      int width = this.MaxControlSize;
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        aPropertyControl.ValueControl.Width = width;
        aPropertyControl.Refresh();
      }
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose (bool disposing)
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
    private void InitializeComponent ()
    {
      // 
      // ObjectControl
      // 
      this.Name = "ObjectControl";
      this.Load += new System.EventHandler(this.ObjectControl_Load);

    }
    #endregion


    public void RefreshView ()
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


    private void ObjectControl_Load (object sender, System.EventArgs e)
    {

    }

    private void ObjectControl_SizeChanged (object sender, EventArgs e)
    {
      foreach (PropertyControl aPropertyControl in this.PropertyControls.Values)
      {
        aPropertyControl.Width = this.Width;
        aPropertyControl.Refresh();
      }
    }
  }
}
