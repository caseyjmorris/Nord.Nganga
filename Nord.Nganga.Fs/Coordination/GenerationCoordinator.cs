﻿using System;
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

    public GenerationCoordinator(WebApiSettingsPackage webApiSettings, SystemPathSettingsPackage pathSettings,
      Action<object> modelVisitor = null)
    {
      this.sourceGenerator = new SourceGenerator(webApiSettings, pathSettings, modelVisitor);
      this.nameSuggester = new NameSuggester();
      this.modelVisitor = modelVisitor;
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