using System;
using System.Linq;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.Fs
{
  /// <summary>
  /// 
  // hosts the coordination form in a separate app domain 
  // this ensures that any loaded assemblies are freed upon completion
  /// </summary>
  public class CoordinationFacade : MarshalByRefObject
  {
      public void GetControllerList(
      string assemblyFileName,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler,
      Action<string> acceptor)
    {
      var wasp = ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();
      var fileSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
      var g = new GenerationCoordinator(wasp, fileSettings);
      var r = g.GetControllerList(assemblyFileName, resourceOnly, logHandler);
      r.ToList().ForEach(acceptor);
    }

    public void Coordinate(
      string assemblyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler,
      Action<CoordinationResult> resultAcceptor)
    {
      var wasp = ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();
      var fileSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
      var g = new GenerationCoordinator(wasp, fileSettings);
      var r = g.Coordinate(assemblyFileName, fuzzyControllerTypeName, projectPath, resourceOnly, logHandler);
      resultAcceptor(r);
    }
  }
}