using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.StringTemplate;

namespace Nord.AngularUiGen.StEngine
{
  public class TemplateFactory
  {

    public enum Context { View, Controller, Resource}
    public static Template GetTemplate(Context context, string name, bool debug = false)
    {
      var fileName = string.Format(@"{0}/{1}.stg", Environment.CurrentDirectory, context);
      if (debug)
      {
        Console.WriteLine(fileName +":");
        Console.Write(File.ReadAllText(fileName));
        Console.WriteLine("---");
      }
      TemplateGroup group = new TemplateGroupFile(fileName);
      var template = group.GetInstanceOf(name);
      return template;
    }
  }
}
