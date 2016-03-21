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
                                ResourceGroupName = r.Name,
                                AutomationAccountName = a.Name
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}
