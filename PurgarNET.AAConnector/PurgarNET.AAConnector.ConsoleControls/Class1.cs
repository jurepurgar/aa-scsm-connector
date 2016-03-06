using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{

    /*
    public class AdminSettingsExample : ConsoleCommand
    {
        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
        
            This GUID is generated automatically when you import the Management Pack with the singleton admin setting class in it. 

            You can get this GUID by running a query like: 

            Select BaseManagedEntityID, FullName where FullName like ‘%<enter your class ID here>%’

            where the GUID you want is returned in the BaseManagedEntityID column in the result set 
         

            String strSingletonBaseManagedObjectID = "79893B9E - 04AC - 9D3A - 51D8 - B085BEE6EA78";
            //Get the server name to connect to and connect to the server 
            String strServerName = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\System Center\\2010\\Service Manager\\Console\\User Settings", "SDKServiceMachine", "localhost").ToString();
            EnterpriseManagementGroup emg = new EnterpriseManagementGroup(strServerName);
            //Get the Object using the GUID from above – since this is a singleton object we can get it by GUID 
            EnterpriseManagementObject emoAdminSetting = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(strSingletonBaseManagedObjectID), ObjectQueryOptions.Default);
            //Create a new "wizard" (also used for property dialogs as in this case), set the title bar, create the data, and add the pages 
            WizardStory wizard = new WizardStory();
            wizard.WizardWindowTitle = "Edit Admin Setting";
            WizardData data = new AdminSettingWizardData(emoAdminSetting);
            wizard.WizardData = data;
            wizard.AddLast(new WizardStep("Configuration", typeof(AdminSettingConfigurationPage), wizard.WizardData));
            //Show the property page 
            PropertySheetDialog wizardWindow = new PropertySheetDialog(wizard);
            //Update the view when done so the new values are shown 
            bool? dialogResult = wizardWindow.ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {
                RequestViewRefresh();
            }
        }
    }


    class AdminSettingWizardData : WizardData
    {
        #region Variables 
        private String strProperty1 = String.Empty;
        private String strProperty2 = String.Empty;
        private Guid guidEnterpriseManagementObjectID = Guid.Empty;

        public String Property1
        {
            get
            {
                return this.strProperty1;
            }
            set
            {
                if (this.strProperty1 != value)
                {
                    this.strProperty1 = value;
                }
            }
        }

        public String Property2
        {
            get
            {
                return this.strProperty2;
            }
            set
            {
                if (this.strProperty2 != value)
                {
                    this.strProperty2 = value;
                }
            }
        }

        public Guid EnterpriseManagementObjectID
        {
            get
            {
               return this.guidEnterpriseManagementObjectID;
            }
            set
            {
                if (this.guidEnterpriseManagementObjectID != value)
                {
                    this.guidEnterpriseManagementObjectID = value;
                }
            }
        }
        #endregion


        internal AdminSettingWizardData(EnterpriseManagementObject emoAdminSetting)
        {
            //Get the server name to connect to and connect 
            String strServerName = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\System Center\\2010\\Service Manager\\Console\\User Settings", "SDKServiceMachine", "localhost").ToString();
            EnterpriseManagementGroup emg = new EnterpriseManagementGroup(strServerName);
            //Get the AdminSettings MP so you can get the Admin Setting class 
            ManagementPack mpAdminSetting = emg.GetManagementPack("Microsoft.Demo.AdminSettings", null, new Version("1.0.0.0"));
            ManagementPackClass classAdminSetting = mpAdminSetting.GetClass("Microsoft.Demo.AdminSetting.Example");
            this.Property1 = emoAdminSetting[classAdminSetting, "Property1"].ToString();
            this.Property2 = emoAdminSetting[classAdminSetting, "Property2"].ToString();
            this.EnterpriseManagementObjectID = emoAdminSetting.Id;
        }


        public override void AcceptChanges(WizardMode wizardMode)
        {
            //Get the server name to connect to and connect 
            String strServerName = Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\System Center\\2010\\Service Manager\\Console\\User Settings", "SDKServiceMachine", "localhost").ToString();
            EnterpriseManagementGroup emg = new EnterpriseManagementGroup(strServerName);
            //Get the AdminSettings MP so you can get the Admin Setting class 
            ManagementPack mpAdminSetting = emg.GetManagementPack("Microsoft.Demo.AdminSettings", null, new Version("1.0.0.0"));
            ManagementPackClass classAdminSetting = mpAdminSetting.GetClass("Microsoft.Demo.AdminSetting.Example");
            //Get the Connector object using the object ID 
            EnterpriseManagementObject emoAdminSetting = emg.EntityObjects.GetObject<EnterpriseManagementObject>(this.EnterpriseManagementObjectID, ObjectQueryOptions.Default);
            //Set the property values to the new values 
            emoAdminSetting[classAdminSetting, "Property1"].Value = this.Property1;
            emoAdminSetting[classAdminSetting, "Property2"].Value = this.Property2;
            //Update Connector instance 
            emoAdminSetting.Commit();
            this.WizardResult = WizardResult.Success;
        }
    } */
}


