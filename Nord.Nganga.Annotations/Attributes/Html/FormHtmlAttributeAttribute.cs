using System;

namespace Nord.Nganga.Annotations.Attributes.Html
{
  /// <summary>
  /// Decorates a generated form element with the attribute (and optional value)
  /// </summary>
  /// <remarks>
  /// This is intended to be applied to a controller method.  The attribute (and optional value) 
  /// declared here will be added to the <code>form</code> tag generated for the declaring method.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class FormHtmlAttributeAttribute : Attribute
  {

    /// <summary>
    /// The name of the HTML attribute (such as <code>style</code>, <code>required</code>, or <code>ng-show</code>)
    /// </summary>
    public string HtmlAttributeName { get; set; }

    /// <summary>
    /// The value of the HTML attribute.
    /// </summary>
    public string HtmlAttributeValue { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="htmlAttributeName"></param>
    public FormHtmlAttributeAttribute(string htmlAttributeName)
    {
      this.HtmlAttributeName = htmlAttributeName;
    }
  }
}