$pkgName = 'Nord.Nganga.Annotations'
$currentPath = (pwd).Path
$packageFile = $($currentPath) + '\' + $($pkgName).nuspec
$backupFile =  $($currentPath) + '\' + $($pkgName).nuspec + '~'
$projectFile = $($currentPath) + '\'+ $($pkgName).csproj
$outputPath = $($currentPath) + '\nuget-packages'
$outputPackageSpec = $($currentPath) + '\nuget-packages\*.nupkg'

$gitRoot = $(git rev-parse --show-toplevel | Out-String).Trim()

cd "$($gitRoot)\$($pkgName)"

$noUncommitedChanges = ((git diff | Out-String).ToString().Trim() | Measure-Object -Line).Lines -eq 0

if (-not $noUncommitedChanges)
{
  throw "Commit or discard all changes before publishing."
}

$commitText = git log -5 --oneline | Out-String

Copy-Item -Force $packageFile $backupFile

$nuspecXml = [xml](Get-Content $packageFile)

$nuspecXml.package.metadata.releaseNotes = $commitText.ToString()

$nuspecXml.Save($packageFile)

msbuild $projectFile /target:Rebuild /property:Configuration=Release
nuget pack $packageFile -Symbols -Prop Configuration=Release -OutputDirectory $outputPath
$recentPackages = ls $outputPackageSpec | Where-Object {[DateTime]::Now.Subtract($_.LastWriteTime).TotalMinutes -le 10}
foreach ($recentPackage in $recentPackages)
{
  nuget push $recentPackage.FullName -source https://www.myget.org/F/nord-pkg/
}

cd "$($gitRoot)"

git add --all .

git commit -m "Published latest version to nuget package"