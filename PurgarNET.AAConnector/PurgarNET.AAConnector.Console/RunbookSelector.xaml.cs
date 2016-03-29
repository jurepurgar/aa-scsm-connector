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
    public partial class RunbookSelector : Window
    {
        private RunbookSelector()
        {
            InitializeComponent();
            RunbooksListView.DataContext = this.Runbooks;
            ConsoleHandler.Current.Initialize();
        }

        private Runbook _selectedRunbook = null;
        private List<Runbook> _runbooks = null;
        private ObservableCollection<Runbook> Runbooks = new ObservableCollection<Runbook>();

        private async void LoadRunbooks()
        {
            this.IsEnabled = false;
            Progress.Visibility = Visibility.Visible;
            try
            {
                _runbooks = await ConsoleHandler.Current.AAClient.GetRunbooksAsync();
            }
            catch (Exception err)
            {
                MessageBox.Show("Unable to load runbooks because: " + err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
            UpdateRunbooks();
            this.IsEnabled = true;
            Progress.Visibility = Visibility.Collapsed;
        }

        private void UpdateRunbooks()
        {
            Runbooks.Clear();
            if (_runbooks != null)
            {
                var list = _runbooks;
                foreach (var i in list.OrderBy(x => x.Name))
                    Runbooks.Add(i);
            }
        }

        public static Runbook SelectRunbook()
        {
            var window = new RunbookSelector();
            window.ShowDialog();
            return window._selectedRunbook;
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedRunbook = null;
            Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseRunbook();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadRunbooks();
        }

        private void RunbooksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var isValid = RunbooksListView.SelectedValue != null && RunbooksListView.SelectedValue is Runbook && ((Runbook)RunbooksListView.SelectedValue).Properties.State != RunbookState.New;

            InvalidRunbookPanel.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            OKButton.IsEnabled = isValid;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRunbooks();
        }

        private void RunbooksListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ChooseRunbook();
        }

        private void ChooseRunbook()
        {
            if (RunbooksListView.SelectedValue != null && RunbooksListView.SelectedValue is Runbook)
            {
                _selectedRunbook = ((Runbook)RunbooksListView.SelectedValue);
            }
            Close();
        }
    }
}
