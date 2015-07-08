using System;

namespace Nord.AngularUiGen.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Supports invocation of custom targets (states and functions)...
  /// When placed on a an IEnumerable&lt;...ViewModel&gt; propery, 
  /// this attribute will result in an area element placed as the last field of each record (item). 
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class SubordinateItemActionAttribute : Attribute
  {

  /// <summary>
  /// the column heading for the action column
  /// </summary>
    public string ColumnHeadingText { get; set; }

    /// <summary>
    /// The attribute for the inserted area element; examples are ng-click or ui-sref.  
    /// The default is value is: "ui-sref".
    /// </summary>
    public string AttributeName { get; set; }

    /// <summary>
    /// The value assigned to the attribute identified in AttributeName
    /// this would typically be the target state for AttributeName="ui-sref" 
    /// or the function reference for AttributeName="ng-click"
    /// </summary>
    public string AttributeValue { get; set; }

    /// <summary>
    /// The class attribute value for the area containing the Attribute name/value pair
    /// The default value is:  "btn btn-primary"
    /// </summary>
    public string AreaClass { get; set; }

    
    /// <summary>
    /// A text string (becomes the button label or link label) to be used on the item
    /// </summary>
    public string ActionText { get; set; }

    /// <summary>
    /// The class attribute to be associated with the ActionText.
    /// The default value is: "glyphicon glyphicon-arrow-right"
    /// </summary>
    public string ActionClass { get; set; }

    /// <summary>
    /// when placed on a an IEnumerable&lt;...ViewModel&gt; propery, 
    /// this attribute will result in an area element placed as the last field of each record (item)
    /// </summary>
    /// <param name="actionText">The action text value.  See property description for more.</param>
    /// <param name="attributeValue">The action target.</param>
    public SubordinateItemActionAttribute(string actionText, string attributeValue)
    {
      this.ColumnHeadingText = "Actions";
      this.AttributeName = "ui-sref";
      this.AreaClass = "btn btn-primary";
      this.ActionClass = "glyphicon glyphicon-arrow-right";
      this.AttributeValue = attributeValue;
      this.ActionText = actionText;
    }
  }
}
