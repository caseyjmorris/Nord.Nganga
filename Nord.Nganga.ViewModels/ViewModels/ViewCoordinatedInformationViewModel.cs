using System.Collections.Generic;

namespace Nord.Nganga.Models.ViewModels
{
  public class ViewCoordinatedInformationViewModel
  {
    public IEnumerable<SectionViewModel> Sections { get; set; }

    public string SaveButtonText { get; set; }

    public class RowViewModel
    {
      public ICollection<ViewModelViewModel.MemberWrapper> Members { get; set; }
    }

    public class SectionViewModel
    {
      public string Title { get; set; }
      public ICollection<RowViewModel> Rows { get; set; }
    }
  }
}