using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// Specifies the name of the Angular module with which this controller will be associated.
  /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
  public class AngularModuleNameAttribute : Attribute
  {
    /// <summary>
    /// The module name.
    /// </summary>
    public string ModuleName { get; private set; }

    /// <summary>
    /// Creates a new AngularModuleNameAttribute.
    /// </summary>
    /// <param name="moduleName">The module name.</param>
    public AngularModuleNameAttribute(string moduleName)
    {
      this.ModuleName = moduleName;
    }
  }
}