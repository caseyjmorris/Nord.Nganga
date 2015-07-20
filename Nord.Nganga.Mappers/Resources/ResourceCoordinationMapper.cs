using System;
using System.Linq;
using Humanizer;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Resources
{
  public class ResourceCoordinationMapper
  {
    private readonly EndpointMapper endpointMapper;

    public ResourceCoordinationMapper(EndpointMapper endpointMapper)
    {
      this.endpointMapper = endpointMapper;
    }

    public ResourceCoordinatedInformationViewModel GetResourceCoordinationInformationViewModel(Type controller)
    {
      var endpoints = this.endpointMapper.GetEnpoints(controller).ToList();

      return new ResourceCoordinatedInformationViewModel
      {
        AppName = controller.GetAttribute<AngularModuleNameAttribute>().ModuleName,
        UseCache =
          controller.HasAttribute<UseAngularLocalCacheAttribute>() ||
          controller.HasAttribute<UseAngularGlobalCacheAttribute>(),
        UseCustomCache = controller.HasAttribute<UseAngularLocalCacheAttribute>(),
        CustomCacheFactory =
          controller.HasAttribute<UseAngularLocalCacheAttribute>()
            ? controller.Name.Replace("Controller", string.Empty).Camelize()
            : null
        ,
        GetEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Get),
        PostEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post),
        ControllerName = controller.Name.Replace("Controller", string.Empty),
      };
    }
  }
}