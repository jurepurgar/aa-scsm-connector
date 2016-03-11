using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.Core.Connection;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.ServiceManager;
using PurgarNET.AAConnector.Shared.ServiceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public static class ConsoleHandler
    {
        private static SMClient _smClient = null;

        private static ConnectorSettings _settings = null;
        private static AAUserClient _aaClient = null;
        //private static EnterpriseManagementGroup _emg = null;

        private static object _lck = new object();
        private static bool _isInitialized = false;

        public static SMClient SMCLient
        {
            get
            {
                CheckInitialized();
                return _smClient;
            }
        }

        public static AAUserClient AAClient
        {
            get
            {
                CheckInitialized();
                return _aaClient;
            }
        }

        public static bool Initialize(string serverName = null)
        {
            lock (_lck)
            {
                if (_smClient == null)
                { 

                    if (!string.IsNullOrEmpty(serverName)) //for testing
                    {
                        var mg = new EnterpriseManagementGroup(serverName);
                        _smClient = new SMClient(mg);
                    }
                    else
                    {
                        IManagementGroupSession session = FrameworkServices.GetService<IManagementGroupSession>();
                        var emg = session.ManagementGroup;
                        _smClient = new SMClient(emg);
                    }
                }
            }

            var s = _smClient.GetSettings();

            if (s.IsConfigured)
            {
                lock (_lck)
                {
                    if (_aaClient == null || _settings == null || !_settings.Equals(s))
                    {
                        if (_aaClient != null)
                            _aaClient.AuthorizationCodeRequired -= _aaClient_AuthorizationCodeRequired;
                        _aaClient = new AAUserClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName, s.UserAppId);
                        _aaClient.AuthorizationCodeRequired += _aaClient_AuthorizationCodeRequired;


                        _settings = s;
                        _isInitialized = true;
                    }
                }

            }
            else
            {
                //TODO: show error that AAConnector is not configured
                return false;
            }

            return true;
        }

        private static void _aaClient_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }

        public static void CheckInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("WorkflowHandler is not initialized!");
        }

        private static void GetPropertyDefinitionsForClass(ManagementPackClass mpClass)
        {
            var props = SMCLient.GetActivityPropertyDefinitions(mpClass);
            var defs = new List<PropertyDefinition>();

            foreach (var p in props)
            {
                defs.Add(new PropertyDefinition()
                {
                    DisplayName = p.DisplayName,
                    Id = $"prop:{p.Id.ToString()}"
                });
            }
            

        }

        
    }
}