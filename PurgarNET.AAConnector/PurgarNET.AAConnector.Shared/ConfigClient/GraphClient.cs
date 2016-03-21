
using PurgarNET.AAConnector.Shared.Client;
using PurgarNET.AAConnector.Shared.Client.Models;
using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared.ConfigClient
{
    public class GraphClient : ClientBase
    {
        public GraphClient() : base(Parameters.GRAPH_API_URI, Parameters.GRAPH_RESOURCE, Parameters.GRAPH_API_VERSION, AuthenticationType.Code, Parameters.CLIENT_ID, null)
        {

        }

 
        public async Task<Application> GetApplication(Guid tenantId, Guid managementGroupId)
        {
            var apps = await GetListAsync<Application>(tenantId, $"/{tenantId}/applications?$filter=identifierUris/any(s:s eq 'https://servicemanager.managementgroup/{managementGroupId.ToString()}')");

            return apps.FirstOrDefault();
        }
    }
}
