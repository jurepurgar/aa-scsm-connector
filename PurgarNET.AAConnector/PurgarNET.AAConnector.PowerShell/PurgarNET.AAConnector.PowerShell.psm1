$ErrorActionPreference = "Stop"

$AppDisplayNamePrefix = "Azure Automation Connector for Service Manager - "
$AppUrlPrefix = "https://servicemanager.managementgroup/"

$Global:SCSession = $null

function Get-SCModulePath
{
	$SMDIR = (Get-ItemProperty 'hklm:/software/microsoft/System Center/2010/Service Manager/Setup').InstallDirectory
	return "$SMDIR\Powershell\System.Center.Service.Manager.psm1"
}


function Assure-SCSession
{
    param(
        [parameter(Mandatory=$false)]
        [string]$SCSMServerName,

        [parameter(Mandatory=$false)]
        [PSCredential]$SCSMCredential
    )

    if (-not $SCSMServerName) {
        $SCSMServerName = "localhost"
    }

    $Global:SCSession = Get-SCSMManagementGroupConnection -ComputerName $SCSMServerName

    if (-not $Global:SCSession) {
        if ($SCSMCredential) {
            $Global:SCSession = New-SCManagementGroupConnection -ComputerName $SCSMServerName -Credential $SCSMCredential -PassThru
        } else {
            $Global:SCSession = New-SCManagementGroupConnection -ComputerName $SCSMServerName -PassThru
        }
    }
    $Global:ManagementGroupID = [Microsoft.EnterpriseManagement.EnterpriseManagementGroup]::Connect($Global:SCSession.Settings).Id
    $Global:ManagementGroup = [Microsoft.EnterpriseManagement.EnterpriseManagementGroup]::Connect($Global:SCSession.Settings)
    $Global:SCSession
}



Import-Module (Get-SCModulePath)



