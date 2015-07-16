using System;
using System.Collections.Generic;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Views
{
  public class ViewCoordinationMapper
  {
    private readonly ViewModelMapper viewModelMapper;

    public ViewCoordinationMapper(ViewModelMapper viewModelMapper)
    {
      this.viewModelMapper = viewModelMapper;
    }

    public IEnumerable<ViewCoordinatedInformationViewModel> GetViewCoordinatedInformationCollection(Type controller)
    {
      throw new NotImplementedException();
    }

    public ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingle(Type vmType)
    {
      var vmVm = this.viewModelMapper.GetViewModelViewModel(vmType);

      var coord = new ViewCoordinatedInformationViewModel
      {
        //TODO:  case pref
        SaveButtonText = "Save changes to " + vmType.GetFriendlyName().ToSpaced().ToLower(),
        Sections = this.SplitSections(vmVm),
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

      foreach (var member in vmVm.Members)
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