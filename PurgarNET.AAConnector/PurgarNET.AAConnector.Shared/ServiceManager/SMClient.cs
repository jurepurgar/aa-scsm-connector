using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using PurgarNET.AAConnector.Shared.ServiceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ServiceManager
{
    public class SMClient 
    {
        private EnterpriseManagementGroup _emg = null;

        public SMClient(string serverName)
        {
            _emg = new EnterpriseManagementGroup(serverName);
        }

        public SMClient(EnterpriseManagementGroup emg)
        {
            _emg = emg;
        }

        public void KeepAlive()
        {
            if (!_emg.IsConnected)
                _emg.Reconnect();
        }

        //helpers

        private ManagementPack _libraryManagementPack = null;
        private ManagementPack LibraryManagementPack
        {
            get
            {
                if (_libraryManagementPack == null)
                    _libraryManagementPack = _emg.ManagementPacks.GetManagementPacks(new ManagementPackCriteria("Name = 'PurgarNET.AAConnector.Library'")).First();
                return _libraryManagementPack;
            }
        }

        private ManagementPackClass _connectorSettingsClass = null;
        private ManagementPackClass ConnectorSettingsClass
        {
            get
            {
                if (_connectorSettingsClass == null)
                    _connectorSettingsClass = _emg.EntityTypes.GetClass("PurgarNET.AAConnector.ConnectorSettings", LibraryManagementPack);
                return _connectorSettingsClass;
            }
        }

        public ConnectorSettings GetSettings()
        {
            var emo = _emg.EntityObjects.GetObject<EnterpriseManagementObject>(ConnectorSettingsClass.Id, ObjectQueryOptions.Default);
            var s = new ConnectorSettings();
            s.TenantId = emo.GetPropertyOrDefault<Guid>(ConnectorSettingsClass, "TenantId");
            s.SubscriptionId = emo.GetPropertyOrDefault<Guid>(ConnectorSettingsClass, "SubscriptionId");
            s.ResourceGroupName = emo.GetPropertyOrDefault<string>(ConnectorSettingsClass, "ResourceGroup");
            s.AutomationAccountName = emo.GetPropertyOrDefault<string>(ConnectorSettingsClass, "AutomationAccountName");
            s.UserAppId = emo.GetPropertyOrDefault<Guid>(ConnectorSettingsClass, "UserAppId");
            return s;
        }

        public ManagementPackEnumeration GetManagementPackEnumeration(string name)
        {
            var enums = _emg.EntityTypes.GetEnumerations(new ManagementPackEnumerationCriteria($"Name = '{name}'"));
            if (enums.Count != 1)
                throw new ObjectNotFoundException($"MP Enumeration '{name}' not found");
            return enums.First();
        }

    }
}
