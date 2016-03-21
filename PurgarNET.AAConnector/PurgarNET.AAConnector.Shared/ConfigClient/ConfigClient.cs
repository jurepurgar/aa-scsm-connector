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

        public ConfigClient() : base(Parameters.AZURE_API_URI, Parameters.AZURE_RESOURCE, Parameters.AZURE_API_VERSION, AuthenticationType.Code, Parameters.CLIENT_ID, null)
        { }



        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            return await GetListAsync<Tenant>(default(Guid), "tenants");
        }

        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(Guid tenantId)
        {
            return await GetListAsync<Subscription>(tenantId, "subscriptions");
        }
    }
}
