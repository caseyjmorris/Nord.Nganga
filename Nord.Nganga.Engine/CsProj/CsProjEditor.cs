using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Nord.Nganga.Engine.CsProj
{
  public class CsProjEditor
  {
    public void AddFileToCsProj(string csProjPath, IEnumerable<string> sourceFiles, Action<string> sourceNameVisitor)
    {
      var csProjLoc = Path.GetDirectoryName(csProjPath);

      if (csProjLoc == null)
      {
        throw new InvalidOperationException("csprojPath must be a fully qualified path");
      }
      var relativePaths = sourceFiles.Select(fp => fp.Replace(csProjLoc, string.Empty).TrimStart('\\'));

      XDocument xml;

      using (var csproj = new FileStream(csProjPath, FileMode.Open))
      {
        xml = this.GetModifiedXml(csproj, relativePaths, sourceNameVisitor);
      }

      File.WriteAllText(csProjPath, xml.Declaration + "\r\n" + xml.ToString());
    }

    private XDocument GetModifiedXml(Stream originalXml, IEnumerable<string> relativePaths, Action<string> sourceNameVisitor)
    {
      var xdoc = XDocument.Load(originalXml);

      var proj = xdoc.Root;

      var ns = proj.GetDefaultNamespace();

      var itemGroups = proj.Elements(ns + "ItemGroup");

      var targetItemGroup = itemGroups.Single(i => i.Elements().Any(e => e.Name == ns + "Content"));

      foreach (var relativePath in relativePaths)
      {
        if (targetItemGroup.Elements(ns + "Content").Any(e => e.Attribute("Include").Value == relativePath))
        {
          continue;
        }
        targetItemGroup.Add(new XElement(ns + "Content", new XAttribute("Include", relativePath)));
        if (sourceNameVisitor != null)
        {
          sourceNameVisitor(relativePath);
        }
      }

      return xdoc;
    }
  }
}