using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Nord.Nganga.ObjectBrowser
{
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

  /// <summary>
  /// Summary description for PropertyBinding.
  /// </summary>
  public class PropertyBinding
  {
    public enum BindingTypes
    {
      String,
      DateTime,
      Enum,
      Number,
      Boolean,
      Object,
      Enumerable,
      Color,
      List,
      Unsupported,
      Unknown
    }

    public System.Windows.Forms.Control Control { get; set; }

    private int pIndex;

    public int Index
    {
      get { return this.pIndex; }
      set
      {
        this.pIndex = value;
        this.IsIndexed = true;
      }
    }

    public bool IsIndexed { get; private set; }

    public bool? IsGeneric => this.PropertyValueType?.IsGenericType;
    public Type GenericType => this.IsGeneric.HasValue && this.IsGeneric.Value  ? this.PropertyValueType.GetGenericTypeDefinition() : null;
    public bool? IsEnumerable => this.PropertyValueType?.IsIEnumerable();
    public string PropertyValueTypeName => this.PropertyValueType?.Name;

    public string PropertyValueBaseTypeName => this.PropertyValueType?.BaseType?.Name ?? string.Empty;
    private Dictionary<Type,BindingTypes> propertyTypeBindingTypesDictionary = new Dictionary<Type, BindingTypes>
    {
      {typeof(string), BindingTypes.String },
            {typeof(DateTime), BindingTypes.DateTime  },
            {typeof(bool), BindingTypes.Boolean },
            {typeof(short), BindingTypes.Number },
            {typeof(int), BindingTypes.Number },
            {typeof(long), BindingTypes.Number },
            {typeof(float), BindingTypes.Number },
            {typeof(double), BindingTypes.Number },
            {typeof(Color), BindingTypes.Number },
            {typeof(byte), BindingTypes.Number },
            {typeof(IntPtr), BindingTypes.Number },
                        {typeof(Size), BindingTypes.Object  },
                        {typeof(Rectangle), BindingTypes.Object },
                        {typeof(Enum), BindingTypes.Enum },
    };

    public BindingTypes BindingType
    {
      get
      {
        if (this.PropertyValue == null)
        {
          return BindingTypes.Unknown;
        }
        if (this.PropertyValueType == null)
        {
          return BindingTypes.Unsupported;
        }

        Func<Type, BindingTypes> resolver =
          type => (this.IsEnumerable ?? false) ? BindingTypes.Enumerable : BindingTypes.Unsupported;

        var bindingType = this.propertyTypeBindingTypesDictionary.ContainsKey(this.PropertyValueType)
          ? this.propertyTypeBindingTypesDictionary[this.PropertyValueType]
          : resolver(this.PropertyValueType);

        return bindingType;
      }
    }

    public PropertyBinding(
      object anObject,
      PropertyInfo aProperty)
    {
      this.BoundObject = anObject;
      this.Property = aProperty;
      //System.Diagnostics.Debug.WriteLine( this.PropertyName + " " + this.PropertyValueType.Name + " " + this.BindingType.ToString() ) ; 
    }

    public PropertyBinding(
      object anObject,
      PropertyInfo aProperty,
      int anIndex)
    {
      this.BoundObject = anObject;
      this.Property = aProperty;
      this.Index = anIndex;
      //System.Diagnostics.Debug.WriteLine( this.PropertyName + " " + this.PropertyValueType.Name + " " + this.BindingType.ToString() ) ; 		
    }

    public object PropertyValue
    {
      get
      {
        if (this.IsIndexed)
        {
          //return this.Property.GetValue( this.BoundObject, this.Index ) ; 
          throw new SystemException("Indexed binding not yet supported.");
        }
        else
        {
          return this.Property.GetValue(this.BoundObject, null);
        }
      }
      set
      {
        if (this.IsIndexed)
        {
          //this.Property.SetValue( this.BoundObject, value, this.Index ) ; 
          throw new SystemException("Indexed binding not yet supported.");
        }
        else
        {
          this.Property.SetValue(this.BoundObject, value, null);
        }
      }
    }

    public PropertyInfo Property { get; set; }

    public string PropertyName => this.Property?.Name;

    public Type PropertyValueType => this.PropertyValue?.GetType();

    public object BoundObject { get; set; }
  }
}
