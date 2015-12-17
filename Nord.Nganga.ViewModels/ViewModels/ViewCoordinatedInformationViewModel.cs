using System.Collections.Generic;
using System.Linq;

namespace Nord.Nganga.Models.ViewModels
{
  public class ViewCoordinationInformationCollectionViewModel
  {
    public string NgControllerName { get; set; }
    public string Header { get; set; }
    public IEnumerable<ViewCoordinatedInformationViewModel> ViewCoordinatedInfo { get; set; }
    public bool EditRestricted { get; set; }
  }

  public class ViewCoordinatedInformationViewModel
  {
    public ViewCoordinatedInformationViewModel()
    {
      this.NgFormAttributes = new Dictionary<string, string>();
    }

    public ViewModelViewModel ViewModel { get; set; }
    public string Title { get; set; }

    public IEnumerable<SectionViewModel> Sections { get; set; }

    public string SaveButtonText { get; set; }

    public string Glyphicon { get; set; }

    public string NgFormName { get; set; }

    public IDictionary<string, string> NgFormAttributes { get; set; }

    public string NgSubmitAction { get; set; }

    public string ParentObjectName { get; set; }

    public Dictionary<string, IEnumerable<string>> HtmlIncludes { get; set; }

    public override string ToString()
    {
      return this.ParentObjectName;
    }

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
      public SectionViewModel()
      {
        this.Attributes = new Dictionary<string, string>();
      }

      public string Title { get; set; }
      public ICollection<RowViewModel> Rows { get; set; }
      public Dictionary<string, string> Attributes { get; set; }

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