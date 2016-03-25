using PurgarNET.AAConnector.Shared.Client;
using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared.ConfigClient
{
    public class ConfigClient : ClientBase
    {

        public ConfigClient() : base(Parameters.AZURE_API_URI, Parameters.AZURE_RESOURCE, AuthenticationType.Code, Parameters.CLIENT_ID, null)
        { }


        public async Task<IEnumerable<AutomationAccountInfo>> GetAutomationAccountsAsync()
        {
            var list = new List<AutomationAccountInfo>();

            var tenants = await GetListAsync<Tenant>(default(Guid), Parameters.AZURE_API_VERSION, "tenants");
            foreach (var t in tenants)
            {
                var subscriptions = await GetListAsync<Subscription>(t.TenantId, Parameters.AZURE_API_VERSION, "subscriptions");
                foreach (var s in subscriptions)
                {
                    var resourceGroups = await GetListAsync<ResourceGroup>(t.TenantId, Parameters.AZURE_API_VERSION, $"subscriptions/{s.SubscriptionId.ToString()}/resourcegroups");
                    foreach (var r in resourceGroups)
                    {
                        var automationAccounts = await GetListAsync<AutomationAccount>(t.TenantId, Parameters.AUTOMATION_API_VERSION, $"/subscriptions/{s.SubscriptionId}/resourceGroups/{r.Name}/providers/Microsoft.Automation/automationAccounts");
                        foreach (var a in automationAccounts)
                        {
                            list.Add(new AutomationAccountInfo()
                            {
                                TenantId = t.TenantId,
                                SubscriptionId = s.SubscriptionId,
                                SubscriptionName = s.DisplayName,
                                ResourceGroupName = r.Name,
                                AutomationAccountName = a.Name
                            });
                        }
                    }
                }
            }
            return list;
        }

        public async Task SetServicePrincipalPermission(AutomationAccountInfo account, Guid principalId)
        {
            var uri = $"subscriptions/{account.SubscriptionId.ToString()}/resourceGroups/{account.ResourceGroupName}/providers/Microsoft.Automation/automationAccounts/{account.AutomationAccountName}/providers/Microsoft.Authorization/roleDefinitions?$filter=roleName eq 'Automation Operator'";
            var role = (await GetListAsync<RoleDefinition>(account.TenantId, "2015-07-01", uri)).FirstOrDefault();

            uri = $"subscriptions/{account.SubscriptionId.ToString()}/resourceGroups/{account.ResourceGroupName}/providers/Microsoft.Automation/automationAccounts/{account.AutomationAccountName}/providers/Microsoft.Authorization/roleAssignments?$filter=principalId eq '{principalId.ToString()}'";

            var existing = await GetListAsync<RoleAsignment>(account.TenantId, "2015-07-01", uri);
            if (existing.FirstOrDefault(x => x.Properties.RoleDefinitionId == role.Id) == null)
            { 
                var assignment = new RoleAsignment();
                assignment.Properties.PrincipalId = principalId;
                assignment.Properties.RoleDefinitionId = role.Id;
                uri = $"subscriptions/{account.SubscriptionId.ToString()}/resourceGroups/{account.ResourceGroupName}/providers/Microsoft.Automation/automationAccounts/{account.AutomationAccountName}/providers/Microsoft.Authorization/roleAssignments/{Guid.NewGuid()}";
                await SendAsync(account.TenantId, "2015-07-01", uri, RestSharp.Method.PUT, assignment);
            }
        }
    }
}
