# This is the "Root Module" file for PowerSharp.

#region Aliases
New-Alias -Name scry -Value Get-ScryfallCard -Scope Global
New-Alias -Name Scry-Card -Value Get-ScryfallCard -Scope Global
#endregion

function Test-PowerSharp(){
    Write-Host "PowerSharp! It's real!"
}