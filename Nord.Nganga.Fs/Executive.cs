using System;
using System.Collections.Generic;
using System.Linq;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.Fs
{
  public static class Executive
  {
    public IEnumerable<string> GetControllerList(string assemlbyFileName, StringFormatProviderVisitor logHandler)
    {
      var l = new List<string>();
      using (var host = new Isolated<Host>())
      {
        host.Value.GetControllerList(assemlbyFileName, logHandler, l.Add);
      }
      return l;
    }

    public static void Coordinate(
      string assemlbyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler,
      Action<CoordinationResult> resultAcceptor)
    {
      using (var host = new Isolated<Host>())
      {
        host.Value.Coordinate(assemlbyFileName, fuzzyControllerTypeName, projectPath, resourceOnly, logHandler,
          resultAcceptor);
      }
    }
  }

  // hosts the coordination form in a separate app domain 
  // this ensures that any loaded assemblies are freed upon completion
  public class Host : MarshalByRefObject
  {
      public void GetControllerList(
      string assemlbyFileName,
      StringFormatProviderVisitor logHandler,
      Action<string> acceptor)
    {
      var wasp = ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();
      var fileSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
      var g = new GenerationCoordinator(wasp, fileSettings);
      var r = g.GetControllerList(assemlbyFileName, logHandler);
      r.ToList().ForEach(acceptor);
    }
    public void Coordinate(
      string assemlbyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler,
      Action<CoordinationResult> resultAcceptor)
    {
      var wasp = ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();
      var fileSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
      var g = new GenerationCoordinator(wasp, fileSettings);
      var r = g.Coordinate(assemlbyFileName, fuzzyControllerTypeName, projectPath, resourceOnly, logHandler);
      resultAcceptor(r);
    }
  }
}