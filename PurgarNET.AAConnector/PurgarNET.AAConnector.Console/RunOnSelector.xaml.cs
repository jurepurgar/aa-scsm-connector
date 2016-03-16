using PurgarNET.AAConnector.Shared.AutomationClient.Models;
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
    public partial class RunOnSelector : Window
    {
        private RunOnSelector()
        {
            InitializeComponent();
            RunOnsListView.DataContext = this.RunOns;
            ConsoleHandler.Current.Initialize();
        }

        private string _selectedRunOn = null;
        private ObservableCollection<string> RunOns = new ObservableCollection<string>();

        private async void LoadRunOns()
        {
            this.IsEnabled = false;
            RunOns.Clear();
            try
            {
                var items = await ConsoleHandler.Current.AAClient.GetHybridRunbookWorkerGroupsAsync();
                RunOns.Add("Azure");
                foreach (var i in items.OrderBy(x => x.Name))
                    RunOns.Add(i.Name);
            }
            catch (Exception err)
            {
                MessageBox.Show("Unable to load runbooks because: " + err.Message);

            }

            this.IsEnabled = true;
        }

        
        public static string SelectRunOn()
        {
            var window = new RunOnSelector();
            window.ShowDialog();
            return window._selectedRunOn;
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedRunOn = null;
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseRunbook();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRunOns();
        }


        private void ChooseRunbook()
        {
            if (RunOnsListView.SelectedValue != null)
            {
                _selectedRunOn = ((string)RunOnsListView.SelectedValue);
            }
            Close();
        }

        private void RunOnsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChooseRunbook();
        }

        private void RunOnsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OKButton.IsEnabled = (RunOnsListView.SelectedItem != null);  
        }
    }
}
