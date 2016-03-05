using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Client
{
    public class AAAccountClientBase : AAClientBase
    {
        public AAAccountClientBase(string accountId, Guid tenantId, AuthenticationType authType, string clientId, string clientSecret) : base(GetUri(accountId), tenantId, authType, clientId, clientSecret)
        {

        }

        private static Uri GetUri(string accountId)
        {
            var url = Parameters.API_ENDPOINT_URI.ToString() + accountId;
            return new Uri(url);
        }



        
    }
}
