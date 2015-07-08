using System;

namespace Nord.AngularUiGen.Annotations.Attributes.Html
{
  /// <summary>
  /// Indicates the div enclosing a section should have the HTML attribute and value for the attribute specified.
  /// </summary>
  /// <remarks>
  /// This is intended to be applied directly to the view model class.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class SectionHtmlAttributeAttribute : Attribute
  {
    /// <summary>
    /// The section to which the rule is applied.
    /// </summary>
    /// <remarks>
    /// If no field in this class declares a <see cref="Nord.AngularUiGen.Annotations.Attributes.ViewModels.UiSectionAttribute"/> referring to this section the rule will not be applied.
    /// </remarks>
    public string SectionName { get; set; }

    /// <summary>
    /// The name of the HTML attribute (such as <code>style</code>, <code>required</code>, or <code>ng-show</code>)
    /// </summary>
    public string HtmlAttributeName { get; set; }

    /// <summary>
    /// The value of the HTML attribute.
    /// </summary>
    public string HtmlAttributeValue
    {
      get { return this.htmlAttributeValue ?? this.HtmlAttributeName; }
      set { this.htmlAttributeValue = value; }
    }

    private string htmlAttributeValue;

    public SectionHtmlAttributeAttribute(string sectionName, string htmlAttributeName)
    {
      this.SectionName = sectionName;
      this.HtmlAttributeName = htmlAttributeName;
    }
  }
}