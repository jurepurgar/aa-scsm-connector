using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{
    public class AutomationAccountInfo
    {
        public Guid TenantId { get; set; }
        public Guid SubscriptionId { get; set; }
        public string ResourceGroupName { get; set;}
        public string AutomationAccountName { get; set; }
    }
}
