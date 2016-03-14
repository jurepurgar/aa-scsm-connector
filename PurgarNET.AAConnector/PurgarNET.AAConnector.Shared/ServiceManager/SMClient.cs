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
        public ManagementPack LibraryManagementPack
        {
            get
            {
                if (_libraryManagementPack == null)
                    _libraryManagementPack = _emg.ManagementPacks.GetManagementPacks(new ManagementPackCriteria("Name = 'PurgarNET.AAConnector.Library'")).First();
                return _libraryManagementPack;
            }
        }

        private ManagementPackClass _connectorSettingsClass = null;
        public ManagementPackClass ConnectorSettingsClass
        {
            get
            {
                if (_connectorSettingsClass == null)
                    _connectorSettingsClass = GetManagementPackClass("PurgarNET.AAConnector.ConnectorSettings", LibraryManagementPack);
                return _connectorSettingsClass;
            }
        }

        private ManagementPackClass _activityClass = null;
        public ManagementPackClass ActivityClass
        {
            get
            {
                if (_activityClass == null)
                    _activityClass = GetManagementPackClass("PurgarNET.AAConnector.RunbookActivity", LibraryManagementPack);
                return _activityClass;
            }
        }

        private ManagementPackRelationship _workItemContainsActivityRelationship = null;
        public ManagementPackRelationship WorkItemContainsActivityRelationship
        {
            get
            {
                if (_workItemContainsActivityRelationship == null)
                    _workItemContainsActivityRelationship = _emg.EntityTypes.GetRelationshipClasses(new ManagementPackRelationshipCriteria("Name = 'System.WorkItemContainsActivity'")).First();
                return _workItemContainsActivityRelationship;
            }
        }


        private ManagementPackRelationship _relatedItemRelationship = null;
        public ManagementPackRelationship RelatedItemRelationship
        {
            get
            {
                if (_relatedItemRelationship == null)
                    _relatedItemRelationship = _emg.EntityTypes.GetRelationshipClasses(new ManagementPackRelationshipCriteria("Name = 'System.WorkItemRelatesToConfigItem'")).First();
                return _relatedItemRelationship;
            }
        }

        private ManagementPackRelationship _affectedItemRelationship = null;
        public ManagementPackRelationship AffectedItemRelationship
        {
            get
            {
                if (_affectedItemRelationship == null)
                    _affectedItemRelationship = _emg.EntityTypes.GetRelationshipClasses(new ManagementPackRelationshipCriteria("Name = 'System.WorkItemAboutConfigItem'")).First();
                return _affectedItemRelationship;
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

        public ManagementPackClass GetManagementPackClass(string Name, ManagementPack mp)
        {
            return _emg.EntityTypes.GetClass(Name, mp);
        }

        public ManagementPackClass GetManagementPackClass(Guid id)
        {
            return _emg.EntityTypes.GetClass(id);
        }

        public ManagementPackEnumeration GetManagementPackEnumeration(string name)
        {
            var enums = _emg.EntityTypes.GetEnumerations(new ManagementPackEnumerationCriteria($"Name = '{name}'"));
            if (enums.Count != 1)
                throw new ObjectNotFoundException($"MP Enumeration '{name}' not found");
            return enums.First();
        }

        public IEnumerable<ManagementPackProperty> GetActivityPropertyDefinitions(ManagementPackClass c)
        {
            if (c.Id != ActivityClass.Id && c.GetBaseTypes().FirstOrDefault(x => x.Id == ActivityClass.Id) == default(ManagementPackType))
                throw new InvalidOperationException("Activity type is not deriven from 'PurgarNET.AAConnector.RunbookActivity'");
            return c.GetProperties();
        }

        public EnterpriseManagementObject GetActivityObject(Guid activityId)
        {
            return _emg.EntityObjects.GetObject<EnterpriseManagementObject>(activityId, ObjectQueryOptions.Default);
        }

    }
}
