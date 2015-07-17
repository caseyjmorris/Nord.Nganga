using System.Collections.Generic;
using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.TestConsumer.Models
{
  public abstract class ProviderBaseViewModel
  {
    [DoNotShow]
    public int Id { get; set; }

    public string SomeString { get; set; }

    public int SomeInt { get; set; }

    public decimal SomeDec { get; set; }

    public float SomeFloat { get; set; }

    [SelectCommon("someCommonRecord")]
    public int SelectCommonValue { get; set; }

    public IEnumerable<string> CollectionOfStrings { get; set; }

    [SelectCommon("someOtherCommonRec")]
    public IEnumerable<int> MultiSelectCommon { get; set; }

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