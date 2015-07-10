using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Nord.Nganga.Annotations.Attributes.Angular;
using Nord.Nganga.Engine.Extensions.Reflection;
using Nord.Nganga.Engine.Extensions.Text;
using Nord.Nganga.Engine.Mapping;

namespace Nord.Nganga.Engine.JavaScript
{
  public class ResourceGenerator
  {
    private string FormatArgsForQueryString(IEnumerable<string> args)
    {
      var hasId = args.Contains("id");
      var hasNonIdArgs = args.Any(a => a != "id");
      var sb = new StringBuilder("/");
      sb
        .AppendIf(":id", hasId)
        .AppendIf("/", hasId && hasNonIdArgs);

      if (hasNonIdArgs)
      {
        sb.Append("?");
        var bodies = args.Where(a => a != "id").Select(a => string.Format("{0}=:{0}", a));
        sb.Append(string.Join("&", bodies));
      }
      return sb.ToString();
    }

    private string FormatArgsForDictionary(IEnumerable<string> args)
    {
      var stringBuilder = new StringBuilder();
      var argv = args.ToList();
      for (var i = 0; i < argv.Count; i++)
      {
        var arg = argv[i];
        stringBuilder
          .Append(arg)
          .Append(": ")
          .Append(arg);

        if (i < argv.Count - 1)
        {
          stringBuilder.Append(", ");
        }
      }

      return stringBuilder.ToString();
    }

    private void AppendGetMethod(StringBuilder sb, Type controller, EndpointViewModel endpoint, bool useCache,
      string customCacheFactory)
    {
      sb
        .Indent(3)
        .Append(endpoint.MethodName.ToCamelCase())
        .Append(": function(")
        .Append(string.Join(", ", endpoint.Arguments))
        .AppendIf(", ", endpoint.Arguments.Any())
        .Append("successFn, failFn)\r\n")
        .Indent(4)
        .Append("{\r\n")
        .Indent(5)
        .Append("var rsc = $resource('/api/")
        .Append(controller.Name.Replace("Controller", string.Empty))
        .Append("/")
        .Append(endpoint.MethodName)
        .AppendIf(() => this.FormatArgsForQueryString(endpoint.Arguments), () => endpoint.Arguments.Any())
        .Append("', {}, {'get': {isArray: ")
        .Append(endpoint.ReturnsIEnumerable ? "true" : "false")
        .AppendIf(", cache: ", useCache)
        .AppendIf("true", useCache && string.IsNullOrWhiteSpace(customCacheFactory))
        .AppendIf("resourceCache", useCache && !string.IsNullOrWhiteSpace(customCacheFactory))
        .Append("}});\r\n\r\n")
        .Indent(5)
        .Append("return rsc.get({")
        .AppendIf(() => this.FormatArgsForDictionary(endpoint.Arguments), () => endpoint.Arguments.Any())
        .Append("}, {}, successFn, failFn);\r\n")
        .Indent(4)
        .Append("}");
    }

    private void AppendPostMethod(StringBuilder sb, Type controller, EndpointViewModel endpoint, bool useCache,
      string customCacheFactory)
    {
      sb
        .Indent(3)
        .Append(endpoint.MethodName.ToCamelCase())
        .Append(": function(model, successFn, failFn)\r\n")
        .Indent(4)
        .Append("{\r\n")
        .Indent(5)
        .Append("var rsc = $resource('/api/")
        .Append(controller.Name.Replace("Controller", string.Empty))
        .Append("/")
        .Append(endpoint.MethodName)
        .Append("', {}, {'save': {isArray: ")
        .Append(endpoint.ReturnsIEnumerable ? "true" : "false")
        .Append(", method: 'POST'}});\r\n\r\n")
        .Indent(5)
        .Append("rsc.save({}, model, ");

      if (useCache && !string.IsNullOrWhiteSpace(customCacheFactory))
      {
        sb.Append("function(result)\r\n")
          .Indent(5)
          .Append("{\r\n")
          .Indent(6)
          .Append("resourceCache")
          .Append(".removeAll();\r\n")
          .Indent(6)
          .Append("successFn(result);\r\n")
          .Indent(5)
          .Append("}");
      }
      else
      {
        sb.Append("successFn");
      }

      sb.Append(", failFn);\r\n")
        .Indent(4)
        .Append("}");
    }

    public string GenerateResource(Type controller)
    {
      var appName = controller.GetAttribute<AngularModuleNameAttribute>().ModuleName;
      var useCache = controller.HasAttribute<UseAngularLocalCacheAttribute>() ||
                     controller.HasAttribute<UseAngularGlobalCacheAttribute>();
      var customCacheFactory = controller.HasAttribute<UseAngularLocalCacheAttribute>()
        ? controller.Name.Replace("Controller", string.Empty).ToCamelCase()
        : null;

      return this.GenerateResource(controller, appName, useCache, customCacheFactory);
    }

    public string GenerateResource(Type controller, string appName, bool useCache, string customCacheFactory = null)
    {
      var serviceName = controller.Name.Replace("Controller", string.Empty).ToCamelCase() + "Service";
      var useCf = useCache && !string.IsNullOrWhiteSpace(customCacheFactory);
      var sb = new StringBuilder("// GENERATED CODE --  " + DateTime.Now + " -- Changes to this file may be lost if the code is regenerated.\r\n")
        .Append(appName)
        .Append(".factory('")
        .Append(serviceName)
        .Append("', ['$resource', ")
        .AppendIf("'$cacheFactory', ", useCf)
        .Append("function($resource")
        .AppendIf(", $cacheFactory", useCf)
        .Append(")\r\n")
        .Indent(1)
        .Append("{\r\n")
        .Indent(2)
        .AppendIf("var resourceCache = $cacheFactory('", useCf)
        .AppendIf(customCacheFactory, useCf)
        .AppendIf("');\r\n", useCf)
        .IndentIf(2, useCf)
        .Append("return {\r\n");

      var methods = EndpointMapper.GetEnpoints(controller).ToList();

      for (var i = 0; i < methods.Count; i++)
      {
        var method = methods[i];
        if (method.HttpMethod == EndpointViewModel.HttpMethodType.Post)
        {
          this.AppendPostMethod(sb, controller, method, useCache, customCacheFactory);
        }
        if (method.HttpMethod == EndpointViewModel.HttpMethodType.Get)
        {
          this.AppendGetMethod(sb, controller, method, useCache, customCacheFactory);
        }
        if (i < methods.Count - 1)
        {
          sb.Append(",");
        }
        sb.Append("\r\n");
      }

      sb
        .Indent(2)
        .Append("};\r\n")
        .Indent(1)
        .Append("}]);");

      return sb.ToString();
    }
  }
}