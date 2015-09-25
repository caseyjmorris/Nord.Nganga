using System;

namespace Nord.Nganga.Annotations.Attributes.Html
{
  /// <summary>
  /// Controls the type of HTML editor to be used for a collection of complex objects.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class CollectionEditorAttribute : Attribute
  {
    /// <summary>
    /// Create a new CollectionEditorAttribute.
    /// </summary>
    /// <param name="editor">The type of editor to be used.</param>
    public CollectionEditorAttribute(EditorType editor)
    {
      this.Editor = editor;
    }

    /// <summary>
    /// If defined, this JSON is used to hydrate objects when the user elects to add a new object.
    /// </summary>
    public string DefaultObjectDefinitionJson { get; set; }

    /// <summary>
    /// Type of editor control to use.
    /// </summary>
    public enum EditorType
    {
      /// <summary>
      /// The simple editor, which displays all fields of all members of the collection
      /// </summary>
      Simple,

      /// <summary>
      /// The complex editor, which uses a table to show all primitive fields of all members and a form showing a single, selected member for editing
      /// </summary>
      Complex,
    }

    /// <summary>
    /// The editor to be used.
    /// </summary>
    public EditorType Editor { get; private set; }
  }
}