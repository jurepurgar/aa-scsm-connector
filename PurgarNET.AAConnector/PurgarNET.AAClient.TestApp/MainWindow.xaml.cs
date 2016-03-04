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
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var code = LoginWindow.InitializeLogin();

            var cl = new AAUserClient();
            cl.SetAuthorizationCode(code);

            await cl.InitializeAsync(default(Guid));

            var r = await cl.Get<Client.Models.Token>("subscriptions");

            var b = "res";

       
        }

    }
}
