using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Humanizer;
using Nord.Nganga.Annotations;
using Nord.Nganga.Annotations.Attributes.Html;
using Nord.Nganga.Annotations.Attributes.ViewModels;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;
using Nord.Nganga.Models.ViewModels;

namespace Nord.Nganga.Mappers.Views
{
  public class ViewCoordinationMapper
  {
    private readonly ViewModelMapper viewModelMapper;

    private readonly EndpointFilter endpointFilter;

    private readonly EndpointMapper endpointMapper;

    private readonly WebApiSettingsPackage webApiSettings;

    private Type httpGetAttribute;

    private Type httpPostAttribute;

    public ViewCoordinationMapper(
      ViewModelMapper viewModelMapper,
      EndpointFilter endpointFilter,
      EndpointMapper endpointMapper,
      WebApiSettingsPackage settings)
    {
      this.viewModelMapper = viewModelMapper;
      this.endpointFilter = endpointFilter;
      this.endpointMapper = endpointMapper;
      this.webApiSettings = settings;
    }

    public ViewCoordinationMapper(WebApiSettingsPackage settings)
    {
      this.webApiSettings = settings;
      var vmMapper = new ViewModelMapper(this);
      var filter = new EndpointFilter(vmMapper);

      this.viewModelMapper = vmMapper;

      this.endpointFilter = filter;

      this.endpointMapper = new EndpointMapper(settings);
    }

    public ViewCoordinationInformationCollectionViewModel GetViewCoordinatedInformationCollection(Type controller)
    {
      this.httpGetAttribute = DependentTypeResolver.GetTypeByName(controller.Assembly,
        this.webApiSettings.HttpGetAttributeName);

      this.httpPostAttribute = DependentTypeResolver.GetTypeByName(controller.Assembly,
        this.webApiSettings.HttpPostAttributeName);

      this.viewModelMapper.AssemblyOptions = new AssemblyOptionsModel(controller);

      var endpoints = this.endpointMapper.GetEnpoints(controller);

      var filteredInfo = this.endpointFilter.ExamineEndpoints(endpoints);

      var coordinatedInfo =
        filteredInfo.TargetComplexTypesAtRoot.Select(
          t => this.GetViewCoordinatedInformationSingleInternal(t, 1, controller)).ToList();

      return new ViewCoordinationInformationCollectionViewModel
      {
        NgControllerName = controller.Name.Replace("Controller", "Ctrl").Camelize(),
        ViewCoordinatedInfo = coordinatedInfo,
        Header =
          controller.GetAttributePropertyValueOrDefault<PageHeaderAttribute, string>(a => a.Header) ??
          controller.Name.Replace("Controller", string.Empty)
            .Humanize(CasingEnumMap.Instance[this.viewModelMapper.AssemblyOptions.GetOption(CasingOptionContext.Header)]),
        EditRestricted = filteredInfo.PrivilegedRoles?.Any() ?? false
      };
    }

    public ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingle(Type vmType, int depthMultiplier = 1,
      Type controllerType = null)
    {
      var vmVm = this.viewModelMapper.GetViewModelViewModel(vmType);

      return this.GetViewCoordinatedInformationSingleInternal(vmVm, depthMultiplier);
    }

    private ViewCoordinatedInformationViewModel GetViewCoordinatedInformationSingleInternal(ViewModelViewModel vmVm,
      int depthMultipler, Type controllerType = null)
    {
      var coord = new ViewCoordinatedInformationViewModel
      {
        ViewModel = vmVm,
        Glyphicon =
          vmVm.UnderlyingType.GetAttributePropertyValueOrDefault<SaveButtonTextAttribute, string>(a => a.Glyphicon) ??
          "glyphicon-floppy-save",
        SaveButtonText =
          vmVm.UnderlyingType.GetAttributePropertyValueOrDefault<SaveButtonTextAttribute, string>(a => a.Text) ??
          ("Save changes to " + vmVm.Name.Humanize(LetterCasing.LowerCase)).Humanize(
            CasingEnumMap.Instance[this.viewModelMapper.AssemblyOptions.GetOption(CasingOptionContext.Button)]),
        Sections = this.SplitSections(vmVm, depthMultipler),
        Title =
          vmVm.Name.Humanize(
            CasingEnumMap.Instance[this.viewModelMapper.AssemblyOptions.GetOption(CasingOptionContext.Header)]),
        NgFormName = vmVm.Name.Camelize() + "Form",
        NgSubmitAction = $"saveChangesTo{vmVm.Name.Pascalize()}()",
        ParentObjectName = vmVm.Name.Camelize(),
        HtmlIncludes = this.GetIncludesFromControllerType(controllerType, vmVm),
        NgFormAttributes = this.GetNgAttributesFromControllerType(controllerType, vmVm)
      };

      return coord;
    }

    private Dictionary<string, string> GetNgAttributesFromControllerType(Type controllerType,
      ViewModelViewModel vmVm)
    {
      if (controllerType == null)
      {
        return null;
      }

      if (vmVm == null)
      {
        throw new ArgumentException("VM VM can't be null", nameof(vmVm));
      }

      var returnTypeName = $"{vmVm.Name.Pascalize()}ViewModel";

      var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(m => Attribute.IsDefined(m, this.httpGetAttribute))
        .Where(m => m.ReturnType.Name == returnTypeName ||
                    (m.ReturnType.IsGenericType && m.ReturnType.GetGenericArguments()[0].Name == returnTypeName))
        .Where(m => m.HasAttribute<FormHtmlAttributeAttribute>());

      var attrs = methods.SelectMany(m => m.GetCustomAttributes(true).OfType<FormHtmlAttributeAttribute>());

      var result = new Dictionary<string, string>();
      foreach (var attr in attrs)
      {
        result[attr.HtmlAttributeName] = attr.HtmlAttributeValue;
      }

      return result;
    }

    private Dictionary<string, IEnumerable<string>> GetIncludesFromControllerType(Type controllerType,
      ViewModelViewModel vmVm)
    {
      if (controllerType == null)
      {
        return null;
      }

      if (vmVm == null)
      {
        throw new ArgumentException("VM VM can't be null", nameof(vmVm));
      }

      var returnTypeName = $"{vmVm.Name.Pascalize()}ViewModel";

      Func<Type, bool> matchesReturnType =
        t => t.Name == returnTypeName || (t.IsGenericType && t.GetGenericArguments()[0].Name == returnTypeName);

      var getMethods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(m => Attribute.IsDefined(m, this.httpGetAttribute))
        .Where(m => matchesReturnType(m.ReturnType))
        .Where(m => m.HasAttribute<InjectHtmlInViewAttribute>());

      var postMethods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(m => Attribute.IsDefined(m, this.httpPostAttribute))
        .Where(m => m.GetParameters().Any())
        .Where(m => matchesReturnType(m.GetParameters()[0].ParameterType))
        .Where(m => m.HasAttribute<InjectHtmlInViewAttribute>());

      var methods = getMethods.Concat(postMethods);

      var attrs = methods.SelectMany(m => m.GetCustomAttributes(true).OfType<InjectHtmlInViewAttribute>());

      var grouped = attrs.GroupBy(a => a.HtmlPosition);

      return grouped.ToDictionary(g => g.Key.ToString(), g => (IEnumerable<string>) g.Select(i => i.Content).ToList());
    }

    private IEnumerable<ViewCoordinatedInformationViewModel.SectionViewModel> SplitSections(ViewModelViewModel vmVm,
      int depthMultipler)
    {
      depthMultipler = 1;
      var sections = new List<ViewCoordinatedInformationViewModel.SectionViewModel>();

      const int rowMax = 12;

      Dictionary<string, string> sectionAttrs;

      var currentSection = new ViewCoordinatedInformationViewModel.SectionViewModel
      {
        Title = vmVm.Members[0].Section,
        Rows = new List<ViewCoordinatedInformationViewModel.RowViewModel>()
      };

      if (
        vmVm.SectionHtmlAttributeValues.TryGetValue(
          currentSection.Title.ToUpperInvariant().Replace(@"\w", string.Empty), out sectionAttrs))
      {
        currentSection.Attributes = sectionAttrs;
      }

      sections.Add(currentSection);

      var currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
      {
        Members = new List<ViewModelViewModel.MemberWrapper>()
      };

      currentSection.Rows.Add(currentRow);

      var currentRowWidth = 0;

      foreach (var member in vmVm.Members.Where(m => !m.IsHidden))
      {
        if (member.Section != currentSection.Title)
        {
          currentSection = new ViewCoordinatedInformationViewModel.SectionViewModel
          {
            Title = member.Section,
            Rows = new List<ViewCoordinatedInformationViewModel.RowViewModel>()
          };

          if (
            vmVm.SectionHtmlAttributeValues.TryGetValue(
              currentSection.Title.ToUpperInvariant().Replace(@"\w", string.Empty), out sectionAttrs))
          {
            currentSection.Attributes = sectionAttrs;
          }

          currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
          {
            Members = new List<ViewModelViewModel.MemberWrapper>()
          };

          currentSection.Rows.Add(currentRow);

          currentRowWidth = 0;

          sections.Add(currentSection);
        }

        var memberWidth = member.Width*depthMultipler;

        if (memberWidth > 12)
        {
          memberWidth = 12;
        }

        currentRowWidth += memberWidth;

        if (currentRowWidth > rowMax)
        {
          currentRow = new ViewCoordinatedInformationViewModel.RowViewModel
          {
            Members = new List<ViewModelViewModel.MemberWrapper>()
          };

          currentSection.Rows.Add(currentRow);

          currentRowWidth = memberWidth;
        }

        currentRow.Members.Add(member);
      }

      return sections;
    }
  }
}