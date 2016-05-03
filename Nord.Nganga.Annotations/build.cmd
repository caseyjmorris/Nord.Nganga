REM msbuild Nord.Nganga.Annotations.csproj /p:TargetFrameworkVersion=v4.0;Configuration=Release;OutputPath="lib\net4" /tv:14.0
msbuild Nord.Nganga.Annotations.csproj  /p:TargetFrameworkVersion=v4.5;Configuration=Release;OutputPath="lib\net45" /tv:14.0
msbuild Nord.Nganga.Annotations.csproj  /p:TargetFrameworkVersion=v4.5.1;Configuration=Release;OutputPath="lib\net451" /tv:14.0
msbuild Nord.Nganga.Annotations.csproj  /p:TargetFrameworkVersion=v4.5.2;Configuration=Release;OutputPath="lib\net452" /tv:14.0
msbuild Nord.Nganga.Annotations.csproj  /p:TargetFrameworkVersion=v4.6;Configuration=Release;OutputPath="lib\net46" /tv:14.0
msbuild Nord.Nganga.Annotations.csproj  /p:TargetFrameworkVersion=v4.6.1;Configuration=Release;OutputPath="lib\net461" /tv:14.0
nuget spec -a lib\net45\Nord.Nganga.Annotations.dll -f
..\packages\Tool.NuPrep.1.0.5966.24554\tools\net45\tool.nuprep.exe /optionsFile:nuprep.options.txt