using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nord.AngularUiGen.Annotations.Attributes.Angular;
using Nord.AngularUiGen.Annotations.Attributes.Html;
using Nord.Nganga.Core.Reflection;
using Nord.Nganga.Models;
using Nord.Nganga.Models.Configuration;

namespace Nord.AngularUiGen.Mappers.Resources
{
  public class EndpointMapper
  {
    private readonly WebApiSettingsPackage webApiSettings;

    public EndpointMapper(WebApiSettingsPackage webApiSettings)
    {
      this.webApiSettings = webApiSettings;
    }

    public IEnumerable<EndpointViewModel> GetEnpoints(Type controller)
    {
      var httpGetAttributeType = DependentTypeResolver.GetTypeByName(controller.Assembly,
        this.webApiSettings.HttpGetAttributeName);

      var httpPostAttributeType = DependentTypeResolver.GetTypeByName(controller.Assembly,
        this.webApiSettings.HttpPostAttributeName);

      var authorizeAttribute = DependentTypeResolver.GetTypeByName(controller.Assembly,
        this.webApiSettings.AuthorizeAttributeName);

      var handler = DependentTypeResolver.CreateResolutionEventHandler(controller.Assembly);
      // set up the dependency resolution handler 
      AppDomain.CurrentDomain.AssemblyResolve += handler;

      // the ToList methods are used here to force the resolution of the queries 
      // while the handler is STILL registered... otherwise the type resolution may fail...
      var controllerMethods = (
        from MethodInfo methodInfo in controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
        let customAttribs = methodInfo.GetCustomAttributes()
        select new {methodInfo, customAttribs}
        ).ToList();

      Func<IEnumerable<object>, Type, bool> containsType = (objects, type) =>
      {
        //controllerMethods[0].customAttribs.First().GetType().Name
        return objects.Any(o => o.GetType().Name == type.Name);
      };

      var decoratedMethods = (
        from dmi in controllerMethods
        let isEnumerable = (dmi.methodInfo.ReturnType.IsGenericType &&
                            typeof(IEnumerable<>).IsAssignableFrom(
                              dmi.methodInfo.ReturnType.GetGenericTypeDefinition()))
        let hasReturnType = isEnumerable || dmi.methodInfo.ReturnType.Name.EndsWith("ViewModel")
        let returnType = hasReturnType
          ? isEnumerable
            ? dmi.methodInfo.ReturnType.GetGenericArguments()[0]
            : dmi.methodInfo.ReturnType
          : null
        select new
        {
          dmi.methodInfo,
          isPost = containsType(dmi.customAttribs, httpPostAttributeType),
          //.CustomAttributes Attribute.IsDefined(methodInfo, httpGetAttributeType), 
          isGet = containsType(dmi.customAttribs, httpGetAttributeType),
          //Attribute.IsDefined(methodInfo, httpPostAttributeType), 
          isEnumerable,
          hasReturnType,
          returnType
        }).ToList();

      var httpMethods = (from x in decoratedMethods
        where x.isGet || x.isPost
        let hasAuthorizeAttribute = Attribute.IsDefined(x.methodInfo, authorizeAttribute)
        let authAttribute = hasAuthorizeAttribute ? x.methodInfo.GetCustomAttribute(authorizeAttribute) : null
        let httpMethod = x.isPost ? EndpointViewModel.HttpMethodType.Post : EndpointViewModel.HttpMethodType.Get
        let specialAuth = hasAuthorizeAttribute
          ? authAttribute.GetProperty<string>("Roles").Split(',')
          : null
        select
          new {x.methodInfo, x.isGet, x.isPost, x.hasReturnType, x.returnType, x.isEnumerable, specialAuth, httpMethod})
        .ToList();


      var endpointModels = httpMethods.Select(m => new EndpointViewModel
      {
        HttpMethod = m.httpMethod,
        MethodName = m.methodInfo.Name,
        Arguments = m.methodInfo.GetParameters().Select(p => p.Name).ToList(),
        ArgumentTypes = m.methodInfo.GetParameters().Select(p => p.ParameterType).ToList(),
        HasReturnValue = m.hasReturnType,
        ReturnsIEnumerable = m.isEnumerable,
        ReturnType = m.returnType,
        SpecialAuthorization = m.specialAuth,
        SectionHeader =
          m.methodInfo.GetAttributePropertyValueOrDefault<AngularControllerActionSectionHeaderAttribute, string>(
            a => a.Header),
        OnPostFailureExpressions =
          m.methodInfo.GetCustomAttributes<JavaScriptOnPostCompleteAttribute>()
            .Where(a => a.Context == JavaScriptOnPostCompleteAttribute.ContextType.Failure)
            .Select(a => a.Expression),
        OnPostSuccessExpressions =
          m.methodInfo.GetCustomAttributes<JavaScriptOnPostCompleteAttribute>()
            .Where(a => a.Context == JavaScriptOnPostCompleteAttribute.ContextType.Success)
            .Select(a => a.Expression),
        ResourceOnly = m.methodInfo.HasAttribute<GenerateResourceOnlyAttribute>(),
      }).ToList();

      AppDomain.CurrentDomain.AssemblyResolve -= handler;
      return endpointModels;
    }
  }
}