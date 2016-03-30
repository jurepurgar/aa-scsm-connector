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
 
            await ConfigHandler.Current.RefreshAccounts();

        }

        private void WorkflowButton_Click(object sender, RoutedEventArgs e)
        {
            

        }
    }
}
