using System.ComponentModel.DataAnnotations;

namespace Nord.Nganga.TestConsumer.Models
{
  public class AddressViewModel
  {
  [Required]
    public string Address1 { get; set; }

    public string Address2 { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string State { get; set; }

    [Required]
    public string ZipCode { get; set; }
  }
}
