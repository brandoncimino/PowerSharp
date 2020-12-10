# This is the "build" script for the entire module, which combines both C# and PowerShell "modules"

Remove-Item "$PSScriptRoot/build" -Recurse -Force -ErrorAction SilentlyContinue

#region Variables
$module = "PowerSharp"
$build_dir = New-Item -ItemType Directory "$PSScriptRoot/build/$module"
$power_dir = Get-Item "$PSScriptRoot/power"
$sharp_dir = Get-Item "$PSScriptRoot/sharp"
$power_out = New-Item -ItemType Directory "$build_dir/power"
$sharp_out = "$build_dir/bin"
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