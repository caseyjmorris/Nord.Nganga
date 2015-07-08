﻿using System;

namespace Nord.AngularUiGen.Annotations.Attributes.ViewModels
{
  /// <summary>
  /// The user may not edit this field or view model from the UI.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
  public class NotUserEditableAttribute : Attribute
  {
  }
}