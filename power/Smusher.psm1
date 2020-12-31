function Get-Smushed(
    [Parameter(Mandatory, ValueFromPipeline)]
    [object[]]$Parts,

    [type]$Type
){
    BEGIN {
        $allParts = @()
        $setProps = @()
    }

    PROCESS {
        $allParts += $Parts
    }

    END {
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