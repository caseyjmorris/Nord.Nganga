using System;

namespace Nord.AngularUiGen.Attributes.Angular
{
  /// <summary>
  /// Specifies the name of the Angular module with which this controller will be associated.
  /// </summary>
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