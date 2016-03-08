$ErrorActionPreference = "Stop"

$AppDisplayNamePrefix = "Azure Automation Connector for Service Manager - "
$AppUrlPrefix = "https://servicemanager.managementgroup/"

$Global:SCSession = $null
$Global:ManagementGroupId = [Guid]::Empty

function Get-SCModulePath
{
	$SMDIR = (Get-ItemProperty 'hklm:/software/microsoft/System Center/2010/Service Manager/Setup').InstallDirectory
	return "$SMDIR\Powershell\System.Center.Service.Manager.psm1"
}


function Assure-AzureRmLogin
{
    param(
        [parameter(Mandatory=$false)]
        [PSCredential]$AzureCredential
    )

    try
    {
        Get-AzureRmContext
    } 
    catch
    {
        if ($AzureCredential) {
            Login-AzureRmAccount -Credential $AzureCredential 
        } else {
            Login-AzureRmAccount 
        }
    }
}

function Assure-AzureRMSubscription
{
    param(
        [parameter(Mandatory=$true)]
        [Guid]$SubscriptionId,

        [parameter(Mandatory=$false)]
        [PSCredential]$AzureCredential
    )

    $context = Assure-AzureRmLogin -AzureCredential $AzureCredential
    Select-AzureRmSubscription -SubscriptionId $SubscriptionId
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
    $Global:ManagementGroupId = [Microsoft.EnterpriseManagement.EnterpriseManagementGroup]::Connect($Global:SCSession.Settings).Id
    $Global:SCSession
}

function New-AAConnectorServicePrincipal
{
    param(
        [parameter(Mandatory=$true)]
        [Microsoft.Azure.Commands.Automation.Model.AutomationAccount]$AutomationAccount,
        [parameter(Mandatory=$true)]
        [Guid]$ManagementGroupId,
        [parameter(Mandatory=$true)]
        [string]$ManagementGroupName,
        [parameter(Mandatory=$true)]
        [DateTime]$Expires,
        [parameter()]
        [Switch]$RemoveExisting
    )

    if ($RemoveExisting) {
        Remove-AAConnectorServicePrincipal -ManagementGroupId $ManagementGroupId
    }

    $appUrl = $AppUrlPrefix + $ManagementGroupId.ToString()
    $appDisplayName = $AppDisplayNamePrefix + $ManagementGroupName

    $password = "bulabulabula"


    $app = New-AzureRmADApplication -DisplayName $appDisplayName -HomePage $appUrl -IdentifierUris $appUrl -Password $password -EndDate $Expires
    $princ = New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId
    Start-Sleep -Seconds 10

    New-AzureRmRoleAssignment -RoleDefinitionName "Automation Operator" -ServicePrincipalName $app.ApplicationId -ResourceGroupName $AutomationAccount.ResourceGroupName -ResourceName $AutomationAccount.AutomationAccountName -ResourceType "Microsoft.Automation/automationAccounts" | Out-Null

    return @{
        "ClientID"=$app.ApplicationId
        "Password"=$password
    }
}

function Remove-AAConnectorServicePrincipal
{
    param(
        [parameter(Mandatory=$true)]
        [Guid]$ManagementGroupId
    )

    $appUrl = $AppUrlPrefix + $ManagementGroupId.ToString()

    $app = $null
    $app = Get-AzureRmADApplication -IdentifierUri $appUrl

    if ($app) {
        $princ = $null
        $princ = Get-AzureRmADServicePrincipal -ServicePrincipalName $app.ApplicationId
        if ($princ) {
            Remove-AzureRmADServicePrincipal -ObjectId $princ.Id -Force
        }
        Remove-AzureRmADApplication -ApplicationObjectId $app.ApplicationObjectId -Force
    }
}

## public
function Register-AAConnectorAutomationAccount 
{
	param (
        [parameter(Mandatory=$true,ValueFromPipeline=$false)]
        [Guid]$SubscriptionId,
        
        [parameter(Mandatory=$true)]
        [string]$AutomationAccountName,

        [parameter(Mandatory=$false)]
        [Guid]$UserClientId,

        [parameter(Mandatory=$false)]
        [string]$SCSMServerName,

        [parameter(Mandatory=$false)]
        [PSCredential]$SCSMCredential,

        [parameter(Mandatory=$false)]
        [PSCredential]$AzureCredential
    )
    
    Assure-AzureRMSubscription -SubscriptionId $SubscriptionId -AzureCredential $AzureAzureCredential | Out-Null
    Assure-SCSession -SCSMServerName $SCSMServerName -SCSMCredential $SCSMCredential | Out-Null
  
    $account = $null
    $account = Get-AzureRmAutomationAccount | Where-Object { $_.AutomationAccountName -like $AutomationAccountName }

    if (-not ($account)) {
        throw "Automation account with name '" + $account.AutomationAccountName + "' was not found!"
    }

    $client = New-AAConnectorServicePrincipal -AutomationAccount $account -ManagementGroupId $Global:ManagementGroupId -Expires (Get-Date).AddYears(10) -ManagementGroupName $Global:SCSession.ManagementGroupName -RemoveExisting

    $client.ClientID
    $client.Password
       


}

Import-Module (Get-SCModulePath)
Import-Module "AzureRM.Automation"
Import-Module "AzureRM.Resources"
Import-Module "AzureRM.Profile"