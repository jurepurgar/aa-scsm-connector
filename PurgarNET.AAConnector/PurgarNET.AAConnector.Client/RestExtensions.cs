using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Client
{
    public static class RestExtensions
    {

        public static Task<IRestResponse> GetResponseAsync(this RestClient client, IRestRequest request)
        {
            var tsc = new TaskCompletionSource<IRestResponse>();

            var handle = client.ExecuteAsync(request, (response) =>
            {
                tsc.SetResult(response);
            });

            return tsc.Task;
        }

        public static Task<IRestResponse<T>> GetResponseAsync<T>(this RestClient client, IRestRequest request) where T : new()
        {
            var tsc = new TaskCompletionSource<IRestResponse<T>>();

            var handle = client.ExecuteAsync<T>(request, (response) =>
            {
                if (response.ErrorException != null)
                    throw response.ErrorException;

                tsc.SetResult(response);
            });

            return tsc.Task;
        }




    }
}
