if (-not ([System.Management.Automation.PSTypeName]'Nord.Nganga.Commands.Commands').Type)
{
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Commands.dll"
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Core.dll"
  Add-Type -Path "$($PSScriptRoot)\bin\Nord.Nganga.Fs.dll"  
}

<#
    .SYNOPSIS 
        Returns the nganga settings types.

    .DESCRIPTION
        Returns the nganga settings types.
#>
Function Get-NgangaSettingsTypes
{
  return [Nord.Nganga.Commands.Commands]::ListOptionTypes()
}

<#
    .SYNOPSIS 
        Returns the named nganga setting object.

    .DESCRIPTION
        Returns the named nganga setting object.

    .PARAMETER SettingName          
        The name of the setting to return.
#>
Function Get-NgangaSettings
{
    param (
        [parameter(Position = 0,
            Mandatory = $true)]
        $SettingName
    )

    return [Nord.Nganga.Commands.Commands]::GetOptions($SettingName)
}

<#
    .SYNOPSIS 
        Updates nganga settings with the specified.

    .DESCRIPTION
        Updates nganga settings with the specified.

    .PARAMETER OptionsObject          
        The new settings object.
#>
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

  Write-Host "Building " $proj.UniqueName ", please wait..."

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

<#
    .SYNOPSIS 
        Lists type names of eligible controllers.

    .DESCRIPTION
        Reflects over the project assembly searching for controller implementations annotated with Nganga annotations.  

    .PARAMETER Filter 
        Scopes the search to type names containing the filter string.

    .PARAMETER ResourceOnly
        Scopes the search - ??? is this parameter actually used on this command? 
        
#>
Function Get-NgangaEligibleControllers
{
  param(
	        [parameter(Position = 0,
            Mandatory = $false)]
            [string] $Filter,
    [switch] $ResourceOnly
  )

  Build-ControllerType

  $assyFileName = Get-AssemblyFileName	

  Write-Host "Searching for eligible controllers..." 

  $result =  [Nord.Nganga.Commands.Commands]::ListControllerNames($assyFileName, $Filter, $ResourceOnly.IsPresent)

   if(!$result) {
        Write-Host "Unknown error!"
        return 
    }

  if(!$result.TypeNames){
        Write-Host "List controller types failed" 
        if($result.MessageText){
            Write-Host $result.MessageText 
        }
        return
    }

  return $result.TypeNames 
}

<#
    .SYNOPSIS 
        Generates for the named controller.

    .DESCRIPTION
        Generates the Angular UI resource, controller and view for the named controller.                    

    .PARAMETER ResourceOnly
        Limits the generation to the Angular resource file only.  No angular view or controller is generated. 

    .PARAMETER Preview
        Supresses disk writes.  Dumps the generated source to the console for re/view. 

    .PARAMETER Force
        The generation step checks for an existing file on disk with the target file name.  If a file is found on disk
        the file is opened and parsed.  If a valid Nganga header is found the header is parsed for the original md5.  
        a new md5 is calculated for the body and compared with the md5 declared on the header.  
        
        If no header can be found or a header parse fails, or a parsed md5 differs from a calculated md5, 
        then the newly generated source will not be written to disk without using -Force.  
        
        If a change is suspected and -Force is not specified, then a -Diff will
        automatically be performed.  Refer to the discussion of Diff for more information on the diff.

    .PARAMETER Diff
        Invokes a diff on the newly generated source against the previous disk version.  If diff is specified
        no changes are written to disk.  Diff may be invoked automatically if changes are detected or suspected 
        in a previous version and force was not specified.

    .PARAMETER Merge
        Invokes the external merge tool identified by Get-NgangaDiffMergeToolPath.        
#>
Function Export-NgangaCode
{
    param(
      [parameter(Position = 0,
            Mandatory = $true)]
            [string] $ControllerName,
       [switch] $ResourceOnly,
       [switch] $Preview,
       [switch] $Force,
       [switch] $Diff,
       [switch] $Merge
    )    

    $proj = Get-Project

    Build-ControllerType

    $assyFileName =  Get-AssemblyFileName

     $projPath = [System.IO.Path]::GetDirectoryName($proj.FullName)

    Write-Host "Generating " $ControllerName  "..."

    $result = [Nord.Nganga.Commands.Commands]::Generate($assyFileName, $ControllerName, $projPath, $ResourceOnly.IsPresent)

    if(!$result) {
        Write-Host "Unknown error!"
        return 
    }

    if(!$result.CoordinationResult){
        Write-Host "Generate failed" 
        if($result.MessageText){
            Write-Host $result.MessageText 
        }
        return
    }

    if($result.MessageText){
        Write-Host $result.MessageText 
    }    

    Write-Host "Generate complete "

    $result = $result.CoordinationResult

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

    
    if($result.ViewTemplateRegression) {
        Write-Host "The possibility of a regression within the view template has been detected."
    }

    if($result.ControllerTemplateRegression) {
        Write-Host "The possibility of a regression within the controller template has been detected."
    }

    if($result.ResourceTemplateRegression) {
        Write-Host "The possibility of a regression within the resource template has been detected."
    }

     
    if($changesDetected){
        Write-Host "The possibility of one or more changes or regressions has been detected."
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
        $proj = Get-Project             

        Write-Host "Integrating " $proj.ProjectName ", please wait..."
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
        try {
            
            Export-SourceFile $source $fileName 

            try {
                $proj = Get-Project  
                Write-Host "Integrating file: " $fileName 
                #$dte.Application.ItemOperations.AddExistingItem($relativeFileName)  | out-null
                #$DTE.ExecuteCommand(“Project.AddExistingItem”, $fileName) | out-null
                $proj.ProjectItems.AddFromFile( $fileName ) | out-null

                try {
                  #Write-Host "Adding file to source control: " $fileName                
                  #$DTE.ExecuteCommand(“File.AddToSourceControl”, $fileName) | out-null

                  try {
                    $DTE.ExecuteCommand(“File.OpenFile”, $fileName) | out-null
                  }
                  catch [System.Exception]{
                    Write-Host $_
                  }
                }
                catch [System.Exception]{
                  Write-Host $_
              } 
            }
            catch [System.Exception]{
                Write-Host $_
            } 
        }
        catch [System.Exception]{
            Write-Host $_
        } 
    }
}

function Export-SourceFile([string] $source, [string]$fileName){
    try {
        Write-Host "Writing file: " $fileName
        $dir = [System.IO.Path]::GetDirectoryName($fileName)
        $dirExists = [System.IO.Directory]::Exists( $dir ) 
        if(!$dirExists) {
            Write-Host "Creating destination directory " $dir
            [System.IO.Directory]::CreateDirectory( $dir ) | out-null
        }
        $source | Out-File $fileName -encoding Unicode
    }
    catch [System.Exception]{
        Write-Host $_
    } 
}

New-Alias nng-gen Export-NgangaCode

New-Alias nng-list Get-NgangaEligibleControllers

New-Alias nng-get-opt Get-NgangaSettings

New-Alias nng-set-opt Update-NgangaSettings

Export-ModuleMember -Function Export-NgangaCode, Get-NgangaDiffMergeToolPath, Get-NgangaDefaultDiffMergeToolPath, Get-NgangaSettingsTypes, Get-NgangaSettings, Update-NgangaSettings, Get-NgangaEligibleControllers -Alias nng-gen, nng-list, nng-get-opt, nng-set-opt