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
            

          /*  var g = new Guid("a25bfae4-c469-98d1-3d7b-774287e28d0a");

            //  var js = await ConsoleHandler.Current.AAClient.GetJobsAsync();

            var automationAccounts = await _configClient.GetAutomationAccountsAsync();
            var tId = automationAccounts.First().TenantId;

            var graphClient = new GraphClient(tId);
            graphClient.AuthorizationCodeRequired += _graphClient_AuthorizationCodeRequired;


            var app = await graphClient.GetApplicationAsync();
            if (app == null)
                app = await graphClient.CreateApplicationAsync();


            var passCred = app.PasswordCredentials.FirstOrDefault(x => x.CustomKeyIdentifier == Convert.ToBase64String(g.ToByteArray()));
            if (passCred != null)
            {
                //remove
                app.PasswordCredentials.Remove(passCred);
                await graphClient.UpdateApplicationAsync(app);
                app = await graphClient.GetApplicationAsync();
            }

            app.PasswordCredentials.Add(new PasswordCredential()
            {
                EndDate = DateTime.UtcNow.AddDays(365),
                StartDate = DateTime.UtcNow,
                KeyId = Guid.NewGuid(),
                Value = "BulaBula1A",
                CustomKeyIdentifier = Convert.ToBase64String(g.ToByteArray())
                
            });

            await graphClient.UpdateApplicationAsync(app);

            app = await graphClient.GetApplicationAsync();



            //  var j = await ConsoleHandler.Current.AAClient.GetJobAsync(new Guid("7cd3c5b3-b95b-46ec-a9a4-a88c607610c4"));

            //var s = await ConsoleHandler.Current.AAClient.GetJobOutput(new Guid("5fa0add0-1b42-41cf-a581-d52ab1f11628"));


            var b = "bula";


            /* var r = await cl.GetRunbookAsync("TestWorkflow");

             var j = new Job();
             j.Properties.Runbook.Name = "TestWorkflow";
             j.Properties.RunOn = "";
             j.Properties.Parameters.Add("DateTime1", DateTime.Now.ToString("G"));
             j.Properties.Parameters.Add("Bool1", "true");
             j.Properties.Parameters.Add("Int1", 1);
             j.Properties.Parameters.Add("String1", "string");
             j.Properties.Parameters.Add("StringArr1", "[\"bula1\", \"bula2\"]");


             var jr = await cl.StartJob(j); */

        }


    }
}
