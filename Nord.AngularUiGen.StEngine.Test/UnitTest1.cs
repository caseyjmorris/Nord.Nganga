using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nord.AngularUiGen.StEngine.Test
{
  [TestClass]
  public class UnitTest1
  {

    [TestMethod]
    public void TestMethod3()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.View, "t1");
      t.Add("model", new { people = new[] { new { lastName = "Zhao", sex = "female" }, new { lastName = "Morris", sex = "male" } } });
      var s = t.Render();
      Console.WriteLine(s);
    }
  }
}
