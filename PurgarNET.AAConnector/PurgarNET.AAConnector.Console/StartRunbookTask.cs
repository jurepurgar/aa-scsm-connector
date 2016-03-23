using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public class StartRunbookTask : ConsoleCommand
    {
        public override async void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            base.ExecuteCommand(nodes, task, parameters);

            if (ConsoleHandler.Current.Initialize())
            {

                var item = ConsoleHandler.Current.GetFormDataContext(nodes);

                if ((bool)item["$IsNew$"])
                    System.Windows.MessageBox.Show("You cannot start runbook from template!");
                else
                { 
                    var res = System.Windows.MessageBox.Show("Make sure the changes you've made are commited before starting runbook. Please note that execution of this job will not be tracked. Do you still want to start runbook?", "Warning!", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);

                    if (res == System.Windows.MessageBoxResult.Yes)
                    {
                        try
                        {

                            var activityId = (Guid)item["$Id$"];
                            var j = ConsoleHandler.Current.CreateStartRunbookJob(activityId);
                            await ConsoleHandler.Current.AAClient.StartJob(j);
                            System.Windows.MessageBox.Show("Runbook started successfully.", "Started", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                        }
                        catch (Exception e)
                        {
                            System.Windows.MessageBox.Show($"Failed to start runbook because: {e.Message}", "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        }
                    }
                }

            }
        }
    }
}
