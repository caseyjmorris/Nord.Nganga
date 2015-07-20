using System;
using Nord.Nganga.Fs.Naming;
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

    public CoordinationResult CoordinateUiGeneration(Type controller)
    {
      var result = new CoordinationResult
      {
        ControllerBody = this.sourceGenerator.GenerateController(controller),
        ResourceBody = this.sourceGenerator.GenerateResource(controller),
        ViewBody = this.sourceGenerator.GenerateView(controller),
        ControllerPath = this.nameSuggester.SuggestControllerFileName(controller),
        ResourcePath = this.nameSuggester.SuggestResourceFileName(controller),
        ViewPath = this.nameSuggester.SuggestViewFileName(controller),
      };

      return result;
    }
  }
}