using PurgarNET.AAConnector.Console;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using PurgarNET.AAConnector.Shared.Client;
using PurgarNET.AAConnector.Shared.ConfigClient;
using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace PurgarNET.AAConnector.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ConsoleHandler.Current.Initialize("SCSM02");
            ConfigHandler.Current.Initialize("SCSM02");

            _configClient = new ConfigClient();
            _configClient.AuthorizationCodeRequired += _graphClient_AuthorizationCodeRequired;
        }

        private void _graphClient_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }

       
        private ConfigClient _configClient = null;

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //var w = new Window1();
            //w.Show();

            var sw = new SettingsWindow();
            sw.Show();


        }

        private async void WorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigHandler.Current.RefreshSettings();
            var s = ConfigHandler.Current.Settings;

            var cl = new AAClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName, AuthenticationType.ClientSecret, new Guid("75d12a6b-0fb7-4fd1-bc51-f25c02c79e49"), "XOP5n#C1VPs4kwl9#I$Tw$f_qG7;nS");
            var t1 = cl.GetRunbooksAsync();
            var t2 = cl.GetRunbooksAsync();

            var r1 = await t1;
            var r2 = await t2;



        }
    }
}
