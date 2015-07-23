namespace Nord.Nganga.DataTypes
{
  /// <summary>
  /// Represents a value from a select list when the select list may have "other" values.
  /// </summary>
  public class UserExpansibleSelectChoice
  {
    /// <summary>
    /// Represents the index of a "canonical" choice.  Always 0 in the case of user-defined values.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Represents the ID of an already-existing user-defined record.  If the record is new, or represents a canonical value, the value of this field will be 0.
    /// </summary>
    public int UserDefinedId { get; set; }

    /// <summary>
    /// The user-defined value of the text.  
    /// </summary>
    public string UserDefinedText { get; set; }
  }
}