using System;
using System.Reflection;

namespace Nord.Nganga.ObjectBrowser
{
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
      Color,
      List,
      Unsupported
    }

    private System.Windows.Forms.Control pControl;
    public System.Windows.Forms.Control Control
    {
      get
      {
        return this.pControl;
      }
      set
      {
        this.pControl = value;
      }
    }

    private int pIndex;
    public int Index
    {
      get
      {
        return this.pIndex;
      }
      set
      {
        this.pIndex = value;
        this.pIsIndexed = true;
      }
    }

    private bool pIsIndexed = false;
    public bool IsIndexed
    {
      get
      {
        return this.pIsIndexed;
      }
    }

    public string PropertyValueTypeName
    {
      get
      {
        return this.PropertyValueType.Name;

      }
    }

    public string PropertyValueBaseTypeName
    {
      get
      {
        if (this.PropertyValueType.BaseType != null)
        {
          return this.PropertyValueType.BaseType.Name;
        }
        else
        {
          return string.Empty;
        }

      }
    }

    public BindingTypes BindingType
    {
      get
      {
        BindingTypes bindingType;

        if (this.PropertyValueTypeName.Equals("String"))
        {
          bindingType = BindingTypes.String;
        }
        else if (this.PropertyValueTypeName.Equals("DateTime"))
        {
          bindingType = BindingTypes.DateTime;
        }

        else if (this.PropertyValueTypeName.Equals("Boolean"))
        {
          bindingType = BindingTypes.Boolean;
        }
        else if (
            this.PropertyValueTypeName.Equals("Int16") ||
            this.PropertyValueTypeName.Equals("Int32") ||
            this.PropertyValueTypeName.Equals("Int64") ||
            this.PropertyValueTypeName.Equals("Single") ||
            this.PropertyValueTypeName.Equals("Double") ||
            this.PropertyValueTypeName.Equals("Color") ||
            this.PropertyValueTypeName.Equals("Byte") ||
            this.PropertyValueTypeName.Equals("IntPtr")
            )
        {
          bindingType = BindingTypes.Number;
        }
        else if (
            this.PropertyValueTypeName.Equals("Size") ||
            this.PropertyValueTypeName.Equals("Rectangle") ||
            this.PropertyValueTypeName.Equals("Point") ||
            this.PropertyValueTypeName.Equals("MainMenu") ||
            this.PropertyValueTypeName.Equals("Font")
            )
        {
          bindingType = BindingTypes.Object;
        }
        //				else if ( typeName.Equals( "ArrayList" ) ) 
        //				{
        //					bindingType = BindingTypes.List ; 
        //				}
        else if (this.PropertyValueBaseTypeName.Equals("Enum"))
        {
          bindingType = BindingTypes.Enum;
        }
        else
        {
          //bindingType = BindingTypes.Object ;

          bindingType = BindingTypes.Unsupported;

          //System.Diagnostics.Debug.WriteLine( 
          //    "Unsupported binding type Type: Hash:" + this.pBoundObject.GetHashCode().ToString() + 
          //    " Base:" + PropertyValueBaseTypeName + " Type:" + PropertyValueTypeName  ) ;
        }
        return bindingType;
      }
    }

    public PropertyBinding (
        object anObject,
        PropertyInfo aProperty)
    {
      this.BoundObject = anObject;
      this.Property = aProperty;
      //System.Diagnostics.Debug.WriteLine( this.PropertyName + " " + this.PropertyValueType.Name + " " + this.BindingType.ToString() ) ; 
    }

    public PropertyBinding (
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

    public PropertyInfo PProperty;
    public PropertyInfo Property
    {
      get
      {
        return this.PProperty;
      }
      set
      {
        this.PProperty = value;
      }
    }

    public string PropertyName
    {
      get
      {
        return this.Property.Name;
      }
    }

    public Type PropertyValueType
    {
      get
      {
        return this.PropertyValue.GetType();
      }
    }

    private object pBoundObject;
    public object BoundObject
    {
      get
      {
        return this.pBoundObject;
      }
      set
      {
        this.pBoundObject = value;
      }
    }

  }
}
