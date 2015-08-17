using System;
using System.IO;
using System.Reflection;
using Nord.Nganga.Core.Reflection;

namespace Nord.Nganga.Commands
{
  [Serializable]
  public sealed class Isolated<T> : IDisposable where T : MarshalByRefObject
  {
    private AppDomain domain;
    private readonly T value;

    public Isolated(AppDomainSetup ads = null)
    {
      this.domain = AppDomain.CreateDomain(
        "Isolated:" + Guid.NewGuid(),
        null,
        ads ?? new AppDomainSetup
        {
          ApplicationBase = Path.GetDirectoryName(typeof(T).Assembly.Location),
          DisallowBindingRedirects = false,
          DisallowCodeDownload = true,
          ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
        });

      var type = typeof(T);
      //string basePath;
      //var resolveEventHandler = DependentTypeResolver.CreateResolutionEventHandler(typeof(T).Assembly, out basePath);
      //AppDomain.CurrentDomain.TypeResolve +=  resolveEventHandler;

      // this.value = (T) this.domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);

        var typeAssy = type.Assembly;
        var host = (T) this.domain.CreateInstanceAndUnwrap( typeAssy.FullName, type.FullName);

        this.value = host;
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