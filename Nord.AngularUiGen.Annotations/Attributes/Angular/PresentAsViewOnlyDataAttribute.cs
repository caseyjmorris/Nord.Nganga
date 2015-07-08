using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// All data presented by this controller should be presented in a view-only format
  /// </summary>
  /// <remarks>
  /// This is applied to a controller.  NOT a view model!  A controller!
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public class PresentAsViewOnlyDataAttribute : Attribute
  {
  }
}