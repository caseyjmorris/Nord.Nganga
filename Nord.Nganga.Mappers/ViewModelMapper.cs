using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.Annotations.Attributes.ViewModels;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Core.Text;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers
{
  public class ViewModelMapper
  {
    private static readonly ICollection<Type> PrimitiveTypes =
      new HashSet<Type>(new[]
      {
        typeof(bool), typeof(bool?),
        typeof(long), typeof(long?),
        typeof(int), typeof(int?),
        typeof(decimal), typeof(decimal?),
        typeof(float), typeof(float?),
        typeof(double), typeof(double?),
        typeof(string),
        typeof(DateTime), typeof(DateTime?),
      });

    public ViewModelViewModel GetViewModelViewModel(Type type)
    {
    // functoin to detect scalar properties 
      Func<PropertyInfo, bool> scalarDetector =
        info => !typeof (IEnumerable).IsAssignableFrom(info.PropertyType) || info.PropertyType == typeof (string);

      Func<PropertyInfo, bool> collectionDetector =
        info => (typeof (IEnumerable).IsAssignableFrom(info.PropertyType) && info.PropertyType != typeof (string));

      // function to detect complex collection properties 
      Func<PropertyInfo, bool> complexCollectionDetector =
        info => collectionDetector(info) && (info.PropertyType.IsGenericType &&
                !PrimitiveTypes.Contains(info.PropertyType.GetGenericArguments().First()));

        // function to detect primitive collection properties
      Func<PropertyInfo, bool> primitiveCollectionDetector =
        info => collectionDetector(info) &&
        ( info.PropertyType.IsGenericType &&
                PrimitiveTypes.Contains(info.PropertyType.GetGenericArguments().First()) ) 
          && !complexCollectionDetector(info);

        // dictionary mapping property detectors to member descriminator enum value 
        var fd = new Dictionary<Func<PropertyInfo, bool>, ViewModelViewModel.MemberDiscriminator>
        {
          { scalarDetector, ViewModelViewModel.MemberDiscriminator.Scalar},
          { primitiveCollectionDetector, ViewModelViewModel.MemberDiscriminator.PrimitiveCollection},
          { complexCollectionDetector, ViewModelViewModel.MemberDiscriminator.ComplexCollection}
        };

        // get the properties 
      var viewModelProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

      // decorate them with the descriminator 
      var decoratedProperties = (from PropertyInfo pi in viewModelProperties
        let descriminator = fd.Single(kvp => kvp.Key(pi)).Value
                                select new { pi, descriminator }).ToList();

                                // VVVVVV OLD CODE VVVVVV

      //var collections =
      //  type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      //    .Where(t => typeof(IEnumerable).IsAssignableFrom(t.PropertyType) && t.PropertyType != typeof(string)).ToList();

      //var scalars =
      //  type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      //    .Where(t => !typeof(IEnumerable).IsAssignableFrom(t.PropertyType) || t.PropertyType == typeof(string));

      Func<IEnumerable<PropertyInfo>, IEnumerable<ViewModelViewModel.FieldViewModel>> getPrimitiveFieldInfo = t =>
      {
        var pfi = t.Select(s =>
        {
          var isSelectCommon = s.HasAttribute<SelectCommonAttribute>();
          var selectCommonAttribute = isSelectCommon ? s.GetAttribute<SelectCommonAttribute>() : null;
          var commonRecordsName = isSelectCommon ? selectCommonAttribute.CommonInformationName.ToCamelCase() : null;

          var fieldModel = new ViewModelViewModel.FieldViewModel
          {
            DataType = s.PropertyType,
            DisplayName =
              s.HasAttribute<DisplayAttribute>()
                ? s.GetAttribute<DisplayAttribute>().Name
                : s.Name.ToSpaced() + (s.PropertyType.GetNonNullableType() == typeof(bool) ? "?" : String.Empty),
            FieldName = s.Name.ToCamelCase(),
            IsHidden = s.HasAttribute<DoNotShowAttribute>(),
            IsRequired = s.HasAttribute<RequiredAttribute>(),
            IsViewOnly = s.HasAttribute<NotUserEditableAttribute>(),
            Section =
              s.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(a => a.SectionName) ?? String.Empty,
            SelectCommon = selectCommonAttribute,
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
        Scalars = getPrimitiveFieldInfo(decoratedProperties.Where(p=>p.descriminator == ViewModelViewModel.MemberDiscriminator.Scalar).Select(p=>p.pi)),
        IsViewOnly = type.HasAttribute<NotUserEditableAttribute>(),
        ComplexCollections =
          decoratedProperties.Where(p => p.descriminator == ViewModelViewModel.MemberDiscriminator.ComplexCollection).Select(p=>p.pi)
            .Select(t =>
            {
              var hasEnumerableItemAction = t.HasAttribute<SubordinateItemActionAttribute>();
              var itemActionAttribute = hasEnumerableItemAction
                ? t.GetAttribute<SubordinateItemActionAttribute>()
                : null;
              var wrapper = new ViewModelViewModel.SubordinateViewModelWrapper
              {
                Model =
                  this.GetViewModelViewModel(t.PropertyType.GetGenericArguments().First()),
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
          getPrimitiveFieldInfo(decoratedProperties.Where(p => p.descriminator == ViewModelViewModel.MemberDiscriminator.PrimitiveCollection).Select(p => p.pi)),
      };

      var sectionsDict = new Dictionary<string, Dictionary<string, string>>();

      foreach (var attr in type.GetCustomAttributes<SectionHtmlAttributeAttribute>())
      {
        Dictionary<string, string> htmlAttrsDict;

        if (!sectionsDict.TryGetValue(attr.SectionName, out htmlAttrsDict))
        {
          htmlAttrsDict = new Dictionary<string, string>();
          sectionsDict[attr.SectionName] = htmlAttrsDict;
        }

        htmlAttrsDict[attr.HtmlAttributeName] = attr.HtmlAttributeValue;
      }

      vm.SectionHtmlAttributeValues = sectionsDict;

      return vm;
    }
  }
}