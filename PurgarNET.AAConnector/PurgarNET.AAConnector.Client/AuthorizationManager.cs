using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace PurgarNET.AAConnector.Client
{
    public class AuthorizationManager
    {
        private static object _lck = new object();
        private static object _clientLck = new object();

        private static WebClient _tokenClient = null;
        private static WebClient tokenClient {
            get
            {
                lock (_clientLck)
                {
                    if (_tokenClient == null)
                    {
                        _tokenClient = new WebClient();
                        _tokenClient.Headers.Add("Accept", "application/json;odata=verbose;charset=utf-8");
                    }
                    return _tokenClient;
                }
            }
        }       

        
        




    }



}
