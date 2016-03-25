
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
 
        public async Task<AdApplication> GetApplicationAsync()
        {
            var apps = await GetListAsync<AdApplication>(_tenantId, Parameters.GRAPH_API_VERSION, $"/applications?$filter=identifierUris/any(s:s eq '{Parameters.AACONNECTOR_APP_URI}')");
            return apps.FirstOrDefault();
        }

        public async Task<AdApplication> CreateApplicationAsync()
        {
            var app = new AdCreateableApplication()
            {
                DisplayName = Parameters.AACONNECTOR_APP_DISPLAYNAME,
                Homepage = Parameters.AACONNECTOR_APP_URI
            };
            app.IdentifierUris.Add(Parameters.AACONNECTOR_APP_URI);
            var resApp = await SendAsync<AdApplication>(_tenantId, Parameters.GRAPH_API_VERSION, "/applications", RestSharp.Method.POST, app);
            return resApp;
        }

        public async Task<ServicePrincipal> GetServicePrincipalAsync(Guid appId)
        {
            var principals = await GetListAsync<ServicePrincipal>(_tenantId, Parameters.GRAPH_API_VERSION, $"/servicePrincipals?$filter=servicePrincipalNames/any(c:c eq '{appId.ToString()}')");
            return principals.FirstOrDefault();
        }

        public async Task<ServicePrincipal> CreateServicePrincipalAsync(Guid appId)
        {
            var principal = new ServicePrincipal()
            {
                AccountEnabled = true,
                AppId = appId
            };
            principal = await SendAsync<ServicePrincipal>(_tenantId, Parameters.GRAPH_API_VERSION, "/servicePrincipals", RestSharp.Method.POST, principal);
            return principal;
        }

        public async Task DeleteServicePrincipalAndApplicationAsync()
        {
            var app = await GetApplicationAsync();
            if (app != null)
            {
                var principal = await GetServicePrincipalAsync(app.AppId);
                if (principal != null)
                    await SendAsync(_tenantId, Parameters.GRAPH_API_VERSION, $"/servicePrincipals/{principal.ObjectId}", RestSharp.Method.DELETE);
                await SendAsync(_tenantId, Parameters.GRAPH_API_VERSION, $"/applications/{app.ObjectId}", RestSharp.Method.DELETE);
            } 
        }

        public async Task<AdApplication> UpdateApplicationAsync(AdApplication app)
        {
            return await SendAsync<AdApplication>(_tenantId, Parameters.GRAPH_API_VERSION, $"/applications/{app.ObjectId}", RestSharp.Method.PATCH, app);
        }        

    }
}
