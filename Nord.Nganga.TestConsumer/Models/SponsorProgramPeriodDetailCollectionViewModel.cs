using System.Collections.Generic;
using Nord.Nganga.Annotations.Attributes.Html;

namespace Nord.Nganga.TestConsumer.Models
{
  public class SponsorProgramPeriodDetailCollectionViewModel : ProviderProgramPeriodDetailCollectionBaseViewModel
  {
    [CollectionEditor(CollectionEditorAttribute.EditorType.Complex)]
    public IEnumerable<SponsorViewModel> Sponsors { get; set; }
  }
}