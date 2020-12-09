<#
    This is Brandon's "Universal" profile, which is to say, the profile that he wants executed on every computer he ever uses PowerShell on.
#>

#region Namespaces
using namespace System.IO
using namespace PowerSharp
using namespace System.Collections
#endregion

Write-Host -ForegroundColor Green "Importing BrandonProfile.ps1"

#region Variables
$Global:ProfileHome = [Path]::GetDirectoryName($Profile)
#endregion

#region Sources

#endregion

#region Aliases
New-Alias -Name psharp -Value Import-PowerSharp
#endregion

#region Startup

#endregion