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
    public abstract class AAClientBase
    {
        private RestClient  _client = null;
        private RestClient _tokenClient = null;

        private Token _token = null;

        private object _lck = new object();

        private Task _initializeTask = null;

        public virtual async Task InitializeAsync(Guid tenantId)
        {
            lock (_lck){
                if (_initializeTask == null)
                    _initializeTask = InitializeInternalAsync(tenantId);
            }
            await _initializeTask;
        }


        private async Task InitializeInternalAsync(Guid tenantId)
        {
            _client = new RestClient(Parameters.API_ENDPOINT_URI);
            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultParameter("api-version", "2015-10-31", ParameterType.QueryString);

            //init the tokenClient
            _tokenClient = new RestClient(Parameters.GetTokenUri(tenantId));
            _tokenClient.AddDefaultHeader("Accept", "application/json;odata=verbose;charset=utf-8");
            _tokenClient.Encoding = Encoding.UTF8;
            await AssureTokenAsync();

            //create API client
        }

        /*public static void Uninitialize()
        {
            //TODO: uninitialize
        }*/

        private async Task AssureTokenAsync(bool force = false)
        {
            if (!force && _token != null )
            {
                if (_token.ExpiresOn < DateTime.UtcNow.AddMinutes(1)) //we make token invalid one minute before epiration date
                {
                    return;
                }
                try
                {
                    var refreshReq = new RestRequest(Method.POST);
                    //todo: add refresh parameters
                    await AssureTokenByRequestAsync(refreshReq);
                    return;
                }
                catch {}
            }
            var req = new RestRequest(Method.POST);
            AddTokenRequestParameters(req);

            await AssureTokenByRequestAsync(req);
        }


        protected abstract void AddTokenRequestParameters(RestRequest request);

        private async Task AssureTokenByRequestAsync(RestRequest request)
        {
            request.AddParameter("resource", Parameters.RESOURCE);
            request.AddParameter("redirect_uri", Parameters.REDIRECT_URI);

            var res = await _tokenClient.GetResponseAsync<Token>(request);
            _token = res.Data;
            _client.AddDefaultHeader("Authorization", "Bearer " + _token.AccessToken);

            //TODO: throw error if not successuf
        }

        public async Task<T> Get<T>(string resource) where T : new()
        {
            var req = new RestRequest(resource, Method.GET);
            var res = await _client.GetResponseAsync<T>(req);

            return default(T);
        }
        
    }
}
