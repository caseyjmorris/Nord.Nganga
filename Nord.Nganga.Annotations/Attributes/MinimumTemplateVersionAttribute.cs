using System;

namespace Nord.Nganga.Annotations.Attributes
{
  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public class MinimumTemplateVersionAttribute : Attribute
  {
    /// <summary>
    /// 
    /// </summary>
    public TemplateContext TemplateContext { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string MinimumVersion { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateContext"></param>
    /// <param name="minimumVersion"></param>
    public MinimumTemplateVersionAttribute(TemplateContext templateContext, string minimumVersion)
    {
      this.TemplateContext = templateContext;
      this.MinimumVersion = minimumVersion;
    }
  }
}