using PurgarNET.AAConnector.ConsoleControls;
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
using RestSharp;
using PurgarNET.AAConnector.Client;

namespace PurgarNET.AAConnector.TestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AAConfigClient cl;
        public MainWindow()
        {
            InitializeComponent();

            cl = new AAConfigClient();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            
            cl.AuthorizationCodeRequired += Cl_AuthorizationCodeRequired;

            //var r = await cl.Get<Client.Models.Token>("subscriptions");

            for (var i = 1; i <= 10; i++)
            {
                var r1 = await cl.GetTenants();

                var r2 = await cl.GetSubscriptions();

                var s = r2.First();

                var r3 = await cl.GetAutomationAccounts(s.SubscriptionId);

            }

            var r = "res";

       
        }

        private void Cl_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin();
        }
    }
}
