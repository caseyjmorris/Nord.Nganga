using System;

namespace Nord.Nganga.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Specifies the collection should be displayed in a ledger table (for financial information).
  /// </summary>
  [AttributeUsage(AttributeTargets.Property)]
  public class LedgerAttribute : Attribute
  {
    /// <summary>
    /// This property will be summed to produce the current value.
    /// </summary>
    public string SumPropertyName { get; private set; }

    /// <summary>
    /// Creates a new <see cref="Nord.Nganga.Annotations.Attributes.ViewModels.LedgerAttribute"/>
    /// </summary>
    /// <param name="sumPropertyName">
    /// This property will be summed to produce the current value.
    /// </param>
    public LedgerAttribute(string sumPropertyName)
    {
      this.SumPropertyName = sumPropertyName;
    }
  }
}