<#
    This file can be used as a launch configuration for debugging the module during development.
    
    args[0] : The optional $BuildFolder to send to build.ps1
    args[1] : The optional path to the file to be run after building the module.
#>
param(
    $BuildFolder,
    $FileToRun
)

Write-Host -ForegroundColor DarkGray "Launching $([System.IO.Path]::GetFileName($FileToRun))"

[ordered]@{
    BuildFolder = $BuildFolder
    FileToRun = [System.IO.Path]::GetFileName($FileToRun)
} | Out-String | Write-Host -ForegroundColor DarkGray

# Build the module
.\build.ps1 -BuildFolder $BuildFolder -ImportBuild

# Run my profile
. .\Brandon.PowerSharp_profile.ps1

# Run the file
$runnableFileTypes = @(
    ".ps1"
)

$excludedFiles = @(
    "build.ps1"
    "launch.ps1"
)

$fileName = [System.IO.Path]::GetFileName($FileToRun)
$extension = [System.IO.Path]::GetExtension($FileToRun)

if($excludedFiles -contains $fileName){
    Write-Host -ForegroundColor DarkGray "Skipping execution of the file $fileName because it is one of the explicitly excluded files."
}
elseif ($runnableFileTypes -contains $extension) {
    . $FileToRun
}
else {
    Write-Host -ForegroundColor DarkGray "Skipping the execution of the file $file of type $extension it is not one of the `$runnableFileTypes: [$runnableFileTypes]"   
}