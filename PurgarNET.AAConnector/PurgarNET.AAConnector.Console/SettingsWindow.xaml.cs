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
            RenewPanel.Visibility = Visibility.Collapsed;
            SettingsPanel.Visibility = Visibility.Collapsed;

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

        private void RenewButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(RenewPanel);
        }

        private async void ConnectCommitButton_Click(object sender, RoutedEventArgs e)
        {
            if (AutomationAccountsComboBox.SelectedItem == null) return;
            var validity = ConfigHandler.ValidityToTimeSpan((string)ValidityComboBox.SelectedValue);
            TogglePanel(ProgressPanel);
            var ok = await ConfigHandler.Current.Connect((AutomationAccountInfo)AutomationAccountsComboBox.SelectedItem, validity);

            if (ok)
            {
                ConfigHandler.Current.RefreshSettings();
                TogglePanel(MainPanel);
            }
            else
                TogglePanel(ConnectPanel);
        }

        private async void RenewCommitButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(ProgressPanel);
            var validity = ConfigHandler.ValidityToTimeSpan((string)RenewValidityComboBox.SelectedValue);
            await ConfigHandler.Current.RenewServiceCredential(validity);
            TogglePanel(MainPanel);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(MainPanel);
        }

        private void ModifySettingsButton_Click(object sender, RoutedEventArgs e)
        {
            TogglePanel(SettingsPanel);
        }

        private void SettingsCommitButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigHandler.Current.CommitSettings();
            TogglePanel(MainPanel);
        }

        private void SelectRunOnButton_Click(object sender, RoutedEventArgs e)
        {
            ConsoleHandler.Current.Initialize();
            var r = RunOnSelector.SelectRunOn(RunOnTextBox.Text, false);
            if (r != null)
                RunOnTextBox.Text = r;
        }
    }
}
