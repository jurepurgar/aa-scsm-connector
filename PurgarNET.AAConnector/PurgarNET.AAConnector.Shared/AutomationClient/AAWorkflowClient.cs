using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;

namespace PurgarNET.AAConnector.Shared.AutomationClient
{
    public class AAWorkflowClient : AAClientBase
    {
        public AAWorkflowClient(Guid tenant, Guid subscriptionId, string resourceGroup, string automationAccountName, Guid clientId, string clientSecret) : base(tenant, subscriptionId, resourceGroup, automationAccountName,  AuthenticationType.ClientSecret, clientId, clientSecret)
        {

        }
    }
}
