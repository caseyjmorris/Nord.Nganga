using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Engine.Extensions;
using Nord.Nganga.Engine.Extensions.Reflection;
using Nord.Nganga.Engine.Extensions.Text;
using Nord.Nganga.Engine.Html;
using Nord.Nganga.Engine.Io;
using Nord.Nganga.Engine.JavaScript;
using Nord.Nganga.Engine.Mapping;
using Nord.CoreLib.Mvc;

namespace Nord.Nganga.Engine.Coordination
{
  public class GenerationCoordinator
  {
    private readonly HtmlGenerator htmlGenerator;
    private readonly ResourceGenerator resourceGenerator;
    private readonly ControllerGenerator controllerGenerator;
    private readonly NameSuggester nameSuggester;

    public GenerationCoordinator(HtmlGenerator htmlGenerator, ResourceGenerator resourceGenerator,
      ControllerGenerator controllerGenerator, NameSuggester nameSuggester)
    {
      this.htmlGenerator = htmlGenerator;
      this.resourceGenerator = resourceGenerator;
      this.controllerGenerator = controllerGenerator;
      this.nameSuggester = nameSuggester;
    }

    public CoordinationResult CoordinateUiGeneration(Type controller)
    {
      var result = new CoordinationResult
      {
        ControllerBody = this.controllerGenerator.GenerateController(controller),
        ResourceBody = this.resourceGenerator.GenerateResource(controller),
        ViewBody = this.CoordinateView(controller),
        ControllerPath = this.nameSuggester.SuggestControllerFileName(controller),
        ResourcePath = this.nameSuggester.SuggestResourceFileName(controller),
        ViewPath = this.nameSuggester.SuggestViewFileName(controller),
      };

      return result;
    }

    private string CoordinateView(Type controller)
    {
      var masterDiv = new TreeTagBuilder("div")
        .WithAttribute("data-ng-controller", controller.Name.Replace("Controller", "Ctrl").ToCamelCase());

      var endpoints = EndpointMapper.GetEnpoints(controller).Where(e => !e.ResourceOnly).ToList();

      var getEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Get).ToList();

      var importantEndpoints =
        endpoints.Where(
          e =>
            e.HttpMethod == EndpointViewModel.HttpMethodType.Get ||
            (e.ArgumentTypes.Any() && getEndpoints.All(gep => gep.ReturnType != e.ArgumentTypes.First())));

      var viewOnly = controller.HasAttribute<PresentAsViewOnlyDataAttribute>();

      var restrictedEdit = !viewOnly &&
                           endpoints.Any(
                             e =>
                               e.HttpMethod == EndpointViewModel.HttpMethodType.Post && e.SpecialAuthorization != null);

      if (viewOnly || restrictedEdit)
      {
        this.AppendViewOnlyView(masterDiv, restrictedEdit, getEndpoints);
      }

      if (!viewOnly)
      {
        this.AppendEditView(masterDiv, restrictedEdit, importantEndpoints);
      }

      return "<!-- GENERATED CODE -- " + DateTime.Now +
             " -- changes to this file may be lost if the code is regenerated -->\r\n" +
             this.htmlGenerator.PrettyPrint(masterDiv.ToString());
    }

    private void AppendViewOnlyView(TreeTagBuilder tag, bool restrictedEdit, IEnumerable<EndpointViewModel> getEndPoints)
    {
      var top = tag.AppendChild("div")
        .WithAttribute("id", "view-section")
        .WithOptionalAttributeIf("data-ng-show", "!canEdit", () => restrictedEdit);

      foreach (var getEndPoint in getEndPoints)
      {
        var coll = new List<TreeTagBuilder>();
        var vmMap = ViewModelMapper.GetViewModelViewModel(getEndPoint.ReturnType);

        this.AppendViewElementCommon(top, vmMap, false, coll, getEndPoint.SectionHeader, getEndPoint.ReturnsIEnumerable);

        foreach (var item in coll)
        {
          top.AppendChild(item);
        }
      }
    }

    private void AppendEditView(TreeTagBuilder tag, bool restrictedEdit, IEnumerable<EndpointViewModel> endPoints)
    {
      var top = tag.AppendChild("div")
        .WithAttribute("id", "edit-section")
        .WithOptionalAttributeIf("data-ng-show", "canEdit", () => restrictedEdit);

      foreach (var endPoint in endPoints)
      {
        var coll = new List<TreeTagBuilder>();

        var vmMap =
          ViewModelMapper.GetViewModelViewModel(endPoint.HttpMethod == EndpointViewModel.HttpMethodType.Get
            ? endPoint.ReturnType
            : endPoint.ArgumentTypes.First().GetNonnullableNonCollectionType());

        var enumerableArgumentType = false;

        if (endPoint.HttpMethod == EndpointViewModel.HttpMethodType.Post)
        {
          var arg = endPoint.ArgumentTypes.First();

          if (arg.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(arg.GetGenericTypeDefinition()) &&
              arg.GetGenericArguments()[0].Name.EndsWith("ViewModel"))
          {
            enumerableArgumentType = true;
          }
        }

        var isEnumerable = (endPoint.ReturnsIEnumerable && endPoint.HttpMethod == EndpointViewModel.HttpMethodType.Get) ||
                           enumerableArgumentType;

        this.AppendViewElementCommon(top, vmMap, true, coll, endPoint.SectionHeader, isEnumerable);

        foreach (var item in coll)
        {
          top.AppendChild(item);
        }
      }
    }

    private void AppendViewElementCommon(TreeTagBuilder tag, ViewModelViewModel vm, bool withEdit,
      ICollection<TreeTagBuilder> childFormsCollection, string header, bool asEnumerable)
    {
      if (!string.IsNullOrWhiteSpace(header))
      {
        tag
          .AppendFullWidthBootStrapRow()
          .AppendChild("h2")
          .WithInnerText(header);
      }

      tag
        .AppendFullWidthBootStrapRow()
        .AppendChild(asEnumerable
          ? this.htmlGenerator.GenerateHtmlDisplayTableTag(vm, withEdit, childFormsCollection)
          : this.htmlGenerator.GenerateHtmlFormTag(vm, withEdit, childFormsCollection));
    }
  }
}