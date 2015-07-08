using System;

namespace Nord.AngularUiGen.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// Indicates user should be provided with input mask when entering information
  /// </summary>
  /// <remarks>
  /// Value entered here is fed to ui-mask
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public class InputMaskAttribute : Attribute
  {
    public string Mask { get; set; }

    public InputMaskAttribute(string mask)
    {
      this.Mask = mask;
    }
  }
}
