using PurgarNET.AAConnector.Shared.ConfigClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PurgarNET.AAConnector.Console
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            ConfigHandler.Current.Initialize();
            DataContext = ConfigHandler.Current;
        }

        private void TogglePanel(StackPanel panel)
        {
            ProgressPanel.Visibility = Visibility.Collapsed;
            MainPanel.Visibility = Visibility.Collapsed;
            ConnectPanel.Visibility = Visibility.Collapsed;
            panel.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigHandler.Current.RefreshSettings();
            TogglePanel(MainPanel);
        }

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(ProgressPanel);
            await ConfigHandler.Current.Disconnect();
            ConfigHandler.Current.RefreshSettings();
            TogglePanel(MainPanel);
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(ProgressPanel);
            await ConfigHandler.Current.RefreshAccounts();
            TogglePanel(ConnectPanel);
        }

        private void ConnectCancelButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(MainPanel);
        }

        private async void ConnectCommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationAccountsComboBox.SelectedItem == null) return;
            TogglePanel(ProgressPanel);
            var ok = await ConfigHandler.Current.Connect((AutomationAccountInfo)AutomationAccountsComboBox.SelectedItem, new TimeSpan(365, 0, 0, 0)); //TODO: get real validity

            if (ok)
            {
                ConfigHandler.Current.RefreshSettings();
                TogglePanel(MainPanel);
            }
            else
                TogglePanel(ConnectPanel);
        }

    }
}
