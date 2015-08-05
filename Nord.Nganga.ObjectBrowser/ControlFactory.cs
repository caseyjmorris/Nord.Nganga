using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace Nord.Nganga.ObjectBrowser
{
  /// <summary>
  /// Summary description for ControlFactory.
  /// </summary>
  class ControlFactory
  {
    public ControlFactory ()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public static Label CreateLabel (
        PropertyBinding aPropertyBidning)
    {
      Label aLabel;
      aLabel = new Label();
      aLabel.AutoSize = true;
      aLabel.Name = "labName";
      aLabel.Size = new System.Drawing.Size(0, 16);
      aLabel.Text = aPropertyBidning.PropertyName;
      aLabel.Visible = true;
      return aLabel;
    }
    private static CheckBox CreateCheckBox (
        PropertyBinding aPropertyBinding)
    {
      CheckBox checkBox;
      checkBox = new CheckBox();
      checkBox.Visible = true;
      checkBox.Name = "chkValue";
      checkBox.TabIndex = 0;
      checkBox.Checked = (bool) System.Convert.ChangeType(
          aPropertyBinding.PropertyValue,
          aPropertyBinding.PropertyValueType);
      return checkBox;
    }

    private static TextBox CreateTextBox (
        PropertyBinding aPropertyBinding)
    {
      TextBox textBox;
      textBox = new TextBox();
      textBox.AutoSize = true;
      textBox.Name = "txtValue";
      textBox.TabIndex = 0;
      textBox.Text = aPropertyBinding.PropertyValue.ToString();
      textBox.Visible = true;
      return textBox;
    }

    private static ComboBox CreateComboBox (
        PropertyBinding aPropertyBinding)
    {
      ComboBox comboBox;
      // create the combo box 
      comboBox = new ComboBox();
      comboBox.Name = "cmbValue";
      comboBox.TabIndex = 0;
      comboBox.Visible = true;

      // load the combo box with the defined enumeration value names 
      // DumpObject( this.SourceType ) ; 
      foreach (var f in aPropertyBinding.PropertyValueType.GetFields())
      {
        if (f.FieldType.Name.Equals(aPropertyBinding.PropertyValueType.Name))
        {
          // DumpObject( f ) ; 
          comboBox.Items.Add(f.Name);
        }
      }

      // select the current value from the combo based on the sourceValue
      // the ToString function on enums returns the value name so 
      // the IndexOf function will work for this 
      comboBox.SelectedItem = comboBox.Items[comboBox.Items.IndexOf(aPropertyBinding.PropertyValue.ToString())];
      //comboBox.SelectedValueChanged += new EventHandler(cb_SelectedValueChanged);
      return comboBox;
    }

    private static Control CreateObjectControl (
        PropertyBinding aPropertyBinding)
    {
      Control objectControl;

      var nestedObj = aPropertyBinding.PropertyValue;
      objectControl = new ObjectControl(nestedObj);
      objectControl.Name = "ocValue";
      return objectControl;
    }

    private static TextBox CreateUnsupportedControl (
        PropertyBinding aPropertyBinding)
    {
      TextBox textBox;
      textBox = new TextBox();
      textBox.Enabled = false;
      textBox.AutoSize = true;
      textBox.Name = "txtValue";
      textBox.TabIndex = 0;
      textBox.Text = "* Unsupported Type *";
      textBox.Visible = true;
      return textBox;
    }

    public static Label XCreateUnsupportedControl (
        PropertyBinding aPropertyBidning)
    {
      Label aLabel;
      aLabel = new Label();
      aLabel.AutoSize = true;
      aLabel.Name = "txtValue";
      aLabel.Text = "* Unsupported Type:" + aPropertyBidning.PropertyValueBaseTypeName + "." + aPropertyBidning.PropertyValueTypeName + " *";
      aLabel.Visible = true;
      aLabel.BorderStyle = BorderStyle.Fixed3D;
      return aLabel;
    }

    public static Control CreateControl (
        PropertyBinding aPropertyBinding)
    {
      Control aControl = null;
      switch (aPropertyBinding.BindingType)
      {
        case PropertyBinding.BindingTypes.Boolean:
          aControl = CreateCheckBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.DateTime:
          aControl = CreateTextBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Number:
          aControl = CreateTextBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.String:
          aControl = CreateTextBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Enum:
          aControl = CreateComboBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Object:
          aControl = CreateObjectControl(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Unsupported:
          aControl = CreateUnsupportedControl(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Enumerable:
          aControl = CreateEnumerableControl(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Unknown:
          aControl = CreateUnknownControl(aPropertyBinding);
          break;
            
        default:
          {
            throw new System.Exception($"Invalid binding type: {aPropertyBinding.BindingType}");
          }
      }
      return aControl;
    }

    private static Control CreateUnknownControl(PropertyBinding aPropertyBinding)
    {
      return new Label() {Text = aPropertyBinding.PropertyName};
    }

    private static Control CreateEnumerableControl(PropertyBinding aPropertyBinding)
    {
      var parent = new Control();
      foreach (var i in (IEnumerable) aPropertyBinding.PropertyValue)
      {
         var child = new ObjectControl(i);
         parent.Controls.Add(child); 
      }
      //var dg = new DataGrid {DataSource = aPropertyBinding.PropertyValue};
      //dg.Click += Dg_Click;
      return parent;
    }

    private static void Dg_Click (object sender, EventArgs e)
    {
      
    }
  }
}
