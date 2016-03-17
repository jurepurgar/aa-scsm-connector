using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.DataModel;

namespace PurgarNET.AAConnector.Console
{
    public class ViewRunbookTask : ConsoleCommand
    {
        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            base.ExecuteCommand(nodes, task, parameters);

            if (ConsoleHandler.Current.Initialize())
            {
                var runbookName = ConsoleHandler.Current.GetPropertyFormNavModel(nodes, "RunbookName");
                if (string.IsNullOrEmpty(runbookName))
                    System.Windows.MessageBox.Show("Runbook is not assigned to activity!");
                else
                    ConsoleHandler.Current.NavigateToPortal($"runbooks/{runbookName}");
            }
        }
    }
}
