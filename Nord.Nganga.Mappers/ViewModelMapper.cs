using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Humanizer;
using Newtonsoft.Json;
using Nord.Nganga.Annotations;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.Annotations.Attributes.ViewModels;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.DataTypes;
using Nord.Nganga.Mappers.Views;
using Nord.Nganga.Models;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers
{
  public class ViewModelMapper
  {
    public AssemblyOptionsModel AssemblyOptions { get; set; }

    private readonly ViewCoordinationMapper viewCoordinationMapper;

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
        typeof (UserExpansibleSelectChoice),
      });

    private static readonly ICollection<Type> Numerics = new HashSet<Type>(new[]
    {
      typeof (long), typeof (long?),
      typeof (int), typeof (int?),
      typeof (decimal), typeof (decimal?),
      typeof (float), typeof (float?),
      typeof (double), typeof (double?),
    });

    #region type detectors

    private static bool IsScalar(PropertyInfo info)
    {
      return !typeof (IEnumerable).IsAssignableFrom(info.PropertyType) || info.PropertyType == typeof (string);
    }

    private static bool IsCollection(PropertyInfo info)
    {
      return (typeof (IEnumerable).IsAssignableFrom(info.PropertyType) && info.PropertyType != typeof (string));
    }

    private static bool IsComplexCollection(PropertyInfo info)
    {
      return IsCollection(info) && (info.PropertyType.IsGenericType &&
                                    !PrimitiveTypes.Contains(info.PropertyType.GetGenericArguments().First()));
    }

    private static bool IsPrimitiveCollection(PropertyInfo info)
    {
      return IsCollection(info) &&
             (info.PropertyType.IsGenericType &&
              PrimitiveTypes.Contains(info.PropertyType.GetGenericArguments().First()))
             && !IsComplexCollection(info);
    }

    private static readonly Dictionary<Func<PropertyInfo, bool>, ViewModelViewModel.MemberDiscriminator>
      DiscriminatorProviderDictionary = new Dictionary
        <Func<PropertyInfo, bool>, ViewModelViewModel.MemberDiscriminator>
      {
        {IsScalar, ViewModelViewModel.MemberDiscriminator.Scalar},
        {IsPrimitiveCollection, ViewModelViewModel.MemberDiscriminator.PrimitiveCollection},
        {IsComplexCollection, ViewModelViewModel.MemberDiscriminator.ComplexCollection}
      };

    public ViewModelMapper(ViewCoordinationMapper viewCoordinationMapper = null)
    {
      this.viewCoordinationMapper = viewCoordinationMapper;
      this.AssemblyOptions = new AssemblyOptionsModel();
    }

    #endregion

    private object GetStep(PropertyInfo info)
    {
      var propType = info.PropertyType.GetNonNullableType();

      var numeric = Numerics.Contains(propType);

      var isInt = numeric && propType == typeof (int) || propType == typeof (long);

      if (!numeric)
      {
        return null;
      }
      if (isInt)
      {
        return 1;
      }
      if (propType == typeof (decimal))
      {
        return ".01";
      }
      return "any";
    }

    private ViewModelViewModel.FieldViewModel GetFieldViewModel(PropertyInfo info, bool isCollection)
    {
      var isSelectCommon = info.HasAttribute<SelectCommonAttribute>();
      var selectCommonAttribute = isSelectCommon ? info.GetAttribute<SelectCommonAttribute>() : null;

      var fieldModel = new ViewModelViewModel.FieldViewModel
      {
        DataType = isCollection ? info.PropertyType.GetGenericArguments()[0] : info.PropertyType,
        DisplayName =
          info.HasAttribute<DisplayAttribute>()
            ? info.GetAttribute<DisplayAttribute>().Name
            : info.Name.Humanize(CasingEnumMap.Instance[this.AssemblyOptions.GetOption(CasingOptionContext.Field)]) +
              (info.PropertyType.GetNonNullableType() == typeof (bool) ? "?" : String.Empty),
        FieldName = info.Name.Camelize(),
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
        Step = this.GetStep(info),
      };

      fieldModel.ControlType =
        this.DetermineControlType(
          isCollection
            ? ViewModelViewModel.MemberDiscriminator.PrimitiveCollection
            : ViewModelViewModel.MemberDiscriminator.Scalar, info);

      return fieldModel;
    }

    private IEnumerable<ViewModelViewModel.FieldViewModel> GetFieldViewModelCollection(IEnumerable<PropertyInfo> t,
      Boolean isCollection)
    {
      var pfi = t.Select(p => this.GetFieldViewModel(p, isCollection));
      return pfi;
    }

    private ViewModelViewModel.SubordinateViewModelWrapper GetSubordinateViewModelWrapper(PropertyInfo info)
    {
      var hasEnumerableItemAction = info.HasAttribute<SubordinateItemActionAttribute>();
      var itemActionAttribute = hasEnumerableItemAction
        ? info.GetAttribute<SubordinateItemActionAttribute>()
        : null;
      var wrapper = new ViewModelViewModel.SubordinateViewModelWrapper
      {
        FieldName = info.Name.Camelize(),
        Model =
          this.GetViewModelViewModel(info.PropertyType.GetGenericArguments().First()),
        Section =
          info.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(s => s.SectionName) ?? string.Empty,
        DisplayName =
          info.GetAttributePropertyValueOrDefault<DisplayAttribute, string>(a => a.Name) ??
          info.Name.Humanize(CasingEnumMap.Instance[this.AssemblyOptions.GetOption(CasingOptionContext.Field)]),
        IsLedger = info.HasAttribute<LedgerAttribute>(),
        LedgerSumProperty =
          info.GetAttributePropertyValueOrDefault<LedgerAttribute, string>(a => a.SumPropertyName),
        ItemActionAttribute = itemActionAttribute,
      };

      if (this.viewCoordinationMapper != null)
      {
        var depthMultiplier = info.HasAttribute<CollectionEditorAttribute>() &&
                              info.GetAttribute<CollectionEditorAttribute>().Editor ==
                              CollectionEditorAttribute.EditorType.Simple
          ? 2
          : 1;
        wrapper.CoordinatedInfo =
          this.viewCoordinationMapper.GetViewCoordinatedInformationSingle(info.PropertyType.GetGenericArguments()[0],
            depthMultiplier);
      }

      wrapper.TableFieldsExpression = this.GetTableVisibleFieldsExpression(wrapper);

      return wrapper;
    }

    private string GetTableVisibleFieldsExpression(ViewModelViewModel.SubordinateViewModelWrapper wrapper)
    {
      var fields = wrapper.Model.Scalars.Where(s => !s.IsHidden);

      var fieldsDesc = fields.Select(f => new {name = f.FieldName, label = f.DisplayName});

      var sb = new StringBuilder();
      using (var sw = new StringWriter(sb))
      using (var writer = new JsonTextWriter(sw))
      {
        writer.QuoteChar = '\'';
        writer.Formatting = Formatting.None;

        var ser = new JsonSerializer();
        ser.Serialize(writer, fieldsDesc);
      }

      return sb.ToString();
    }

    private NgangaControlType DetermineControlType(ViewModelViewModel.MemberDiscriminator discriminator,
      PropertyInfo info)
    {
      if (discriminator == ViewModelViewModel.MemberDiscriminator.PrimitiveCollection)
      {
        return NgangaControlType.MultipleSimpleEditorForPrimitive;
      }
      if (discriminator == ViewModelViewModel.MemberDiscriminator.ComplexCollection)
      {
        if (info.HasAttribute<CollectionEditorAttribute>() &&
            info.GetAttribute<CollectionEditorAttribute>().Editor == CollectionEditorAttribute.EditorType.Complex)
        {
          return NgangaControlType.MultipleComplexEditor;
        }
        return NgangaControlType.MultipleSimpleEditorForComplex;
      }

      if (info.HasAttribute<SelectCommonAttribute>() && info.PropertyType == typeof (UserExpansibleSelectChoice))
      {
        return NgangaControlType.CommonSelectExpansible;
      }

      if (info.HasAttribute<SelectCommonAttribute>() && info.PropertyType != typeof (UserExpansibleSelectChoice))
      {
        return NgangaControlType.CommonSelect;
      }

      var underlyingType = info.PropertyType.GetNonNullableType();

      if (underlyingType == typeof (string))
      {
        return NgangaControlType.TextControl;
      }

      if (underlyingType == typeof (bool))
      {
        return NgangaControlType.BoolControl;
      }
      if (underlyingType == typeof (DateTime))
      {
        return NgangaControlType.DateControl;
      }
      if (Numerics.Contains(underlyingType))
      {
        return NgangaControlType.NumberControl;
      }

      var msg = string.Format("Couldn't find control for type {0} (property {1} of {2})", underlyingType, info.Name,
        info.DeclaringType);

      throw new KeyNotFoundException(msg);
    }

    private int GetMemberWidth(PropertyInfo info, NgangaControlType controlType)
    {
      var specified = info.GetAttributePropertyValueOrDefault<FieldWidthAttribute, int>(a => a.Twelfths);

      if (specified != 0)
      {
        return specified;
      }

      if (controlType == NgangaControlType.MultipleComplexEditor)
      {
        return 12;
      }
      return 3;
    }

    private ViewModelViewModel.MemberWrapper GetWrappedMember(ViewModelViewModel.MemberDiscriminator discriminator,
      PropertyInfo info)
    {
      var result = new ViewModelViewModel.MemberWrapper
      {
        Discriminator = discriminator,
        ControlType = this.DetermineControlType(discriminator, info),
        Section =
          info.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(a => a.SectionName) ?? string.Empty,
      };

      result.Width = this.GetMemberWidth(info, result.ControlType);

      switch (discriminator)
      {
        case ViewModelViewModel.MemberDiscriminator.Scalar:
          result.Member = this.GetFieldViewModel(info, false);
          var field = (ViewModelViewModel.FieldViewModel) result.Member;
          result.IsHidden = field.IsHidden;
          break;
        case ViewModelViewModel.MemberDiscriminator.PrimitiveCollection:
          result.Member = this.GetFieldViewModel(info, true);
          ;
          break;
        case ViewModelViewModel.MemberDiscriminator.ComplexCollection:
          result.Member = this.GetSubordinateViewModelWrapper(info);
          break;
        default:
          break;
      }
      return result;
    }

    public ViewModelViewModel GetViewModelViewModel(Type type)
    {
      if (!type.Name.EndsWith("ViewModel"))
      {
        throw new Exception(string.Format("{0} is not a view model", type.FullName));
      }
      // get the properties 
      var viewModelProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

      // decorate them with the descriminator 
      var decoratedProperties = (from PropertyInfo pi in viewModelProperties
        let discriminator = DiscriminatorProviderDictionary.Single(kvp => kvp.Key(pi)).Value
        let memberWrapper = this.GetWrappedMember(discriminator, pi)
        select new {pi, discriminator, memberWrapper}).ToList();

      var vm = new ViewModelViewModel
      {
        Name = type.Name.Replace("ViewModel", string.Empty).Camelize(),
        Scalars = this.GetFieldViewModelCollection(
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.Scalar)
            .Select(p => p.pi), false),
        IsViewOnly = type.HasAttribute<NotUserEditableAttribute>(),
        ComplexCollections =
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.ComplexCollection)
            .Select(p => p.pi)
            .Select(this.GetSubordinateViewModelWrapper),
        PrimitiveCollections = this.GetFieldViewModelCollection(
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.PrimitiveCollection)
            .Select(p => p.pi), true),
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