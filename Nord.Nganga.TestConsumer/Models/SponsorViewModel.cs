using System.ComponentModel.DataAnnotations;

namespace Nord.Nganga.TestConsumer.Models
{
  public class SponsorViewModel : ProviderBaseViewModel
  {
    [Required]
    public string SponsorName { get; set; }
  }
}