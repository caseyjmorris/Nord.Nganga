using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// When provided on collection of <see cref="Nganga.DataTypes.UserFileViewModel"/> objects, defines the expression used to supply file types.  If blank this field will not be shown.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class FileTypeSourceProviderAttribute : Attribute
  {
    /// <summary>
    /// Body of expression to be used to retrieve document types
    /// </summary>
    public string Expression { get; private set; }

    /// <summary>
    /// Instantiate a new <see cref="FileTypeSourceProviderAttribute"/>
    /// </summary>
    /// <param name="expression">Body of expression to be used to retrieve document types</param>
    public FileTypeSourceProviderAttribute(string expression)
    {
      this.Expression = expression;
    }
  }
}