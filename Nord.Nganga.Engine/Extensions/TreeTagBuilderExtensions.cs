using Nord.CoreLib.Mvc;

namespace Nord.Nganga.Engine.Extensions
{
  public static class TreeTagBuilderExtensions
  {
    public static TreeTagBuilder AppendFullWidthBootStrapRow(this TreeTagBuilder tag)
    {
      return tag.AppendChild("div")
        .WithCssClass("row")
        .AppendChild("div")
        .WithCssClass("col-md-12");
    }
  }
}
