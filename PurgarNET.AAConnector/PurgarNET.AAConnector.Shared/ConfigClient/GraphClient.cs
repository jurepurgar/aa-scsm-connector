
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
        private Guid _tenantId;
        public GraphClient(Guid tenantId) : base(GetUri(tenantId), Parameters.GRAPH_RESOURCE, AuthenticationType.Code, Parameters.CLIENT_ID, null)
        {
            _tenantId = tenantId;
        }

        private static Uri GetUri(Guid tenantId)
        {
            return new Uri(Parameters.GRAPH_API_URI + tenantId.ToString() + "/");
        }
 
        public async Task<Application> GetApplicationAsync()
        {
            var apps = await GetListAsync<Application>(_tenantId, Parameters.GRAPH_API_VERSION, $"/applications?$filter=identifierUris/any(s:s eq '{Parameters.AACONNECTOR_APP_URI}')");
            return apps.FirstOrDefault();
        }

        public async Task<Application> CreateApplicationAsync()
        {
            var app = new CreateableApplication()
            {
                DisplayName = "Azure Automation Connector for Service Manager",
                Homepage = Parameters.AACONNECTOR_APP_URI
                //AvailableToOtherTenants = false
            };
            app.IdentifierUris.Add(Parameters.AACONNECTOR_APP_URI);
            var resApp = await SendAsync<Application>(_tenantId, Parameters.GRAPH_API_VERSION, "/applications", RestSharp.Method.POST, app);
            return resApp;
        }

        public async Task<Application> UpdateApplicationAsync(Application app)
        {
            return await SendAsync<Application>(_tenantId, Parameters.GRAPH_API_VERSION, $"/applications/{app.ObjectId}", RestSharp.Method.PATCH, app);
        }

      /*  public async Task<string> CreateApplicationCredentialAsync(Guid AppObjectId, Guid KeyId)
        {
            var app = new CredApplication()
            {
                
            };

            app.PasswordCredentials.Add(new PasswordCredential()
            {
                //CustomKeyIdentifier = KeyId.ToString(),
                KeyId = KeyId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(365),
                Value = "BulaBula1A"
            });

            var res = await SendAsync<Application>(_tenantId, Parameters.GRAPH_API_VERSION, $"applications/{AppObjectId.ToString()}", RestSharp.Method.PATCH, app);

            return null;
        }*/

    }
}
