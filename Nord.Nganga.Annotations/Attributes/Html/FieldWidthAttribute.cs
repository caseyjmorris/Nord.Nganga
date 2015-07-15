using System;

namespace Nord.Nganga.Annotations.Attributes.Html
{
  /// <summary>
  /// Controls the width in Bootstrap columns of a field within HTML rows.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class FieldWidthAttribute : Attribute
  {
    /// <summary>
    /// Twelfths of the row to be used.
    /// </summary>
    public int Twelfths { get; private set; }

    /// <summary>
    /// Creates a new FieldWidthAttribute.
    /// </summary>
    /// <param name="twelfths">The number of Bootstrap columns the field is to occupy</param>
    public FieldWidthAttribute(int twelfths)
    {
      if (twelfths < 1 || twelfths > 12)
      {
        throw new ArgumentOutOfRangeException("twelfths", "Twelfths must be between one and twelve.");
      }
      this.Twelfths = twelfths;
    }
  }
}