using System;

namespace Nord.AngularUiGen.Attributes.Angular
{
  /// <summary>
  /// Indicates the Angular route parameter that should be used as an ID when communicating with this controller's GET methods.
  /// </summary>
  public class AngularRouteIdParameterAttribute : Attribute
  {
    /// <summary>
    /// Name of the parameter, as stored in UI-Router parameters dictionary.
    /// </summary>
    public string ParameterName { get; private set; }

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