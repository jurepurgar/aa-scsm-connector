using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Client
{
    public class AAAccountClientBase : AAClientBase
    {
        public AAAccountClientBase(string accountId, Guid tenantId, AuthenticationType authType, string clientId, string clientSecret) : base(GetUri(accountId), tenantId, Parameters.AZURE_RESOURCE, Parameters.AZURE_API_VERSION, authType, clientId, clientSecret)
        {

        }

        private static Uri GetUri(string accountId)
        {
            var url = Parameters.AZURE_API_URI.ToString() + accountId;
            return new Uri(url);
        }



        
    }
}
