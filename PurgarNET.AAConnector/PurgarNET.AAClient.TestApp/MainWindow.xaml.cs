using PurgarNET.AAConnector.Console;
using PurgarNET.AAConnector.Shared.AutomationClient;
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

        AAUserClient cl;
        public MainWindow()
        {
            InitializeComponent();

            cl = new AAUserClient("kolektor.com", new Guid("5c8eb01e-d914-41f1-bb3a-e7ff67fe8cbe"), "kolit-resources-prod", "kolit-automation-prod", "71daa18d-2e2a-4417-8903-b084e0d7ae44");
            cl.AuthorizationCodeRequired += Cl_AuthorizationCodeRequired;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //var r = await cl.Get<Client.Models.Token>("subscriptions");

           // for (var i = 1; i <= 10; i++)
            //{
                var r1 = await cl.GetRunbooks();

            var r2 = await cl.GetHybridRunbookWorkerGroups();

            var r3 = await cl.GetJob(new Guid("7bb89564-6703-4d32-b24f-20b8ddc1b5c2"));

            //}

            var r = "res";

        }


        private void Cl_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }
    }
}
