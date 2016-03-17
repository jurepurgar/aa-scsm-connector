using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Console
{
    public class ViewJobTask : ConsoleCommand
    {
        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            base.ExecuteCommand(nodes, task, parameters);

            if (ConsoleHandler.Current.Initialize())
            {
                var jobId = ConsoleHandler.Current.GetPropertyFormNavModel(nodes, "JobId");
                if (string.IsNullOrEmpty(jobId))
                    System.Windows.MessageBox.Show("Runbook was not started yet!");
                else
                    ConsoleHandler.Current.NavigateToPortal($"jobs/{jobId}");
            }
        }
    }
}
