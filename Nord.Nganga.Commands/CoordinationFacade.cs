using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.Commands
{
  /// <summary>
  /// 
  // hosts the coordination form in a separate app domain 
  // this ensures that any loaded assemblies are freed upon completion
  /// </summary>
  [Serializable]
  public class CoordinationFacade : MarshalByRefObject
  {
    public IEnumerable<string> GetControllerList(
      string assemblyFileName,
      ICollection<string> logRecords,
      bool resourceOnly = false 
     )
    {
      var g = new GenerationCoordinator();
      var result = g.GetControllerList(assemblyFileName, logRecords, resourceOnly ).ToList();
      return result; 
    }

    public CoordinationResult Coordinate(
      string assemblyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
       ICollection<string> logRecords,
       bool resourceOnly = false )
    {
      var g = new GenerationCoordinator();
      var result = g.Coordinate(assemblyFileName, fuzzyControllerTypeName, projectPath, logRecords, resourceOnly );
      return result;
    }
  }
}