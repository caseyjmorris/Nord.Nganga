using System;

namespace Nord.Nganga.Annotations.Attributes.Html
{
  /// <summary>
  /// Indicates the view generated based on this controller method should have the header specified.
  /// </summary>
  /// <remarks>
  /// This attribute is intended to be applied directly to the get methods of a controller (or POST methods in the case of POST-only types).
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public class AngularControllerActionSectionHeaderAttribute : Attribute
  {
    /// <summary>
    /// The text of the header
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    /// Creates a new <see cref="Nord.Nganga.Annotations.Attributes.Html.AngularControllerActionSectionHeaderAttribute" />
    /// </summary>
    /// <param name="header">
    /// The text of the header
    /// </param>
    public AngularControllerActionSectionHeaderAttribute(string header)
    {
      this.Header = header;
    }
  }
}