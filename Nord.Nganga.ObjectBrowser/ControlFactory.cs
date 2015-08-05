using System.Reflection;
using System.Windows.Forms;
using OBS.Nord.Nganga.ObjectBrowser;

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

    public static System.Windows.Forms.Label CreateLabel (
        PropertyBinding aPropertyBidning)
    {
      System.Windows.Forms.Label aLabel;
      aLabel = new Label();
      aLabel.AutoSize = true;
      aLabel.Name = "labName";
      aLabel.Size = new System.Drawing.Size(0, 16);
      aLabel.Text = aPropertyBidning.PropertyName;
      aLabel.Visible = true;
      return aLabel;
    }
    private static System.Windows.Forms.CheckBox CreateCheckBox (
        PropertyBinding aPropertyBinding)
    {
      System.Windows.Forms.CheckBox checkBox;
      checkBox = new CheckBox();
      checkBox.Visible = true;
      checkBox.Name = "chkValue";
      checkBox.TabIndex = 0;
      checkBox.Checked = (bool) System.Convert.ChangeType(
          aPropertyBinding.PropertyValue,
          aPropertyBinding.PropertyValueType);
      return checkBox;
    }

    private static System.Windows.Forms.TextBox CreateTextBox (
        PropertyBinding aPropertyBinding)
    {
      System.Windows.Forms.TextBox textBox;
      textBox = new System.Windows.Forms.TextBox();
      textBox.AutoSize = true;
      textBox.Name = "txtValue";
      textBox.TabIndex = 0;
      textBox.Text = aPropertyBinding.PropertyValue.ToString();
      textBox.Visible = true;
      return textBox;
    }

    private static System.Windows.Forms.ComboBox CreateComboBox (
        PropertyBinding aPropertyBinding)
    {
      System.Windows.Forms.ComboBox comboBox;
      // create the combo box 
      comboBox = new System.Windows.Forms.ComboBox();
      comboBox.Name = "cmbValue";
      comboBox.TabIndex = 0;
      comboBox.Visible = true;

      // load the combo box with the defined enumeration value names 
      // DumpObject( this.SourceType ) ; 
      foreach (FieldInfo f in aPropertyBinding.PropertyValueType.GetFields())
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

    private static System.Windows.Forms.Control CreateObjectControl (
        PropertyBinding aPropertyBinding)
    {
      System.Windows.Forms.Control objectControl;

      object nestedObj = aPropertyBinding.PropertyValue;
      objectControl = new ObjectControl(nestedObj);
      objectControl.Name = "ocValue";
      return objectControl;
    }

    private static System.Windows.Forms.TextBox CreateUnsupportedControl (
        PropertyBinding aPropertyBinding)
    {
      System.Windows.Forms.TextBox textBox;
      textBox = new System.Windows.Forms.TextBox();
      textBox.Enabled = false;
      textBox.AutoSize = true;
      textBox.Name = "txtValue";
      textBox.TabIndex = 0;
      textBox.Text = "* Unsupported Type *";
      textBox.Visible = true;
      return textBox;
    }

    public static System.Windows.Forms.Label xCreateUnsupportedControl (
        PropertyBinding aPropertyBidning)
    {
      System.Windows.Forms.Label aLabel;
      aLabel = new Label();
      aLabel.AutoSize = true;
      aLabel.Name = "txtValue";
      aLabel.Text = "* Unsupported Type:" + aPropertyBidning.PropertyValueBaseTypeName + "." + aPropertyBidning.PropertyValueTypeName + " *";
      aLabel.Visible = true;
      aLabel.BorderStyle = BorderStyle.Fixed3D;
      return aLabel;
    }

    public static System.Windows.Forms.Control CreateControl (
        PropertyBinding aPropertyBinding)
    {
      System.Windows.Forms.Control aControl = null;
      switch (aPropertyBinding.BindingType)
      {
        case PropertyBinding.BindingTypes.Boolean:
          aControl = ControlFactory.CreateCheckBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.DateTime:
          aControl = ControlFactory.CreateTextBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Number:
          aControl = ControlFactory.CreateTextBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.String:
          aControl = ControlFactory.CreateTextBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Enum:
          aControl = ControlFactory.CreateComboBox(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Object:
          aControl = ControlFactory.CreateObjectControl(aPropertyBinding);
          break;

        case PropertyBinding.BindingTypes.Unsupported:
          aControl = ControlFactory.CreateUnsupportedControl(aPropertyBinding);
          break;

        default:
          {
            throw new System.Exception("Invalid binding type");
          }
      }
      return aControl;
    }
  }
}
