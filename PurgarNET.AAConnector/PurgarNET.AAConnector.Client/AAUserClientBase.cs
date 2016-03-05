using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace PurgarNET.AAConnector.Client
{
    public class AAUserClient : AAAccountClientBase
    {
        public AAUserClient(string accountId, Guid tenantId, string clientId) : base(accountId, tenantId, AuthenticationType.Code, clientId, null)
        {

        }
    }
}
