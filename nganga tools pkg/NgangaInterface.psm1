if (-not ([System.Management.Automation.PSTypeName]'Nord.Nganga.Commands.Commands').Type)
{
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Commands.dll"
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

Function Get-ControllerDllLocation
{
  $proj = Get-Project

  $OutputPath = $proj.ConfigurationManager.ActiveConfiguration.Properties('OutputPath').Value.ToString()

  $Directory = [System.IO.Path]::GetDirectoryName($proj.FullName)

  $AsmName = [System.IO.Path]::GetFileNameWithoutExtension($proj.FullName)

  return [System.IO.Path]::Combine($OutputPath, $Directory, $AsmName)
}

Function List-NgangaEligibleControllers
{
  param(
    [switch] $ResourceOnly,
    [switch] $UseVerboseOutput
  )

  Build-ControllerType

  $asmLoc = Get-ControllerDllLocation

  return [Nord.Nganga.Commands.Commands]::GetEligibleWebApiControllers($asmLoc, $ResourceOnly.IsPresent, $UseVerboseOutput.IsPresent)
}

Function Generate-NgangaCode
{
    param(
      [parameter(Position = 0,
            Mandatory = $true)]
            [string] $Controller,
       [switch] $ResourceOnly,
       [switch] $UseVerboseOutput,
       [switch] $Preview,
       [switch] $Force
    )

    $proj = Get-Project

    Build-ControllerType

    $asmLoc = Get-ControllerDllLocation

    $result = $null

    if ($ResourceOnly.IsPresent)
    {
      $result = [Nord.Nganga.Commands.Commands]::GenerateResource($asmLoc, $Controller, $proj.FullName, $UseVerboseOutput.IsPresent)
    }
    else
    {
      $result = [Nord.Nganga.Commands.Commands]::GenerateResource($asmLoc, $Controller, $proj.FullName, $UseVerboseOutput.IsPresent)
    }

    if ($Preview.IsPresent)
    {
      return $result;
    }

    if ($ResourceOnly.IsPresent)
    {
      [Nord.Nganga.Commands.Commands]::WriteResourceGenerationResult($result, $Force.IsPresent)
      [Nord.Nganga.Commands.Commands]::EditCsProj($proj.FullName, $result.ResourcePath)
      $dte.ItemOperations.OpenFile($result.ResourcePath)
    }
    else
    {
      [Nord.Nganga.Commands.Commands]::WriteUiGenerationResult($result, $Force.IsPresent)
      [Nord.Nganga.Commands.Commands]::EditCsProj($proj.FullName, $result.ResourcePath, $result.ViewPath, $result.ControllerPath)
      $dte.ItemOperations.OpenFile($result.ResourcePath)
      $dte.ItemOperations.OpenFile($result.ViewPath)
      $dte.ItemOperations.OpenFile($result.ControllerPath)
    }
}

Export-ModuleMember -Function Generate-NgangaCode,Get-NgangaSettingsTypes, Get-NgangaSettings, Update-NgangaSettings, List-NgangaEligibleControllers