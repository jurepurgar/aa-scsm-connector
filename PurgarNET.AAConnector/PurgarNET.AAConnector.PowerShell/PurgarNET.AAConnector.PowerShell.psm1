$AppDisplayNamePrefix = "Azure Automation Connector for Service Manager - "
$AppUrlPrefix = "https://servicemanager.managementgroup/"


<#
	My Function
#>
function Register-AAConnectorAutomationAccount 
{



	Import-Module AzureRM

	#check if module is installed


	Login-AzureRmAccount 



    $ManagementGroupId = [Guid]::NewGuid() #get from actual management group
    $appUrl = $AppUrlPrefix + $ManagementGroupId.ToString()
    $appDisplayName = $AppDisplayNamePrefix + $ManagementGroupId.ToString()

    $account = Get-AzureRmAutomationAccount | Where-Object { $_.AutomationAccountName -like "kolit-automation-dev" }

    $subscription = Get-AzureRmSubscription -SubscriptionId $account.SubscriptionId

    $app = Get-AzureRmADApplication -DisplayNameStartWith $AdApplicationDisplayName

    $app = New-AzureRmADApplication -DisplayName $appDisplayName -HomePage $appUrl -IdentifierUris $appUrl -Password "bulabulabula" -EndDate (Get-Date).AddYears(10)

    $principal = New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId

    New-AzureRmRoleAssignment -RoleDefinitionName Reader -ServicePrincipalName $app.ApplicationId
    
    #Get-AzureRmADServicePrincipal -ErrorVariable


    if (-not $principal) {

        $principal = New-AzureRmADServicePrincipal -ApplicationId $app. 

    }


}