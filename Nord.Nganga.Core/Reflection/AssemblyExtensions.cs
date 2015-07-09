﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nord.AngularUiGen.Annotations.Attributes.Angular;

namespace Nord.Nganga.Core.Reflection
{
  public static class AssemblyExtensions
  {
    public static IEnumerable<Type> FindWebApiControllers(
      this Assembly assembly,
      string apiControllerName,
      bool assertApiController = true,
      bool assertAngularModuleNameAttribute = true,
      bool assertAngularRouteIdParmAttribute = true)
    {
      var result = new List<Type>();

      var apiControllerType = DependentTypeResolver.GetTypeByName(assembly, apiControllerName);

      // if we cannot find  the ApiController type, then we're not going to find any controllers either 
      if (apiControllerType == null) return result;

      var candidateTypes = assembly.GetTypes();
      result.AddRange(
        candidateTypes.Where(
          t =>
            (!assertApiController || (!t.IsAbstract && apiControllerType.IsSuperTypeOf(t))) &&
            (!assertAngularRouteIdParmAttribute || t.HasAttribute<AngularRouteIdParameterAttribute>()) &&
            (!assertAngularModuleNameAttribute || t.HasAttribute<AngularModuleNameAttribute>())
          ));

      return result;
    }
  }
}