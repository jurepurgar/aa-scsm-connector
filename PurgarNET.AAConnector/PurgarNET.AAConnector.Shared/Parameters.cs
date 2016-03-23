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
        
        internal static string AUTOMATION_API_VERSION = "2015-10-31";
        internal static string AZURE_API_VERSION = "2016-02-01";
        

        internal static string AZURE_RESOURCE = "https://management.core.windows.net/";

        internal static Uri GRAPH_API_URI = new Uri("https://graph.windows.net/");

        internal static string GRAPH_API_VERSION = "1.6";

        internal static string GRAPH_RESOURCE = "https://graph.windows.net/";

        internal static Uri AACONNECTOR_APP_URI = new Uri("http://AzureAutomation.ConnectorForSCSM.local/");
        internal static string AACONNECTOR_APP_DISPLAYNAME = "Azure Automation Connector for Service Manager";

        public static Guid CLIENT_ID = new Guid("9baae959-debc-4bf2-8343-fc6efabe9d00"); //PurgarNET

        //public static Guid CLIENT_ID = new Guid("71daa18d-2e2a-4417-8903-b084e0d7ae44");
        //public static Guid CLIENT_ID = new Guid("1950a258-227b-4e31-a9cf-717495945fc2"); //PowerSHell


        public static string SECURE_REFERENCE_NAME = "PurgarNET.AAConnector.ConnectorCredential";
        public static string SECURE_REFERENCE_OVERRIDE_NAME = $"{SECURE_REFERENCE_NAME}.Override";


        public static string CONFIGMP_NAME = "PurgarNET.AAConnector.Configuration";
        public static string CONFIGMP_DISPLAYNAME = "PurgarNET Azure Automation Connector Configuration";
        public static Version CONFIGMP_VERSION = new Version(1, 0);


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
