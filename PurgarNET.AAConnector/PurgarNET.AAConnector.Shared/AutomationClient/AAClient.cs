using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared.AutomationClient
{
    public enum AuthenticationType { Undefined, Code, ClientSecret }

    public class AAClient
    {
        private RestClient _client = null;
        private RestClient _tokenClient = null;
        private string _clientSecret = null;
        private Guid _clientId = Guid.Empty;
        //private string _resource = null;
        private Guid _tenantId = Guid.Empty;

        private AuthenticationType _authType = AuthenticationType.Undefined;

        private Token _token = null;
        private object _lck = new object();

        private Task _tokenTask = null;

        public AAClient(Guid tenantId, Guid subscriptionId, string resourceGroup, string automationAccountName, AuthenticationType authType, Guid clientId, string clientSecret)
        {
            _authType = authType;
            _clientId = clientId;
            _clientSecret = clientSecret;
            //_resource = resource;
            _tenantId = tenantId;

            //TODO: verify parameters based on authType
            var uri = new Uri($"{Parameters.AZURE_API_URI}subscriptions/{subscriptionId.ToString()}/resourceGroups/{resourceGroup}/providers/Microsoft.Automation/automationAccounts/{automationAccountName}/");

            _client = new RestClient(uri);
            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultParameter("api-version", Parameters.AZURE_API_VERSION, ParameterType.QueryString);

            //init the tokenClient
            _tokenClient = new RestClient(Parameters.GetTokenUri(_tenantId));
            _tokenClient.AddDefaultHeader("Accept", "application/json;odata=verbose;charset=utf-8");
            _tokenClient.Encoding = Encoding.UTF8;
        }

        public async Task AssureTokenAsync()
        {
            lock (_lck)
            {
                if (_tokenTask == null || _tokenTask.IsCanceled || _tokenTask.IsCompleted || _tokenTask.IsFaulted)
                    _tokenTask = AssureTokenInternalAsync(false);
            }
            await _tokenTask;
        }


        private async Task AssureTokenInternalAsync(bool force = false)
        {
            if (!force && _token != null)
            {
                if (_token.ExpiresOn > DateTime.UtcNow.AddMinutes(1)) //we make token invalid one minute before epiration date
                {
                    return;
                }
                try
                {
                    if (_token.RefreshToken != null)
                    {
                        var refreshReq = new RestRequest(Method.POST);
                        refreshReq.AddParameter("grant_type", "refresh_token");
                        refreshReq.AddParameter("refresh_token", _token.RefreshToken);
                        await AssureTokenByRequestAsync(refreshReq);
                        return;
                    }
                }
                catch { }
            }
            var req = new RestRequest(Method.POST);
            AddTokenRequestParameters(req);

            await AssureTokenByRequestAsync(req);
        }


        private void AddTokenRequestParameters(RestRequest request)
        {
            request.AddParameter("client_id", _clientId);
            request.AddParameter("redirect_uri", Parameters.REDIRECT_URI);

            if (_authType == AuthenticationType.Code)
            {
                if (AuthorizationCodeRequired == null)
                    throw new InvalidOperationException("AuthorizationCodeRequired event does not have an event handler!");
                else
                {
                    var args = new AuthorizationCodeRequiredEventArgs();
                    string tenant = "common";
                    if (_tenantId != Guid.Empty)
                        tenant = _tenantId.ToString();

                    args.LoginUri = new Uri(string.Format(Parameters.USER_LOGIN_URL, tenant, Parameters.GetUrlEncodedResource(), _clientId, Parameters.REDIRECT_URI.ToString()));
                    AuthorizationCodeRequired(this, args);
                    if (args.Code == null)
                        throw new InvalidOperationException("AuthorizationCode retreived is null.");

                    request.AddParameter("grant_type", "authorization_code");
                    request.AddParameter("code", args.Code);
                }
            }
            else if (_authType == AuthenticationType.ClientSecret)
            {
                request.AddParameter("grant_type", "client_credentials");
                request.AddParameter("client_secret", _clientSecret);
            }
            else
                throw new InvalidOperationException($"Unsuported authentication type: {_authType.ToString()}");
        }

        public event EventHandler<AuthorizationCodeRequiredEventArgs> AuthorizationCodeRequired;

        private async Task AssureTokenByRequestAsync(RestRequest request)
        {
            request.AddParameter("resource", Parameters.AZURE_RESOURCE);
            

            var res = await _tokenClient.GetResponseAsync<Token>(request);
            _token = res.Data;
            _client.AddDefaultHeader("Authorization", "Bearer " + _token.AccessToken);

            //TODO: throw error if not successuf
        }

        protected async Task<T> SendAsync<T>(string resource, Method method, object data = null) where T : new()
        {
            await AssureTokenAsync();

            var req = new RestRequest(resource, method);
            if (data != null)
                req.AddJsonBody(data);

            var res = await _client.GetResponseAsync<T>(req); // TODO: handle other type of errors, like no internet, etc...

            return res.Data;
        }

        protected async Task<T> GetAsync<T>(string resource) where T : new()
        {
            return await SendAsync<T>(resource, Method.GET);
        }

        protected async Task<List<T>> GetListAsync<T>(string resource)
        {
            var res = await GetAsync<ApiResponse<List<T>>>(resource);
            
            if (res.Error != null)
                throw new ApiException(res.Error.Code, res.Error.Message);
            else
                return res.Value;
        }
        

        public async Task<List<Runbook>> GetRunbooksAsync()
        {
            return await GetListAsync<Runbook>("runbooks");
        }

        public async Task<Runbook> GetRunbookAsync(string runbookName)
        {
            return await GetAsync<Runbook>($"runbooks/{runbookName}");
        }


        public async Task<Job> StartJob(Job j)
        {
            var jobId = Guid.NewGuid();
            return await SendAsync<Job>($"jobs/{jobId}", Method.PUT, j);
        }


        public async Task<List<HybridRunbookWorkerGroup>> GetHybridRunbookWorkerGroupsAsync()
        {
            return await GetListAsync<HybridRunbookWorkerGroup>("hybridRunbookWorkerGroups");
        }

        public async Task<List<Job>> GetJobsAsync()
        {
            return await GetListAsync<Job>("jobs");
        }

        public async Task<Job> GetJobAsync(Guid jobId)
        {
            return await GetAsync<Job>($"jobs/{jobId.ToString()}");
        }

    }

    public class AuthorizationCodeRequiredEventArgs : EventArgs
    {
        public string Code { get; set; }

        public Uri LoginUri { get; set; }
    }

}
