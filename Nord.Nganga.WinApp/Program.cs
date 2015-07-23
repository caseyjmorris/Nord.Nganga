using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Nord.Nganga.Core;
using Nord.Nganga.Fs.Coordination;

namespace Nord.Nganga.WinApp
{
  internal static class Program
  {

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new NgangaMain(Coordinate));
    }


    private static AppDomain CreateAppDomain()
    {
      // Construct and initialize settings for AppDomain.
      var ads = new AppDomainSetup
      {
        ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
        DisallowBindingRedirects = false,
        DisallowCodeDownload = true,
        ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
      };

      // Create the AppDomain.
      var ngandaDomain = AppDomain.CreateDomain("Nganga", null, ads);
      return ngandaDomain;
    }

    // creates the coordination app domain and 
    // instantiates the host and invokes the run method 
    // unloads the app domain to ensure the client assemblies are unloaded 
    // returns the coordination result
    private static IEnumerable<CoordinationResult> Coordinate(StringFormatProviderVisitor logHandler)
    {
      var domain = CreateAppDomain();
      var exeAssembly = Assembly.GetEntryAssembly().FullName;
      var host = (CoordinatorHost) domain.CreateInstanceAndUnwrap(
        exeAssembly,
        typeof (CoordinatorHost).FullName);
      var result = host.Run(logHandler);
      AppDomain.Unload(domain);
      return result;
    }
  }

  // hosts the coordination form in a separate app domain 
  // this ensures that any loaded assemblies are freed upon completion
  public class CoordinatorHost : MarshalByRefObject
  {
    public IEnumerable<CoordinationResult> Run(StringFormatProviderVisitor logHandler)
    {
      var result = new List<CoordinationResult>();
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new CoordinationForm(r => result.Add(r), logHandler));
      return result;
    }
  }
}
