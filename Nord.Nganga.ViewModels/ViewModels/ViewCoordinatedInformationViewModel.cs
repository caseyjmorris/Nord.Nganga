using System.Collections.Generic;
using System.Linq;

namespace Nord.Nganga.Models.ViewModels
{
  public class ViewCoordinatedInformationViewModel
  {
    public IEnumerable<SectionViewModel> Sections { get; set; }

    public string SaveButtonText { get; set; }

    public class RowViewModel
    {
      public ICollection<ViewModelViewModel.MemberWrapper> Members { get; set; }

      public override string ToString()
      {
        if (this.Members == null || !this.Members.Any())
        {
          return "(Row)";
        }

        return string.Join(", ", this.Members.Select(m => m.ToString()));
      }
    }

    public class SectionViewModel
    {
      public string Title { get; set; }
      public ICollection<RowViewModel> Rows { get; set; }

      public override string ToString()
      {
        if (string.IsNullOrEmpty(this.Title))
        {
          return "(No title)";
        }

        return this.Title;
      }
    }
  }
}