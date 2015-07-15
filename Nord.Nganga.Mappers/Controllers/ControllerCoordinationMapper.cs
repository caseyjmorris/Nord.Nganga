using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Controllers
{
  public class ControllerCoordinationMapper
  {
    private readonly EndpointMapper endpointMapper;

    private readonly ViewModelMapper viewModelMapper;

    public ControllerCoordinationMapper(EndpointMapper endpointMapper, ViewModelMapper viewModelMapper)
    {
      this.endpointMapper = endpointMapper;
      this.viewModelMapper = viewModelMapper;
    }

    public ControllerCoordinatedInformationViewModel GetControllerCoordinatedInformationViewModel(Type controller)
    {
      if (!controller.HasAttribute<AngularRouteIdParameterAttribute>())
      {
        throw new Exception("Please attribute your controller with AngularRouteIdParameterAttribute.");
      }

      if (!controller.HasAttribute<AngularModuleNameAttribute>())
      {
        throw new Exception("Please attribute your controller with AngularModuleNameAttribute.");
      }


      IList<EndpointViewModel> getEndpoints;
      IEnumerable<EndpointViewModel> postEndpoints;
      IEnumerable<string> privilegedRoles;
      IEnumerable<ViewModelViewModel> complexTypes;

      this.ExamineEndpoints(controller, out getEndpoints, out postEndpoints, out complexTypes, out privilegedRoles);

      var model = new ControllerCoordinatedInformationViewModel
      {
        RouteIdParameter = controller.GetAttribute<AngularRouteIdParameterAttribute>().ParameterName,
        RouteIdParameterIsNullable =
          controller.GetAttributePropertyValueOrDefault<AngularRouteIdParameterAttribute, bool>(a => a.IsNullable),
        NgModuleName = controller.GetAttribute<AngularModuleNameAttribute>().ModuleName,
        NgControllerName = controller.Name.Replace("Controller", "Ctrl").ToCamelCase(),
        GetEndpoints = getEndpoints,
        PostEndpoints = postEndpoints,
        RetrievalTargetGetEndpoints = getEndpoints.Where(ge => ge.HasReturnValue),
        EditRestrictedToRoles = privilegedRoles.ToList(),
        ForViewOnlyData = controller.HasAttribute<PresentAsViewOnlyDataAttribute>(),
        ServiceName = controller.Name.Replace("Controller", "Service").ToCamelCase(),
        AdditionalNgServices =
          controller.GetAttributePropertyValueOrDefault<InjectAngularServicesAttribute, IEnumerable<string>>(
            a => a.Services),
        CommonRecordsWithResolvers = this.GetCommonRecordsWithResolvers(complexTypes.ToList())
      };

      model.EditRestricted = model.EditRestrictedToRoles.Any();

      model.HasCommonRecords = model.CommonRecordsWithResolvers.Any();

      return model;
    }

    private IEnumerable<KeyValuePair<string, string>> GetCommonRecordsWithResolvers(
      IList<ViewModelViewModel> complexTypes)
    {
      var targetScalars =
        complexTypes.SelectMany(ct => ct.Scalars)
          .Where(sc => sc.SelectCommon != null)
          .Select(sc => sc.SelectCommon);

      var targetPrimitiveCollections =
        complexTypes.SelectMany(ct => ct.PrimitiveCollections).Where(pc => pc.SelectCommon != null)
          .Select(pc => pc.SelectCommon);

      var targets = targetPrimitiveCollections.Concat(targetScalars);

      var result = new Dictionary<string, string>();

      foreach (var item in targets)
      {
        if (result.ContainsKey(item.QualifiedName))
        {
          if (result[item.QualifiedName] != item.ProviderExpression)
          {
            var errorMsg = string.Format("Conflicting definitions for common record {0}:  ```{1}``` and ```{2}```",
              item.QualifiedName, item.ProviderExpression, result[item.QualifiedName]);

            throw new InvalidOperationException(errorMsg);
          }
          continue;
        }
        result[item.QualifiedName] = item.ProviderExpression;
      }

      return result;
    }

    private void ExamineEndpoints(Type controller, out IList<EndpointViewModel> getEndpoints,
      out IEnumerable<EndpointViewModel> postEndpoints, out IEnumerable<ViewModelViewModel> complexTypes,
      out IEnumerable<string> privilegedRoles)
    {
      var endpoints = this.endpointMapper.GetEnpoints(controller).Where(e => !e.ResourceOnly).ToList();

      getEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Get).ToList();

      postEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post).ToList();

      var viewModels = endpoints.Where(e => e.HasReturnValue).Select(e => e.ReturnType).ToList();

      var vmHash = new HashSet<Type>(viewModels);

      var postEndpointTypes =
        endpoints.Where(
          e =>
            e.HttpMethod == EndpointViewModel.HttpMethodType.Post && e.ArgumentTypes.Any() &&
            !vmHash.Contains(e.ArgumentTypes.First()))
          .Select(e => e.ArgumentTypes.First())
          .Where(t => t.Name.EndsWith("ViewModel"));

      viewModels = viewModels.Concat(postEndpointTypes).Distinct().ToList();

      var vmVms = viewModels.Select(this.viewModelMapper.GetViewModelViewModel).ToList();

      privilegedRoles =
        endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post)
          .Select(e => e.SpecialAuthorization)
          .FirstOrDefault(a => a != null);

      complexTypes = vmVms.SelectMany(m => this.GetAllComplexSubordinates(m)).ToList();
    }

    private IEnumerable<ViewModelViewModel> GetAllComplexSubordinates(ViewModelViewModel model,
      HashSet<ViewModelViewModel> examined = null)
    {
      examined = examined ?? new HashSet<ViewModelViewModel>();

      foreach (var sub in model.ComplexCollections)
      {
        if (!examined.Contains(sub.Model))
        {
          this.GetAllComplexSubordinates(sub.Model, examined);
        }
      }

      examined.Add(model);

      return examined;
    }
  }
}