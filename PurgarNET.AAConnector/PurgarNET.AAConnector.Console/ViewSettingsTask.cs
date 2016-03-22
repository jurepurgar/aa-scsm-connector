using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using Microsoft.EnterpriseManagement.UI.WpfWizardFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public class ViewSettingsTask : ConsoleCommand
    {
        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            base.ExecuteCommand(nodes, task, parameters);


            WizardStory wizard = new WizardStory();


            wizard.WizardWindowTitle = "Azure Automation Connector Configuration";


            //WizardData data = new AdminSettingWizardData(emoAdminSetting);


            //wizard.WizardData = data;

            wizard.AddLast(new WizardStep("Configuration", typeof(SettingsPage), null));

            //wizard.AddLast(new WizardStep("Configuration", typeof(AdminSettingConfigurationPage), wizard.WizardData));


            //Show the property page 


            PropertySheetDialog wizardWindow = new PropertySheetDialog(wizard);


            bool? dialogResult = wizardWindow.ShowDialog();


            //var wnd = new SettingsWindow();
            //var res = wnd.ShowDialog();





        }





    }
}
