using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Allows customization of the labels of yes and no elements of boolean controls.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class BoolLabelAttribute : Attribute
  {
    /// <summary>
    /// The text to show for the "yes" or true label.  Default value is "Yes."
    /// </summary>
    public string YesLabelText { get; set; } = "Yes";

    /// <summary>
    /// Text to show for the "no" or false label.  Default value is "no."
    /// </summary>
    public string NoLabelText { get; set; } = "No";
  }
}