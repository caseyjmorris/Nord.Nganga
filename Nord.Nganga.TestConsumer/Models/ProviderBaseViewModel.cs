using System.Collections.Generic;
using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.TestConsumer.Models
{
  public abstract class ProviderBaseViewModel
  {
    [DoNotShow]
    public int Id { get; set; }

    [UiSection("Contacts")]
    public IEnumerable<ContactViewModel> Contacts { get; set; }

    [UiSection("Addresses for provider")]
    public IEnumerable<AddressViewModel> Addresses { get; set; }

    [UiSection("E-mail addresses for provider")]
    public IEnumerable<EmailViewModel> Emails { get; set; }

    [UiSection("Phone numbers for provider")]
    public IEnumerable<PhoneNumberViewModel> PhoneNumbers { get; set; }
  }
}