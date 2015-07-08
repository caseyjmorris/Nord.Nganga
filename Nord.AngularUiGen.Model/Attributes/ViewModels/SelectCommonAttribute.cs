using System;

namespace Nord.AngularUiGen.Attributes
{
  /// <summary>
  /// Property maps to the key of a common record
  /// </summary>
  public class SelectCommonAttribute : Attribute
  {
    /// <summary>
    /// The name of the common records
    /// </summary>
    public string CommonInformationName { get; set; }


    /// <summary>
    /// Generates a new <code>SelectCommonAttribute</code>
    /// </summary>
    /// <param name="commonInformationName">The name of the common records</param>
    public SelectCommonAttribute(string commonInformationName)
    {
      if (string.IsNullOrEmpty(commonInformationName))
      {
        throw new ArgumentException();
      }
      this.CommonInformationName = commonInformationName;
    }
  }
}
