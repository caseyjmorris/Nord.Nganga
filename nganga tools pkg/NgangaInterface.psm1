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

    $result = [Nord.Nganga.Commands.Commands]::Generate($assyFileName, $ControllerName, $projPath,$ResourceOnly.IsPresent, $Echo.IsPresent)

    if ($Preview.IsPresent)
    {
      return $result;
    }

    if($Diff.IsPresent){
        [Nord.Nganga.Commands.Commands]::IntegrateResults($result, $Force.IsPresent, $Echo.IsPresent, $dte)
        }
        else{
        [Nord.Nganga.Commands.Commands]::IntegrateResults($result, $Force.IsPresent, $Echo.IsPresent)
        }

	  $proj = Get-Project
	  $Directory = [System.IO.Path]::GetDirectoryName($proj.FullName)

	$resourceFileName =  [System.IO.Path]::Combine($Directory, $result.NgResourcesPath, $result.ResourcePath)

	$dte.ItemOperations.OpenFile($resourceFileName)

	if(-not $ResourceOnly.IsPresent) {
			$viewFileName =  [System.IO.Path]::Combine($Directory, $result.NgViewsPath, $result.ViewPath)
		    $dte.ItemOperations.OpenFile($viewFileName)
		}

	if(-not $ResourceOnly.IsPresent) {
			$CtrlFileName =  [System.IO.Path]::Combine($Directory, $result.NgControllersPath, $result.ControllerPath)
			$dte.ItemOperations.OpenFile($CtrlFileName)
		}

}

New-Alias nnggen Export-NgangaCode

New-Alias nnglist Get-NgangaEligibleControllers

New-Alias nnggetopt Get-NgangaSettings

New-Alias nngsetopt Update-NgangaSettings

Export-ModuleMember -Function Export-NgangaCode, Get-NgangaSettingsTypes, Get-NgangaSettings, Update-NgangaSettings, Get-NgangaEligibleControllers -Alias nnggen, nnglist, nnggetopt, nngsetopt