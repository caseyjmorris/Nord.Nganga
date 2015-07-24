using System;
using System.IO;
using Antlr4.StringTemplate;
using Nord.Nganga.Models.Configuration;

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

    public static Template GetTemplate(SystemPathSettingsPackage settings, Context context, string name,
      bool debug = false)
    {
      var filename = Path.Combine(settings.TemplatesDirectory, context.ToString() + ".stg");
      if (debug)
      {
        Console.WriteLine(filename + ":");
        Console.WriteLine(File.ReadAllText(filename));
        Console.WriteLine("---");
      }
      TemplateGroup group = new TemplateGroupFile(filename);
      var template = group.GetInstanceOf(name);
      return template;
    }
  }
}