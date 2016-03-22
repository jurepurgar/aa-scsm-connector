using Microsoft.EnterpriseManagement;
using PurgarNET.AAConnector.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public class ConfigHandler : HandlerBase, INotifyPropertyChanged
    {
        private static ConfigHandler _current = null;
        private static object _lck = new object();

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


        public void Refresh()
        {
            Settings = GetSettings();
            NotifyPropertyChanged(nameof(Settings));

        }

        private void NotifyPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
