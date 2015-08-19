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

  $result =  [Nord.Nganga.Commands.Commands]::ListControllerNames($assyFileName, $Filter, $ResourceOnly.IsPresent, $Echo.IsPresent)

  
    if($result -eq $null){
        Write-Host "List controller types failed - are you sure you have selected a valid webapi project in package manger console?"
        return 
    }

  return $result 
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

    if($result -eq $null){
        Write-Host "Generate failed - are you sure you have selected a valid webapi project in package manger console?"
        return 
    }

    Write-Host "Generate complete "

    if ($Preview.IsPresent )
    {
        Write-Host "Preview only.  No data saved."
      return $result
    }

    if($Merge.IsPresent -and $Diff.IsPresent){
        Write-Host "-Merge and -Diff are mutually exclusive options and may not be used together."
        return
    }

    $changesDetected = $result.ViewMergeOrDiffRecommended -or $result.ResourceMergeOrDiffRecommended -or $result.ControllerMergeOrDiffRecommended

    if($changesDetected){
        Write-Host "The possibility of one or more changes have been detected."
    }

    $doDiff = (-not $force.IsPresent) -and ( $Diff.IsPresent -or  $changesDetected ) 
    
    if($doDiff -or $Merge.IsPresent){
        if($Merge.IsPresent){
            Start-MergeResult $result  $ResourceOnly.IsPresent 
        }
        else {
            Start-DiffResult $result  $ResourceOnly.IsPresent 
            }
    }
    else {
        Start-IntegrateResult $result  $ResourceOnly.IsPresent $Force.IsPresent
    }
}

function Start-MergeResult( [object]$result, [bool] $resourceOnly){
        Write-Host "Opening diff on files, please wait..."

        if(-not $resourceOnly) {        
            Start-MergeFile $result.ViewFileName $result.ViewText 
            Start-MergeFile $result.ControllerFileName $result.ControllerText 
        }
        Start-MergeFile $result.ResourceFileName $result.ResourceText 
}

function Start-MergeFile([string]$existingFile, [string]$newSource){
    Start-DiffMergeFile $existingFile $newSource "DiffMerge" 
}

function Start-DiffResult( [object]$result, [bool] $resourceOnly){
        Write-Host "Opening diff on files, please wait..."

        if(-not $resourceOnly) {        
            Start-DiffFile $result.ViewFileName $result.ViewText 
            Start-DiffFile $result.ControllerFileName $result.ControllerText 
        }
        Start-DiffFile $result.ResourceFileName $result.ResourceText 
}

function Start-DiffFile([string]$existingFile, [string]$newSource){
    Start-DiffMergeFile $existingFile $newSource "DiffFiles"  
}

function Start-DiffMergeFile([string]$existingFile, [string]$newSource, [string]$verb){
    Write-Host $verb " : " $existingFile
    $newFile = Save-TemporaryFile $existingFile $newSource 
    if($verb -eq "DiffFiles"){
        $dte.ExecuteCommand("Tools." + $verb, ' "' + $existingFile + '" "' + $newFile + '" "Old" "New" ' )  
        return 
        }
    if($verb -eq "DiffMerge"){        
        $toolPath = Get-NgangaDiffMergeToolPath
        $cmd = '"' + $toolPath + '" "' + $existingFile + '" "' + $newFile +'"'
        Invoke-Expression -Command:$cmd
        return 
        }
}


function Save-TemporaryFile([string]$existingFileName, [string] $source){
    $newFile = Get-TempDiffFileName $existingFileName
    Write-Host "Creating temporary diff/merge file: " $newFile
    $source | Out-File $newFile -encoding Unicode    
    return $newFile 
}


function Get-TempDiffFileName([string] $existingFileName){
    $tmp = New-TemporaryFile
    $baseDir = [System.IO.Path]::GetDirectoryName($tmp)
    $baseName = [System.IO.Path]::GetFileNameWithoutExtension($tmp)
    $extension = [System.IO.Path]::GetExtension($existingFileName)
    $newFileName = [System.IO.Path]::Combine($baseDir, $baseName) + $extension    
    return $newFileName;
}

function Get-NgangaDiffMergeToolPath(){
    $toolPath = $systemPathSettings.DiffMergeToolPath 
    if($toolPath -eq $null -or $toolPath -eq ""){
        $toolPath = Get-NgangaDefaultDiffMergeToolPath
    }
    return $toolPath
}

function Get-NgangaDefaultDiffMergeToolPath() {    
    return $env:VS140COMNTOOLS -replace "\\TOOLS\\", "\IDE\vsDiffMerge.exe"    
}

function Start-IntegrateResult( [object]$result, [bool] $resourceOnly, [bool]$force){
        
        Write-Host "Integrating files, please wait..."
        if(-not $ResourceOnly) {        
            $noSaveRequired =  Get-NormalizedNoSaveRequred $result.ViewGenerationIsRedundantNoSaveRequired 
            Update-Project $result.ViewText $result.ViewFileName $noSaveRequired $Force

            $noSaveRequired =  Get-NormalizedNoSaveRequred $result.ControllerGenerationIsRedundantNoSaveRequired 
            Update-Project $result.ControllerText $result.ControllerFileName $noSaveRequired $Force
        }

        $noSaveRequired =  Get-NormalizedNoSaveRequred $result.ResourceGenerationIsRedundantNoSaveRequired 
        Update-Project $result.ResourceText $result.ResourceFileName $noSaveRequired $Force
}

function Get-NormalizedNoSaveRequred([object] $nsr){
    try {        
        if($nsr -eq $null -or $nsr -eq ""){ 
            return $false
            }
        return $nsr -as [bool]
        }
   catch{
        return $false 
        }
}

function Update-Project([string]$source, [string]$fileName, [bool]$noSaveRequired, [bool]$force){
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

Export-ModuleMember -Function Export-NgangaCode, Get-NgangaDiffMergeToolPath, Get-NgangaDefaultDiffMergeToolPath, Get-NgangaSettingsTypes, Get-NgangaSettings, Update-NgangaSettings, Get-NgangaEligibleControllers -Alias nng-gen, nng-list, nng-get-opt, nng-set-opt