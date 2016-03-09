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

        public static Guid CLIENT_ID = new Guid("444b22df-0f28-4ff6-831f-039c6b8565dc");
        

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

        internal static string GetUrlEncodedResource()
        {
            return Uri.EscapeDataString(AZURE_RESOURCE);
        }
    }
}
