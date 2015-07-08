using System;
using System.Collections.Generic;
using System.Linq;

namespace Nord.AngularUiGen.Annotations.Attributes.Angular
{
  /// <summary>
  /// Include Angular services in addition to the default services.
  /// </summary>
  /// <remarks>
  /// Apply to WebAPI controller.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public class InjectAngularServicesAttribute : Attribute
  {
    /// <summary>
    /// Additional services to inject
    /// </summary>
    public IEnumerable<string> Services { get; set; }

    /// <summary>
    /// Creates a nwe InjectAngularServicesAttribute.
    /// </summary>
    /// <param name="services">A comma-separated list of additional services.</param>
    public InjectAngularServicesAttribute(string services)
    {
      this.Services = services.Split(',').Select(s => s.Trim()).ToList();
    }
  }
}