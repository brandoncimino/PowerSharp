<#
    This is Brandon's "Universal" profile, which is to say, the profile that he wants executed on every computer he ever uses PowerShell on.
#>

#region Namespaces
using namespace System.IO
using namespace PowerSharp
using namespace System.Collections
#endregion

Write-Host -ForegroundColor Green "Importing Brandon.PowerSharp_profile.ps1"

#region Variables
$Global:ProfileHome = [Path]::GetDirectoryName($Profile)
#endregion

#region Sources
$env:PSModulePath += "$([Path]::PathSeparator)$PSScriptRoot/build"
#endregion

#region Aliases

#endregion

#region Startup

#endregion