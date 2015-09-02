using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nord.Nganga.Commands.Test
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestListControllerNames()
    {
      var x =
        Commands.ListControllerNames(
          @"C:\Users\mlloyd\Source\Repos\Nord.RareCare\NORD.RareCare.Presentation.Web\bin\Nord.RareCare.Presentation.Web.dll");
      if (!string.IsNullOrEmpty(x.MessageText))
      {
        Console.WriteLine(x.MessageText);
      }
      else
      {
        foreach (var n in x.TypeNames)
        {
          Console.WriteLine(n);
        }
      }
    }

    [TestMethod]
    public void TestGenerate()
    {
      var x =
        Commands.Generate(
          @"C:\Users\mlloyd\Source\Repos\Nord.RareCare\NORD.RareCare.Presentation.Web\bin\Nord.RareCare.Presentation.Web.dll",
          "badControllerName", "bad path");
      if (!string.IsNullOrEmpty(x.MessageText))
      {
        Console.WriteLine(x.MessageText);
      }
      else
      {
        Console.WriteLine(x.CoordinationResult.ControllerText);
      }
    }
  }
}