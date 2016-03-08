using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared.AutomationClient
{
    public class AAUserClient : AAClientBase
    {
        public AAUserClient(string tenant, Guid subscriptionId, string resourceGroup, string automationAccountName, string clientId) : base(tenant, subscriptionId, resourceGroup, automationAccountName,  AuthenticationType.Code, clientId, null)
        {

        }

        

        public async Task<List<HybridRunbookWorkerGroup>> GetHybridRunbookWorkerGroups()
        {
            return await GetList<HybridRunbookWorkerGroup>("hybridRunbookWorkerGroups");
        }

        

        public async Task<List<Job>> GetJobs()
        {
            return await GetList<Job>("jobs");
        }
    }
}
