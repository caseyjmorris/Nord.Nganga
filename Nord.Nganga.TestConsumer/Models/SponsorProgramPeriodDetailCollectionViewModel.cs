using System.Collections.Generic;

namespace Nord.Nganga.TestConsumer.Models
{
  public class SponsorProgramPeriodDetailCollectionViewModel : ProviderProgramPeriodDetailCollectionBaseViewModel
  {
    public IEnumerable<SponsorViewModel> Sponsors { get; set; }
  }
}