$ErrorActionPreference = "Stop"

$AppDisplayNamePrefix = "Azure Automation Connector for Service Manager - "
$AppUrlPrefix = "https://servicemanager.managementgroup/"

$Global:SCSession = $null
$Global:ManagementGroupId = [Guid]::Empty

$SecureReferenceName = "PurgarNET.AAConnector.ConnectorCredential"
$SecureReferenceOverrideName = "$SecureReferenceName.Override"

$ConfigurationManagementPackName = "PurgarNET.AAConnector.Configuration"
$ConfigurationManagementPackDisplayName = "PurgarNET Azure Automation Connector Configuration"

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
    $Global:ManagementGroupID = [Microsoft.EnterpriseManagement.EnterpriseManagementGroup]::Connect($Global:SCSession.Settings).Id
    $Global:ManagementGroup = [Microsoft.EnterpriseManagement.EnterpriseManagementGroup]::Connect($Global:SCSession.Settings)
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
        [TimeSpan]$ValidFor,
        [parameter(Mandatory=$false)]
        [Switch]$RemoveExisting
    )

    if (-not $ValidFor -or $ValidFor -eq [TimeSpan]::Zero) {
        $ValidFor = [TimeSpan]::FromDays(365 * 5)
    }

    $expires = $((Get-Date) + $ValidFor)

    if ($RemoveExisting) {
        Remove-AAConnectorServicePrincipal -ManagementGroupId $ManagementGroupId
    }
    

    $appUrl = $AppUrlPrefix + $ManagementGroupId.ToString()
    $appDisplayName = $AppDisplayNamePrefix + $ManagementGroupName

    $password = "bulabulabula"


    $app = New-AzureRmADApplication -DisplayName $appDisplayName -HomePage $appUrl -IdentifierUris $appUrl -Password $password -EndDate $expires
    $princ = New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId
    Start-Sleep -Seconds 10

    New-AzureRmRoleAssignment -RoleDefinitionName "Automation Operator" -ServicePrincipalName $app.ApplicationId -ResourceGroupName $AutomationAccount.ResourceGroupName -ResourceName $AutomationAccount.AutomationAccountName -ResourceType "Microsoft.Automation/automationAccounts" | Out-Null

    return @{
        "ClientID"=$app.ApplicationId
        "Password"=$password
        "Expires"=$expires
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

function Get-AAAccountOrDie
{
    param (
        [parameter(Mandatory=$true)]
        [string]$AutomationAccountName
    )
    $a = $null
    $a = Get-AzureRmAutomationAccount | Where-Object { $_.AutomationAccountName -like $AutomationAccountName }

    if (-not ($a)) {
        throw "Automation account with name '" + $AutomationAccountName + "' was not found!"
    }
    $a
}

function Assure-ConfigManagementPack
{
    $confMP = $null
    $confMp = Get-SCManagementPack -Name $ConfigurationManagementPackName
    
    if (-not $confMP) {
        $ver = New-Object -TypeName Version -ArgumentList (1, 0 , 0, 0)
        $confMP = New-Object -TypeName Microsoft.EnterpriseManagement.Configuration.ManagementPack -ArgumentList ($ConfigurationManagementPackName, $ConfigurationManagementPackName, $ver,  $Global:ManagementGroup)
        $confMP.DisplayName = $ConfigurationManagementPackDisplayName
        $confMP.AcceptChanges()
        $Global:ManagementGroup.ManagementPacks.ImportManagementPack($confMP)
    }
    $confMP
}

function Save-ServiceCredential
{
    param(
        [parameter(Mandatory=$true)]
        [string]$ClientId,
        [parameter(Mandatory=$true)]
        [SecureString]$SecureSecret
    )

    $mg = $Global:ManagementGroup

    $secData = $null
    $secData = $mg.Security.GetSecureData() | where Name -like $SecureReferenceName
    $inserted = [bool]$secData

    if (-not $secData) {
        $secData = New-Object -TypeName Microsoft.EnterpriseManagement.Security.BasicCredentialSecureData
    }

    $secData.UserName = $ClientId
    $secData.Data = $SecureSecret
    $secData.Name = $SecureReferenceName

    if (-not $inserted) {
        $mg.Security.InsertSecureData($secData)
    }

    $secData.Update()

    $secRef = (Get-SCRunasAccount -Name $SecureReferenceName).SecureReference

    $secRefOver = $mg.Overrides.GetOverrides() | where { $_.Name -like $SecureReferenceOverrideName } 
    if (-not $secRefOver) {
        $confMp = Assure-ConfigManagementPack
        $secRefOver = New-Object -TypeName Microsoft.EnterpriseManagement.Configuration.ManagementPackSecureReferenceOverride -ArgumentList ($confMp, $SecureReferenceOverrideName)
    }
    $secRefOver.SecureReference = $secRef
    $secRefOver.DisplayName = "$SecureReferenceOverrideName"
    $secRefOver.Context = Get-SCClass -Name "System.Entity"

    $secRefOver.Value = [BitConverter]::ToString($secData.SecureStorageId, 0, $secData.SecureStorageId.Length).Replace("-", "")

    $secRefOver.GetManagementPack().AcceptChanges()

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
        [PSCredential]$AzureCredential,

        [parameter(Mandatory = $false)]
        [TimeSpan]$ServicePrincipalValidFor
    )

    if (-not $ServicePrincipalValidFor) { $ServicePrincipalValidFor = [TImeSpan]::Zero }
    
    
    Assure-AzureRMSubscription -SubscriptionId $SubscriptionId -AzureCredential $AzureAzureCredential | Out-Null
    Assure-SCSession -SCSMServerName $SCSMServerName -SCSMCredential $SCSMCredential | Out-Null
  
    $account = Get-AAAccountOrDie -AutomationAccountName $AutomationAccountName

    $client = New-AAConnectorServicePrincipal -AutomationAccount $account -ManagementGroupId $Global:ManagementGroupId -ManagementGroupName $Global:SCSession.ManagementGroupName -ValidFor $ServicePrincipalValidFor -RemoveExisting

    Save-ServiceCredential -ClientId $client.ClientId -SecureSecret (ConvertTo-SecureString -String $client.Password -AsPlainText -Force)

}

Import-Module (Get-SCModulePath)
Import-Module "AzureRM.Automation"
Import-Module "AzureRM.Resources"
Import-Module "AzureRM.Profile"





#sec data


