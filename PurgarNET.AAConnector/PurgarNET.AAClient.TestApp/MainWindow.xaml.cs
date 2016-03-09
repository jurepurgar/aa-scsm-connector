using PurgarNET.AAConnector.Console;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.ServiceManager;
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
        AAWorkflowClient wcl;
        public MainWindow()
        {
            InitializeComponent();

            var sm = new SMClient("SCSM02");

            var s = sm.GetSettings();

            var s1 = sm.GetSettings();

            cl = new AAUserClient(new Guid("a577f43f-f7b8-42c9-ba99-27708e35b62b"), new Guid("5c8eb01e-d914-41f1-bb3a-e7ff67fe8cbe"), "kolit-resources-prod", "kolit-automation-prod");
            cl.AuthorizationCodeRequired += Cl_AuthorizationCodeRequired;

            wcl = new AAWorkflowClient(new Guid("a577f43f-f7b8-42c9-ba99-27708e35b62b"), new Guid("5c8eb01e-d914-41f1-bb3a-e7ff67fe8cbe"), "kolit-resources-prod", "kolit-automation-prod", new Guid("bd432e3f-a25c-4d92-b2ed-bd87d7392421"), "");
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //var r = await cl.Get<Client.Models.Token>("subscriptions");

            // for (var i = 1; i <= 10; i++)
            //{

            
            //var wr1 = await wcl.GetRunbooks();




                var r1 = await cl.GetRunbooksAsync();

            var rr1 = cl.GetRunbooks();

            var r2 = await cl.GetHybridRunbookWorkerGroups();

            var r3 = await cl.GetJobAsync(new Guid("c9855fda-cc29-4942-9355-131a837f8e39"));

            //}

            var r = "res";

        }


        private void Cl_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }
    }
}
