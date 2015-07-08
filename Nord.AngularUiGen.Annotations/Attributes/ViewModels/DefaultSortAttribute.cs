using System;

namespace Nord.AngularUiGen.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Indicates that the property should be the default method of sorting when the view model is used in a table.
  /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
  public class DefaultSortAttribute : Attribute { }
}
