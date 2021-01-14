#This is the "build" script for the entire module, which combines both C# and PowerShell "modules"
#Passing an alternate value as the -BuildFolder is primarily for whe you need to create temporary PowerShell sessions
param (
    [ValidateNotNullOrEmpty()]
    $BuildFolder = 'build',
    [switch]$ImportBuild
)

#region cleaning the old build
$build_path = "$PSScriptRoot/$BuildFolder"

Write-Host -ForegroundColor Yellow "Building PowerSharp into $build_path..."

$old_build_dir = Get-Item $build_path -ErrorAction SilentlyContinue
if ($old_build_dir) {
    try {
        Remove-Item $old_build_dir -Recurse -Force -ErrorAction Stop
    } catch [System.UnauthorizedAccessException] {
        $_.ErrorDetails = "Couldn't remove $old_build_dir.
It is likely loaded into the current PowerShell session, locking access to it.
The only way to unload it (as far as I know) is to completely exit the session.
See https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/create-standard-library-binary-module?view=powershell-7.1#important-details"
        $_.Exception.HelpLink = 'https://docs.microsoft.com/en-us/powershell/scripting/dev-cross-plat/create-standard-library-binary-module?view=powershell-7.1#important-details'
        throw $_
    }
}
#endregion

#region Variables
$module = "PowerSharp"
$build_dir = New-Item -ItemType Directory "$build_path/$module"
$power_dir = Get-Item "$PSScriptRoot/power"
$sharp_dir = Get-Item "$PSScriptRoot/sharp"
$power_out = New-Item -ItemType Directory "$build_dir/power"
$sharp_out = "$build_dir/bin"

[ordered]@{
    BuildFolder = $BuildFolder
    build_dir = $build_dir
    power_dir = $power_dir
    sharp_dir = $sharp_dir
    power_out = $power_out
    sharp_out = $sharp_out
} | Out-String | Write-Host -ForegroundColor darkgray
#endregion

#region Move the manifest (.psd1) file
$manifest = Get-Item "$PSScriptRoot/$module.psd1"
Copy-Item -Path $manifest -Destination $build_dir
#endregion

#region Move the PowerShell module files
Copy-Item -Path $power_dir -Destination $power_out -Recurse
#endregion

#region Build the C# .dll
dotnet build $sharp_dir -o $sharp_out
#endregion

#region Optionally import the build into the current session
if($ImportBuild){
    Write-Host -ForegroundColor DarkGray "Explicitly importing the new build into the current session!"
    Import-Module $build_dir
}
else {
    Write-Host -ForegroundColor DarkGray "Skipping import of the new build."
}
#enregion