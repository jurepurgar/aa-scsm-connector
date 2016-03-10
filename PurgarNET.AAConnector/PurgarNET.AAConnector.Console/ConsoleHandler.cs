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
        private static EnterpriseManagementGroup _emg = null;

        private static object _lck = new object();
        private static bool _isInitialized = false;

        public static bool Initialize()
        {
            lock (_lck)
            {
                IManagementGroupSession session = FrameworkServices.GetService<IManagementGroupSession>();
                _emg = session.ManagementGroup;
            }

            var s = _smClient.GetSettings();

            if (s.IsConfigured)
            {
                lock (_lck)
                {
                    if (_aaClient == null || _settings == null || !_settings.Equals(s))
                    {
                        _aaClient = new AAUserClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName, s.UserAppId);
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

        public static void CheckInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("WorkflowHandler is not initialized!");
        }

        public static ManagementPackEnumeration GetManagementPackEnumeration(string name)
        {
            CheckInitialized();
            var enums = _emg.EntityTypes.GetEnumerations(new ManagementPackEnumerationCriteria($"Name = '{name}'"));
            if (enums.Count != 1)
                throw new ObjectNotFoundException($"MP Enumeration '{name}' not found");
            return enums.First();
        }
    }
}