using System;

namespace Nord.Nganga.Annotations.Attributes.Angular
{
  /// <summary>
  /// When used on methods of a WebAPI controller allowing nullable parameters for the $stateParams property (i.e., dual-use pages supporting both "save new" and "edit" functionality), this attribute allows a backup, default object definitoin for methods if the client-side paremter is null.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  public class DefaultClientObjectDefinitionAttribute : Attribute
  {
    /// <summary>
    /// The definition, in JavaScript, of the default object.  If no Definition is defined, the default definition is ```{}```.
    /// </summary>
    public string Definition { get; private set; }

    /// <summary>
    /// Create a new DefaultClientObjectDefinitionAttribute
    /// </summary>
    /// <param name="definition">The definition, in JavaScript, of the default object.  If no Definition is defined, the default definition is ```{}```.</param>
    public DefaultClientObjectDefinitionAttribute(string definition)
    {
      this.Definition = definition ?? "{}";
    }
  }
}