using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// Indicates the Angular route parameter that should be used as an ID when communicating with this controller's GET methods.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class AngularRouteIdParameterAttribute : Attribute
  {
    /// <summary>
    /// Name of the parameter, as stored in UI-Router parameters dictionary.
    /// </summary>
    public string ParameterName { get; private set; }

    /// <summary>
    /// ID can be null.  Null IDs represent new records whereas filled-in ones represent existing records.
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Creates a new AngularRouteIdParameterAttribute.
    /// </summary>
    /// <param name="parameterName">Name of the parameter, as stored in UI-Router parameters dictionary.</param>
    public AngularRouteIdParameterAttribute(string parameterName)
    {
      this.ParameterName = parameterName;
    }
  }
}