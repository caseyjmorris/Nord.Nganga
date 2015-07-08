namespace Nord.Nganga.Models.Configuration
{
  /// <summary>
  /// Data class representing user settings.
  /// </summary>
  public interface IConfigurationPackage
  {
    /// <summary>
    /// Set all properties of this instance of the class to the default.
    /// </summary>
    void SetPropertiesToDefault();
  }
}