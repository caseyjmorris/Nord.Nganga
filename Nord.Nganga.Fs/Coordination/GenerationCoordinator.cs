using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Nord.Nganga.Core;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Fs.Naming;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.StEngine;

namespace Nord.Nganga.Fs.Coordination
{
  public class GenerationCoordinator
  {
    private readonly SourceGenerator sourceGenerator;
    private readonly NameSuggester nameSuggester;
    private Action<object> modelVisitor;
    private const string Controller = "Controller";

    public GenerationCoordinator(WebApiSettingsPackage webApiSettings, SystemPathSettingsPackage pathSettings,
      Action<object> modelVisitor = null)
    {
      this.sourceGenerator = new SourceGenerator(webApiSettings, pathSettings, modelVisitor);
      this.nameSuggester = new NameSuggester();
      this.modelVisitor = modelVisitor;
    }

    public IEnumerable<string> GetControllerList(
      string assemlbyFileName,
      StringFormatProviderVisitor logHandler)
    {
      if (string.IsNullOrEmpty(assemlbyFileName)) return null;
      var types = DependentTypeResolver.GetTypesFrom(assemlbyFileName,
        DependentTypeResolver.CreateResolveEventLogger(logHandler));
      return types.Select(t => t.FullName);
    }

    public CoordinationResult Coordinate(
      string assemlbyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      bool resourceOnly,
      StringFormatProviderVisitor logHandler)
    {
      if (string.IsNullOrEmpty(assemlbyFileName)) return null;
      var types = DependentTypeResolver.GetTypesFrom(assemlbyFileName,
        DependentTypeResolver.CreateResolveEventLogger(logHandler));
      var assy = types[0].Assembly;
      var type = ResolveController(assy, fuzzyControllerTypeName, resourceOnly);
      return this.Coordinate(type, projectPath, resourceOnly);
    }

    private static string StripControllerName(string name)
    {
      return Regex.Replace(name, $"{Controller}$", string.Empty).ToUpperInvariant();
    }

    private static IEnumerable<Type> GetEligibleWebApiControllers(Assembly asm, bool resourceOnly)
    {
      return asm.FindWebApiControllers("ApiController", true, true, assertAngularRouteIdParmAttribute: !resourceOnly);
    }

    private static Type ResolveController(Assembly asm, string fuzzyControllerTypeName, bool resourceOnly)
    {
      var scrubbed = StripControllerName(fuzzyControllerTypeName);

      var noNs = !scrubbed.Contains(".");

      var ctrl = GetEligibleWebApiControllers(asm, resourceOnly);

      var matching = (noNs
        ? ctrl.Where(c => StripControllerName(c.Name).StartsWith(scrubbed))
        : ctrl.Where(c => StripControllerName(c.FullName).EndsWith(scrubbed)))
        .ToList();

      if (matching.Count > 1)
      {
        throw new InvalidOperationException(
          $"Ambiguous match:  {fuzzyControllerTypeName} could refer to the following controllers in {asm.FullName}:  {string.Join(", ", matching.Select(c => c.FullName))}");
      }
      if (matching.Count == 0)
      {
        throw new KeyNotFoundException(
          $"No eligible controller matching {fuzzyControllerTypeName} could be found in {asm.FullName}");
      }

      return matching.Single();
    }

    private CoordinationResult Coordinate(Type controllerType, string projectPath, bool resourceOnly)
    {
      if (controllerType == null) return null;

      var wasp = ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>();
      var fileSettings = ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>();
      var coordinationResult =
        resourceOnly
          ? (new GenerationCoordinator(wasp, fileSettings)).CoordinateResourceGeneration(controllerType, projectPath)
          : (new GenerationCoordinator(wasp, fileSettings)).CoordinateUiGeneration(controllerType, projectPath)
        ;

      return coordinationResult;
    }

    public CoordinationResult CoordinateUiGeneration(Type controllerType, string vsProjectPath)
    {
      var assemblyOps = new AssemblyOptionsModel(controllerType);
      var result = new CoordinationResult
      {
        ControllerBody = this.sourceGenerator.GenerateController(controllerType),
        ResourceBody = this.sourceGenerator.GenerateResource(controllerType),
        ViewBody = this.sourceGenerator.GenerateView(controllerType),
        ControllerPath = this.nameSuggester.SuggestControllerFileName(controllerType),
        ResourcePath = this.nameSuggester.SuggestResourceFileName(controllerType),
        ViewPath = this.nameSuggester.SuggestViewFileName(controllerType),
        NgResourcesPath = assemblyOps.NgResourcesPath,
        NgControllersPath = assemblyOps.NgControllersPath,
        NgViewsPath = assemblyOps.NgViewsPath,
        VsProjectName = assemblyOps.CsProjectName,
        SourceAssemblyLocation = controllerType.Assembly.Location,
        ControllerTypeName = controllerType.FullName,
        VsProjectPath = vsProjectPath
      };

      return result;
    }

    public CoordinationResult CoordinateResourceGeneration(Type controllerType, string vsProjectPath)
    {
      var assemblyOps = new AssemblyOptionsModel(controllerType);
      var result = new CoordinationResult
      {
        ResourceBody = this.sourceGenerator.GenerateResource(controllerType),
        ResourcePath = this.nameSuggester.SuggestResourceFileName(controllerType),
        NgResourcesPath = assemblyOps.NgResourcesPath,
        VsProjectName = assemblyOps.CsProjectName,
        SourceAssemblyLocation = controllerType.Assembly.Location,
        ControllerTypeName = controllerType.FullName,
        VsProjectPath = vsProjectPath
      };

      return result;
    }
  }
}