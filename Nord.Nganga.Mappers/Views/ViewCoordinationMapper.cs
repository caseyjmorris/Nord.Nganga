using System;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Nord.Nganga.Core.Text;
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
        Header = controller.Name.Replace("Controller", string.Empty).ToSpaced() //TODO:  casing
      };
    }

    public ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingle(Type vmType)
    {
      var vmVm = this.viewModelMapper.GetViewModelViewModel(vmType);

      return this.GetViewCoordinatedInformationSingleInternal(vmVm);
    }

    private ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingleInternal(ViewModelViewModel vmVm)
    {
      var coord = new ViewCoordinatedInformationViewModel
      {
        //TODO:  case pref
        SaveButtonText = "Save changes to " + vmVm.Name.Humanize(LetterCasing.LowerCase),
        Sections = this.SplitSections(vmVm),
        Title = vmVm.Name.Humanize(LetterCasing.Sentence), //TODO:  CASING
        NgFormName = vmVm.Name.Camelize(),
        NgSubmitAction = string.Format("saveChangesTo{0}()", vmVm.Name.Camelize())
      };

      return coord;
    }

    private IEnumerable<ViewCoordinatedInformationViewModel.SectionViewModel> SplitSections(ViewModelViewModel vmVm)
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

        currentRowWidth += member.Width;

        if (currentRowWidth > rowMax)
        {
          currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
          {
            Members = new List<ViewModelViewModel.MemberWrapper>(),
          };

          currentSection.Rows.Add(currentRow);

          currentRowWidth = member.Width;
        }

        currentRow.Members.Add(member);
      }

      return sections;
    }
  }
}