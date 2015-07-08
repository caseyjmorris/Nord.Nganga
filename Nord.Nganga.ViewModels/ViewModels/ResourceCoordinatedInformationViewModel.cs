using System.Collections.Generic;

namespace Nord.Nganga.Models.ViewModels
{
  public class ResourceCoordinatedInformationViewModel
  {
    public string AppName { get; set; }
    public bool UseCache { get; set; }
    public string CustomCacheFactory { get; set; }
    public string ServiceName { get; set; }
    public IEnumerable<EndpointViewModel> GetEndpoints { get; set; }
    public IEnumerable<EndpointViewModel> PostEndpoints { get; set; }
    public string ControllerName { get; set; }
  }
}