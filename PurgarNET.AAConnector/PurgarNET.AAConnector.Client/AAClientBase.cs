using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RestSharp;
using System.Threading.Tasks;
using PurgarNET.AAConnector.Client.Models;

namespace PurgarNET.AAConnector.Client
{
    public enum AuthenticationType { Undefined, Code, ClientSecret }

    public abstract class AAClientBase
    {
        private RestClient _client = null;
        private RestClient _tokenClient = null;
        private string _clientSecret = null;
        private string _clientId = null;

        private AuthenticationType _authType = AuthenticationType.Undefined;

        private Token _token = null;
        private object _lck = new object();
        private Task _tokenTask = null;

        public AAClientBase(Uri baseUri, Guid tenantId, AuthenticationType authType, string clientId, string clientSecret)
        {
            _authType = authType;
            _clientId = clientId;
            _clientSecret = clientSecret;

            //TODO: verify parameters based on authType

            _client = new RestClient(baseUri);
            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultParameter("api-version", "2015-10-31", ParameterType.QueryString);

            //init the tokenClient
            _tokenClient = new RestClient(Parameters.GetTokenUri(tenantId));
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
            request.AddParameter("resource", Parameters.RESOURCE);
            

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

            var res = await _client.GetResponseAsync<ApiResponse<T>>(req); // TODO: handle other type of errors, like no internet, etc...

            if (res.Data.Error != null)
                throw new ApiException(res.Data.Error.Code, res.Data.Error.Message);
            else
                return res.Data.Value;
        }

        protected async Task<List<T>> GetList<T>(string resource)
        {
            return await Get<List<T>>(resource);
        }


    }

    public class AuthorizationCodeRequiredEventArgs : EventArgs
    {
        public string Code { get; set; }
    }

}
