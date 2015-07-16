using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
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
        var discriminatorProviderDictionary = new Dictionary<Func<PropertyInfo, bool>, ViewModelViewModel.MemberDiscriminator>
        {
          { scalarDetector, ViewModelViewModel.MemberDiscriminator.Scalar},
          { primitiveCollectionDetector, ViewModelViewModel.MemberDiscriminator.PrimitiveCollection},
          { complexCollectionDetector, ViewModelViewModel.MemberDiscriminator.ComplexCollection}
        };

      Func<PropertyInfo, ViewModelViewModel.FieldViewModel> fieldViewModelProvider = info =>
      {
        var isSelectCommon = info.HasAttribute<SelectCommonAttribute>();
        var selectCommonAttribute = isSelectCommon ? info.GetAttribute<SelectCommonAttribute>() : null;
        var commonRecordsName = isSelectCommon ? selectCommonAttribute.CommonInformationName.ToCamelCase() : null;

        var fieldModel = new ViewModelViewModel.FieldViewModel
        {
          DataType = info.PropertyType,
          DisplayName =
            info.HasAttribute<DisplayAttribute>()
              ? info.GetAttribute<DisplayAttribute>().Name
              : info.Name.ToSpaced() + (info.PropertyType.GetNonNullableType() == typeof(bool) ? "?" : String.Empty),
          FieldName = info.Name.ToCamelCase(),
          IsHidden = info.HasAttribute<DoNotShowAttribute>(),
          IsRequired = info.HasAttribute<RequiredAttribute>(),
          IsViewOnly = info.HasAttribute<NotUserEditableAttribute>(),
          Section =
            info.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(a => a.SectionName) ?? String.Empty,
          SelectCommon = selectCommonAttribute,
          IsDefaultSort = info.HasAttribute<DefaultSortAttribute>(),
          InputMask = info.HasAttribute<InputMaskAttribute>() ? info.GetAttribute<InputMaskAttribute>().Mask : null,
          Minimum = info.HasAttribute<RangeAttribute>() ? info.GetAttribute<RangeAttribute>().Minimum : null,
          Maximum = info.HasAttribute<RangeAttribute>() ? info.GetAttribute<RangeAttribute>().Maximum : null,
        };

        return fieldModel;
      };

      Func<IEnumerable<PropertyInfo>, IEnumerable<ViewModelViewModel.FieldViewModel>> fieldViewModelCollectionProvider = t =>
      {
        var pfi = t.Select(s => fieldViewModelProvider(s));
        return pfi;
      };


      Func<PropertyInfo, ViewModelViewModel.SubordinateViewModelWrapper> subordinateViewModelWrapperProvider = (info) =>
      {
        var hasEnumerableItemAction = info.HasAttribute<SubordinateItemActionAttribute>();
        var itemActionAttribute = hasEnumerableItemAction
          ? info.GetAttribute<SubordinateItemActionAttribute>()
          : null;
        var wrapper = new ViewModelViewModel.SubordinateViewModelWrapper
        {
          Model =
            this.GetViewModelViewModel(info.PropertyType.GetGenericArguments().First()),
          Section =
            info.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(s => s.SectionName) ?? string.Empty,
          DisplayName =
            info.GetAttributePropertyValueOrDefault<DisplayAttribute, string>(a => a.Name) ?? info.Name.ToSpaced(),
          IsLedger = info.HasAttribute<LedgerAttribute>(),
          LedgerSumProperty =
            info.GetAttributePropertyValueOrDefault<LedgerAttribute, string>(a => a.SumPropertyName),
          ItemActionAttribute = itemActionAttribute,
        };
        return wrapper;
      };

      Func<ViewModelViewModel.MemberDiscriminator, PropertyInfo, ViewModelViewModel.MemberWrapper> memberWrapperProvider =
        (discriminator, info) =>
        {
          var result = new ViewModelViewModel.MemberWrapper{Discriminator = discriminator};
          switch (discriminator)
          {
            case ViewModelViewModel.MemberDiscriminator.Scalar:
              result.Member = fieldViewModelProvider(info);
              break;
            case ViewModelViewModel.MemberDiscriminator.PrimitiveCollection:
              result.Member = subordinateViewModelWrapperProvider(info);
              break;
            case ViewModelViewModel.MemberDiscriminator.ComplexCollection:
              result.Member = subordinateViewModelWrapperProvider(info);
              break;
          }
          return result;
        };

        // get the properties 
      var viewModelProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

      // decorate them with the descriminator 
      var decoratedProperties = (from PropertyInfo pi in viewModelProperties
        let discriminator = discriminatorProviderDictionary.Single(kvp => kvp.Key(pi)).Value
        let memberWrapper = memberWrapperProvider(discriminator, pi )
                                select new { pi, discriminator,  memberWrapper }).ToList();

                                // VVVVVV OLD CODE VVVVVV

      //var collections =
      //  type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      //    .Where(t => typeof(IEnumerable).IsAssignableFrom(t.PropertyType) && t.PropertyType != typeof(string)).ToList();

      //var scalars =
      //  type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
      //    .Where(t => !typeof(IEnumerable).IsAssignableFrom(t.PropertyType) || t.PropertyType == typeof(string));



      var vm = new ViewModelViewModel
      {
        Name = type.Name.Replace("ViewModel", string.Empty).ToCamelCase(),
        Scalars = fieldViewModelCollectionProvider(decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.Scalar).Select(p => p.pi)),
        IsViewOnly = type.HasAttribute<NotUserEditableAttribute>(),
        ComplexCollections =
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.ComplexCollection).Select(p=>p.pi)
            .Select(t => subordinateViewModelWrapperProvider(t)),
        PrimitiveCollections =
          fieldViewModelCollectionProvider(decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.PrimitiveCollection).Select(p => p.pi)),
        Members = decoratedProperties.Select(dp => dp.memberWrapper).ToList()
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