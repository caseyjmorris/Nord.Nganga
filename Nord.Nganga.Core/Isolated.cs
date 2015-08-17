using System;
namespace Nord.Nganga.Core
{
  public sealed class Isolated<T> : IDisposable where T : MarshalByRefObject
  {
    private AppDomain domain;
    private readonly T value;

    public Isolated(AppDomainSetup ads = null)
    {
      this.domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(),
        null, ads ?? AppDomain.CurrentDomain.SetupInformation);

      var type = typeof(T);

      this.value = (T) this.domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
    }

    public T Value
    {
      get { return this.value; }
    }

    public void Dispose()
    {
      if (this.domain == null) return;
      AppDomain.Unload(this.domain);

      this.domain = null;
    }
  }
}