using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// Indicates Angular resource services accessing this controller should use a global cache.  Use this option if the data changes very infrequently; the cache will not be invalidated unless the user refreshes the whole page.
  /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
  public class UseAngularGlobalCacheAttribute : Attribute
  {
  }
}