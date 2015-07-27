using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Allows definition of input group "caps" (such as a dollar sign before a field).
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class CapAttribute : Attribute
  {
    public string StartCap { get; set; }
    public string EndCap { get; set; }
  }
}