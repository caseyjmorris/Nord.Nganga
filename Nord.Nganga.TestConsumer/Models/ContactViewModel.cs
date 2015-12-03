using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nord.Nganga.Annotations.Attributes.ViewModels;
using Nord.Nganga.DataTypes;

namespace Nord.Nganga.TestConsumer.Models
{
  public class ContactViewModel
  {
    [DoNotShow]
    public int Id { get; set; }

    public bool IsPrimaryContact { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string MiddleName { get; set; }

    public string Description { get; set; }

    [UiSection("Addresses for contact")]
    public IEnumerable<AddressViewModel> Addresses { get; set; }

    [UiSection("E-mail addresses for contact")]
    public IEnumerable<EmailViewModel> Emails { get; set; }

    [UiSection("Phone numbers for contact")]
    public IEnumerable<PhoneNumberViewModel> PhoneNumbers { get; set; }
  }
}