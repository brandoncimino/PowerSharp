function Edit-Content(
    [Parameter(Mandatory, ValueFromPipeline)]
    [string[]]$Path,

    [Parameter(Mandatory)]
    [string]$Original,

    [Parameter(Mandatory)]
    [string]$Replacement
){
    PROCESS {
        $items = Get-ChildItem $Path -Recurse

        foreach($it in $items){
            [string]$content = Get-Content $it
            $new_content = $content -replace $Original, $Replacement
            Set-Content -Path $it -Value $new_content
        }
    }
}