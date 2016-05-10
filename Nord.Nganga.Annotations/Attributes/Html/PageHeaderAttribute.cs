using System;

namespace Nord.Nganga.Annotations.Attributes.Html
{
  /// <summary>
  /// If specified, override the default behavior of using the controller name as the header for the page.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class PageHeaderAttribute : Attribute
  {
    /// <summary>
    /// Instantiate a new <see cref="PageHeaderAttribute"/>
    /// </summary>
    /// <param name="header">The text of the header</param>
    public PageHeaderAttribute(string header)
    {
      this.Header = header;
    }

    /// <summary>
    /// The text of the header
    /// </summary>
    public string Header { get; private set; }
  }
}