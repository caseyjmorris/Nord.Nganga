using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Nord.AngularUiGen.StEngine.Test
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestMethod1()
    {
      var t = TemplateFactory.GetTemplate(TemplateFactory.Context.View, "t1");
      t.Add("model",new { lastName ="Lloyd", sex="male"} );
      var s = t.Render();
      Console.WriteLine(s);
    }
  }
}
