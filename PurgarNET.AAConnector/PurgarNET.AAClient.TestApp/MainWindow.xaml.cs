using PurgarNET.AAConnector.Console;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
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


        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //*var w = new Window1();
            //w.Show();

            var g = new Guid("a25bfae4-c469-98d1-3d7b-774287e28d0d");

            var j = ConsoleHandler.Current.CreateStartRunbookJob(g);

            var runbooks = await ConsoleHandler.Current.AAClient.GetHybridRunbookWorkerGroupsAsync();

            var startedJob = await ConsoleHandler.Current.AAClient.StartJob(j);


            var b = startedJob;


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


        private void Cl_AuthorizationCodeRequired(object sender, AuthorizationCodeRequiredEventArgs e)
        {
            e.Code = LoginWindow.InitializeLogin(e.LoginUri);
        }
    }
}
