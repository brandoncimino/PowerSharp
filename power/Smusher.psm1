<#
    .SYNOPSIS
        Combines multiple C# and/or PSCustomObjects into a single *NEW* object.
    
    .DESCRIPTION
        The objects from the `$Parts` array will be combined together into a single, *new* object.

        The resulting object's `Type` will be, in prioritized order:
            1. The explicitly defined `$Type`
            2. The first non-`PSObject` `Type` encountered in the `$Parts` array
            3. `PSObject`

        The resulting object's properties will be set using values from the objects in the `$Parts` array.

        If encountered properties are read-only, they are skipped.
#>
function Get-Smushed(
    [Parameter(Mandatory, ValueFromPipeline)]
    [object[]]$Parts,

    [type]$Type,

    [ValidateSet('First','Last')]
    [string]$Prefer = 'First'
){
    BEGIN {
        $allParts = @()
        $setProps = @()
    }

    PROCESS {
        $allParts += $Parts
    }

    END {
        # Possibly reverse $allParts, based on $Prefer
        if($Prefer -eq 'Last'){
            [array]::Reverse($allParts);
        }

        if(!$Type){
            foreach($part in $allParts){
                if($part -isnot [PSObject]){
                    $Type = $part.GetType()
                    break
                }
            }

            if(!$Type){
                $Type = [pscustomobject]
            }
        }

        $smushed = New-Object -TypeName $Type

        foreach($part in $allParts){
            $props = $part | Get-Properties -Settable

            foreach($p in $props){
                Write-Host -ForegroundColor DarkGray "Checking if prop $p is good to add"
                if(
                    $setProps -contains $p.Name -or
                    $null -eq $part.($p.Name) -or 
                    $part.($p.Name).Length -eq 0 -or 
                    [string]::IsNullOrWhiteSpace($part.($p.Name))
                ){
                    continue
                }

                $setProps += @($p.Name)

                $smushProps = $smushed | Get-Properties -Settable

                if($smushProps.Name -contains $p.Name){
                    $smushed.($p.Name) = $part.($p.Name)
                }
                else {
                    $mType = $p.MemberType -eq [System.Management.Automation.PSMemberTypes]::Property ? [System.Management.Automation.PSMemberTypes]::NoteProperty : $p.MemberType
                    $smushed | Add-Member -MemberType $mType -TypeName $p.TypeName -Name $p.Name -Value $part.($p.Name) -Force
                }
            }
        }

        return $smushed;
    }
}

New-Alias -Name 'smush' -Value Get-Smushed