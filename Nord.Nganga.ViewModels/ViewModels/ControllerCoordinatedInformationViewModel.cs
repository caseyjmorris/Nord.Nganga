﻿using System.Collections.Generic;

namespace Nord.Nganga.Models.ViewModels
{
  public class ControllerCoordinatedInformationViewModel
  {
    public string NgModuleName { get; set; }
    public string RouteIdParameter { get; set; }
    public bool RouteIdParameterIsNullable { get; set; }
    public string ServiceName { get; set; }
    public IEnumerable<EndpointViewModel> GetEndpoints { get; set; }
    public IEnumerable<EndpointViewModel> PostEndpoints { get; set; }
    public string NgControllerName { get; set; }
    public IEnumerable<string> AdditionalNgServices { get; set; }
    public bool ForViewOnlyData { get; set; }
    public bool EditRestricted { get; set; }
    public IEnumerable<string> EditRestrictedToRoles { get; set; }
    public IEnumerable<KeyValuePair<string, string>> CommonRecordsWithResolvers { get; set; }
  }
}