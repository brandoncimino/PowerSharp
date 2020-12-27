<#
    This file can be used as a launch configuration for debugging the module during development.

    args[0] : The optional path to the file to be run after building the module.
#>

# Build the module
.\build.ps1

# Run my profile
. .\Brandon.PowerSharp_profile.ps1

# Run the file
$file = $args[0]

$runnable = @(
    ".ps1"
)

$extension = [System.IO.Path]::GetExtension($file)

if ($runnables -contains $extension) {
    . $file
}
else {
    Write-Host -ForegroundColor DarkGray "Skipping the execution of the file $file as it is not one of the `$runnable file types: [$runnable]"   
}