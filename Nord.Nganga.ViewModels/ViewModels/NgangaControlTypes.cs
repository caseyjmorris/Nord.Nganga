﻿namespace Nord.Nganga.Models.ViewModels
{
  /// <summary>
  /// Possible user controls in Nganga
  /// </summary>
  public enum NgangaControlType
  {
    /// <summary>
    /// Basic scalar (not currently used...)
    /// </summary>
    BasicScalar,
    BoolControl,
    CommonSelect,
    DateControl,
    MultipleComplexEditor,
    MultipleSimpleEditorForComplex,
    MultipleSimpleEditorForPrimitive,
    NumberControl,
    TextControl,
  }
}