using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Nord.Nganga.Fs.VsIntegration
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

    private XDocument GetModifiedXml(Stream originalXml, IEnumerable<string> relativePaths,
      Action<string> sourceNameVisitor)
    {
      var xdoc = XDocument.Load(originalXml);

      var proj = xdoc.Root;

      var ns = proj.GetDefaultNamespace();

      var itemGroups = proj.Elements(ns + "ItemGroup").ToList();

      var targetItemGroup = itemGroups.FirstOrDefault(i => i.Elements().Any(e => e.Name == ns + "Content"));

      if (targetItemGroup == null)
      {
        targetItemGroup = new XElement(ns + "ItemGroup");

        proj.Add(targetItemGroup);
      }

      var integratedNames = new HashSet<string>();

      // Clean up dupes...
      foreach (var el in itemGroups.Elements(ns + "Content"))
      {
        var path = el.Attribute("Include").Value.ToUpperInvariant();

        if (!integratedNames.Add(path))
        {
          el.Remove();
        }
      }

      foreach (var relativePath in relativePaths)
      {
        if (integratedNames.Contains(relativePath.ToUpperInvariant()))
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