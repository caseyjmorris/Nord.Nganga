using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;
using Newtonsoft.Json;
using Nord.Nganga.Annotations;
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

    private static readonly HashSet<Type> BarredFromTables =
      new HashSet<Type>(new[] {typeof (UserFileCollection)});

    private static readonly Dictionary<Type, string> ClientTypes =
      new Dictionary<Type, string>
      {
        {typeof (bool), "bool"},
        {typeof (bool?), "bool"},
        {typeof (long), "number"},
        {typeof (long?), "number"},
        {typeof (int), "number"},
        {typeof (int?), "number"},
        {typeof (decimal), "number"},
        {typeof (decimal?), "number"},
        {typeof (float), "number"},
        {typeof (float?), "number"},
        {typeof (double), "number"},
        {typeof (double?), "number"},
        {typeof (string), "string"},
        {typeof (DateTime), "date"},
        {typeof (DateTime?), "date"},
        {typeof (UserExpansibleSelectChoice), "selectcommon"},
        {typeof (UserFileCollection), "userfilecollection"},
      };


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
        typeof (UserFileCollection),
      });

    private static readonly ICollection<Type> Numerics = new HashSet<Type>(new[]
    {
      typeof (long), typeof (long?),
      typeof (int), typeof (int?),
      typeof (decimal), typeof (decimal?),
      typeof (float), typeof (float?),
      typeof (double), typeof (double?),
    });

    private readonly ICollection<string> usedUniqueIdentifiers = new HashSet<string>();

    #region type detectors

    private static bool IsScalar(PropertyInfo info)
    {
      return !typeof (IEnumerable).IsAssignableFrom(info.PropertyType) || info.PropertyType == typeof (string) ||
             info.PropertyType == typeof (UserFileCollection);
    }

    private static bool IsCollection(PropertyInfo info)
    {
      return (typeof (IEnumerable).IsAssignableFrom(info.PropertyType) && info.PropertyType != typeof (string)) &&
             info.PropertyType != typeof (UserFileCollection);
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

    private string GetDefaultDisplayName(PropertyInfo info)
    {
      var name = info.Name;
      if (name.Length > 2 && name.EndsWith("Id"))
      {
        name = name.Substring(0, name.Length - 2);
      }

      var convention = CasingEnumMap.Instance[this.AssemblyOptions.GetOption(CasingOptionContext.Field)];

      name = name.Humanize(convention);

      if (info.PropertyType.GetNonNullableType() == typeof (bool))
      {
        name += "?";
      }

      return name;
    }

    private string GetUniqueIdentifier(PropertyInfo info)
    {
      var name = info.Name.Camelize();

      if (!this.usedUniqueIdentifiers.Contains(name))
      {
        this.usedUniqueIdentifiers.Add(name);
        return name;
      }


      var qualifiedName = info.DeclaringType == null ? name : info.DeclaringType.Name.Camelize() + info.Name;

      if (!this.usedUniqueIdentifiers.Contains(qualifiedName))
      {
        this.usedUniqueIdentifiers.Add(qualifiedName);
        return qualifiedName;
      }

      var i = 0;

      while (this.usedUniqueIdentifiers.Contains(name))
      {
        name = qualifiedName + i;

        i++;
      }

      this.usedUniqueIdentifiers.Add(name);
      return name;
    }

    private ViewModelViewModel.FieldViewModel GetFieldViewModel(PropertyInfo info, bool isCollection)
    {
      var isSelectCommon = info.HasAttribute<SelectCommonAttribute>();
      var selectCommonAttribute = isSelectCommon ? info.GetAttribute<SelectCommonAttribute>() : null;

      var fieldModel = new ViewModelViewModel.FieldViewModel
      {
        UniqueId = this.GetUniqueIdentifier(info),
        DocumentTypeSourceProvider =
          info.GetAttributePropertyValueOrDefault<DocumentTypeSourceProviderAttribute, string>(a => a.Expression),
        DataType = isCollection ? info.PropertyType.GetGenericArguments()[0] : info.PropertyType,
        DisplayName =
          info.HasAttribute<DisplayAttribute>()
            ? info.GetAttribute<DisplayAttribute>().Name
            : this.GetDefaultDisplayName(info),
        FieldName = info.Name.Camelize(),
        IsHidden = info.HasAttribute<DoNotShowAttribute>(),
        IsRequired = info.HasAttribute<RequiredAttribute>(),
        IsViewOnly = info.HasAttribute<NotUserEditableAttribute>(),
        Section =
          info.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(a => a.SectionName) ?? string.Empty,
        SelectCommon = selectCommonAttribute,
        IsDefaultSort = info.HasAttribute<DefaultSortAttribute>(),
        InputMask = info.HasAttribute<InputMaskAttribute>() ? info.GetAttribute<InputMaskAttribute>().Mask : null,
        Minimum = info.HasAttribute<RangeAttribute>() ? info.GetAttribute<RangeAttribute>().Minimum : null,
        Maximum = info.HasAttribute<RangeAttribute>() ? info.GetAttribute<RangeAttribute>().Maximum : null,
        StartCap = info.GetAttributePropertyValueOrDefault<CapAttribute, string>(a => a.StartCap),
        EndCap = info.GetAttributePropertyValueOrDefault<CapAttribute, string>(a => a.EndCap),
        Step = this.GetStep(info),
        YesLabelText = info.GetAttributePropertyValueOrDefault<BoolLabelAttribute, string>(a => a.YesLabelText) ?? "Yes",
        NoLabelText = info.GetAttributePropertyValueOrDefault<BoolLabelAttribute, string>(a => a.NoLabelText) ?? "No",
        IsExcludedFromComplexCollectionEditorTable =
          info.HasAttribute<ExcludeFromComplexCollectionEditorTableAttribute>(),
      };

      fieldModel.ControlType =
        this.DetermineControlType(
          isCollection
            ? ViewModelViewModel.MemberDiscriminator.PrimitiveCollection
            : ViewModelViewModel.MemberDiscriminator.Scalar, info);

      return fieldModel;
    }

    private IEnumerable<ViewModelViewModel.FieldViewModel> GetFieldViewModelCollection(IEnumerable<PropertyInfo> t,
      bool isCollection)
    {
      var pfi = t.Select(p => this.GetFieldViewModel(p, isCollection));
      return pfi;
    }

    private ViewModelViewModel.SubordinateViewModelWrapper GetSubordinateViewModelWrapper(PropertyInfo info)
    {
      var hasEnumerableItemAction = info.HasAttribute<SubordinateItemActionAttribute>();
      var itemActionAttribute = hasEnumerableItemAction
        ? info.GetCustomAttributes(typeof (SubordinateItemActionAttribute))
          .Select(a => (SubordinateItemActionAttribute) a)
        : new SubordinateItemActionAttribute[0];
      var defaultObjectDef =
        info.GetAttributePropertyValueOrDefault<CollectionEditorAttribute, string>(a => a.DefaultObjectDefinitionJson);

      if (defaultObjectDef != null)
      {
        defaultObjectDef = WebUtility.HtmlEncode(defaultObjectDef);
      }
      var wrapper = new ViewModelViewModel.SubordinateViewModelWrapper
      {
        FieldName = info.Name.Camelize(),
        Model =
          this.GetViewModelViewModel(info.PropertyType.GetGenericArguments().First()),
        Section =
          info.GetAttributePropertyValueOrDefault<UiSectionAttribute, string>(s => s.SectionName) ?? string.Empty,
        IsHidden = info.HasAttribute<DoNotShowAttribute>(),
        DisplayName =
          info.GetAttributePropertyValueOrDefault<DisplayAttribute, string>(a => a.Name) ??
          info.Name.Humanize(CasingEnumMap.Instance[this.AssemblyOptions.GetOption(CasingOptionContext.Field)]),
        IsLedger = info.HasAttribute<LedgerAttribute>(),
        LedgerSumProperty =
          info.GetAttributePropertyValueOrDefault<LedgerAttribute, string>(a => a.SumPropertyName),
        ItemActionAttributes = itemActionAttribute,
        DefaultObjectJsonDefintion =
          defaultObjectDef,
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

      wrapper.SubordinateActionsExpression = this.GetAdditionalSubordinateActions(wrapper);

      return wrapper;
    }

    private string GetTableVisibleFieldsExpression(ViewModelViewModel.SubordinateViewModelWrapper wrapper)
    {
      var fields =
        wrapper.Model.Scalars
          .Where(s => !s.IsHidden)
          .Where(s => !s.IsExcludedFromComplexCollectionEditorTable)
          .Where(s => !BarredFromTables.Contains(s.DataType));

      var fieldsDesc =
        fields.Select(
          f =>
            new
            {
              name = f.FieldName,
              label = f.DisplayName,
              clientType = f.SelectCommon != null ? "selectcommon" : ClientTypes[f.DataType],
              filterArguments = f.SelectCommon?.QualifiedName
            });

      return this.GetOneLineSerialization(fieldsDesc, noEscapeBraces: true);
    }

    private string GetAdditionalSubordinateActions(ViewModelViewModel.SubordinateViewModelWrapper wrapper)
    {
      if (wrapper.ItemActionAttributes == null || !wrapper.ItemActionAttributes.Any())
      {
        return null;
      }

      var results =
        wrapper.ItemActionAttributes
          .GroupBy(iaa => iaa.ColumnHeadingText)
          .ToDictionary(
            g => g.Key,
            g => g.ToList().Select(iaa => (object)
              new
              {
                attributeName = iaa.AttributeName,
                attributeValue = iaa.AttributeValue,
                actionLabel = iaa.ActionText,
                cssClass = iaa.AreaClass,
                glyphicon = iaa.ActionClass,
              }));


      return this.GetOneLineSerialization(results,
        noEscapeBraces: wrapper.ItemActionAttributes.Any(iaa => iaa.DoNotEscapeAngularBraces));
    }

    private string GetOneLineSerialization(object input, bool noEscapeBraces)
    {
      var sb = new StringBuilder();
      using (var sw = new StringWriter(sb))
      using (var writer = new JsonTextWriter(sw))
      {
        writer.QuoteChar = '\'';
        writer.Formatting = Formatting.None;

        var ser = new JsonSerializer();
        ser.Serialize(writer, input);
      }

      var result = sb.ToString();

      //hack around escaping issues...

      return noEscapeBraces
        ? result
        : Regex.Replace(result, @"{{([^}]+)}}", @"\{\{$1}}");
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

      if (underlyingType == typeof (UserFileCollection))
      {
        return NgangaControlType.FileUploadControl;
      }

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
          break;
        case ViewModelViewModel.MemberDiscriminator.PrimitiveCollection:
          result.Member = this.GetFieldViewModel(info, true);
          break;
        case ViewModelViewModel.MemberDiscriminator.ComplexCollection:
          result.Member = this.GetSubordinateViewModelWrapper(info);
          break;
      }

      result.IsHidden = result.Member.IsHidden;
      return result;
    }

    public ViewModelViewModel GetViewModelViewModel(Type type)
    {
      if (!type.Name.EndsWith("ViewModel"))
      {
        throw new Exception($"{type.FullName} is not a view model");
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
        UnderlyingType = type,
        Name = type.Name.Replace("ViewModel", string.Empty).Camelize(),
        Scalars = this.GetFieldViewModelCollection(
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.Scalar)
            .Select(p => p.pi), false).ToList(),
        IsViewOnly = type.HasAttribute<NotUserEditableAttribute>(),
        ComplexCollections =
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.ComplexCollection)
            .Select(p => p.pi)
            .Select(this.GetSubordinateViewModelWrapper)
            .ToList(),
        PrimitiveCollections = this.GetFieldViewModelCollection(
          decoratedProperties.Where(p => p.discriminator == ViewModelViewModel.MemberDiscriminator.PrimitiveCollection)
            .Select(p => p.pi), true)
          .ToList(),
        Members = decoratedProperties.Select(dp => dp.memberWrapper).ToList()
      };

      var sectionsDict = new Dictionary<string, Dictionary<string, string>>();

      foreach (var attr in type.GetCustomAttributes<SectionHtmlAttributeAttribute>())
      {
        Dictionary<string, string> htmlAttrsDict;

        if (!sectionsDict.TryGetValue(attr.SectionName, out htmlAttrsDict))
        {
          htmlAttrsDict = new Dictionary<string, string>();
          sectionsDict[attr.SectionName.ToUpperInvariant().Replace(@"\w", string.Empty)] = htmlAttrsDict;
        }

        htmlAttrsDict[attr.HtmlAttributeName] = attr.HtmlAttributeValue;
      }

      vm.SectionHtmlAttributeValues = sectionsDict;

      if (!vm.IsViewOnly)
      {
        return vm;
      }

      foreach (var coll in vm.ComplexCollections)
      {
        coll.Model.IsViewOnly = true;
      }

      foreach (var coll in vm.PrimitiveCollections)
      {
        coll.IsViewOnly = true;
      }

      foreach (var memb in vm.Scalars)
      {
        memb.IsViewOnly = true;
      }

      foreach (var memb in vm.Members)
      {
        if (memb.Discriminator == ViewModelViewModel.MemberDiscriminator.ComplexCollection)
        {
          ((ViewModelViewModel.SubordinateViewModelWrapper) memb.Member).Model.IsViewOnly = true;
        }
        else
        {
          ((ViewModelViewModel.FieldViewModel) memb.Member).IsViewOnly = true;
        }
      }

      return vm;
    }
  }
}