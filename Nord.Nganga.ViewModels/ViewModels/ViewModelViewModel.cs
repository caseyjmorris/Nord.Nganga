using System;
using System.Collections.Generic;
using Nord.Nganga.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.Models.ViewModels
{
  public class ViewModelViewModel
  {
    public string Name { get; set; }
    public bool IsViewOnly { get; set; }
    public IList<MemberWrapper> Members { get; set; }
    public IEnumerable<SubordinateViewModelWrapper> ComplexCollections { get; set; }
    public IEnumerable<FieldViewModel> PrimitiveCollections { get; set; }
    public IEnumerable<FieldViewModel> Scalars { get; set; }
    public Dictionary<string, Dictionary<string, string>> SectionHtmlAttributeValues { get; set; }

    public override string ToString()
    {
      return this.Name;
    }

    public interface IMember
    {
      string FieldName { get; set; }
      string DisplayName { get; set; }
    }

    public enum MemberDiscriminator
    {
      PrimitiveCollection,
      ComplexCollection,
      Scalar,
    }

    public class MemberWrapper
    {
      public MemberDiscriminator Discriminator { get; set; }
      public IMember Member { get; set; }
      public string Section { get; set; }
      public int Width { get; set; }
      public NgangaControlType ControlType { get; set; }

      public string NgangaControlTypeSt
      {
        get { return this.ControlType.ToString(); }
      }

      public string ViewOnlyExpression { get; set; }
      public bool IsHidden { get; set; }

      public override string ToString()
      {
        if (this.Member == null)
        {
          return "(Invalid member)";
        }

        return this.Member.FieldName;
      }
    }

    public class SubordinateViewModelWrapper : IMember
    {
      public ViewModelViewModel Model { get; set; }
      public string DisplayName { get; set; }
      public string Section { get; set; }
      public bool IsLedger { get; set; }
      public string LedgerSumProperty { get; set; }
      public IEnumerable<SubordinateItemActionAttribute> ItemActionAttributes { get; set; }
      public ViewCoordinatedInformationViewModel CoordinatedInfo { get; set; }
      public string TableFieldsExpression { get; set; }
      public string SubordinateActionsExpression { get; set; }

      public override string ToString()
      {
        return this.DisplayName;
      }

      public string FieldName { get; set; }
    }

    public class FieldViewModel : IMember
    {
      public string FieldName { get; set; }
      public string DisplayName { get; set; }
      public bool IsRequired { get; set; }
      public bool IsHidden { get; set; }
      public bool IsViewOnly { get; set; }
      public SelectCommonAttribute SelectCommon { get; set; }
      public string Section { get; set; }
      public bool IsDefaultSort { get; set; }
      public string InputMask { get; set; }
      public object Minimum { get; set; }
      public object Maximum { get; set; }
      public string StartCap { get; set; }
      public string EndCap { get; set; }
      public object Step { get; set; } //1, .01, "any" are the most likely values here
      public Type DataType { get; set; }

      public NgangaControlType ControlType { get; set; }

      public string NgangaControlTypeSt
      {
        get { return this.ControlType.ToString(); }
      }


      public override string ToString()
      {
        return this.FieldName;
      }
    }
  }
}