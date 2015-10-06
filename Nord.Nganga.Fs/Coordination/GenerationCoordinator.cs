using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Nord.Nganga.Annotations;
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
    private const string Controller = "Controller";

    public GenerationCoordinator(Action<object> modelVisitor = null)
    {
      this.sourceGenerator = new SourceGenerator(
        ConfigurationFactory.GetConfiguration<WebApiSettingsPackage>(),
        ConfigurationFactory.GetConfiguration<SystemPathSettingsPackage>(),
        modelVisitor);
      this.nameSuggester = new NameSuggester();
    }

    public GenerationCoordinator(
      WebApiSettingsPackage webApiSettings,
      SystemPathSettingsPackage pathSettings,
      Action<object> modelVisitor = null)
    {
      this.sourceGenerator = new SourceGenerator(webApiSettings, pathSettings, modelVisitor);
      this.nameSuggester = new NameSuggester();
    }

    public IEnumerable<string> GetControllerList(
      string assemlbyFileName,
      ICollection<string> logRecords,
      bool resourceOnly = false)
    {
      if (string.IsNullOrEmpty(assemlbyFileName)) return null;
      var allTypes = DependentTypeResolver.GetTypesFrom(assemlbyFileName,
        DependentTypeResolver.CreateResolveEventLogger((s, p) => logRecords.Add(string.Format(s, p))));

      if (!allTypes.Any())
      {
        return new List<string>();
      }

      var controllerTypes = allTypes.First().Assembly.FindWebApiControllers(
        "ApiController",
        true,
        true,
        !resourceOnly).ToList();

      return controllerTypes.Select(t => t.FullName);
    }


    public CoordinationResult Coordinate(
      string assemblyFileName,
      string fuzzyControllerTypeName,
      string projectPath,
      ICollection<string> logRecords,
      bool resourceOnly = false)
    {
      if (string.IsNullOrEmpty(assemblyFileName)) return null;
      var types = DependentTypeResolver.GetTypesFrom(assemblyFileName,
        DependentTypeResolver.CreateResolveEventLogger((s, p) => logRecords.Add(string.Format(s, p))));
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
        Controller =
          new GenerationResult(
            s => SourceParser.ParseFile(TemplateContext.Controller, s),
            () => this.sourceGenerator.GenerateController(controllerType),
            () => this.nameSuggester.SuggestControllerFileName(controllerType),
            vsProjectPath,
            assemblyOps.NgControllersPath),
        Resource =
          new GenerationResult(
            s => SourceParser.ParseFile(TemplateContext.Resource, s),
            () => this.sourceGenerator.GenerateResource(controllerType),
            () => this.nameSuggester.SuggestResourceFileName(controllerType),
            vsProjectPath,
            assemblyOps.NgResourcesPath),
        View =
          new GenerationResult(
            s => SourceParser.ParseFile(TemplateContext.View, s),
            () => this.sourceGenerator.GenerateView(controllerType),
            () => this.nameSuggester.SuggestViewFileName(controllerType),
            vsProjectPath,
            assemblyOps.NgViewsPath),
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
        Resource =
          new GenerationResult(
            s => SourceParser.ParseFile(TemplateContext.Resource, s),
            () => this.sourceGenerator.GenerateResource(controllerType),
            () => this.nameSuggester.SuggestResourceFileName(controllerType),
            vsProjectPath,
            assemblyOps.NgResourcesPath),
        VsProjectName = assemblyOps.CsProjectName,
        SourceAssemblyLocation = controllerType.Assembly.Location,
        ControllerTypeName = controllerType.FullName,
        VsProjectPath = vsProjectPath
      };

      return result;
    }
  }
}