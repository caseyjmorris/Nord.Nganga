using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Nord.AngularUiGen.Annotations.Attributes.Html;
using Nord.AngularUiGen.Annotations.Attributes.ViewModels;
using Nord.AngularUiGen.Engine.Extensions.Reflection;
using Nord.AngularUiGen.Engine.Extensions.Text;
using Nord.AngularUiGen.Engine.Html;
using Nord.CoreLib.Collections;

namespace Nord.AngularUiGen.Engine.Mapping
{
  public static class ViewModelMapper
  {
    private static readonly ICollection<Type> PrimitiveTypes =
      new HashSet<Type>(new[]
      {
        typeof (bool), typeof (bool?),
        typeof (long), typeof (long?),
        typeof (int), typeof (int?),
        typeof (decimal), typeof (decimal?),
        typeof (float), typeof (float?),
        typeof (double), typeof (double?),
        typeof (string),
        typeof (DateTime), typeof (DateTime?),
      });

    public static ViewModelViewModel GetViewModelViewModel(Type type)
    {
      var collections =
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
          .Where(t => typeof (IEnumerable).IsAssignableFrom(t.PropertyType) && t.PropertyType != typeof (string)).ToList();

      var scalars =
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
          .Where(t => !typeof (IEnumerable).IsAssignableFrom(t.PropertyType) || t.PropertyType == typeof (string));

      Func<IEnumerable<PropertyInfo>, IEnumerable<ViewModelViewModel.FieldViewModel>> getPrimitiveFieldInfo = t =>
      {
        var pfi= t.Select(s =>
        {
          var isSelectCommon = s.HasAttribute<SelectCommonAttribute>();
          var selectCommonAttribute = isSelectCommon ? s.GetAttribute<SelectCommonAttribute>() : null;
          var commonRecordsName = isSelectCommon ? selectCommonAttribute.CommonInformationName.ToCamelCase() : null;
          //var commonRecordsIndex = isSelectCommon ? selectCommonAttribute.Index : null;
          //var commonRecordResolveFunction =
          //  s.GetAttributePropertyValueOrDefault<SelectCommonAttribute, string>(a => a.ResolveFunction);
          //var commonRecordsRestrictUntilResolved = isSelectCommon && selectCommonAttribute.RestrictUntilResolved;



          var fieldModel = new ViewModelViewModel.FieldViewModel
          {
            DataType = s.PropertyType,
            DisplayName =
              s.HasAttribute<DisplayAttribute>()
                ? s.GetAttribute<DisplayAttribute>().Name
                : s.Name.ToSpaced() + (s.PropertyType.GetNonNullableType() == typeof (bool) ? "?" : String.Empty),
            FieldName = s.Name.ToCamelCase(),
            IsHidden = s.HasAttribute<DoNotShowAttribute>(),
            IsRequired = s.HasAttribute<RequiredAttribute>(),
            IsViewOnly = s.HasAttribute<NotUserEditableAttribute>(),
            Section =
              s.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(a => a.SectionName) ?? String.Empty,
              SelectCommon = selectCommonAttribute,

            //CommonRecordsObject = s.GetAttributePropertyValueOrDefault<SelectCommonAttribute, string>(a => a.ObjectName),
            //CommonRecordsName = commonRecordsName,
            //CommonRecordsIndex = commonRecordsIndex,
            //CommonRecordResolveFunction = commonRecordResolveFunction,
            //CommonRecordsRestrictUntilResolved = commonRecordsRestrictUntilResolved,

            IsDefaultSort = s.HasAttribute<DefaultSortAttribute>(),
            InputMask = s.HasAttribute<InputMaskAttribute>() ? s.GetAttribute<InputMaskAttribute>().Mask : null,
            Minimum = s.HasAttribute<RangeAttribute>() ? s.GetAttribute<RangeAttribute>().Minimum : null,
            Maximum = s.HasAttribute<RangeAttribute>() ? s.GetAttribute<RangeAttribute>().Maximum : null,
          };

          return fieldModel;

        });

        return pfi;
      };

      var vm = new ViewModelViewModel
      {
        Name = type.Name.Replace("ViewModel", string.Empty).ToCamelCase(),
        Scalars = getPrimitiveFieldInfo(scalars),
        IsViewOnly = type.HasAttribute<NotUserEditableAttribute>(),
        ComplexCollections =
          collections.Where(
            t =>
              t.PropertyType.IsGenericType &&
              !PrimitiveTypes.Contains(t.PropertyType.GetGenericArguments().First()))
            .Select(t =>
            {
              var hasEnumerableItemAction = t.HasAttribute<SubordinateItemActionAttribute>();
              var itemActionAttribute = hasEnumerableItemAction ? t.GetAttribute<SubordinateItemActionAttribute>() : null;
              var wrapper = new SubordinateViewModelWrapper
              {
                Model =
                  GetViewModelViewModel(t.PropertyType.GetGenericArguments().First()),
                Section =
                  t.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(s => s.SectionName) ?? string.Empty,
                DisplayName =
                  t.GetAttributePropertyValueOrDefault<DisplayAttribute, string>(a => a.Name) ?? t.Name.ToSpaced(),
                IsLedger = t.HasAttribute<LedgerAttribute>(),
                LedgerSumProperty =
                  t.GetAttributePropertyValueOrDefault<LedgerAttribute, string>(a => a.SumPropertyName),
                 ItemActionAttribute = itemActionAttribute,
              };
              return wrapper;
            }),
        PrimitiveCollections =
          getPrimitiveFieldInfo(
            collections.Where(
              c =>
                c.PropertyType.IsGenericType &&
                PrimitiveTypes.Contains(c.PropertyType.GetGenericArguments().First()))),
      };

      var sectionsDict = new Dictionary<string, Dictionary<string, string>>();

      foreach (var attr in type.GetCustomAttributes<SectionHtmlAttributeAttribute>())
      {
        var htmlAttrsDict = sectionsDict.ResolveIfNotCached(attr.SectionName, () => new Dictionary<string, string>());

        htmlAttrsDict[attr.HtmlAttributeName] = attr.HtmlAttributeValue;
      }

      vm.SectionHtmlAttributeValues = sectionsDict;

      return vm;
    }
  }
}