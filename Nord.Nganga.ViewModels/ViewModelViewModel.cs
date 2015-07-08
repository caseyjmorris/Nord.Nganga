using System;
using System.Collections.Generic;
using Nord.AngularUiGen.Annotations.Attributes.ViewModels;

namespace Nord.Nganga.Models
{
  public class ViewModelViewModel
  {
    public string Name { get; set; }
    public bool IsViewOnly { get; set; }
    public IEnumerable<SubordinateViewModelWrapper> ComplexCollections { get; set; }
    public IEnumerable<FieldViewModel> PrimitiveCollections { get; set; }
    public IEnumerable<FieldViewModel> Scalars { get; set; }
    public Dictionary<string, Dictionary<string, string>> SectionHtmlAttributeValues { get; set; }

    public override string ToString()
    {
      return this.Name;
    }

    public class FieldViewModel
    {
      public string FieldName { get; set; }
      public string DisplayName { get; set; }
      public bool IsRequired { get; set; }
      public bool IsHidden { get; set; }
      public bool IsViewOnly { get; set; }
      public SelectCommonAttribute SelectCommon { get; set; }
      //public string CommonRecordResolveFunction { get; set; }
      //public string CommonRecordsObject { get; set; }
      //public string CommonRecordsName { get; set; }
      //public string CommonRecordsIndex { get; set; }
      //public bool CommonRecordsRestrictUntilResolved { get; set; }
      public string Section { get; set; }
      public bool IsDefaultSort { get; set; }
      public string InputMask { get; set; }
      public object Minimum { get; set; }
      public object Maximum { get; set; }
      public Type DataType { get; set; }


      public override string ToString()
      {
        return this.FieldName;
      }
    }
  }

  public class SubordinateViewModelWrapper
  {
    public ViewModelViewModel Model { get; set; }
    public string DisplayName { get; set; }
    public string Section { get; set; }
    public bool IsLedger { get; set; }
    public string LedgerSumProperty { get; set; }
    public SubordinateItemActionAttribute ItemActionAttribute { get; set; }

    public override string ToString()
    {
      return this.DisplayName;
    }
  }

  public class FormFieldWrapper
  {
    public ViewModelViewModel.FieldViewModel Model { get; set; }
    public string Section { get; set; }
    public bool IsCollection { get; set; }
  }
}