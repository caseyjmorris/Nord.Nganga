if (-not ([System.Management.Automation.PSTypeName]'Nord.Nganga.Commands.Commands').Type)
{
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Commands.dll"
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Core.dll"
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Fs.dll"  
}

Function Get-NgangaSettingsTypes
{
  return [Nord.Nganga.Commands.Commands]::ListOptionTypes()
}

Function Get-NgangaSettings
{
    param (
        [parameter(Position = 0,
            Mandatory = $true)]
        $SettingName
    )

    return [Nord.Nganga.Commands.Commands]::GetOptions($SettingName)
}

Function Update-NgangaSettings
{
  param(
        [parameter(Position = 0,
            Mandatory = $true)]
         $OptionsObject
  )

  [Nord.Nganga.Commands.Commands]::SetOptions($OptionsObject)
}

Function Build-ControllerType
{
  $proj = Get-Project

  $cfg = $dte.Solution.SolutionBuild.ActiveConfiguration.Name

  $dte.Solution.SolutionBuild.BuildProject($cfg, $proj.UniqueName, $true)

  if ($dte.Solution.SolutionBuild.LastBuildInfo)
  {
    throw "Couldn't build $($proj.Name)"
  }
}

Function Get-AssemblyFileName
{
  $proj = Get-Project

  $OutputPath = $proj.ConfigurationManager.ActiveConfiguration.Properties('OutputPath').Value.ToString()

  $Directory = [System.IO.Path]::GetDirectoryName($proj.FullName)

  $AsmName = [System.IO.Path]::GetFileNameWithoutExtension($proj.FullName)

  return [System.IO.Path]::Combine($Directory, $OutputPath, $AsmName ) + ".dll"
}

Function Get-NgangaEligibleControllers
{
  param(
	        [parameter(Position = 0,
            Mandatory = $false)]
            [string] $Filter,
    [switch] $ResourceOnly,
    [switch] $Echo
  )

  Build-ControllerType

  $assyFileName = Get-AssemblyFileName	

  Write-Host "Searching for eligible controllers..." 

  return [Nord.Nganga.Commands.Commands]::ListControllerNames($assyFileName, $Filter, $ResourceOnly.IsPresent, $Echo.IsPresent)
}

Function Export-NgangaCode
{
    param(
      [parameter(Position = 0,
            Mandatory = $true)]
            [string] $ControllerName,
       [switch] $ResourceOnly,
       [switch] $Echo,
       [switch] $Preview,
       [switch] $Force,
       [switch] $Diff
    )

    $proj = Get-Project

    Build-ControllerType

    $assyFileName =  Get-AssemblyFileName

     $projPath = [System.IO.Path]::GetDirectoryName($proj.FullName)

    $result = $null

    Write-Host "Generating " $ControllerName  "..."

    $result = [Nord.Nganga.Commands.Commands]::Generate($assyFileName, $ControllerName, $projPath, $ResourceOnly.IsPresent, $Echo.IsPresent)

    Write-Host "Generate complete "

    if ($Preview.IsPresent)
    {
        Write-Host "Preview only.  No data saved."
      return $result;
    }

    if($Diff.IsPresent){
        Write-Host "Opening diff on files, please wait..."
        if(-not $ResourceOnly.IsPresent) {        
            DiffFile $result.ViewFileName $result.ViewText 
            DiffFile $result.ControllerFileName $result.ControllerText 
        }
        DiffFile $result.ResourceFileName $result.ResourceText 
    }
    else {
        Write-Host "Integrating files, please wait..."
        if(-not $ResourceOnly.IsPresent) {        
            SaveAndIntegrateFile $result.ViewText $result.ViewFileName 
            SaveAndIntegrateFile $result.ControllerText $result.ControllerFileName 
        }
        SaveAndIntegrateFile $result.ResourceText $result.ResourceFileName 
    }
}

function DiffFile([string]$existingFile, [string]$newSource){
    Write-Host "Diff file: " $existingFile
    $tmp = New-TemporaryFile
    $newSource | Out-File $tmp -encoding Unicode
    $dte.ExecuteCommand("Tools.DiffFiles", ' "' + $tmp + '" "' + $existingFile + '" "New" "Old" ' )  
}

function SaveAndIntegrateFile([string]$source, [string]$fileName){
         Write-Host "Saving file: " $fileName
        $source | Out-File $fileName -encoding Unicode
        try {
            $dte.Application.ItemOperations.AddExistingItem($fileName) 
        }
        catch{} 
        $DTE.ExecuteCommand(“File.OpenFile”, $fileName)
}

New-Alias nnggen Export-NgangaCode

New-Alias nnglist Get-NgangaEligibleControllers

New-Alias nnggetopt Get-NgangaSettings

New-Alias nngsetopt Update-NgangaSettings

Export-ModuleMember -Function Export-NgangaCode, Get-NgangaSettingsTypes, Get-NgangaSettings, Update-NgangaSettings, Get-NgangaEligibleControllers -Alias nnggen, nnglist, nnggetopt, nngsetopt