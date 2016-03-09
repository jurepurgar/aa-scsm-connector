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
        //public static Uri GRAPH_API_URI = new Uri("https://graph.windows.net/");

        internal static string AZURE_API_VERSION = "2015-10-31";
        //public static string GRAPH_API_VERSION = "1.5";

        internal static string AZURE_RESOURCE = "https://management.core.windows.net/";
        //public static string GRAPH_RESOURCE = "https://graph.windows.net/";

        //public static string POWERSHELL_CLIENT_ID = "1950a258-227b-4e31-a9cf-717495945fc2";
        

        internal static string USER_LOGIN_URL = "https://login.windows.net/{0}/oauth2/authorize?resource={1}&client_id={2}&response_type=code&redirect_uri={3}&display=popup&site_id=501358";

        internal static string TOKEN_URL = "https://login.windows.net/{0}/oauth2/token?api-version=1.0";


        /*public static Uri GetUserLoginUri(Guid tenantId)
        {
            var tenant = "common";
            if (tenantId != default(Guid))
                tenant = tenantId.ToString();
            return new Uri(string.Format(USER_LOGIN_URL, tenant, GetUrlEncodedResource(), POWERSHELL_CLIENT_ID, REDIRECT_URI.ToString()));
        } */

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

        internal static string GetUrlEncodedResource()
        {
            return Uri.EscapeDataString(AZURE_RESOURCE);
        }
    }
}
