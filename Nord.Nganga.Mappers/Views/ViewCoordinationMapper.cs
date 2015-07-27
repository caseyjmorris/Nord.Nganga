using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Nord.Nganga.Annotations;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Views
{
  public class ViewCoordinationMapper
  {
    private readonly ViewModelMapper viewModelMapper;

    private readonly EndpointFilter endpointFilter;

    private readonly EndpointMapper endpointMapper;

    public ViewCoordinationMapper(
      ViewModelMapper viewModelMapper,
      EndpointFilter endpointFilter,
      EndpointMapper endpointMapper)
    {
      this.viewModelMapper = viewModelMapper;
      this.endpointFilter = endpointFilter;
      this.endpointMapper = endpointMapper;
    }

    public ViewCoordinationMapper(WebApiSettingsPackage settings)
    {
      var vmMapper = new ViewModelMapper(this);
      var filter = new EndpointFilter(vmMapper);

      this.viewModelMapper = vmMapper;

      this.endpointFilter = filter;

      this.endpointMapper = new EndpointMapper(settings);
    }

    public ViewCoordinationInformationCollectionViewModel GetViewCoordinatedInformationCollection(Type controller)
    {
      this.viewModelMapper.AssemblyOptions = new AssemblyOptionsModel(controller);

      var endpoints = this.endpointMapper.GetEnpoints(controller);

      var filteredInfo = this.endpointFilter.ExamineEndpoints(endpoints);

      var coordinatedInfo =
        filteredInfo.TargetComplexTypesAtRoot.Select(this.GetViewCoordinatedInformationSingleInternal).ToList();

      return new ViewCoordinationInformationCollectionViewModel
      {
        NgControllerName = controller.Name.Replace("Controller", "Ctrl").Camelize(),
        ViewCoordinatedInfo = coordinatedInfo,
        Header =
          controller.Name.Replace("Controller", string.Empty)
            .Humanize(CasingEnumMap.Instance[this.viewModelMapper.AssemblyOptions.GetOption(CasingOptionContext.Header)]),
      };
    }

    public ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingle(Type vmType, int depthMultiplier = 1)
    {
      var vmVm = this.viewModelMapper.GetViewModelViewModel(vmType);

      return this.GetViewCoordinatedInformationSingleInternal(vmVm, depthMultiplier);
    }

    private ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingleInternal(ViewModelViewModel vmVm,
      int depthMultipler)
    {
      var coord = new ViewCoordinatedInformationViewModel
      {
        SaveButtonText =
          ("Save changes to " + vmVm.Name.Humanize(LetterCasing.LowerCase)).Humanize(
            CasingEnumMap.Instance[this.viewModelMapper.AssemblyOptions.GetOption(CasingOptionContext.Button)]),
        Sections = this.SplitSections(vmVm, depthMultipler),
        Title =
          vmVm.Name.Humanize(
            CasingEnumMap.Instance[this.viewModelMapper.AssemblyOptions.GetOption(CasingOptionContext.Header)]),
        NgFormName = vmVm.Name.Camelize() + "Form",
        NgSubmitAction = string.Format("saveChangesTo{0}()", vmVm.Name.Pascalize()),
        ParentObjectName = vmVm.Name.Camelize(),
      };

      return coord;
    }

    private IEnumerable<ViewCoordinatedInformationViewModel.SectionViewModel> SplitSections(ViewModelViewModel vmVm,
      int depthMultipler)
    {
      depthMultipler = 1;
      var sections = new List<ViewCoordinatedInformationViewModel.SectionViewModel>();

      const int rowMax = 12;

      Dictionary<string, string> sectionAttrs;

      var currentSection = new ViewCoordinatedInformationViewModel.SectionViewModel
      {
        Title = vmVm.Members[0].Section,
        Rows = new List<ViewCoordinatedInformationViewModel.RowViewModel>()
      };

      if (
        vmVm.SectionHtmlAttributeValues.TryGetValue(
          currentSection.Title.ToUpperInvariant().Replace(@"\w", string.Empty), out sectionAttrs))
      {
        currentSection.Attributes = sectionAttrs;
      }

      sections.Add(currentSection);

      var currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
      {
        Members = new List<ViewModelViewModel.MemberWrapper>(),
      };

      currentSection.Rows.Add(currentRow);

      var currentRowWidth = 0;

      foreach (var member in vmVm.Members.Where(m => !m.IsHidden))
      {
        if (member.Section != currentSection.Title)
        {
          currentSection = new ViewCoordinatedInformationViewModel.SectionViewModel
          {
            Title = member.Section,
            Rows = new List<ViewCoordinatedInformationViewModel.RowViewModel>(),
          };

          if (
            vmVm.SectionHtmlAttributeValues.TryGetValue(
              currentSection.Title.ToUpperInvariant().Replace(@"\w", string.Empty), out sectionAttrs))
          {
            currentSection.Attributes = sectionAttrs;
          }

          currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
          {
            Members = new List<ViewModelViewModel.MemberWrapper>(),
          };

          currentSection.Rows.Add(currentRow);

          currentRowWidth = 0;

          sections.Add(currentSection);
        }

        var memberWidth = member.Width*depthMultipler;

        if (memberWidth > 12)
        {
          memberWidth = 12;
        }

        currentRowWidth += memberWidth;

        if (currentRowWidth > rowMax)
        {
          currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
          {
            Members = new List<ViewModelViewModel.MemberWrapper>(),
          };

          currentSection.Rows.Add(currentRow);

          currentRowWidth = memberWidth;
        }

        currentRow.Members.Add(member);
      }

      return sections;
    }
  }
}