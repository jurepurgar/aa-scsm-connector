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
            RunOnsComboBox.DataContext = RunOns;
            ConsoleHandler.Current.Initialize();
        }

        private string _selectedRunOn = null;
        private ObservableCollection<string> RunOns = new ObservableCollection<string>();
        private bool _allowDefault = false;

        private async void LoadRunOns()
        {
            this.IsEnabled = false;
            RunOns.Clear();
            try
            {
                var items = await ConsoleHandler.Current.AAClient.GetHybridRunbookWorkerGroupsAsync();
                if (_allowDefault)
                    RunOns.Add("Default");
                RunOns.Add("Azure");
                foreach (var i in items.OrderBy(x => x.Name))
                    RunOns.Add(i.Name);


                var selected = RunOns.FirstOrDefault(x => x == _selectedRunOn);
                if (!string.IsNullOrEmpty(selected))
                    RunOnsComboBox.SelectedValue = selected;
                else
                    RunOnsComboBox.SelectedValue = RunOns.First();
            }
            catch (Exception err)
            {
                MessageBox.Show("Unable to load runbooks because: " + err.Message);

            }
            this.IsEnabled = true;
        }

        
        public static string SelectRunOn(string current, bool allowDefault)
        {
            var window = new RunOnSelector();
            window._allowDefault = allowDefault;
            window._selectedRunOn = current;
            window.RunOnsComboBox.SelectedValue = current;
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
            if (RunOnsComboBox.SelectedValue != null)
                _selectedRunOn = ((string)RunOnsComboBox.SelectedValue);
            Close();
        }

       /* private void RunOnsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChooseRunbook();
        } */

        private void RunOnsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OKButton.IsEnabled = (RunOnsComboBox.SelectedItem != null);  
        }
    }
}
