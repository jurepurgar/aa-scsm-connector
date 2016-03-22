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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigHandler.Current.RefreshSettings();

        }


        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            ConnectWindow.Connect();
        }
    }
}
