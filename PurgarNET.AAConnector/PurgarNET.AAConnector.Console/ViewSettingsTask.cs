using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
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

            var wnd = new SettingsWindow();
            var res = wnd.ShowDialog();

        }

    }
}
