﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace Nord.Nganga.Core.Reflection
{
  /// <summary>
  /// the primary purpose of this is to remove the need to statically bind System.Web.Http to this library
  /// statically binding caused failures on both ResourceGenerator and ControllerFinder when the subject
  /// assembly used different System.Web.Http versions than those statically bound to this tool 
  /// </summary>
  public static class DependentTypeResolver
  {
    public static Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly> ResolveEventVisitor { get; set; }

    public static Action<ResolveEventArgs, DirectoryInfo, FileInfo, Assembly>
      CreateResolveEventLogger(StringFormatProviderVisitor formattingLogger)
    {
        return (args, dirInfo, fileInfo, assy) =>
          formattingLogger(
            "RESOLVE - for:{2}" +
            "{0}{1}On behalf of:{3}" +
            "{0}{1}Base dir:{4}" +
            "{0}{1}Module:{5}" +
            "{0}{1}Result Assy:{6}",
            '\n',
            '\t',
            args.Name,
            args.RequestingAssembly.FullName,
            dirInfo.FullName,
            fileInfo.FullName,
            assy == null ? "- RESOLVE FAILED!" : assy.Location);
    }

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
      var systemWebHttpAssemblyName =
        assembly.GetReferencedAssemblies()
          .FirstOrDefault(a => a.Name.Equals(expectedDependencyName, StringComparison.InvariantCultureIgnoreCase));

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
        try
        {
          type = referencedWebAssy.GetTypes().FirstOrDefault(t => t.Name.Equals(typeName));
        }
        catch (ReflectionTypeLoadException reflectionTypeLoadException)
        {
          throw reflectionTypeLoadException.LoaderExceptions.First();
        }
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
        resolveEventVistior?.Invoke(args, basePathDirectoryInfo, module, assy);
        ResolveEventVisitor?.Invoke(args, basePathDirectoryInfo, module, assy);
        return assy;
      };

      return assemblyResolutionHandler;
    }

    public static object[] GetCustomAssemblyAttributes<T>(Assembly assy)
    {

      // determine where the assembly lives on disk... 
      // we may need this path later in the assemblyResolutionHandler 
      // to deal with any dependencies Fusion cannot work out on it's own
      var basePath = Path.GetDirectoryName(assy.Location);

      // declare this a anonymous to explout the closure on the basePathDirectoryInfo
      var assemblyResolutionHandler = CreateResolutionEventHandler(basePath);

      // set up the dependency resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve += assemblyResolutionHandler;

      var attributeList = assy.GetCustomAttributes(typeof(T), true);

      // de-register the assembly resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve -= assemblyResolutionHandler;

      return attributeList;
    }
  }
}