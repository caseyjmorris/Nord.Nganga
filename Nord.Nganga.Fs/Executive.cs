using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.Fs
{
  public static class Executive
  {
    public static void Coordinate(
      string assemlbyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler,
      Action<CoordinationResult> resultAcceptor)
    {
      using (var host = new Isolated<CoordinatorHost>())
      {
        host.Value.Run(assemlbyFileName, fuzzyControllerTypeName, projectPath, resourceOnly, logHandler,
          resultAcceptor);
      }
    }
  }

  // hosts the coordination form in a separate app domain 
  // this ensures that any loaded assemblies are freed upon completion
  public class CoordinatorHost : MarshalByRefObject
  {
    public void Run(
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