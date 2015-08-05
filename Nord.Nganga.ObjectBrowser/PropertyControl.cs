using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using Nord.Nganga.ObjectBrowser;

namespace OBS.Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for PropertyControl.
  /// </summary>
  class PropertyControl : System.Windows.Forms.UserControl
  {
    public enum LabelPositionOption
    {
      Left,
      Top,
      Default
    }

    private PropertyControl.LabelPositionOption pLabelPosition = PropertyControl.LabelPositionOption.Default;
    public PropertyControl.LabelPositionOption LabelPosition
    {
      get
      {
        return pLabelPosition;
      }
      set
      {
        pLabelPosition = value;
      }
    }

    private System.Drawing.Color DefaultValueControlBackColor;
    private System.Windows.Forms.Label pLabelControl;
    private System.Windows.Forms.Control pValueControl;
    private bool pShowTypeName = false;
    private PropertyBinding pPropertyBinding;
    private bool pAutoUpdate = true;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.ComponentModel.IContainer components;

    public System.Windows.Forms.Label LabelControl
    {
      get
      {
        return pLabelControl;
      }
    }

    public System.Windows.Forms.Control ValueControl
    {
      get
      {
        return pValueControl;
      }
    }

    public bool ShowTypeName
    {
      get
      {
        return pShowTypeName;
      }
      set
      {
        pShowTypeName = value;
      }
    }

    public PropertyBinding PropertyBinding
    {
      get
      {
        return pPropertyBinding;
      }
      set
      {
        pPropertyBinding = value;
        this.InitializeControls();
      }
    }

    public object SelectedEnum
    {
      get
      {
        object sEnum;
        string enumName;
        System.Windows.Forms.ComboBox cb;
        cb = (System.Windows.Forms.ComboBox) this.ValueControl;
        enumName = cb.Text;
        sEnum = Enum.Parse(this.PropertyBinding.PropertyValueType, enumName);
        return sEnum;
      }
    }

    public object Value
    {
      get
      {
        object o = null;
        switch (this.PropertyBinding.BindingType)
        {
          case PropertyBinding.BindingTypes.Boolean:
            System.Windows.Forms.CheckBox cb = (System.Windows.Forms.CheckBox) this.ValueControl;
            o = System.Convert.ChangeType(
                cb.Checked,
                this.PropertyBinding.PropertyValueType);
            break;

          case PropertyBinding.BindingTypes.Number:
            o = System.Convert.ChangeType(
                this.ValueControl.Text,
                this.PropertyBinding.PropertyValueType);
            break;

          case PropertyBinding.BindingTypes.String:
            o = ValueControl.Text;
            break;

          case PropertyBinding.BindingTypes.Enum:
            o = SelectedEnum;
            break;

          case PropertyBinding.BindingTypes.DateTime:
            DateTime temp;
            DateTime.TryParse(this.ValueControl.Text, out temp);
            o = temp;
            break;

          case PropertyBinding.BindingTypes.Object:
            o = this.PropertyBinding.BoundObject;
            break;

          default:
            {
              throw new System.Exception(
                  "Unable to bind type " + this.PropertyBinding.PropertyValueTypeName + ".");
            }
        }
        return o;
      }
      set
      {
        System.Windows.Forms.CheckBox ck;
        System.Windows.Forms.TextBox tb;
        System.Windows.Forms.ComboBox cb;
        ObjectControl oc;

        switch (this.PropertyBinding.BindingType)
        {
          case PropertyBinding.BindingTypes.Object:
            object nestedObj = this.PropertyBinding.PropertyValue;
            oc = new ObjectControl(nestedObj);
            oc.Location = new System.Drawing.Point(CumulativeWidth, 0);
            oc.Name = "ocValue";
            this.Controls.Add(oc);
            pValueControl = oc;
            break;

          case PropertyBinding.BindingTypes.Boolean:
            ck = (System.Windows.Forms.CheckBox) this.ValueControl;
            ck.Checked = (System.Boolean) System.Convert.ChangeType(
                value, //this.PropertyBinding.PropertyValue, 
                this.PropertyBinding.PropertyValueType);
            break;

          case PropertyBinding.BindingTypes.Number:
            tb = (System.Windows.Forms.TextBox) this.ValueControl;
            tb.Text = value.ToString(); // this.PropertyBinding.PropertyValue.ToString() ; 
            break;

          case PropertyBinding.BindingTypes.String:
            tb = (System.Windows.Forms.TextBox) this.ValueControl;
            tb.Text = value.ToString(); // this.PropertyBinding.PropertyValue.ToString() ; 
            break;

          case PropertyBinding.BindingTypes.Enum:
            cb = (System.Windows.Forms.ComboBox) this.ValueControl;
            // select the current value from the combo based on the sourceValue
            // the ToString function on enums returns the value name so 
            // the IndexOf function will work for this 
            cb.SelectedItem = cb.Items[cb.Items.IndexOf(value.ToString())]; // this.PropertyBinding.PropertyValue.ToString() ) ]  ; 
            break;

          default:
            {
              throw new System.Exception(
                  this.PropertyBinding.BindingType.ToString() + " binding type " +
                  " encountered in " + this.GetType().FullName + ".Value+Set");
            }
        }
      }
    }

    public PropertyControl (
        object anObject,
        PropertyInfo aPropertyInfo)
    {
      PropertyBinding aPropertyBinding = new PropertyBinding(anObject, aPropertyInfo);

      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call
      if (!aPropertyBinding.Property.CanRead)
      {
        throw new System.Exception("Property " + aPropertyBinding.PropertyName + " is not marked as CanRead. ");
      }
      //aPropertyBinding.Control = this; 
      this.PropertyBinding = aPropertyBinding;
      this.PropertyBinding.Control = this;
    }

    public PropertyControl (PropertyBinding aPropertyBinding)
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call
      if (!aPropertyBinding.Property.CanRead)
      {
        throw new System.Exception("Property " + aPropertyBinding.PropertyName + " is not marked as CanRead. ");
      }
      //aPropertyBinding.Control = this; 
      this.PropertyBinding = aPropertyBinding;
      this.PropertyBinding.Control = this;
    }

    public PropertyControl ()
    {
      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call

    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose (bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
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
      this.components = new System.ComponentModel.Container();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      // 
      // PropertyControl
      // 
      this.Name = "PropertyControl";
      this.Load += new System.EventHandler(this.PropertyControl_Load);

    }
    #endregion

    private void PropertyControl_Load (object sender, System.EventArgs e)
    {

    }

    public bool AutoUpdate
    {
      get
      {
        return pAutoUpdate;
      }
      set
      {
        pAutoUpdate = value;
      }
    }

    private void InitializeControls ()
    {

      this.Controls.Clear();

      pLabelControl = ControlFactory.CreateLabel(this.PropertyBinding);
      pLabelControl.Location = new System.Drawing.Point(0, 0);
      pLabelControl.SizeChanged += new EventHandler(LabelControl_SizeChanged);
      this.Controls.Add(pLabelControl);

      pValueControl = ControlFactory.CreateControl(this.PropertyBinding);
      switch (this.LabelPosition)
      {
        case PropertyControl.LabelPositionOption.Default:
          pValueControl.Location = new System.Drawing.Point(CumulativeWidth, 0);
          break;
        case PropertyControl.LabelPositionOption.Left:
          pValueControl.Location = new System.Drawing.Point(CumulativeWidth, 0);
          break;
        case PropertyControl.LabelPositionOption.Top:
          pValueControl.Location = new System.Drawing.Point(0, CumulativeHeight);
          break;
      }

      this.Controls.Add(pValueControl);

      #region old
      /*
 * 
 OBS.Nord.Nganga.ObjectBrowser.ObjectControl oc ; 
System.Windows.Forms.TextBox tb ; 
System.Windows.Forms.ComboBox cb ;
System.Windows.Forms.CheckBox ck ; 
switch ( this.PropertyBinding.BindingType ) 
{
#region bind bool
  case PropertyBinding.BindingTypes.Boolean :
    ck = new CheckBox() ; 
    ck.Location = new System.Drawing.Point(CumulativeWidth, 0);
    ck.Name = "chkValue";
    //tb.Size = new System.Drawing.Size(0, 16);
    ck.TabIndex = 0;
    ck.Checked = (bool) System.Convert.ChangeType(
      this.PropertyBinding.PropertyValue, 
      this.PropertyBinding.PropertyValueType ) ; 
    ck.Visible = true ;
    ck.CheckStateChanged +=new EventHandler(ck_CheckStateChanged); 
    this.Controls.Add(ck);
    pValueControl = ck ; 
    break ; 
#endregion
#region bind number 				
  case PropertyBinding.BindingTypes.Number : 
    tb = new System.Windows.Forms.TextBox();
    tb.AutoSize = true ;
    tb.Location = new System.Drawing.Point(CumulativeWidth, 0);
    tb.Name = "txtValue";
    //tb.Size = new System.Drawing.Size(0, 16);
    tb.TabIndex = 0;
    tb.Text = this.PropertyBinding.PropertyValue.ToString() ; 
    tb.Visible = true ;
    tb.KeyDown += new KeyEventHandler(txtValue_KeyDown);
    this.Controls.Add(tb);
    pValueControl = tb ; 
    break ;
#endregion
#region bind string 				
  case PropertyBinding.BindingTypes.String :
    tb = new System.Windows.Forms.TextBox();
    tb.AutoSize = true ;
    tb.Location = new System.Drawing.Point(CumulativeWidth, 0);
    tb.Name = "txtValue";
    //tb.Size = new System.Drawing.Size(0, 16);
    tb.TabIndex = 0;
    tb.Text = this.PropertyBinding.PropertyValue.ToString() ; 
    tb.Visible = true ;
    tb.KeyDown += new KeyEventHandler(txtValue_KeyDown);
    this.Controls.Add(tb);
    pValueControl = tb ; 
    break ; 
#endregion
#region bind enum
  case PropertyBinding.BindingTypes.Enum  :
    // create the combo box 
    cb = new System.Windows.Forms.ComboBox();
    cb.Location = new System.Drawing.Point(CumulativeWidth, 0);
    cb.Name = "cmbValue";
    //cb.Width = 500 ; 
    //tb.Size = new System.Drawing.Size(0, 16);
    cb.TabIndex = 0;
    cb.Visible = true ;

    // load the combo box with the defined enumeration value names 
    // DumpObject( this.SourceType ) ; 
    foreach( FieldInfo f in this.PropertyBinding.PropertyValueType.GetFields() ) 
    {
      if ( f.FieldType.Name.Equals( this.PropertyBinding.PropertyValueType.Name ) )
      {						
        // DumpObject( f ) ; 
        cb.Items.Add( f.Name ) ;
      }
    }

    // select the current value from the combo based on the sourceValue
    // the ToString function on enums returns the value name so 
    // the IndexOf function will work for this 
    cb.SelectedItem = cb.Items[ cb.Items.IndexOf( this.PropertyBinding.PropertyValue.ToString() ) ]  ; 
    cb.SelectedValueChanged += new EventHandler(cb_SelectedValueChanged);
    this.Controls.Add(cb);
    pValueControl = cb ; 
    break ; 
#endregion
#region bind object
  case PropertyBinding.BindingTypes.Object  :
    object nestedObj = this.PropertyBinding.PropertyValue  ;
    oc = new ObjectControl( nestedObj ) ;
    oc.Location = new System.Drawing.Point(CumulativeWidth, 0);
    oc.Name = "ocValue";
    this.Controls.Add(oc);
    pValueControl = oc ; 
    break ;  
#endregion
#region unsupported bind
  case PropertyBinding.BindingTypes.Unsupported : 
    tb = new System.Windows.Forms.TextBox();
    tb.Enabled = false ;
    tb.AutoSize = true ;
    tb.Location = new System.Drawing.Point(CumulativeWidth, 0);
    tb.Name = "txtValue";
    //tb.Size = new System.Drawing.Size(0, 16);
    tb.TabIndex = 0;
    tb.Text = "* Unsupported Type *" ; 
    tb.Visible = true ;
    //tb.KeyDown += new KeyEventHandler(txtValue_KeyDown);
    this.Controls.Add(tb);
    pValueControl = tb ;
    break ; 
#endregion
  default :
  {
    throw new System.Exception( "Unsupported bind" ) ; 
  }
}	

*/
      #endregion

      if (pValueControl != null)
      {
        pValueControl.Move += new EventHandler(ValueControl_Move);
        pValueControl.SizeChanged += new EventHandler(ValueControl_SizeChanged);
        pValueControl.TextChanged += new EventHandler(ValueControl_TextChanged);
        pValueControl.LostFocus += new EventHandler(ValueControl_LostFocus);

        if (pValueControl.GetType().Name.Equals("TextBox"))
        {
          ((System.Windows.Forms.TextBox) pValueControl).KeyDown += new KeyEventHandler(ValueControl_KeyDown);
        }
        else if (pValueControl.GetType().Name.Equals("ComboBox"))
        {
          ((System.Windows.Forms.ComboBox) pValueControl).SelectedValueChanged += new EventHandler(ValueControl_SelectedValueChanged);
        }
        else if (pValueControl.GetType().Name.Equals("CheckBox"))
        {
          ((System.Windows.Forms.CheckBox) pValueControl).CheckStateChanged += new EventHandler(ValueControl_CheckStateChanged);
        }

        // we can only edit properties marked as CanWrite 
        pValueControl.Enabled = this.PropertyBinding.Property.CanWrite;
        pValueControl.TabStop = this.PropertyBinding.Property.CanWrite;
        this.TabStop = this.PropertyBinding.Property.CanWrite;

        DefaultValueControlBackColor = pValueControl.BackColor;
      }

      this.toolTip1.SetToolTip(
          LabelControl,
          this.PropertyBinding.PropertyValueType.FullName);

      UpdateClientSize();

      this.SizeChanged += new EventHandler(PropertyControl_SizeChanged);

      //this.Width = CumulativeWidth ; 
      //this.Height = MaxHeight ; 
    }

    private int MaxHeight
    {
      get
      {
        int h = 0;
        foreach (Control c in this.Controls)
        {
          h = System.Math.Max(h, c.Height);
        }
        return h;
      }
    }

    private int CumulativeWidth
    {
      get
      {
        int w = 0;
        foreach (Control c in this.Controls)
        {
          w += c.Width;
        }
        return w + 1;
      }
    }

    private int CumulativeHeight
    {
      get
      {
        int h = 0;
        foreach (Control c in this.Controls)
        {
          h += c.Height;
        }
        return h + 1;
      }
    }

    public bool IsDirty
    {
      get
      {
        return !this.Value.Equals(this.PropertyBinding.PropertyValue);
      }
    }

    private void UpdateBoundProperty ()
    {
      if (this.ValueControl != null)
      {
        if (this.ValueControl.Enabled)
        {
          try
          {
            object o = this.PropertyBinding.PropertyValue;

            if (Comparer.Default.Compare(o, Value) != 0)
            {
              this.PropertyBinding.PropertyValue = Value;
              SetColors();
            }
          }
          catch (Exception e)
          {
            System.Windows.Forms.MessageBox.Show(
                "UpdateBoundProperty failed for " +
                this.PropertyBinding.PropertyValueTypeName + " " +
                this.PropertyBinding.BoundObject.GetType().FullName + "." +
                this.PropertyBinding.PropertyName +
                " due to: " + e.Message);
          }
        }
      }
    }

    private void SetColors ()
    {
      try
      {
        if (this.IsDirty)
        {
          pValueControl.BackColor = Color.LightCyan;
          this.toolTip1.SetToolTip(
              LabelControl,
              "Data changed.");
          this.toolTip1.Active = true;
        }
        else
        {
          pValueControl.BackColor = DefaultValueControlBackColor;
          this.toolTip1.SetToolTip(
              LabelControl,
              this.PropertyBinding.PropertyValueType.FullName);
        }
      }
      catch (System.FormatException fe)
      {
        pValueControl.BackColor = Color.Red;
        this.toolTip1.SetToolTip(
            LabelControl,
            fe.Message +
            "  Cannot convert to " +
            this.PropertyBinding.PropertyValueType.FullName);
      }
    }

    private int SizeOf (string text)
    {
      System.Windows.Forms.Label l = new Label();
      l.AutoSize = true;
      l.Text = text;
      return l.Width;
    }

    #region Event Handlers
    private void ValueControl_SelectedValueChanged (object sender, EventArgs e)
    {
      //if ( !bindInProcess && !refreshInProgress  ) 
      //{
      if (this.AutoUpdate)
      {
        UpdateBoundProperty();
      }
      //}
    }

    private bool ClientAreaResizeInProcess = false;
    private void UpdateClientSize ()
    {
      ClientAreaResizeInProcess = true;
      this.ClientSize = new System.Drawing.Size(CumulativeWidth + 2, this.MaxHeight);
      ClientAreaResizeInProcess = false;
    }

    private void LabelControl_SizeChanged (object sender, EventArgs e)
    {
      if (ValueControl != null)
      {
        ValueControl.Left = LabelControl.Left + LabelControl.Width;
      }
      UpdateClientSize();
      //this.Width = this.CumulativeWidth ; 
    }

    private void ValueControl_Move (object sender, EventArgs e)
    {
      //this.Width = this.CumulativeWidth ; 
      UpdateClientSize();
    }

    private void ValueControl_SizeChanged (object sender, EventArgs e)
    {
      //this.Width = this.CumulativeWidth ; 
      UpdateClientSize();
    }

    private void ValueControl_TextChanged (object sender, EventArgs e)
    {
      SetColors();
    }
    private void ValueControl_KeyDown (object sender, System.Windows.Forms.KeyEventArgs e)
    {
      switch (e.KeyCode)
      {
        case Keys.Enter:
          {
            //if ( !bindInProcess && !refreshInProgress  ) 
            //{
            if (this.AutoUpdate)
            {
              this.UpdateBoundProperty();
            }
            //}
          }
          break;
      }
    }

    #endregion

    private void ValueControl_LostFocus (object sender, EventArgs e)
    {
      if (this.AutoUpdate)
      {
        this.UpdateBoundProperty();
      }
    }

    private void PropertyControl_SizeChanged (object sender, EventArgs e)
    {
      if (!ClientAreaResizeInProcess)
      {
        this.ValueControl.Width = ((this.Width) - (this.LabelControl.Width));
      }
    }

    private void ValueControl_CheckStateChanged (object sender, EventArgs e)
    {
      if (this.AutoUpdate)
      {
        this.UpdateBoundProperty();
      }
    }
  }
}
