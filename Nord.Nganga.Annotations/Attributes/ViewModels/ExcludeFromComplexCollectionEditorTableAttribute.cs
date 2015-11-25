using System;


namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// If this attribute is defined, the property will be excluded from the summary table used by the complex collection editor.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class ExcludeFromComplexCollectionEditorTableAttribute : Attribute
  {
  }
}
