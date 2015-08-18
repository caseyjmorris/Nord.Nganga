if (-not ([System.Management.Automation.PSTypeName]'Nord.Nganga.Commands.Commands').Type)
{
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Commands.dll"
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Core.dll"
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Fs.dll"  
}

Function nng-get-settings
{
  return [Nord.Nganga.Commands.Commands]::ListOptionTypes()
}

Function nng-get-settings
{
    param (
        [parameter(Position = 0,
            Mandatory = $true)]
        $SettingName
    )

    return [Nord.Nganga.Commands.Commands]::GetOptions($SettingName)
}

Function nng-update-settings
{
  param(
        [parameter(Position = 0,
            Mandatory = $true)]
         $OptionsObject
  )

  [Nord.Nganga.Commands.Commands]::SetOptions($OptionsObject)
}

Function nng-build-controller-type
{
  $proj = Get-Project

  $cfg = $dte.Solution.SolutionBuild.ActiveConfiguration.Name

  $dte.Solution.SolutionBuild.BuildProject($cfg, $proj.UniqueName, $true)

  if ($dte.Solution.SolutionBuild.LastBuildInfo)
  {
    throw "Couldn't build $($proj.Name)"
  }
}

Function nng-get-assy-filename
{
  $proj = Get-Project

  $OutputPath = $proj.ConfigurationManager.ActiveConfiguration.Properties('OutputPath').Value.ToString()

  $Directory = [System.IO.Path]::GetDirectoryName($proj.FullName)

  $AsmName = [System.IO.Path]::GetFileNameWithoutExtension($proj.FullName)

  return [System.IO.Path]::Combine($Directory, $OutputPath, $AsmName ) + ".dll"
}

Function nng-list
{
  param(
    [switch] $ResourceOnly,
    [switch] $Echo
  )

  nng-build-controller-type

  $assyFileName = nng-get-assy-filename

  return [Nord.Nganga.Commands.Commands]::ListControllerNames($assyFileName, $ResourceOnly.IsPresent, $Echo.IsPresent)
}

Function nng-gen
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

    nng-build-controller-type

    $assyFileName =  nng-get-assy-filename

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

}

Export-ModuleMember -Function nng-gen,nng-get-types, nng-get-settings, nng-update-settings, nng-list
