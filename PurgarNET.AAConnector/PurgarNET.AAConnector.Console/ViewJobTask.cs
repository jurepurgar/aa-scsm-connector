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
                var jobId = ConsoleHandler.Current.GetPropertyFormNavModel<Guid>(nodes, "JobId");
                if (jobId != default(Guid))
                    System.Windows.MessageBox.Show("Runbook was not started yet!", "Error!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                else
                    ConsoleHandler.Current.NavigateToPortal($"jobs/{jobId}");
            }
        }
    }
}
