using System;
using System.IO;
using System.Linq;
using Nord.Nganga.Engine.Extensions.Text;

namespace Nord.Nganga.Engine.Io
{
  public class NameSuggester
  {
    public string SuggestResourceFileName(Type controller)
    {
      var namespaceTail = controller.Namespace.Split('.').Last().ToLowerInvariant();

      if (namespaceTail == "api")
      {
        namespaceTail = string.Empty;
      }
      var controllerName = controller.Name.Replace("Controller", string.Empty).ToLowerInvariant();

      return string.Format("resource.{0}{2}{1}.js", namespaceTail, controllerName,
        namespaceTail == string.Empty ? string.Empty : ".");
    }

    public string SuggestViewFileName(Type controller)
    {
      return this.SuggestViewOrControllerFileNameInternal(controller, "Html");
    }

    public string SuggestControllerFileName(Type controller)
    {
      return this.SuggestViewOrControllerFileNameInternal(controller, "Js");
    }

    private string SuggestViewOrControllerFileNameInternal(Type controller, string extensionCapitalized)
    {
      var namespaceParts = controller.Namespace.Split('.').Select(x => x.ToCamelCase()).ToArray();

      var idx = Array.IndexOf(namespaceParts, "api");

      var htmlFileName =
        controller.Name.Replace("Controller", extensionCapitalized).ToSpaced().ToLowerInvariant().Replace(" ", ".");

      var reduced = namespaceParts.Skip(idx + 1).Concat(new[] { htmlFileName }).ToArray();

      return Path.Combine(reduced);
    }
  }
}