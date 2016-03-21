using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    public static class Parameters
    {
        public static Uri REDIRECT_URI = new Uri("urn:ietf:wg:oauth:2.0:oob");
        internal static Uri AZURE_API_URI = new Uri("https://management.azure.com/");
        
        internal static string AZURE_API_VERSION = "2015-10-31";

        internal static string AZURE_RESOURCE = "https://management.core.windows.net/";

        internal static Uri GRAPH_API_URI = new Uri("https://graph.windows.net/");

        internal static string GRAPH_API_VERSION = "1.6";

        internal static string GRAPH_RESOURCE = "https://graph.windows.net/";



        public static Guid CLIENT_ID = new Guid("9baae959-debc-4bf2-8343-fc6efabe9d00");

        //public static Guid CLIENT_ID = new Guid("71daa18d-2e2a-4417-8903-b084e0d7ae44");




        internal static string USER_LOGIN_URL = "https://login.windows.net/{0}/oauth2/authorize?resource={1}&client_id={2}&response_type=code&redirect_uri={3}&display=popup&site_id=501358";

        internal static string TOKEN_URL = "https://login.windows.net/{0}/oauth2/token?api-version=1.0";


        internal static Uri GetTokenUri(Guid tenantId)
        {
            var tenantStr = tenantId.ToString();
            if (tenantId == default(Guid))
                tenantStr = null;
                
            return GetTokenUri(tenantStr);
        }

        internal static Uri GetTokenUri(string tenant = null)
        {
            if (string.IsNullOrEmpty(tenant))
                tenant = "common";
            return new Uri(string.Format(TOKEN_URL, tenant));
        }

        
    }
}
