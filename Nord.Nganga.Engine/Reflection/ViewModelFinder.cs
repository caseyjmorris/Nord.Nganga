using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nord.Nganga.Engine.Reflection
{
  public class ViewModelFinder
  {
    public IEnumerable<Type> FindViewModels(Assembly asm)
    {
      return asm.GetTypes().Where(t => t.Name.EndsWith("ViewModel"));
    }
  }
}
