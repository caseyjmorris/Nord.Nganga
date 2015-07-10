using System;

namespace Nord.Nganga.Annotations.Attributes.Angular
{
  /// <summary>
  /// Indicates an Angular cache should be used for Angular resource services interacting with this controller, but invalidated with each call to a POST method.  The cache will be local to this controller and not shared with others.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class UseAngularLocalCacheAttribute : Attribute
  {
  }
}