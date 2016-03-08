using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;
using System.Threading.Tasks;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;

namespace PurgarNET.AAConnector.Shared.AutomationClient
{
    public enum AuthenticationType { Undefined, Code, ClientSecret }

    public abstract class AAClientBase
    {
        private RestClient _client = null;
        private RestClient _tokenClient = null;
        private string _clientSecret = null;
        private string _clientId = null;
        //private string _resource = null;
        private string _tenant = null;

        private AuthenticationType _authType = AuthenticationType.Undefined;

        private Token _token = null;
        private object _lck = new object();
        private Task _tokenTask = null;

        public AAClientBase(string tenant, Guid subscriptionId, string resourceGroup, string automationAccountName, AuthenticationType authType, string clientId, string clientSecret)
        {
            _authType = authType;
            _clientId = clientId;
            _clientSecret = clientSecret;
            //_resource = resource;
            _tenant = tenant;

            //TODO: verify parameters based on authType
            var uri = new Uri($"{Parameters.AZURE_API_URI}subscriptions/{subscriptionId.ToString()}/resourceGroups/{resourceGroup}/providers/Microsoft.Automation/automationAccounts/{automationAccountName}/");

            _client = new RestClient(uri);

            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultParameter("api-version", Parameters.AZURE_API_VERSION, ParameterType.QueryString);

            //init the tokenClient
            _tokenClient = new RestClient(Parameters.GetTokenUri(_tenant));
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
                    args.LoginUri = new Uri(string.Format(Parameters.USER_LOGIN_URL, _tenant, Parameters.GetUrlEncodedResource(), _clientId, Parameters.REDIRECT_URI.ToString()));
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



        //api calls
        protected async Task<T> Get<T>(string resource) where T : new()
        {
            await AssureTokenAsync();

            var req = new RestRequest(resource, Method.GET);

            var res = await _client.GetResponseAsync<T>(req); // TODO: handle other type of errors, like no internet, etc...
            
            return res.Data;
        }

        protected async Task<List<T>> GetList<T>(string resource)
        {
            var res = await Get<ApiResponse<List<T>>>(resource);
            
            if (res.Error != null)
                throw new ApiException(res.Error.Code, res.Error.Message);
            else
                return res.Value;
        }



        public async Task<Job> GetJob(Guid jobId)
        {
            return await Get<Job>($"jobs/{jobId.ToString()}/");
        }

        public async Task<List<Runbook>> GetRunbooks()
        {
            return await GetList<Runbook>("runbooks");
        }


    }

    public class AuthorizationCodeRequiredEventArgs : EventArgs
    {
        public string Code { get; set; }

        public Uri LoginUri { get; set; }
    }

}
