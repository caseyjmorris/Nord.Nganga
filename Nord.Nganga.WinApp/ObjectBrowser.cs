﻿using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Nord.Nganga.WinApp
{
  public partial class ObjectBrowser : TreeView
  {
    private object dataSource;

    public ObjectBrowser()
    {
      this.InitializeComponent();
    }

    public object DataSource
    {
      set
      {
        this.dataSource = value;
        this.BindObject(value, this.Nodes);
        this.ExpandAll();
      }
    }

    private void BindObject(object instance, TreeNodeCollection parenTreeNodeCollection)
    {
      var iType = instance.GetType();
      var isEnumerable = iType.IsIEnumerable();
      if (isEnumerable)
      {
        foreach (var member in (IEnumerable) instance)
        {
          this.BindObject(member, parenTreeNodeCollection);
        }
      }
      else
      {
        var piList = iType.GetProperties().Where(p => p.CanRead);
        foreach (var pi in piList)
        {
          this.BindProperty(pi, instance, parenTreeNodeCollection);
        }
      }
    }

    private void BindProperty(PropertyInfo propertyInfo, object instance, TreeNodeCollection parenTreeNodeCollection)
    {
      var propertyName = propertyInfo.Name;

      var instanceType = instance.GetType();
      if ((!instanceType.IsClass) || instanceType == typeof(string))
      {
        var propertyNode = new TreeNode($"{propertyName}: {instance}") {Tag = instance};
        parenTreeNodeCollection.Add(propertyNode);
        return;
      }

      var propertyValue = propertyInfo.GetValue(instance);

      var enumerableValue = propertyValue as IEnumerable;
      if (enumerableValue != null && propertyInfo.PropertyType != typeof(string))
      {
        var propertyNode = new TreeNode($"{propertyName}") {Tag = instance};
        parenTreeNodeCollection.Add(propertyNode);
        foreach (var member in enumerableValue)
        {
          this.BindObject(member, propertyNode.Nodes);
        }
      }
      else
      {
        var propertyNode = new TreeNode($"{propertyName}: {propertyValue}") {Tag = instance};
        parenTreeNodeCollection.Add(propertyNode);
      }
    }
  }

  internal static class TypeTools
  {
    public static bool IsIEnumerable(this Type objectType)
    {
      if (objectType.IsPrimitive)
        return false;
      if (objectType.GetConstructor(Type.EmptyTypes) == null) return false;
      var tempObject = Activator.CreateInstance(objectType);
      return tempObject is IEnumerable;
    }
  }
}