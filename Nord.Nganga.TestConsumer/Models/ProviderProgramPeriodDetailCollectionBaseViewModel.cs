using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.TestConsumer.Models
{
  public abstract class ProviderProgramPeriodDetailCollectionBaseViewModel
  {
    [DoNotShow]
    public virtual int ProgramPeriodId { get; set; }
  }
}