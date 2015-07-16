using System.ComponentModel.DataAnnotations;
using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.TestConsumer.Models
{
  public class EmailViewModel
  {
    [DoNotShow]
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Address { get; set; }
  }
}