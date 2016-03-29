using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System.Threading.Tasks;
using PurgarNET.AAConnector.Shared.Client.Models;
using PurgarNET.AAConnector.Shared.Client;

namespace PurgarNET.AAConnector.Shared.AutomationClient
{
    //public enum AuthenticationType { Undefined, Code, ClientSecret }

    public class AAClient : ClientBase
    {

        private Guid _tenantId = Guid.Empty;

        public AAClient(Guid tenantId, Guid subscriptionId, string resourceGroup, string automationAccountName, AuthenticationType authType, Guid clientId, string clientSecret)
            : base(GetUri(subscriptionId, resourceGroup, automationAccountName), Parameters.AZURE_RESOURCE, authType, clientId, clientSecret)
        {
            _tenantId = tenantId;
        }

        public static Uri GetUri(Guid subscriptionId, string resourceGroup, string automationAccountName)
        {
            return new Uri($"{Parameters.AZURE_API_URI}subscriptions/{subscriptionId.ToString()}/resourceGroups/{resourceGroup}/providers/Microsoft.Automation/automationAccounts/{automationAccountName}/");
        }

       
        public async Task<List<Runbook>> GetRunbooksAsync()
        {
            return await GetListAsync<Runbook>(_tenantId, Parameters.AUTOMATION_API_VERSION, "runbooks");
        }

        public async Task<Runbook> GetRunbookAsync(string runbookName)
        {
            return await GetAsync<Runbook>(_tenantId, Parameters.AUTOMATION_API_VERSION, $"runbooks/{runbookName}");
        }


        public async Task<Job> StartJob(Job j)
        {
            var jobId = Guid.NewGuid();
            return await SendAsync<Job>(_tenantId, Parameters.AUTOMATION_API_VERSION, $"jobs/{jobId}", Method.PUT, j);
        }


        public async Task<List<HybridRunbookWorkerGroup>> GetHybridRunbookWorkerGroupsAsync()
        {
            return await GetListAsync<HybridRunbookWorkerGroup>(_tenantId, Parameters.AUTOMATION_API_VERSION, "hybridRunbookWorkerGroups");
        }

        public async Task<List<Job>> GetJobsAsync()
        {
            return await GetListAsync<Job>(_tenantId, Parameters.AUTOMATION_API_VERSION, "jobs");
        }

        public async Task<Job> GetJobAsync(Guid jobId)
        {
            return await GetAsync<Job>(_tenantId, Parameters.AUTOMATION_API_VERSION, $"jobs/{jobId.ToString()}");
        }

        public async Task<string> GetJobOutput(Guid jobId)
        {
            var streams = await GetListAsync<OutputItem>(_tenantId, Parameters.AUTOMATION_API_VERSION, $"jobs/{jobId}/streams?$filter=properties/streamType eq 'Output'");
            var sb = new StringBuilder();
            streams.ForEach((x) => sb.AppendLine(x.Properties.Summary));
            return sb.ToString();
        }

    }

   /* public class AuthorizationCodeRequiredEventArgs : EventArgs
    {
        public string Code { get; set; }

        public Uri LoginUri { get; set; }
    }*/

}
