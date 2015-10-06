using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Nord.Nganga.Annotations;
using Nord.Nganga.Annotations.Attributes;
using Nord.Nganga.Annotations.Attributes.Html;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Nord.Nganga.TestConsumer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Nord.Nganga.TestConsumer")]
[assembly: AssemblyCopyright("Copyright ©  2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("ec8959cc-529c-4890-a317-08040bc5de37")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly:
  ProjectStructure(@"client\app\js\ctrl\", @"client\app\js\ui\views\", @"client\app\js\svc\",
    @"Nord.Nganga.TestConsumer.csproj")]
[assembly: TextCasePreferences(CasingOptionContext.Default, CasingOption.Sentence)]
[assembly: TextCasePreferences(CasingOptionContext.Button, CasingOption.Title)]
[assembly: TextCasePreferences(CasingOptionContext.Field, CasingOption.Sentence)]
[assembly: TextCasePreferences(CasingOptionContext.Header, CasingOption.Title)]
[assembly: MinimumTemplateVersion(TemplateContext.Controller, "1.1.0")]
[assembly: MinimumTemplateVersion(TemplateContext.Resource, "1.1.0")]
[assembly: MinimumTemplateVersion(TemplateContext.View, "1.1.3")]