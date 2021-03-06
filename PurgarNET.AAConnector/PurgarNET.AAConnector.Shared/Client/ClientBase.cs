﻿using PurgarNET.AAConnector.Shared.Client;
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
        private string _clientSecret = null;
        protected Guid _clientId = Guid.Empty;
        private string _resource = null;
 
        private AuthenticationType _authType = AuthenticationType.Undefined;

        private static Dictionary<Guid, List<Token>> _tokens = new Dictionary<Guid, List<Token>>();
  
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ClientBase(Uri baseUri, string resource, AuthenticationType authType, Guid clientId, string clientSecret)
        {
            _authType = authType;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _resource = resource;

            var uri = new Uri(baseUri.ToString());

            _client = new RestClient(uri);
            _client.AddDefaultHeader("Accept", "application/json");
                       

            SimpleJson.CurrentJsonSerializerStrategy = new CamelJsonSerializerStrategy();
        }

        private string GetUrlEncodedResource(string resource)
        {
            return Uri.EscapeDataString(resource);
        }

        private async Task<Token> AcquireTokenAsync(Guid tenantId, bool force = false)
        {
            await Task.Factory.StartNew(() => _semaphore.Wait());

            try
            {
                Token token = null;
                Token refreshToken = null;
                if (_tokens.ContainsKey(tenantId))
                {
                    token = _tokens[tenantId].FirstOrDefault(x => x.Resource == _resource);
                    refreshToken = _tokens[tenantId].FirstOrDefault();
                }

                if (refreshToken == null && _tokens.ContainsKey(default(Guid))) //if current token does not have refresh token, we get it from any other token for the same tenant (multi-resource refresh token)
                    refreshToken = _tokens[default(Guid)].FirstOrDefault();


                if (force || token == null || token.ExpiresOn < DateTime.UtcNow.AddMinutes(-1)) //if we dont have token or it is expired we set it to null so it will be retreived
                    token = null;


                if (!force && token == null && refreshToken != null && refreshToken.RefreshToken != null) // if we dont have token we try to get it by refresh token if we have it
                {
                    try
                    {
                        var refreshReq = new RestRequest(Method.POST);
                        refreshReq.AddParameter("grant_type", "refresh_token");
                        refreshReq.AddParameter("refresh_token", refreshToken.RefreshToken);
                        token = await AcquireTokenByRequestAsync(tenantId, refreshReq);
                    }
                    catch
                    {
                        token = null; // if refresh token does not work we continue
                    }
                }

                if (token == null) //get token by login or client secret
                {
                    var req = new RestRequest(Method.POST);
                    AddTokenRequestParameters(req, tenantId);
                    token = await AcquireTokenByRequestAsync(tenantId, req);
                }

                if (!_tokens.ContainsKey(tenantId))
                    _tokens.Add(tenantId, new List<Token>());

                var existingToken = _tokens[tenantId].FirstOrDefault(x => x.Resource == _resource);
                if (existingToken == null || existingToken.AccessToken != token.AccessToken) //if we have new token we save it to cache
                {
                    if (existingToken != null)
                        _tokens[tenantId].Remove(existingToken);
                    _tokens[tenantId].Add(token);
                }

                return token;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<Token> AcquireTokenByRequestAsync(Guid tenantId, RestRequest request)
        {
            request.AddParameter("resource", _resource);

            var tokenClient = new RestClient(Parameters.GetTokenUri(tenantId));
            tokenClient.AddDefaultHeader("Accept", "application/json;odata=verbose;charset=utf-8");
            tokenClient.Encoding = Encoding.UTF8;

            var res = await tokenClient.GetResponseAsync<Token>(request);
            CheckResponse(res);
            return res.Data;
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


        private async Task<RestRequest> CreateRequestAsync(Guid tenantId, string apiVersion, string resource, Method method, object data = null)
        {
            if (string.IsNullOrEmpty(apiVersion))
                throw new ArgumentNullException(nameof(apiVersion));
            if (string.IsNullOrEmpty(resource))
                throw new ArgumentNullException(nameof(resource));

            var token = await AcquireTokenAsync(tenantId);
            var req = new RestRequest(resource, method);
            req.AddHeader("Authorization", "Bearer " + token.AccessToken);
            req.AddParameter("api-version", apiVersion, ParameterType.QueryString);
            if (data != null)
                req.AddJsonBody(data);
            return req;
        }

        private void CheckResponse(IRestResponse response)
        {
            if ((int)response.StatusCode >= 400)
                throw new HttpException(response.StatusCode, 
                    $"Http error ({((int)response.StatusCode).ToString()} - {response.StatusCode.ToString()}) for resource '/{response.Request.Resource}'\n\r: {response.Content}");
        }

        protected async Task<T> SendAsync<T>(Guid tenantId, string apiVersion, string resource, Method method, object data = null) where T : new()
        {
            var req = await CreateRequestAsync(tenantId, apiVersion, resource, method, data);
            var res = await _client.GetResponseAsync<T>(req); // TODO: handle other type of errors, like no internet, etc...
            CheckResponse(res);
            return res.Data;
        }

        protected async Task SendAsync(Guid tenantId, string apiVersion, string resource, Method method, object data = null)
        {
            var req = await CreateRequestAsync(tenantId, apiVersion, resource, method, data);
            var res = await _client.GetResponseAsync(req); // TODO: handle other type of errors, like no internet, etc...
            CheckResponse(res);
        }

        protected async Task<T> GetAsync<T>(Guid tenantId, string apiVersion, string resource) where T : new()
        {
            return await SendAsync<T>(tenantId, apiVersion, resource, Method.GET);
        }

        protected async Task<List<T>> GetListAsync<T>(Guid tenantId, string apiVersion, string resource)
        {
            var res = await GetAsync<ApiResponse<List<T>>>(tenantId, apiVersion, resource);

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
