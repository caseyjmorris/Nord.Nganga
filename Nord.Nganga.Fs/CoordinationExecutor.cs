using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.Fs
{
  public static class CoordinationExecutor
  {
    public static IEnumerable<string> GetControllerList(
    string assemblyFileName, 
      ICollection<string> logRecords,
      bool resourceOnly = false )
    {
      using (var host = new Isolated<CoordinationFacade>())
      {
        var result = host.Value.GetControllerList(assemblyFileName, logRecords, resourceOnly);
        return result;
      }
    }

    public static CoordinationResult Coordinate(
      string assemblyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      ICollection<string> logRecords,
      bool resourceOnly = false )
    {
      using (var host = new Isolated<CoordinationFacade>())
      {
        var result = host.Value.Coordinate(
        assemblyFileName, 
        fuzzyControllerTypeName, 
        projectPath, 
        logRecords,
        resourceOnly  );
        return result ; 
      }

    }
  }
}