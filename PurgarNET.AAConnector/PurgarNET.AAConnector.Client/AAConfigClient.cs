using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Threading.Tasks;
using PurgarNET.AAConnector.Client.Models;

namespace PurgarNET.AAConnector.Client
{
    public class AAConfigClient : AAClientBase
    {
        public AAConfigClient() : base(Parameters.AZURE_API_URI, default(Guid), Parameters.AZURE_RESOURCE, Parameters.AZURE_API_VERSION, AuthenticationType.Code, Parameters.POWERSHELL_CLIENT_ID, null)
        { }

        //Api methods


        public async Task<List<Tenant>> GetTenants()
        {
            return await GetList<Tenant>("tenants");
        }

        public async Task<List<Subscription>> GetSubscriptions()
        {
            return await GetList<Subscription>("subscriptions");
        }

        public async Task<List<AutomationAccount>> GetAutomationAccounts(Guid subscriptionId)
        {
            return await GetList<AutomationAccount>($"/subscriptions/{subscriptionId}/providers/Microsoft.Automation/automationAccounts");
        }
    }
}
