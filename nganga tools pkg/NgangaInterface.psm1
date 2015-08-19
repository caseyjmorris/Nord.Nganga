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
       [switch] $Diff,
       [switch] $Merge
    )

    $proj = Get-Project

    Build-ControllerType

    $assyFileName =  Get-AssemblyFileName

     $projPath = [System.IO.Path]::GetDirectoryName($proj.FullName)

    $result = $null

    Write-Host "Generating " $ControllerName  "..."

    $result = [Nord.Nganga.Commands.Commands]::Generate($assyFileName, $ControllerName, $projPath, $ResourceOnly.IsPresent, $Echo.IsPresent)

    Write-Host "Generate complete "

    if ($Preview.IsPresent )
    {
        Write-Host "Preview only.  No data saved."
      return $result;
    }

    $doDiff = (-not $force.IsPresent) -and ( $Diff.IsPresent -or $result.ViewMergeOrDiffRecommended -or $result.ResourceMergeOrDiffRecommended -or $result.ControllerMergeOrDiffRecommended ) 

    if($Merge.IsPresent){
      Write-Host "The merge option has not yet been implemented - please try a later version."
    }
    
    if($doDiff){

        if(-not $Diff.IsPresent){
            Write-Host "The possibility of one or more changes have been detected."
        }

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
            SaveAndIntegrateFile $result.ViewText $result.ViewFileName $result.ViewGenerationIsRedundantNoSaveRequired $Force.IsPresent
            SaveAndIntegrateFile $result.ControllerText $result.ControllerFileName $result.ControllerGenerationIsRedundantNoSaveRequired $Force.IsPresent
        }
        SaveAndIntegrateFile $result.ResourceText $result.ResourceFileName $result.ResourceGenerationIsRedundantNoSaveRequired $Force.IsPresent
    }
}


function DiffFile([string]$existingFile, [string]$newSource){
    Write-Host "Diff file: " $existingFile
    $tmp = New-TemporaryFile
    $newSource | Out-File $tmp -encoding Unicode
    $dte.ExecuteCommand("Tools.DiffFiles", ' "' + $tmp + '" "' + $existingFile + '" "New" "Old" ' )  
}

function SaveAndIntegrateFile([string]$source, [string]$fileName, [bool]$noSaveRequired, [bool]$force){
    if($noSaveRequired -and -not $force){
        Write-Host "No changes were detected in file: " $fileName " and -Force was not specified.  File not saved." 
    }
    else {
        Write-Host "Saving file: " $fileName
        try {
            $source | Out-File $fileName -encoding Unicode
            try {
                $dte.Application.ItemOperations.AddExistingItem($fileName) 
            }
            catch{} 
            try {
                $DTE.ExecuteCommand(“File.OpenFile”, $fileName)
            }
            catch{}
            }
        catch{}
    }
}

New-Alias nng-gen Export-NgangaCode

New-Alias nng-list Get-NgangaEligibleControllers

New-Alias nng-get-opt Get-NgangaSettings

New-Alias nng-set-opt Update-NgangaSettings

Export-ModuleMember -Function Export-NgangaCode, Get-NgangaSettingsTypes, Get-NgangaSettings, Update-NgangaSettings, Get-NgangaEligibleControllers -Alias nng-gen, nng-list, nng-get-opt, nng-set-opt