using System;
using System.IO;
using Antlr4.StringTemplate;

namespace Nord.Nganga.StEngine
{
  public static class TemplateFactory
  {
    public enum Context
    {
      View,
      Controller,
      Resource
    }

    public static Template GetTemplate(Context context, string name, bool debug = false)
    {
      var fileName = string.Format(@"{0}/templates/{1}.stg", Environment.CurrentDirectory, context);
      if (debug)
      {
        Console.WriteLine(fileName + ":");
        Console.WriteLine(File.ReadAllText(fileName));
        Console.WriteLine("---");
      }
      TemplateGroup group = new TemplateGroupFile(fileName);
      var template = group.GetInstanceOf(name);
      return template;
    }
  }
}