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
  internal class PropertyControl : UserControl
  {
    public enum LabelPositionOption
    {
      Left,
      Top,
      Default
    }

    public LabelPositionOption LabelPosition { get; set; } = LabelPositionOption.Default;

    private Color defaultValueControlBackColor;
    private PropertyBinding pPropertyBinding;
    private ToolTip toolTip1;
    private IContainer components;

    public Label LabelControl { get; private set; }

    public Control ValueControl { get; private set; }

    public bool ShowTypeName { get; set; } = false;

    public PropertyBinding PropertyBinding
    {
      get { return this.pPropertyBinding; }
      private set
      {
        this.pPropertyBinding = value;
        this.InitializeControls();
      }
    }

    public object SelectedEnum
    {
      get
      {
        var cb = (ComboBox) this.ValueControl;
        var enumName = cb.Text;
        var sEnum = Enum.Parse(this.PropertyBinding.PropertyValueType, enumName);
        return sEnum;
      }
    }

    public object Value
    {
      get
      {
        object o;
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

          case PropertyBinding.BindingTypes.Enumerable:
            o = this.ValueControl;
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
        TextBox tb;

        switch (this.PropertyBinding.BindingType)
        {
          case PropertyBinding.BindingTypes.Object:
            var nestedObj = this.PropertyBinding.PropertyValue;
            var oc = new ObjectControl(nestedObj)
            {
              Location = new Point(this.CumulativeWidth, 0),
              Name = "ocValue"
            };
            this.Controls.Add(oc);
            this.ValueControl = oc;
            break;

          case PropertyBinding.BindingTypes.Boolean:
            var ck = (CheckBox) this.ValueControl;
            var changeType = Convert.ChangeType(
              value,
              this.PropertyBinding.PropertyValueType);
            if (changeType != null)
              ck.Checked = (Boolean) changeType;
            break;

          case PropertyBinding.BindingTypes.Number:
            tb = (TextBox) this.ValueControl;
            tb.Text = value.ToString();
            break;

          case PropertyBinding.BindingTypes.String:
            tb = (TextBox) this.ValueControl;
            tb.Text = value.ToString();
            break;

          case PropertyBinding.BindingTypes.Enum:
            var cb = (ComboBox) this.ValueControl;
            // select the current value from the combo based on the sourceValue
            // the ToString function on enums returns the value name so 
            // the IndexOf function will work for this 
            cb.SelectedItem = cb.Items[cb.Items.IndexOf(value.ToString())];
            break;

          case PropertyBinding.BindingTypes.Enumerable:
            var c = new Control();
//            tb.Text = value.ToString();
            break;

          default:
          {
            throw new Exception(
              this.PropertyBinding.BindingType + " binding type " +
              " encountered in " + this.GetType().FullName + ".Value+Set");
          }
        }
      }
    }

    public PropertyControl(
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

    public PropertyControl()
    {
      // This call is required by the Windows.Forms Form Designer.
      this.InitializeComponent();

      // TODO: Add any initialization after the InitializeComponent call
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.components?.Dispose();
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
      this.components = new System.ComponentModel.Container();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      // 
      // PropertyControl
      // 
      this.Name = "PropertyControl";
      this.Load += new System.EventHandler(this.PropertyControl_Load);
    }

    #endregion

    private void PropertyControl_Load(object sender, EventArgs e)
    {
    }

    public bool AutoUpdate { get; set; } = true;

    private void InitializeControls()
    {
      this.Controls.Clear();

      this.LabelControl = ControlFactory.CreateLabel(this.PropertyBinding);
      this.LabelControl.Location = new Point(0, 0);
      this.LabelControl.SizeChanged += this.LabelControl_SizeChanged;
      this.Controls.Add(this.LabelControl);

      this.ValueControl = ControlFactory.CreateControl(this.PropertyBinding);
      switch (this.LabelPosition)
      {
        case LabelPositionOption.Default:
          this.ValueControl.Location = new Point(this.CumulativeWidth, 0);
          break;
        case LabelPositionOption.Left:
          this.ValueControl.Location = new Point(this.CumulativeWidth, 0);
          break;
        case LabelPositionOption.Top:
          this.ValueControl.Location = new Point(0, this.CumulativeHeight);
          break;
      }

      this.Controls.Add(this.ValueControl);

      if (this.ValueControl != null)
      {
        this.ValueControl.Move += this.ValueControl_Move;
        this.ValueControl.SizeChanged += this.ValueControl_SizeChanged;
        this.ValueControl.TextChanged += this.ValueControl_TextChanged;
        this.ValueControl.LostFocus += this.ValueControl_LostFocus;

        if (this.ValueControl.GetType().Name.Equals("TextBox"))
        {
          ((TextBox) this.ValueControl).KeyDown += this.ValueControl_KeyDown;
        }
        else if (this.ValueControl.GetType().Name.Equals("ComboBox"))
        {
          ((ComboBox) this.ValueControl).SelectedValueChanged += this.ValueControl_SelectedValueChanged;
        }
        else if (this.ValueControl.GetType().Name.Equals("CheckBox"))
        {
          ((CheckBox) this.ValueControl).CheckStateChanged += this.ValueControl_CheckStateChanged;
        }

        // we can only edit properties marked as CanWrite 
        this.ValueControl.Enabled = this.PropertyBinding.Property.CanWrite;
        this.ValueControl.TabStop = this.PropertyBinding.Property.CanWrite;
        this.TabStop = this.PropertyBinding.Property.CanWrite;

        this.defaultValueControlBackColor = this.ValueControl.BackColor;
      }

      if (this.PropertyBinding.PropertyValueType != null)
      {
        this.toolTip1.SetToolTip(this.LabelControl,
          this.PropertyBinding.PropertyValueType.FullName);
      }

      this.UpdateClientSize();

      this.SizeChanged += this.PropertyControl_SizeChanged;

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

    public bool IsDirty => !this.Value.Equals(this.PropertyBinding.PropertyValue);

    private void UpdateBoundProperty()
    {
      return; // todo we only want browse - so no updates to the underlying object! 
      if (this.ValueControl != null)
      {
        if (this.ValueControl.Enabled)
        {
          try
          {
            var o = this.PropertyBinding.PropertyValue;

            if (Comparer.Default.Compare(o, this.Value) == 0) return;
            this.PropertyBinding.PropertyValue = this.Value;
            this.SetColors();
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

    private void SetColors()
    {
      try
      {
        if (this.IsDirty)
        {
          this.ValueControl.BackColor = Color.LightCyan;
          this.toolTip1.SetToolTip(this.LabelControl,
            "Data changed.");
          this.toolTip1.Active = true;
        }
        else
        {
          this.ValueControl.BackColor = this.defaultValueControlBackColor;
          this.toolTip1.SetToolTip(this.LabelControl,
            this.PropertyBinding.PropertyValueType.FullName);
        }
      }
      catch (FormatException fe)
      {
        this.ValueControl.BackColor = Color.Red;
        this.toolTip1.SetToolTip(this.LabelControl,
          fe.Message +
          "  Cannot convert to " +
          this.PropertyBinding.PropertyValueType.FullName);
      }
    }

    private int SizeOf(string text)
    {
      var l = new Label
      {
        AutoSize = true,
        Text = text
      };
      return l.Width;
    }

    #region Event Handlers

    private void ValueControl_SelectedValueChanged(object sender, EventArgs e)
    {
      //if ( !bindInProcess && !refreshInProgress  ) 
      //{
      if (this.AutoUpdate)
      {
        this.UpdateBoundProperty();
      }
      //}
    }

    private bool clientAreaResizeInProcess;

    private void UpdateClientSize()
    {
      this.clientAreaResizeInProcess = true;
      this.ClientSize = new Size(this.CumulativeWidth + 2, this.MaxHeight);
      this.clientAreaResizeInProcess = false;
    }

    private void LabelControl_SizeChanged(object sender, EventArgs e)
    {
      if (this.ValueControl != null)
      {
        this.ValueControl.Left = this.LabelControl.Left + this.LabelControl.Width;
      }
      this.UpdateClientSize();
      //this.Width = this.CumulativeWidth ; 
    }

    private void ValueControl_Move(object sender, EventArgs e)
    {
      //this.Width = this.CumulativeWidth ; 
      this.UpdateClientSize();
    }

    private void ValueControl_SizeChanged(object sender, EventArgs e)
    {
      //this.Width = this.CumulativeWidth ; 
      this.UpdateClientSize();
    }

    private void ValueControl_TextChanged(object sender, EventArgs e)
    {
      this.SetColors();
    }

    private void ValueControl_KeyDown(object sender, KeyEventArgs e)
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

    private void ValueControl_LostFocus(object sender, EventArgs e)
    {
      if (this.AutoUpdate)
      {
        this.UpdateBoundProperty();
      }
    }

    private void PropertyControl_SizeChanged(object sender, EventArgs e)
    {
      if (!this.clientAreaResizeInProcess)
      {
        this.ValueControl.Width = ((this.Width) - (this.LabelControl.Width));
      }
    }

    private void ValueControl_CheckStateChanged(object sender, EventArgs e)
    {
      if (this.AutoUpdate)
      {
        this.UpdateBoundProperty();
      }
    }
  }
}