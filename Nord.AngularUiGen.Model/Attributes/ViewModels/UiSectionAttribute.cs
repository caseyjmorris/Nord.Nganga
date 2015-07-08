using System;

namespace Nord.AngularUiGen.Attributes
{
  /// <summary>
  /// Indicates this field should be treated as part of a particular section
  /// </summary>
  public class UiSectionAttribute : Attribute
  {
    public string SectionName { get; set; }

    public UiSectionAttribute(string sectionName)
    {
      this.SectionName = sectionName;
    }
  }
}
