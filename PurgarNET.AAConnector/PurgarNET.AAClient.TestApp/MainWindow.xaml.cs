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
        AAWorkflowClient wcl;
        AAUserClient cl;
        public MainWindow()
        {
            InitializeComponent();

            var sm = new SMClient("SCSM02");
            var s = sm.GetSettings();
            wcl = new AAWorkflowClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName, new Guid("767125d1-5e6c-43cc-8a8d-f68aad65ad0d"), "bulabulabula");

            cl = new AAUserClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName);
            cl.AuthorizationCodeRequired += Cl_AuthorizationCodeRequired;

            ConsoleHandler.Initialize("SCSM02");
        }



        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var r = await cl.GetRunbookAsync("TestWorkflow");

            //var r = await cl.Get<Client.Models.Token>("subscriptions");

            // for (var i = 1; i <= 10; i++)
            //{


            //var wr1 = await wcl.GetRunbooks();


/*            var runbooks = wcl.Get("Runbooks");

                var r1 = wcl.GetRunbooks();

            var rr1 = wcl.GetRunbooks();

            var r2 = cl.GetHybridRunbookWorkerGroups();

            var r3 = cl.GetJob(new Guid("c9855fda-cc29-4942-9355-131a837f8e39"));

            //}

            var r = "res";
            */

            var res = RunbookSelector.SelectRunbook();

        }


        private void Cl_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }
    }
}
