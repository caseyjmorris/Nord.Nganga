using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Allows definition of input group "caps" (such as a dollar sign before a field).
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class CapAttribute : Attribute
  {
    /// <summary>
    /// Text to appear before the field.
    /// </summary>
    public string StartCap { get; set; }

    /// <summary>
    /// Text to appear after the field.
    /// </summary>
    public string EndCap { get; set; }
  }
}