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
    public enum SpecialProperty
    {
        ActivityId,
        ActivityGuid,
        ParentWorkItemId,
        ParentWorkItemGuid,
        ReletedItems,
        AffectedItems
    }

    public static class ConsoleHandler
    {
        private static SMClient _smClient = null;

        private static ConnectorSettings _settings = null;
        private static AAUserClient _aaClient = null;

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


        public static List<PropertyDefinition> GetPropertyDefinitionsForClass(Guid mpClassId)
        {
            var c = SMCLient.GetManagementPackClass(mpClassId);
            return GetPropertyDefinitionsForClass(c);
        }

        public static List<PropertyDefinition> GetPropertyDefinitionsForClass(ManagementPackClass mpClass)
        {
            var props = SMCLient.GetActivityPropertyDefinitions(mpClass);
            var defs = new List<PropertyDefinition>();

            defs.Add(new PropertyDefinition("_ActivityID (guid)", SpecialProperty.ActivityGuid.ToString(), new List<string>() { typeof(Guid).FullName, typeof(string).FullName }));
            defs.Add(new PropertyDefinition("_ActivityID", SpecialProperty.ActivityId.ToString(), new List<string>() { typeof(Guid).FullName, typeof(string).FullName }));
            defs.Add(new PropertyDefinition("_Parent WorkItem ID (guid)", SpecialProperty.ParentWorkItemGuid.ToString(), new List<string>() { typeof(Guid).FullName, typeof(string).FullName }));
            defs.Add(new PropertyDefinition("_Parent WorkItem ID", SpecialProperty.ParentWorkItemId.ToString(), new List<string>() { typeof(Guid).FullName, typeof(string).FullName }));

            defs.Add(new PropertyDefinition("_Parent WorkItem Releted Items", SpecialProperty.ReletedItems.ToString(), new List<string>() { typeof(object).FullName, typeof(object[]).FullName }));
            defs.Add(new PropertyDefinition("_Parent WorkItem Affected Items", SpecialProperty.AffectedItems.ToString(), new List<string>() { typeof(object).FullName, typeof(object[]).FullName }));

            foreach (var p in props)
            {
                Type t = null;
                switch (p.Type)
                {
                    case ManagementPackEntityPropertyTypes.datetime :
                        t = typeof(DateTime);
                        break;
                    case ManagementPackEntityPropertyTypes.@bool :
                        t = typeof(bool);
                        break;
                    case ManagementPackEntityPropertyTypes.@decimal :
                        t = typeof(Decimal);
                        break;
                    case ManagementPackEntityPropertyTypes.@double :
                        t = typeof(Double);
                        break;
                    case ManagementPackEntityPropertyTypes.@int :
                        t = typeof(int);
                        break;
                    case ManagementPackEntityPropertyTypes.guid :
                        t = typeof(Guid);
                        break;
                    default:
                        t = null;
                        break;
                }
                var vft = new List<string>();
                if (t != null)
                    vft.Add(t.FullName);
                vft.Add(typeof(string).FullName);
                vft.Add(typeof(string[]).FullName);
                vft.Add(typeof(object).FullName);
                vft.Add(typeof(object[]).FullName);

                defs.Add(new PropertyDefinition(p.Name, $"prop:{p.Id.ToString()}", vft));
            }
            return defs;
        }
    }
}