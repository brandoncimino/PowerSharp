# This is the "build" script for the entire module, which combines both C# and PowerShell "modules"
Write-Host "Building PowerSharp..."

$old_build_dir = Get-Item "$PSScriptRoot/build" -ErrorAction SilentlyContinue
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

#region Variables
$module = "PowerSharp"
$build_dir = New-Item -ItemType Directory "$PSScriptRoot/build/$module"
$power_dir = Get-Item "$PSScriptRoot/power"
$sharp_dir = Get-Item "$PSScriptRoot/sharp"
$power_out = New-Item -ItemType Directory "$build_dir/power"
$sharp_out = "$build_dir/bin"

[ordered]@{
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

Import-Module "$build_dir/$module.psd1"