$DefaultPrimaryKeys = "id", "name"
$PrimaryKeyName = "PrimaryKey";

function Add-PrimaryKey(
    [Parameter(ValueFromPipeline)]
    $Object,

    [ValidateNotNullOrEmpty]
    [string[]]$Property = $DefaultPrimaryKeys,

    [switch]$IncludeDefaults
) {
    PROCESS {
        if ($IncludeDefaults) {
            $Property += $DefaultPrimaryKeys
        }
            
        $Property = $Property | Select-Object -Unique

        $Object | Add-AliasCascade -Alias $PrimaryKeyName -Property $Property
    }
}

function Add-Alias(
    [Parameter(ValueFromPipeline)]
    $Object,

    [Parameter(Mandatory)]
    [string]$Alias,

    [Parameter(Mandatory)]
    [Alias("Properties")]
    [string[]]$Property
) {
    PROCESS {
        Write-Host -ForegroundColor DarkGray "Checking for the following properties to alias as $Alias, in order: [$Property]"

        foreach ($prop in $Property) {
            $member = $Object | Get-Member -Name $prop

            switch ($member.Count) {
                1 {
                    Write-Host -ForegroundColor Blue "Found the property '$prop'"
                    $Object | Add-Member -MemberType AliasProperty -Name $Alias -Value $Property
                    return
                }

                { $_ -gt 1 } {
                    throw "Found $($member.Count) properties satisfying the name '$prop': [$($member.Name)]. Please specify a more specific name."
                }

                0 {
                    Write-Host -ForegroundColor DarkGray "Didn't find the property '$prop', continuing..."
                }
            }
        }

        throw "None of the specified properties were available to alias as '${Alias}': [$Property]"
    }
}

function Add-PropertyCascade(
    [Parameter(ValueFromPipeline)]
    $Object,

    [Parameter(Mandatory)]
    [string]$Alias,

    [Parameter(Mandatory)]
    [object[]]$Getters
) {
    PROCESS {
        throw [System.NotImplementedException]::new()
    }
}

function Get-Properties(
    [Parameter(ValueFromPipeline)]
    $Object,

    [Switch]$Settable
) {
    PROCESS {
        $props = $Object | gm -MemberType Properties
        if ($Settable) {
            $props = $props | where { $_ | Get-IsSettable }   
        }

        return $props
    }
}

function Get-IsSettable(
    [Parameter(ValueFromPipeline)]
    [Microsoft.PowerShell.Commands.MemberDefinition]$Member
) {
    PROCESS {
        return (
            $Member.MemberType -eq [System.Management.Automation.PSMemberTypes]::NoteProperty -or
            $Member.Definition -match 'set;'
        )
    }
}