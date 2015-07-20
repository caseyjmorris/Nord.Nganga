using System;
using System.Collections.Generic;
using Humanizer;
using Nord.Nganga.Annotations;

namespace Nord.Nganga.Mappers
{
  public class CasingEnumMap : Dictionary<CasingOption,LetterCasing>
  {
    private static readonly Lazy<CasingEnumMap> CasingMap = new Lazy<CasingEnumMap>(() => new CasingEnumMap());

    public static CasingEnumMap Instance
    {
      get { return CasingMap.Value; }
    }

    private CasingEnumMap()
    {
      this[CasingOption.Sentence] = LetterCasing.Sentence;
      this[CasingOption.Title] = LetterCasing.Title;

    }
  }
}
