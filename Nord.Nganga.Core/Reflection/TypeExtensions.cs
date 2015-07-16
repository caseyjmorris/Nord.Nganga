using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Humanizer;

namespace Nord.Nganga.Core.Reflection
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

    public static string GetFriendlyName(this Type type)
    {
      string name;
      if (!type.IsGenericType)
      {
        name = type.Name;
      }
      else if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
      {
        name = type.GetGenericArguments()[0].Name.Pluralize();
      }
      else
      {
        var start =
          type.GetGenericTypeDefinition().Name;

        start = Regex.Replace(start, @"[^A-Za-z]", string.Empty);

        var sb = new StringBuilder(start).Append("Of");

        var args = type.GetGenericArguments();

        for (var i = 0; i < args.Length; i++)
        {
          if (i > 0)
          {
            sb.Append("To");
          }
          sb.Append(args[i].Name);
        }
        name = sb.ToString();
      }

      return name.Replace("ViewModel", string.Empty);
    }
  }
}