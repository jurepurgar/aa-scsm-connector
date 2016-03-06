using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Client
{
    public class GraphClient : AAClientBase
    {
        Guid _tenantId = default(Guid);

        public GraphClient(Guid tenantId) : base(GetUri(tenantId), tenantId, Parameters.GRAPH_RESOURCE, Parameters.GRAPH_API_VERSION, AuthenticationType.Code, Parameters.POWERSHELL_CLIENT_ID, null)
        {
            _tenantId = tenantId;
        }

        private static Uri GetUri(Guid tenantId)
        {
            var url = Parameters.GRAPH_API_URI.ToString() + tenantId.ToString();
            return new Uri(url);
        }

        public async Task<List<string>> GetApplications() todo //TODO: Do JSON to C# string is not OK
        {
            return await GetList<string>($"applications");
        }

    }
}
