using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for PropertyControl.
  /// </summary>
  class PropertyControl : UserControl
  {
    public enum LabelPositionOption
    {
      Left,
      Top,
      Default
    }

    private LabelPositionOption pLabelPosition = LabelPositionOption.Default;
    public LabelPositionOption LabelPosition
    {
      get
      {
        return this.pLabelPosition;
      }
      set
      {
        this.pLabelPosition = value;
      }
    }

    private Color defaultValueControlBackColor;
    private Label pLabelControl;
    private Control pValueControl;
    private bool pShowTypeName = false;
    private PropertyBinding pPropertyBinding;
    private bool pAutoUpdate = true;
    private ToolTip toolTip1;
    private IContainer components;

    public Label LabelControl
    {
      get
      {
        return this.pLabelControl;
      }
    }

    public Control ValueControl
    {
      get
      {
        return this.pValueControl;
      }
    }

    public bool ShowTypeName
    {
      get
      {
        return this.pShowTypeName;
      }
      set
      {
        this.pShowTypeName = value;
      }
    }

    public PropertyBinding PropertyBinding
    {
      get
      {
        return this.pPropertyBinding;
      }
      set
      {
        this.pPropertyBinding = value;
        this.InitializeControls();
      }
    }

    public object SelectedEnum
    {
      get
      {
        object sEnum;
        string enumName;
        ComboBox cb;
        cb = (ComboBox) this.ValueControl;
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
            var cb = (CheckBox) this.ValueControl;
            o = Convert.ChangeType(
                cb.Checked,
                this.PropertyBinding.PropertyValueType);
            break;

          case PropertyBinding.BindingTypes.Number:
            o = Convert.ChangeType(
                this.ValueControl.Text,
                this.PropertyBinding.PropertyValueType);
            break;

          case PropertyBinding.BindingTypes.String:
            o = this.ValueControl.Text;
            break;

          case PropertyBinding.BindingTypes.Enum:
            o = this.SelectedEnum;
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
              throw new Exception(
                  "Unable to bind type " + this.PropertyBinding.PropertyValueTypeName + ".");
            }
        }
        return o;
      }
      set
      {
        CheckBox ck;
        TextBox tb;
        ComboBox cb;
        ObjectControl oc;

        switch (this.PropertyBinding.BindingType)
        {
          case PropertyBinding.BindingTypes.Object:
            var nestedObj = this.PropertyBinding.PropertyValue;
            oc = new ObjectControl(nestedObj);
            oc.Location = new Point(this.CumulativeWidth, 0);
            oc.Name = "ocValue";
            this.Controls.Add(oc);
            this.pValueControl = oc;
            break;

          case PropertyBinding.BindingTypes.Boolean:
            ck = (CheckBox) this.ValueControl;
            ck.Checked = (Boolean) Convert.ChangeType(
                value, //this.PropertyBinding.PropertyValue, 
                this.PropertyBinding.PropertyValueType);
            break;

          case PropertyBinding.BindingTypes.Number:
            tb = (TextBox) this.ValueControl;
            tb.Text = value.ToString(); // this.PropertyBinding.PropertyValue.ToString() ; 
            break;

          case PropertyBinding.BindingTypes.String:
            tb = (TextBox) this.ValueControl;
            tb.Text = value.ToString(); // this.PropertyBinding.PropertyValue.ToString() ; 
            break;

          case PropertyBinding.BindingTypes.Enum:
            cb = (ComboBox) this.ValueControl;
            // select the current value from the combo based on the sourceValue
            // the ToString function on enums returns the value name so 
            // the IndexOf function will work for this 
            cb.SelectedItem = cb.Items[cb.Items.IndexOf(value.ToString())]; // this.PropertyBinding.PropertyValue.ToString() ) ]  ; 
            break;

          default:
            {
              throw new Exception(
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
      var aPropertyBinding = new PropertyBinding(anObject, aPropertyInfo);

      // This call is required by the Windows.Forms Form Designer.
      this.InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call
      if (!aPropertyBinding.Property.CanRead)
      {
        throw new Exception("Property " + aPropertyBinding.PropertyName + " is not marked as CanRead. ");
      }
      //aPropertyBinding.Control = this; 
      this.PropertyBinding = aPropertyBinding;
      this.PropertyBinding.Control = this;
    }

    public PropertyControl (PropertyBinding aPropertyBinding)
    {
      // This call is required by the Windows.Forms Form Designer.
      this.InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call
      if (!aPropertyBinding.Property.CanRead)
      {
        throw new Exception("Property " + aPropertyBinding.PropertyName + " is not marked as CanRead. ");
      }
      //aPropertyBinding.Control = this; 
      this.PropertyBinding = aPropertyBinding;
      this.PropertyBinding.Control = this;
    }

    public PropertyControl ()
    {
      // This call is required by the Windows.Forms Form Designer.
      this.InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call

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
      this.components = new System.ComponentModel.Container();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      // 
      // PropertyControl
      // 
      this.Name = "PropertyControl";
      this.Load += new System.EventHandler(this.PropertyControl_Load);

    }
    #endregion

    private void PropertyControl_Load (object sender, EventArgs e)
    {

    }

    public bool AutoUpdate
    {
      get
      {
        return this.pAutoUpdate;
      }
      set
      {
        this.pAutoUpdate = value;
      }
    }

    private void InitializeControls ()
    {

      this.Controls.Clear();

      this.pLabelControl = ControlFactory.CreateLabel(this.PropertyBinding);
      this.pLabelControl.Location = new Point(0, 0);
      this.pLabelControl.SizeChanged += new EventHandler(this.LabelControl_SizeChanged);
      this.Controls.Add(this.pLabelControl);

      this.pValueControl = ControlFactory.CreateControl(this.PropertyBinding);
      switch (this.LabelPosition)
      {
        case LabelPositionOption.Default:
          this.pValueControl.Location = new Point(this.CumulativeWidth, 0);
          break;
        case LabelPositionOption.Left:
          this.pValueControl.Location = new Point(this.CumulativeWidth, 0);
          break;
        case LabelPositionOption.Top:
          this.pValueControl.Location = new Point(0, this.CumulativeHeight);
          break;
      }

      this.Controls.Add(this.pValueControl);

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

      if (this.pValueControl != null)
      {
        this.pValueControl.Move += new EventHandler(this.ValueControl_Move);
        this.pValueControl.SizeChanged += new EventHandler(this.ValueControl_SizeChanged);
        this.pValueControl.TextChanged += new EventHandler(this.ValueControl_TextChanged);
        this.pValueControl.LostFocus += new EventHandler(this.ValueControl_LostFocus);

        if (this.pValueControl.GetType().Name.Equals("TextBox"))
        {
          ((TextBox) this.pValueControl).KeyDown += new KeyEventHandler(this.ValueControl_KeyDown);
        }
        else if (this.pValueControl.GetType().Name.Equals("ComboBox"))
        {
          ((ComboBox) this.pValueControl).SelectedValueChanged += new EventHandler(this.ValueControl_SelectedValueChanged);
        }
        else if (this.pValueControl.GetType().Name.Equals("CheckBox"))
        {
          ((CheckBox) this.pValueControl).CheckStateChanged += new EventHandler(this.ValueControl_CheckStateChanged);
        }

        // we can only edit properties marked as CanWrite 
        this.pValueControl.Enabled = this.PropertyBinding.Property.CanWrite;
        this.pValueControl.TabStop = this.PropertyBinding.Property.CanWrite;
        this.TabStop = this.PropertyBinding.Property.CanWrite;

        this.defaultValueControlBackColor = this.pValueControl.BackColor;
      }

      this.toolTip1.SetToolTip(this.LabelControl,
          this.PropertyBinding.PropertyValueType.FullName);

      this.UpdateClientSize();

      this.SizeChanged += new EventHandler(this.PropertyControl_SizeChanged);

      //this.Width = CumulativeWidth ; 
      //this.Height = MaxHeight ; 
    }

    private int MaxHeight
    {
      get
      {
        var h = 0;
        foreach (Control c in this.Controls)
        {
          h = Math.Max(h, c.Height);
        }
        return h;
      }
    }

    private int CumulativeWidth
    {
      get
      {
        var w = 0;
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
        var h = 0;
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
            var o = this.PropertyBinding.PropertyValue;

            if (Comparer.Default.Compare(o, this.Value) != 0)
            {
              this.PropertyBinding.PropertyValue = this.Value;
              this.SetColors();
            }
          }
          catch (Exception e)
          {
            MessageBox.Show(
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
          this.pValueControl.BackColor = Color.LightCyan;
          this.toolTip1.SetToolTip(this.LabelControl,
              "Data changed.");
          this.toolTip1.Active = true;
        }
        else
        {
          this.pValueControl.BackColor = this.defaultValueControlBackColor;
          this.toolTip1.SetToolTip(this.LabelControl,
              this.PropertyBinding.PropertyValueType.FullName);
        }
      }
      catch (FormatException fe)
      {
        this.pValueControl.BackColor = Color.Red;
        this.toolTip1.SetToolTip(this.LabelControl,
            fe.Message +
            "  Cannot convert to " +
            this.PropertyBinding.PropertyValueType.FullName);
      }
    }

    private int SizeOf (string text)
    {
      var l = new Label();
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
        this.UpdateBoundProperty();
      }
      //}
    }

    private bool clientAreaResizeInProcess = false;
    private void UpdateClientSize ()
    {
      this.clientAreaResizeInProcess = true;
      this.ClientSize = new Size(this.CumulativeWidth + 2, this.MaxHeight);
      this.clientAreaResizeInProcess = false;
    }

    private void LabelControl_SizeChanged (object sender, EventArgs e)
    {
      if (this.ValueControl != null)
      {
        this.ValueControl.Left = this.LabelControl.Left + this.LabelControl.Width;
      }
      this.UpdateClientSize();
      //this.Width = this.CumulativeWidth ; 
    }

    private void ValueControl_Move (object sender, EventArgs e)
    {
      //this.Width = this.CumulativeWidth ; 
      this.UpdateClientSize();
    }

    private void ValueControl_SizeChanged (object sender, EventArgs e)
    {
      //this.Width = this.CumulativeWidth ; 
      this.UpdateClientSize();
    }

    private void ValueControl_TextChanged (object sender, EventArgs e)
    {
      this.SetColors();
    }
    private void ValueControl_KeyDown (object sender, KeyEventArgs e)
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
      if (!this.clientAreaResizeInProcess)
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
