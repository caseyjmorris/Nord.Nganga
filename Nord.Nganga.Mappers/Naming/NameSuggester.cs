using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Humanizer;

namespace Nord.Nganga.Mappers.Naming
{
  public class NameSuggester
  {
    public string SuggestResourceFileName(Type controller)
    {
      Debug.Assert(controller.Namespace != null, "controller.Namespace != null");
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
      Debug.Assert(controller.Namespace != null, "controller.Namespace != null");
      var namespaceParts = controller.Namespace.Split('.').Select(x => x.Camelize()).ToArray();

      var idx = Array.IndexOf(namespaceParts, "api");

      var htmlFileName =
        controller.Name.Replace("Controller", extensionCapitalized).Humanize(LetterCasing.LowerCase).Replace(' ','.');

      var reduced = namespaceParts.Skip(idx + 1).Concat(new[] { htmlFileName }).ToArray();

      return Path.Combine(reduced);
    }
  }
}