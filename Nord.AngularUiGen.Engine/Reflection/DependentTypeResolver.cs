using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nord.AngularUiGen.Engine.Reflection
{

  /// <summary>
  /// the primary purpose of this is to remove the need to statically bind System.Web.Http to this library
  /// statically binding caused failures on both ResourceGenerator and ControllerFinder when the subject
  /// assembly used different System.Web.Http versions than those statically bound to this tool 
  /// </summary>
  public static class DependentTypeResolver
  {

    public static Type[] GetTypesFrom(
    string fileName, 
    Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly> resolveEventVistior = null)
    {
      // determine where the assembly lives on disk... 
      // we may need this path later in the assemblyResolutionHandler 
      // to deal with any dependencies Fusion cannot work out on it's own
      var basePath = Path.GetDirectoryName(fileName) ?? Environment.CurrentDirectory;

      // declare this a anonymous to explout the closure on the basePathDirectoryInfo
      var assemblyResolutionHandler = CreateResolutionEventHandler(basePath, resolveEventVistior);

      // set up the dependency resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve += assemblyResolutionHandler;

      // load the assembly
      // note that this call does NOT trigger fusion to load dependent assemblies.... 
      var assy = Assembly.LoadFile(fileName);

      var types = assy.GetTypes();
      foreach (var t in types)
      {
        var x = t.GetConstructors();
      }

      // de-register the assembly resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolutionHandler;

      // finally done ... 

      return types;
    }

    public static Type GetTypeByName(
    Assembly assembly,
    string typeName,
    string expectedDependencyName = "System.Web.Http")
    {
      Type type = null;
      string basePath;

      var assemblyResolutionHandler = CreateResolutionEventHandler(assembly, out basePath);

      // set up the dependency resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve += assemblyResolutionHandler;

      // see if this assembly is dependent upon our expectedDependencyName (default is System.Web.Http)
      var systemWebHttpAssemblyName = assembly.GetReferencedAssemblies().FirstOrDefault(a => a.Name.Equals(expectedDependencyName, StringComparison.InvariantCultureIgnoreCase));

      // if it's not, then there our type is probably not inside 
      if (systemWebHttpAssemblyName != null)
      {
        // get the file name 
        var systemWebHttpAssemblyFileName = Path.Combine(basePath, systemWebHttpAssemblyName.Name + ".dll");

        // load the assembly
        // note that this call does NOT trigger fusion to load dependent assemblies.... 
        var referencedWebAssy = Assembly.LoadFile(systemWebHttpAssemblyFileName);

        // attempt to load the type 
        // note this call (and not the LoadFile call above) triggers loading of assembly dependencies by Fusion... 
        type = referencedWebAssy.GetTypes().FirstOrDefault(t => t.Name.Equals(typeName));
      }
      // de-register the assembly resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolutionHandler;

      // finally done ... 

      return type;
    }

    public static ResolveEventHandler CreateResolutionEventHandler(Assembly assembly)
    {
      string p;
      var h = CreateResolutionEventHandler(assembly, out p);
      return h;
    }

    public static ResolveEventHandler CreateResolutionEventHandler(
    Assembly assembly, 
    out string basePath,
    Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly> resolveEventVistior = null)
    {
      // determine where the assembly lives on disk... 
      // we may need this path later in the assemblyResolutionHandler 
      // to deal with any dependencies Fusion cannot work out on it's own
      basePath = Path.GetDirectoryName(assembly.Location) ?? Environment.CurrentDirectory;

      return CreateResolutionEventHandler(basePath, resolveEventVistior); 
    }

    public static ResolveEventHandler CreateResolutionEventHandler(
    string basePath,
    Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly> resolveEventVistior = null)
    {
      // determine where the assembly lives on disk... 
      // we may need this path later in the assemblyResolutionHandler 
      // to deal with any dependencies Fusion cannot work out on it's own
      basePath = basePath ?? Environment.CurrentDirectory;

      // get a dir object on the base path 
      var basePathDirectoryInfo = new DirectoryInfo(basePath);

      // declare this a anonymous to explout the closure on the basePathDirectoryInfo
      ResolveEventHandler assemblyResolutionHandler = (o, args) =>
      {
        // get the file name of the assembly 
        var assyFileName = new AssemblyName(args.Name).Name;
        var module = basePathDirectoryInfo.GetFiles().FirstOrDefault(i => i.Name == assyFileName + ".dll");
        var assy = module != null ? Assembly.LoadFrom(module.FullName) : null;
        if (resolveEventVistior != null)
        {
          resolveEventVistior(args, basePathDirectoryInfo, module, assy);
        }
        return assy;
      };

      return assemblyResolutionHandler;
    }
  }
}
