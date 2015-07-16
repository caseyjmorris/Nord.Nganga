using System.ComponentModel.DataAnnotations;
using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.TestConsumer.Models
{
  public class PhoneNumberViewModel
  {
    [DoNotShow]
    public int Id { get; set; }

    [Required]
    [RegularExpression(@"^[\d \(\)\-]+$", ErrorMessage = "Please enter a valid phone number")]
    public string Number { get; set; }

    public string Extension { get; set; }

    public string Description { get; set; }

    [Display(Name = "This number is preferred")]
    public bool IsPreferred { get; set; }

    [SelectCommon("phoneNumberClasses")]
    public int PhoneNumberClass { get; set; }
  }
}