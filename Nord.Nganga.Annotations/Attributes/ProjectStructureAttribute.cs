using System;

namespace Nord.Nganga.Annotations.Attributes
{
  [AttributeUsage(AttributeTargets.Assembly)]
  public class ProjectStructureAttribute:System.Attribute
  {
    public ProjectStructureAttribute(string ngControllersPath, string ngViewsPath, string ngResourcesPath, string csProjectPath)
    {
      this.NgControllersPath = ngControllersPath;
      this.NgViewsPath = ngViewsPath;
      this.NgResourcesPath = ngResourcesPath;
      this.CsProjectPath = csProjectPath;
    }

    public string NgControllersPath { get; set; }
    public string NgViewsPath { get; set; }
    public string NgResourcesPath { get;set; }
    public string CsProjectPath { get; set; }
  }
}
