$pkgName = 'Nord.Nganga.Annotations'

$noUncommitedChanges = ((git status -sb | Out-String).ToString().Trim() | Measure-Object -Line).Lines -eq 1

if (-not $noUncommitedChanges)
{
  throw "Commit or discard all changes before publishing."
}

$commitText = git log -5 --oneline | Out-String

Copy-Item -Force .\$($pkgName).nuspec .\$($pkgName).nuspec~

$nuspecXml = [xml](Get-Content .\$($pkgName).nuspec)

$nuspecXml.package.metadata.releaseNotes = $commitText.ToString()

$currentPath = (pwd).Path

$nuspecXml.Save($currentPath + "\$($pkgName).nuspec")

msbuild $($pkgName).csproj /target:Rebuild /property:Configuration=Release
nuget pack $($pkgName).nuspec -Symbols -Prop Configuration=Release -OutputDirectory .\nuget-packages
$recentPackages = ls .\nuget-packages\*.nupkg | Where-Object {[DateTime]::Now.Subtract($_.LastWriteTime).TotalMinutes -le 10}
foreach ($recentPackage in $recentPackages)
{
  nuget push $recentPackage.FullName -source https://www.myget.org/F/nord-pkg/
}