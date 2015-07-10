namespace Nord.Nganga.Engine.Reflection
{
  public static class ObjectExtensions
  {
    public static TProperty GetProperty<TProperty>(this object instance, string name)
    {
      var pi = instance.GetType().GetProperty(name);
      var val = (TProperty)pi.GetValue(instance);
      return val;
    }
  }
}
