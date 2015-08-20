param($installPath, $toolsPath, $package, $project)

if (Get-Module | ?{ $_.Name -eq 'NgangaInterface' })
{
    Remove-Module NgangaInterface
}

Import-Module (Join-Path $($toolsPath) 'NgangaInterface.psm1')