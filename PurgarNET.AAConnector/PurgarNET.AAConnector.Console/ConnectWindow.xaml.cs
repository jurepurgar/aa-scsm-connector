using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PurgarNET.AAConnector.Console
{
    /// <summary>
    /// Interaction logic for RunbookSelector.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        private ConnectWindow()
        {
            InitializeComponent();
            ConfigHandler.Current.Initialize();
            DataContext = ConfigHandler.Current;
        }

        public static void Connect()
        {
            var w = new ConnectWindow();
            w.ShowDialog();
        }

        private ObservableCollection<AutomationAccountInfo> AutomationAccounts = new ObservableCollection<AutomationAccountInfo>();

        private async void LoadAutomationAccounts()
        {
            this.IsEnabled = false;
            try
            {
                await ConfigHandler.Current.RefreshAccounts();
            }
            catch (Exception err)
            {
                MessageBox.Show("Unable to get automation accounts because: " + err.Message);

            }
            this.IsEnabled = true;
        }

        private async void CheckSelectedAutomationAccount()
        {

        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAutomationAccounts();
        }

        
        private void AutomationAccountsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OKButton.IsEnabled = (AutomationAccountsComboBox.SelectedItem != null);
        }
    }
}
