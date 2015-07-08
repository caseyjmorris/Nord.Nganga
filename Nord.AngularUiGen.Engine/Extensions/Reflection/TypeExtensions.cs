using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Nord.AngularUiGen.Engine.Extensions.Reflection
{
  public static class TypeExtensions
  {
    public static bool IsSuperTypeOf(this Type superType, Type subType)
    {
      var node = subType;
      while (node.BaseType != null)
      {
        if (string.IsNullOrEmpty(node.BaseType.AssemblyQualifiedName))
          continue;

        if (node.BaseType.AssemblyQualifiedName.Equals(superType.AssemblyQualifiedName,
          StringComparison.InvariantCultureIgnoreCase))
        {
          return true;
        }

        node = node.BaseType;
      }
      return false;
    }

    public static Type GetNonnullableNonCollectionType(this Type type)
    {
      return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) && typeof(string) != type
        ? type.GetGenericArguments().First().GetNonNullableType()
        : type.GetNonNullableType();
    }

    public static Type GetNonNullableType(this Type type)
    {
      if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        return type.GetGenericArguments().First();
      }
      return type;
    }

    public static bool HasAttribute<T>(this Type type) where T : Attribute
    {
      return Attribute.IsDefined(type, typeof(T));
    }

    public static bool HasAttribute<T>(this MemberInfo member) where T : Attribute
    {
      return Attribute.IsDefined(member, typeof(T));
    }

    public static T GetAttribute<T>(this Type type) where T : Attribute
    {
      return (T) Attribute.GetCustomAttribute(type, typeof(T));
    }

    public static T GetAttribute<T>(this MemberInfo member) where T : Attribute
    {
      return (T) Attribute.GetCustomAttribute(member, typeof(T));
    }

    public static bool TryGetAttributePropertyValue<TAttribute, TReturn>(this Type type, out TReturn returnValue,
      Func<TAttribute, TReturn> property) where TAttribute : Attribute
    {
      if (type.HasAttribute<TAttribute>())
      {
        returnValue = property(type.GetAttribute<TAttribute>());
        return true;
      }
      returnValue = default(TReturn);
      return false;
    }

    public static bool TryGetAttributePropertyValue<TAttribute, TReturn>(this MemberInfo member, out TReturn returnValue,
      Func<TAttribute, TReturn> property) where TAttribute : Attribute
    {
      if (member.HasAttribute<TAttribute>())
      {
        returnValue = property(member.GetAttribute<TAttribute>());
        return true;
      }
      returnValue = default(TReturn);
      return false;
    }

    public static TReturn GetAttributePropertyValueOrDefault<TAttribute, TReturn>(this Type type,
      Func<TAttribute, TReturn> property) where TAttribute : Attribute
    {
      return type.HasAttribute<TAttribute>() ? property(type.GetAttribute<TAttribute>()) : default(TReturn);
    }


    public static TReturn GetAttributePropertyValueOrDefault<TAttribute, TReturn>(this MemberInfo member,
      Func<TAttribute, TReturn> property) where TAttribute : Attribute
    {
      return member.HasAttribute<TAttribute>() ? property(member.GetAttribute<TAttribute>()) : default(TReturn);
    }
  }
}