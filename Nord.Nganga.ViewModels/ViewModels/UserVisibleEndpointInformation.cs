using System.Collections.Generic;

namespace Nord.Nganga.Models.ViewModels
{
  public class UserVisibleEndpointInformation
  {
    public IEnumerable<EndpointViewModel> GetEndpoints { get; set; }
    public IEnumerable<EndpointViewModel> PostEndpoints { get; set; }
    public IEnumerable<ViewModelViewModel> TargetComplexTypesAtRoot { get; set; }
    public IEnumerable<ViewModelViewModel> TargetComplexTypesWithChildren { get; set; }
    public IEnumerable<string> PrivilegedRoles { get; set; }
  }
}