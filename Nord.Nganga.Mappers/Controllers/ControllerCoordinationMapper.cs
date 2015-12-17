using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Controllers
{
  public class ControllerCoordinationMapper
  {
    private readonly EndpointMapper endpointMapper;

    private readonly EndpointFilter endpointFilter;

    public ControllerCoordinationMapper(EndpointMapper endpointMapper, EndpointFilter endpointFilter)
    {
      this.endpointMapper = endpointMapper;
      this.endpointFilter = endpointFilter;
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


      var endpoints = this.endpointMapper.GetEnpoints(controller);

      var filteredInfo = this.endpointFilter.ExamineEndpoints(endpoints);
      var crObjects = new HashSet<string>();

      var model = new ControllerCoordinatedInformationViewModel
      {
        RouteIdParameter = controller.GetAttribute<AngularRouteIdParameterAttribute>().ParameterName,
        RouteIdParameterIsNullable =
          controller.GetAttributePropertyValueOrDefault<AngularRouteIdParameterAttribute, bool>(a => a.IsNullable),
        NgModuleName = controller.GetAttribute<AngularModuleNameAttribute>().ModuleName,
        NgControllerName = controller.Name.Replace("Controller", "Ctrl").Camelize(),
        GetEndpoints = filteredInfo.GetEndpoints,
        PostEndpoints = filteredInfo.PostEndpoints,
        RetrievalTargetGetEndpoints = filteredInfo.GetEndpoints.Where(ge => ge.HasReturnValue),
        EditRestrictedToRoles = filteredInfo.PrivilegedRoles?.ToList(),
        ForViewOnlyData = controller.HasAttribute<PresentAsViewOnlyDataAttribute>(),
        ServiceName = controller.Name.Replace("Controller", "Service").Camelize(),
        AdditionalNgServices =
          controller.GetAttributePropertyValueOrDefault<InjectAngularServicesAttribute, IEnumerable<string>>(
            a => a.Services),
        CommonRecordsWithResolvers =
          this.GetCommonRecordsWithResolvers(filteredInfo.TargetComplexTypesWithChildren.ToList(), crObjects),
        DocumentTypeSourceExpressions =
          this.GetDocumentTypeSourceExpressions(filteredInfo.TargetComplexTypesWithChildren).ToList(),
        CommonRecordObjects = crObjects,
        InjectedJavaScript = this.GetInjectedJavaScriptDictionary(controller),
      };

      model.EditRestricted = model.EditRestrictedToRoles != null && model.EditRestrictedToRoles.Any();

      model.HasCommonRecords = model.CommonRecordsWithResolvers.Any();

      return model;
    }

    private Dictionary<string, string> GetCommonRecordsWithResolvers(
      IList<ViewModelViewModel> complexTypes, HashSet<string> crObjects)
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
            var errorMsg =
              $"Conflicting definitions for common record {item.QualifiedName}:  ```{item.ProviderExpression}``` and ```{result[item.QualifiedName]}```";

            throw new InvalidOperationException(errorMsg);
          }
          continue;
        }
        result[item.QualifiedName] = item.ProviderExpression;
        crObjects.Add(item.ObjectName);
      }

      return result;
    }

    public IEnumerable<string> GetDocumentTypeSourceExpressions(IEnumerable<ViewModelViewModel> complexTypes)
    {
      var typeSourceProviders =
        complexTypes.SelectMany(ct => ct.Scalars)
          .Where(s => s.DocumentTypeSourceProvider != null)
          .GroupBy(s => s.DocumentTypeSourceProvider);

      foreach (var typeSourceProvider in typeSourceProviders)
      {
        var sb = new StringBuilder();

        foreach (var member in typeSourceProvider.Distinct())
        {
          sb.Append("$scope.")
            .Append(member.UniqueId)
            .Append("TypeSource = ");
        }

        sb.Append(typeSourceProvider.Key);

        yield return sb.ToString();
      }
    }

    private Dictionary<string, IEnumerable<string>> GetInjectedJavaScriptDictionary(Type controller)
    {
      var attrs = controller.GetCustomAttributes(inherit: true).OfType<InjectJavaScriptAttribute>();

      var grouped = attrs.GroupBy(a => a.ControllerPosition.ToString());

      return grouped.ToDictionary(g => g.Key, g => (IEnumerable<string>) g.Select(i => i.Content).ToList());
    }
  }
}