using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.Annotations.Attributes.ViewModels;
using Nord.Nganga.DataTypes;

namespace Nord.Nganga.TestConsumer.Models
{
  [NotUserEditable]
  public class SponsorProgramPeriodDetailCollectionViewModel : ProviderProgramPeriodDetailCollectionBaseViewModel
  {
    [CollectionEditor(CollectionEditorAttribute.EditorType.Complex, DefaultObjectDefinitionJson = "{test: 'test'}")]
    public IEnumerable<SponsorViewModel> Sponsors { get; set; }

    public DateTime SomeDate { get; set; }

    [SelectCommon("drugs")]
    [Display(Name = "Medications")]
    public IEnumerable<UserExpansibleSelectChoice> Drugs { get; set; }


    [DocumentTypeSourceProvider("service.getDocumentTypes();")]
    public UserFileCollection Attachments { get; set; }
  }
}