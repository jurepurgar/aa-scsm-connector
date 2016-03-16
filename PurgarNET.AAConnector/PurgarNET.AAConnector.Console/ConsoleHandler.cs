using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.Core.Connection;
using PurgarNET.AAConnector.Shared;
using PurgarNET.AAConnector.Shared.AutomationClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    

    public class ConsoleHandler : HandlerBase
    {

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
                return (ConsoleHandler)GetCurrent(() => new ConsoleHandler());
            }
        }

        public bool Initialize(string serverName = null)
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

    }
}