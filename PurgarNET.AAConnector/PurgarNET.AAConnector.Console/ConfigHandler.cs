using Microsoft.EnterpriseManagement;
using PurgarNET.AAConnector.Shared;
using PurgarNET.AAConnector.Shared.ConfigClient;
using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Console
{
    public class ConfigHandler : HandlerBase, INotifyPropertyChanged
    {
        private static ConfigHandler _current = null;
        private static object _lck = new object();
        private ConfigClient _configClient = null;

        public ConfigHandler()
        {
            _configClient = new ConfigClient();
            _configClient.AuthorizationCodeRequired += _configClient_AuthorizationCodeRequired;
        }

        private void _configClient_AuthorizationCodeRequired(object sender, Shared.Client.AuthorizationCodeRequiredEventArgs e)
        {
            var code = LoginWindow.InitializeLogin(e.LoginUri);
            e.Code = code;
        }

        public static ConfigHandler Current
        {
            get
            {
                lock (_lck)
                {
                    if (_current == null)
                        _current = new ConfigHandler();
                }
                return _current;
            }
        }

        public ConnectorSettings Settings { get; set; }

        public List<AutomationAccountInfo> AvailableAutomationAccounts { get; set; }
        

        public override void Initialize(Microsoft.EnterpriseManagement.EnterpriseManagementGroup emg)
        {
            base.Initialize(emg);
            _isInitialized = true;
        }

        public override void Initialize(string serverName = null)
        {
            if (string.IsNullOrEmpty(serverName))
            {
                EnterpriseManagementGroup emg = _emg;
                if (_emg == null)
                    emg = Microsoft.EnterpriseManagement.ConsoleFramework.FrameworkServices.GetService<Microsoft.EnterpriseManagement.UI.Core.Connection.IManagementGroupSession>().ManagementGroup;
                base.Initialize(emg);
            }
            else
                base.Initialize(serverName);
        }


        public void RefreshSettings()
        {
            Settings = GetSettings();
            NotifyPropertyChanged(nameof(Settings));
        }

        public async Task RefreshAccounts()
        {
            AvailableAutomationAccounts = (await _configClient.GetAutomationAccountsAsync()).ToList();
            NotifyPropertyChanged(nameof(AvailableAutomationAccounts));
        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
