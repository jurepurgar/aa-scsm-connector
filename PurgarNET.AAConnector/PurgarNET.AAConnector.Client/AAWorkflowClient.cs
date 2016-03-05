using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace PurgarNET.AAConnector.Client
{
    public class AAWorkflowClient : AAAccountClientBase
    {
        public AAWorkflowClient(string accountId, Guid tenantId, string clientId, string clientSecret) : base(accountId, tenantId, AuthenticationType.ClientSecret, clientId, clientSecret)
        {

        }
    }
}
