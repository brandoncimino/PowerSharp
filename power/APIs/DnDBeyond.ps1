#requires #Requires -Module PowerSharp

using namespace PowerSharp;

$DDB = New-Object RestApi

# 'https://character-service.dndbeyond.com/character/v3/character/number/'
$DDB.BaseUrl = 'https://character-service.dndbeyond.com'
$DDB.ApiVersion = 'v4'
$DDB.BasePath = 'character'
$DDB.Endpoint = 'character'

function Get-DDBCharacter(
    $ID
){
    $response = Invoke-LongRest $DDB -Endpoint $ID
    return $response.data
}

# Characters
$Bufo_ID = 39110600
$Tsant_ID = 7000631 # Tsant doesn't work - maybe because he's using retired content? Weird...
$Dasy_ID = 27915845
$Bufo = Get-DDBCharacter $Bufo_ID
$Tsant = Get-DDBCharacter $Tsant_ID
$Dasy = Get-DDBCharacter $Dasy_ID