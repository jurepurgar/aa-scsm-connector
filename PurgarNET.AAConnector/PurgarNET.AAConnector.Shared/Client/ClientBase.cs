using PurgarNET.AAConnector.Shared.Client;
using PurgarNET.AAConnector.Shared.Client.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared.Client
{
    public enum AuthenticationType { Undefined, Code, ClientSecret }

    public abstract class ClientBase
    {
        private RestClient _client = null;
        private RestClient _tokenClient = null;
        private string _clientSecret = null;
        protected Guid _clientId = Guid.Empty;
        //protected Guid _tenantId = default(Guid);
        private string _resource = null;
        //private string _resource = null;

        private AuthenticationType _authType = AuthenticationType.Undefined;

        private static Dictionary<Guid, List<Token>> _tokens = new Dictionary<Guid, List<Token>>();
        //private static List<Token> _tokens = new List<Token>();

        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ClientBase(Uri baseUri, string resource, string apiVersion, AuthenticationType authType, Guid clientId, string clientSecret)
        {
            _authType = authType;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _resource = resource;

            var uri = new Uri(baseUri.ToString());

            _client = new RestClient(uri);
            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultParameter("api-version", apiVersion, ParameterType.QueryString);

            //init the tokenClient
            _tokenClient = new RestClient(Parameters.GetTokenUri());
            _tokenClient.AddDefaultHeader("Accept", "application/json;odata=verbose;charset=utf-8");
            _tokenClient.Encoding = Encoding.UTF8;
        }

        private string GetUrlEncodedResource(string resource)
        {
            return Uri.EscapeDataString(resource);
        }

        private async Task<Token> AcquireTokenAsync(Guid tenantId, bool force = false)
        {
            await Task.Factory.StartNew(() => _semaphore.Wait());

            Token token = null;
            Token refreshToken = null;
            if (_tokens.ContainsKey(tenantId))
            {
                token = _tokens[tenantId].FirstOrDefault(x => x.Resource == _resource);
                refreshToken = _tokens[tenantId].FirstOrDefault();
            }

            if (refreshToken == null && _tokens.ContainsKey(default(Guid)))
                refreshToken = _tokens[default(Guid)].FirstOrDefault();


            if (!force && (token != null || refreshToken != null ))
            {
                if (token != null && token.ExpiresOn > DateTime.UtcNow.AddMinutes(1)) //we make token invalid one minute before epiration date
                {
                    _semaphore.Release();
                    return token;
                }
                try
                {
                    if (refreshToken != null && refreshToken.RefreshToken != null)
                    {
                        var refreshReq = new RestRequest(Method.POST);
                        refreshReq.AddParameter("grant_type", "refresh_token");
                        refreshReq.AddParameter("refresh_token", refreshToken.RefreshToken);
                        token = await AcquireTokenByRequestAsync(refreshReq);
                    }
                }
                catch { }
            }
            else
            { 
                var req = new RestRequest(Method.POST);
                AddTokenRequestParameters(req, tenantId);
                token = await AcquireTokenByRequestAsync(req);
            }


            if (!_tokens.ContainsKey(tenantId))
                _tokens.Add(tenantId, new List<Token>());

            var existingToken = _tokens[tenantId].FirstOrDefault(x => x.Resource == _resource);
            if (existingToken != null)
                _tokens[tenantId].Remove(existingToken);
                
            _tokens[tenantId].Add(token);
            _semaphore.Release();
            return token;

        }

        private async Task<Token> AcquireTokenByRequestAsync(RestRequest request)
        {
            request.AddParameter("resource", _resource);

            var res = await _tokenClient.GetResponseAsync<Token>(request);
            return res.Data;
           
            //TODO: throw error if not successuf
        }


        private void AddTokenRequestParameters(RestRequest request, Guid tenantId)
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
                    if (tenantId != Guid.Empty)
                        tenant = tenantId.ToString();

                    args.LoginUri = new Uri(string.Format(Parameters.USER_LOGIN_URL, tenant, GetUrlEncodedResource(_resource), _clientId, Parameters.REDIRECT_URI.ToString()));
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

        
        protected async Task<T> SendAsync<T>(Guid tenantId, string resource, Method method, object data = null) where T : new()
        {
            var token = await AcquireTokenAsync(tenantId);
            var req = new RestRequest(resource, method);
            req.AddHeader("Authorization", "Bearer " + token.AccessToken);
            if (data != null)
                req.AddJsonBody(data);

            var res = await _client.GetResponseAsync<T>(req); // TODO: handle other type of errors, like no internet, etc...

            return res.Data;
        }

        protected async Task<T> GetAsync<T>(Guid tenantId, string resource) where T : new()
        {
            return await SendAsync<T>(tenantId, resource, Method.GET);
        }

        protected async Task<List<T>> GetListAsync<T>(Guid tenantId, string resource)
        {
            var res = await GetAsync<ApiResponse<List<T>>>(tenantId, resource);

            if (res.Error != null)
                throw new ApiException(res.Error.Code, res.Error.Message);
            else
                return res.Value;
        }

    }

    public class AuthorizationCodeRequiredEventArgs : EventArgs
    {
        public string Code { get; set; }

        public Uri LoginUri { get; set; }
    }
}
