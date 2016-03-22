using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.Core.Connection;
using Microsoft.EnterpriseManagement.UI.DataModel;
using PurgarNET.AAConnector.Shared;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    

    public class ConsoleHandler : AAHandlerBase
    {
        private static ConsoleHandler _current = null;
        private static object _lck = new object();
        public AAClient AAClient
        {
            get
            {
                CheckInitialized();
                return _aaClient;
            }
        }

        public static ConsoleHandler Current
        {
            get
            {
                lock (_lck)
                {
                    if (_current == null)
                        _current = new ConsoleHandler();
                }
                return _current;
            }
        }

        public new bool Initialize(string serverName = null)
        {

            if (string.IsNullOrEmpty(serverName)) 
            {
                EnterpriseManagementGroup emg = _emg;
                if (_emg == null)
                    emg = FrameworkServices.GetService<IManagementGroupSession>().ManagementGroup;
                return base.Initialize(emg, AuthenticationType.Code, AuthorizationCodeRequired);
            }
            else 
                return base.Initialize(serverName, AuthenticationType.Code, AuthorizationCodeRequired);            
        }

        private void AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }

        
        public List<PropertyDefinition> GetPropertyDefinitionsForClass(Guid mpClassId)
        {
            var c = GetManagementPackClass(mpClassId);
            return PropertyDefinitions.CreateForClass(c);
        }


        public string GetPropertyFormNavModel(IList<NavigationModelNodeBase> nodes, string propertyName)
        {
            var dataItem = GetFormDataContext(nodes);
            if(dataItem != null)
                return (dataItem[propertyName] as string);
            return null;
        }

        public IDataItem GetFormDataContext(IList<NavigationModelNodeBase> nodes)
        {
            foreach (NavigationModelNodeBase node in nodes)
            {
                if (Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.IsNodeWithinForm(nodes[0]))
                    return Microsoft.EnterpriseManagement.GenericForm.FormUtilities.Instance.GetFormDataContext(node);
            }
            return null;
        }

        public void NavigateToPortal(string resource)
        {
            var uri = $"https://portal.azure.com/#resource/subscriptions/{_settings.SubscriptionId}/resourceGroups/{_settings.ResourceGroupName}/providers/Microsoft.Automation/automationAccounts/{_settings.AutomationAccountName}/{resource}";
            System.Diagnostics.Process.Start(uri);
        }

    }
}