using System;

namespace Nord.Nganga.Annotations.Attributes.Html
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  public class TextCasePreferencesAttribute : System.Attribute
  {
    public enum CasingOption
    {
      Sentence,
      Title
    }

    public enum CasingOptionContext
    {
      Button,
      Field,
      Header,
    }

    public CasingOption Option { get; set; }
    public CasingOptionContext Context { get; set; }

    public TextCasePreferencesAttribute(CasingOptionContext context, CasingOption option)
    {
      this.Context = context;
      this.Option = option;
    }
  }
}

