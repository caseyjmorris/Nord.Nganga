using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.Fs
{
  public static class CoordinationExecutor
  {
    public static IEnumerable<string> GetControllerList(string assemblyFileName, bool resourceOnly,
      StringFormatProviderVisitor logHandler)
    {
      var l = new List<string>();
      using (var host = new Isolated<CoordinationFacade>())
      {
        host.Value.GetControllerList(assemblyFileName, resourceOnly, logHandler, l.Add);
      }
      return l;
    }

    public static CoordinationResult Coordinate(
      string assemblyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler)
    {
      var list = new List<CoordinationResult>(1);
      using (var host = new Isolated<CoordinationFacade>())
      {
        host.Value.Coordinate(assemblyFileName, fuzzyControllerTypeName, projectPath, resourceOnly, logHandler,
          list.Add);
      }

      return list.Single();
    }
  }
}