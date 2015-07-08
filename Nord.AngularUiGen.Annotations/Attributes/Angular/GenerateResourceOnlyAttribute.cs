using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// If this attribute is applied, the generation will not consider the method when generating controllers or views.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class GenerateResourceOnlyAttribute : Attribute
  {
  }
}