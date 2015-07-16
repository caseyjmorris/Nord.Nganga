using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers
{
  public class EndpointFilter
  {
    private readonly ViewModelMapper viewModelMapper;

    public EndpointFilter(ViewModelMapper viewModelMapper)
    {
      this.viewModelMapper = viewModelMapper;
    }

    public UserVisibleEndpointInformation ExamineEndpoints(IEnumerable<EndpointViewModel> endpoints)
    {
      var rtn = new UserVisibleEndpointInformation();

      endpoints = endpoints.Where(e => !e.ResourceOnly).ToList();

      rtn.GetEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Get).ToList();

      rtn.PostEndpoints = endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post).ToList();

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

      rtn.TargetComplexTypesAtRoot = vmVms;

      rtn.PrivilegedRoles =
        endpoints.Where(e => e.HttpMethod == EndpointViewModel.HttpMethodType.Post)
          .Select(e => e.SpecialAuthorization)
          .FirstOrDefault(a => a != null);

      rtn.TargetComplexTypesWithChildren = vmVms.SelectMany(m => this.GetAllComplexSubordinates(m)).ToList();

      return rtn;
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