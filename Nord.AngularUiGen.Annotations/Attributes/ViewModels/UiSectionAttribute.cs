using System;

namespace Nord.AngularUiGen.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Indicates this field should be treated as part of a particular section
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class UiSectionAttribute : Attribute
  {
    public string SectionName { get; set; }

    public UiSectionAttribute(string sectionName)
    {
      this.SectionName = sectionName;
    }
  }
}
