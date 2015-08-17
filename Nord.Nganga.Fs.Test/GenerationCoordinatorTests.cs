using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.Fs.Test
{
  [TestClass]
  public class GenerationCoordinatorTests
  {
    [TestMethod]
    public void TestGetControllerList()
    {
      var log = new List<string>();
      var gc = new GenerationCoordinator();

      var controllerNameList = gc.GetControllerList( Globals.AssemblyFileName,  log).ToList();
      Assert.IsTrue(controllerNameList.Any(), "Did not find any controller type names!");
      Console.WriteLine($"Found {controllerNameList.Count()} controller type names.");
    }

        [TestMethod]
    public void TestCoordinate()
    {
      var log = new List<string>();
      var gc = new GenerationCoordinator();

      var controllerNameList = gc.GetControllerList( Globals.AssemblyFileName,  log).ToList();
      Assert.IsTrue(controllerNameList.Any(), "Did not find any controller type names!");
      Console.WriteLine($"Found {controllerNameList.Count()} controller type names.");

          var cr = gc.Coordinate(Globals.AssemblyFileName, controllerNameList.First(), Globals.ProjectPath, log);

          Assert.IsInstanceOfType(cr, typeof(CoordinationResult), "Failed to instantiate coordination result");
    }
  }
}