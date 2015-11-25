using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Replaces the default "Save changes to modelname" text with custom save text
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class SaveButtonTextAttribute : Attribute
  {
    /// <summary>
    /// The text of the save button
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Glyphicon to use for save button.  Default value is "glyphicon-floppy-save".
    /// </summary>
    public string Glyphicon { get; set; } = "glyphicon-floppy-save";

    /// <summary>
    /// Instantiate a new <see cref="SaveButtonTextAttribute"/>
    /// </summary>
    /// <param name="text">Text of the save button</param>
    public SaveButtonTextAttribute(string text)
    {
      this.Text = text;
    }
  }
}