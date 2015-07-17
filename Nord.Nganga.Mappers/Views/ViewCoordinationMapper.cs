using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Views
{
  public class ViewCoordinationMapper
  {
    private readonly ViewModelMapper viewModelMapper;

    private readonly EndpointFilter endpointFilter;

    private readonly EndpointMapper endpointMapper;

    public ViewCoordinationMapper(ViewModelMapper viewModelMapper, EndpointFilter endpointFilter,
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
      var endpoints = this.endpointMapper.GetEnpoints(controller);

      var filteredInfo = this.endpointFilter.ExamineEndpoints(endpoints);

      var coordinatedInfo =
        filteredInfo.TargetComplexTypesAtRoot.Select(this.GetViewCoordinatedInformationSingleInternal).ToList();

      return new ViewCoordinationInformationCollectionViewModel
      {
        NgControllerName = controller.Name.Replace("Controller", string.Empty).ToCamelCase(),
        ViewCoordinatedInfo = coordinatedInfo,
        Header = controller.Name.Replace("Controller", string.Empty).ToSpaced(), //TODO:  casing
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
        //TODO:  case pref
        SaveButtonText = "Save changes to " + vmVm.Name.Humanize(LetterCasing.LowerCase),
        Sections = this.SplitSections(vmVm, depthMultipler),
        Title = vmVm.Name.Humanize(LetterCasing.Sentence), //TODO:  CASING
        NgFormName = vmVm.Name.Camelize() + "Form",
        NgSubmitAction = string.Format("saveChangesTo{0}()", vmVm.Name.Pascalize()),
        ParentObjectName = vmVm.Name.Camelize(),
      };

      return coord;
    }

    private IEnumerable<ViewCoordinatedInformationViewModel.SectionViewModel> SplitSections(ViewModelViewModel vmVm,
      int depthMultipler)
    {
      var sections = new List<ViewCoordinatedInformationViewModel.SectionViewModel>();

      const int rowMax = 12;

      var currentSection = new ViewCoordinatedInformationViewModel.SectionViewModel
      {
        Title = vmVm.Members[0].Section,
        Rows = new List<ViewCoordinatedInformationViewModel.RowViewModel>()
      };

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