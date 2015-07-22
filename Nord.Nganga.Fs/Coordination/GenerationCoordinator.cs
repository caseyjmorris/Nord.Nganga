using System;
using Nord.Nganga.Fs.Naming;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;

namespace Nord.Nganga.Fs.Coordination
{
  public class GenerationCoordinator
  {
    private readonly SourceGenerator sourceGenerator ;
    private readonly NameSuggester nameSuggester;

    public GenerationCoordinator(WebApiSettingsPackage settingsPackage)
    {
      this.sourceGenerator = new SourceGenerator(settingsPackage);
      this.nameSuggester = new NameSuggester();
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