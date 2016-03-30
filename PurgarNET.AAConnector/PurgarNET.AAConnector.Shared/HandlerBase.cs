using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using PurgarNET.AAConnector.Shared.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared
{
    public abstract class HandlerBase 
    {
        private static object _lck = new object();
        
        protected EnterpriseManagementGroup _emg = null;
        protected bool _isInitialized = false;


        public virtual void Initialize(string ServerName)
        {
            EnterpriseManagementGroup emg;
            if (_emg != null && _emg.ConnectionSettings.ServerName == ServerName)
                emg = _emg;
            else
                emg = new EnterpriseManagementGroup(ServerName);
            
            Initialize(emg);
        }    

        public virtual void Initialize(EnterpriseManagementGroup emg)
        {
            lock (_lck)
            {
                if (_emg == null || _emg.ConnectionSettings.ServerName != emg.ConnectionSettings.ServerName)
                {
                    _emg = emg;
                    if (!_emg.IsConnected)
                        _emg.Reconnect();
                }
            }
        }

        protected void CheckInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Handler is not initialized!");
        }

        //management packs
        private ManagementPack _libraryManagementPack = null;
        public ManagementPack LibraryManagementPack
        {
            get
            {
                if (_libraryManagementPack == null)
                    _libraryManagementPack = GetManagementPack("PurgarNET.AAConnector.Library");
                return _libraryManagementPack;
            }
        }

        private ManagementPack _systemCenterManagementPack = null;
        public ManagementPack SystemCenterManagementPack
        {
            get
            {
                if (_systemCenterManagementPack == null)
                    _systemCenterManagementPack = GetManagementPack("Microsoft.SystemCenter.Library");
                return _systemCenterManagementPack;
            }
            
        }

        private ManagementPack _workItemManagementPack = null;
        public ManagementPack WorkItemManagementPack
        {
            get
            {
                if (_workItemManagementPack == null)
                    _workItemManagementPack = GetManagementPack("System.WorkItem.Library");
                return _workItemManagementPack;
            }
        }

        private ManagementPack _activityManagementPack = null;
        public ManagementPack ActivityManagementPack
        {
            get
            {
                if (_activityManagementPack == null)
                    _activityManagementPack = GetManagementPack("System.WorkItem.Activity.Library");
                return _activityManagementPack;
            }
        }

        private ManagementPack _systemManagementPack = null;
        public ManagementPack SystemManagementPack
        {
            get
            {
                if (_systemManagementPack == null)
                    _systemManagementPack = GetManagementPack("System.Library");
                return _systemManagementPack;
            }
        }

        //classes

        private ManagementPackClass _entityClass = null;
        public ManagementPackClass EntityClass
        {
            get
            {
                if (_entityClass == null)
                    _entityClass = GetManagementPackClass("System.Entity", SystemManagementPack);
                return _entityClass;
            }
        }

        private ManagementPackClass _subscriptionWorkflowTargetClass = null;
        public ManagementPackClass SubscriptionWorkflowTargetClass
        {
            get
            {
                if (_subscriptionWorkflowTargetClass == null)
                    _subscriptionWorkflowTargetClass = GetManagementPackClass("Microsoft.SystemCenter.SubscriptionWorkflowTarget", SystemCenterManagementPack);
                return _subscriptionWorkflowTargetClass;
            }
            
        }

        

        private ManagementPackClass _workItemClass = null;
        public ManagementPackClass WorkItemClass
        {
            get
            {
                if (_workItemClass == null)
                    _workItemClass = GetManagementPackClass("System.WorkItem", WorkItemManagementPack);
                return _workItemClass;
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

        //SCSM relationships
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

        //secure references
        private ManagementPackSecureReference _connectorSecureReference = null;
        public ManagementPackSecureReference ConnectorSecureReference
        {
            get
            {
                if (_connectorSecureReference == null)
                    _connectorSecureReference = _emg.Security.GetSecureReferences(new ManagementPackSecureReferenceCriteria($"Name = '{Parameters.SECURE_REFERENCE_NAME}'")).First();
                return _connectorSecureReference;
            }
        }

        //enums
        private  ManagementPackEnumeration _activityInProgressEnum = null;
        public ManagementPackEnumeration ActivityInProgressEnum
        {
            get
            {
                if (_activityInProgressEnum == null)
                    _activityInProgressEnum = GetActivityStatusEnum("ActivityStatusEnum.Active");
                return _activityInProgressEnum;
            }
        }

        private ManagementPackEnumeration _activityCompletedEnum = null;
        public ManagementPackEnumeration ActivityCompletedEnum
        {
            get
            {
                if (_activityCompletedEnum == null)
                    _activityCompletedEnum = GetActivityStatusEnum("ActivityStatusEnum.Completed");
                return _activityCompletedEnum;
            }
        }

        private ManagementPackEnumeration _activityOnHoldEnum = null;
        public ManagementPackEnumeration ActivityOnHoldEnum
        {
            get
            {
                if (_activityOnHoldEnum == null)
                    _activityOnHoldEnum = GetActivityStatusEnum("ActivityStatusEnum.OnHold");
                return _activityOnHoldEnum;
            }
        }

        private ManagementPackEnumeration _activityFailedEnum = null;
        public ManagementPackEnumeration ActivityFailedEnum
        {
            get
            {
                if (_activityFailedEnum == null)
                    _activityFailedEnum = GetActivityStatusEnum("ActivityStatusEnum.Failed");
                return _activityFailedEnum;
            }
        }

        protected ConnectorSettings GetSettings(bool getSecData = false)
        {
            var emo = _emg.EntityObjects.GetObject<EnterpriseManagementObject>(ConnectorSettingsClass.Id, ObjectQueryOptions.Default);
            var s = new ConnectorSettings();
            s.TenantId = emo.GetPropertyOrDefault<Guid>(ConnectorSettingsClass, "TenantId");
            s.SubscriptionId = emo.GetPropertyOrDefault<Guid>(ConnectorSettingsClass, "SubscriptionId");
            s.ResourceGroupName = emo.GetPropertyOrDefault<string>(ConnectorSettingsClass, "ResourceGroup");
            s.AutomationAccountName = emo.GetPropertyOrDefault<string>(ConnectorSettingsClass, "AutomationAccountName");
            s.UserAppId = emo.GetPropertyOrDefault<Guid>(ConnectorSettingsClass, "UserAppId");
            s.LastChanged = emo.GetPropertyOrDefault<DateTime>(ConnectorSettingsClass, "LastChanged");
            s.CredentialExpirationDate = emo.GetPropertyOrDefault<DateTime>(ConnectorSettingsClass, "CredentialExpirationDate");
            s.DefaultRunOn = emo.GetPropertyOrDefault<string>(ConnectorSettingsClass, "DefaultRunOn");

            if (getSecData)
            { 
                var secData = (Microsoft.EnterpriseManagement.Security.BasicCredentialSecureData)_emg.Security.GetSecureData(new SecureDataCriteria($"Name = '{Parameters.SECURE_REFERENCE_NAME}'")).FirstOrDefault();
                if (secData != null)
                    s.WorkflowAppId = new Guid(secData.UserName);
            }
            return s;
        }

        protected void ClearSettings()
        {
            var emo = _emg.EntityObjects.GetObject<EnterpriseManagementObject>(ConnectorSettingsClass.Id, ObjectQueryOptions.Default);
            emo.ClearProperty(ConnectorSettingsClass, "TenantId");
            emo.ClearProperty(ConnectorSettingsClass, "SubscriptionId");
            emo.ClearProperty(ConnectorSettingsClass, "ResourceGroup");
            emo.ClearProperty(ConnectorSettingsClass, "AutomationAccountName");
            emo.ClearProperty(ConnectorSettingsClass, "CredentialExpirationDate");

            emo.SetProperty(ConnectorSettingsClass, "DefaultRunOn", "Azure");

            emo.SetProperty(ConnectorSettingsClass, "LastChanged", DateTime.UtcNow);
            
            emo.Overwrite();
        }

        protected void SaveSettings(ConnectorSettings s)
        {
            var emo = _emg.EntityObjects.GetObject<EnterpriseManagementObject>(ConnectorSettingsClass.Id, ObjectQueryOptions.Default);
            emo.SetProperty(ConnectorSettingsClass, "TenantId", s.TenantId);
            emo.SetProperty(ConnectorSettingsClass, "SubscriptionId", s.SubscriptionId);
            emo.SetProperty(ConnectorSettingsClass, "ResourceGroup", s.ResourceGroupName);
            emo.SetProperty(ConnectorSettingsClass, "AutomationAccountName", s.AutomationAccountName);
            emo.SetProperty(ConnectorSettingsClass, "UserAppId", s.UserAppId);
            emo.SetProperty(ConnectorSettingsClass, "CredentialExpirationDate", s.CredentialExpirationDate);
            emo.SetProperty(ConnectorSettingsClass, "DefaultRunOn", s.DefaultRunOn);

            emo.SetProperty(ConnectorSettingsClass, "LastChanged", DateTime.UtcNow);

            emo.Overwrite();
        }

        public ManagementPackEnumeration GetActivityStatusEnum(string name)
        {
            return _emg.EntityTypes.GetEnumeration(name, ActivityManagementPack);
        }

        public ManagementPack GetManagementPack(string name)
        {
            return _emg.ManagementPacks.GetManagementPacks(new ManagementPackCriteria($"Name = '{name}'")).First();
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

        
        
    }

    /*public class ConsoleHandler : HandlerBase
    {
        public static ConsoleHandler Current
        {
            get
            {
                return (ConsoleHandler)GetCurrent(() => new ConsoleHandler());
            }
        }
    } */

    
}
