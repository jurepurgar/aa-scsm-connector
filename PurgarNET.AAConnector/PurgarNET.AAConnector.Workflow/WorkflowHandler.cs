using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.ServiceManager;
using PurgarNET.AAConnector.Shared.ServiceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Workflows
{
    public static class WorkflowHandler
    {
        private static SMClient _smClient = null;

        private static ConnectorSettings _settings = null;
        private static AAWorkflowClient _aaClient = null;
        private static Guid _clientId = default(Guid);
        private static string _clientSecret = null;

        private static object _lck = new object();

        private static bool _isInitialized = false;

        public static bool Initialize(string clientIdStr, string clientSecret)
        {
            Guid clientId;
            try
            {
                clientId = new Guid(clientIdStr);
            }
            catch (Exception e)
            {
                //TODO: write event log that clientId is not guid
                throw e;
            }
            
            lock (_lck)
            {
                if (_smClient == null)
                    _smClient = new SMClient("localhost");

                _smClient.KeepAlive();
            }

            var s = _smClient.GetSettings();

            if (s.IsConfigured)
            {
                lock (_lck)
                {
                    if (_aaClient == null || _settings == null || !_settings.Equals(s) || _clientId != clientId || _clientSecret != clientSecret)
                    { 
                        _aaClient = new AAWorkflowClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName, clientId, clientSecret);
                        _clientId = clientId;
                        _clientSecret = clientSecret;
                        _settings = s;
                        _isInitialized = true;
                    }
                }
                
            }
            else
            {
                //TODO: write to event log that connector is not configured
                return false;
            }
            
            return true;
        }

        public static void CheckInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("WorkflowHandler is not initialized!");
        }

        public static void ProcessActivities()
        {
            CheckInitialized();


            using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"C:\Windows\Temp\TestWorkflow.txt", true))
            {
                outputFile.WriteLine(" ");
                outputFile.WriteLine("Date: " + DateTime.Now.ToString());
               
                outputFile.WriteLine("Id: " + _clientId.ToString());
                outputFile.WriteLine("Secret: " + _clientSecret);

                outputFile.WriteLine(" ");
            }


            var rt = _aaClient.GetRunbooksAsync();
            var runbooks = rt.Result;
            string str = string.Empty;
            foreach (var r in runbooks)
                str += "\n\r" + r.Name;

            using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"C:\Windows\Temp\TestWorkflowRunbooks.txt", true))
            {
                outputFile.WriteLine(" ");
                outputFile.WriteLine("Date: " + DateTime.Now.ToString());
                outputFile.WriteLine("Runbooks:");
                
                outputFile.WriteLine(runbooks);

                //outputFile.WriteLine("Id: " + ClientId);
                //outputFile.WriteLine("Secret: " + ClientSecret);

                outputFile.WriteLine(" ");
            }

            

        }

    }
}
