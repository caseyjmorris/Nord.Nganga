using System;
using System.Collections.Generic;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.TestConsumer.Models
{
  [NotUserEditable]
  public class SponsorProgramPeriodDetailCollectionViewModel : ProviderProgramPeriodDetailCollectionBaseViewModel
  {
    [CollectionEditor(CollectionEditorAttribute.EditorType.Complex)]
    public IEnumerable<SponsorViewModel> Sponsors { get; set; }

    public DateTime SomeDate { get; set; }
  }
}