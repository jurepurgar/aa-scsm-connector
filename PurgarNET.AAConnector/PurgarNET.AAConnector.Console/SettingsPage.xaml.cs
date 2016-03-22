using Microsoft.EnterpriseManagement.UI.WpfWizardFramework;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : WizardRegularPageBase
    {
        /*public SettingsPage()
        {
            InitializeComponent();
        } */

        //private AdminSettingWizardData adminSettingWizardData = null;


        public SettingsPage(WizardData wizardData)
        {
            InitializeComponent();

//            this.DataContext = wizardData;
//            this.adminSettingWizardData = this.DataContext as AdminSettingWizardData;
            ConfigHandler.Current.Initialize();
            DataContext = ConfigHandler.Current;
        }

        
        private void WizardRegularPageBase_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigHandler.Current.Refresh();

        }


    }



}
