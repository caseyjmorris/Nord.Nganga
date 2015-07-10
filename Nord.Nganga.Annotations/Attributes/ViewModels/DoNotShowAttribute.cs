using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Indicates user should not be shown the value of this property.
  /// </summary>
  /// <remarks>
  /// While there is already a <see cref="System.Web.Mvc.HiddenInputAttribute"/> which serves much the same purpose, it's MVC-specific and requires a reference to MVC.  Furthermore, I ran into issues with MVC versions being out-of-sync, causing it not to be detected.  Using this custom attribute instead is my fix.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public class DoNotShowAttribute : Attribute { }
}
