using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// When provided on collection of <see cref="Nganga.DataTypes.UserFileViewModel"/> objects, defines the expression used to supply file types.  If blank this field will not be shown.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class DocumentTypeSourceProviderAttribute : Attribute
  {
    /// <summary>
    /// Body of expression to be used to retrieve document types
    /// </summary>
    public string Expression { get; private set; }

    /// <summary>
    /// Instantiate a new <see cref="DocumentTypeSourceProviderAttribute"/>
    /// </summary>
    /// <param name="expression">Body of expression to be used to retrieve document types</param>
    public DocumentTypeSourceProviderAttribute(string expression)
    {
      this.Expression = expression;
    }
  }
}