using System;

namespace Nord.AngularUiGen.Attributes
{
  /// <summary>
  /// Indicates user should be provided with input mask when entering information
  /// </summary>
  /// <remarks>
  /// Value entered here is fed to ui-mask
  /// </remarks>
  public class InputMaskAttribute : Attribute
  {
    public string Mask { get; set; }

    public InputMaskAttribute(string mask)
    {
      this.Mask = mask;
    }
  }
}
