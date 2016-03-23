using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared
{
    public class ConnectorSettings : IEquatable<ConnectorSettings>
    {
        public Guid TenantId { get; set; }
        public Guid SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }
        public string AutomationAccountName { get; set; }
        public Guid UserAppId { get; set; }
        public DateTime LastChanged { get; set; }

        public Guid WorkflowAppId { get; set; }
        public DateTime CredentialExpirationDate { get; set; }
        public bool IsConfigured
        {
            get
            {
                return ((SubscriptionId != default(Guid)) && !string.IsNullOrEmpty(ResourceGroupName) && !string.IsNullOrEmpty(AutomationAccountName));
            }
        }

        public bool Equals(ConnectorSettings other)
        {
            return (
                TenantId == other.TenantId &&
                SubscriptionId == other.SubscriptionId &&
                ResourceGroupName == other.ResourceGroupName &&
                AutomationAccountName == other.AutomationAccountName
            );

        }
    }
}
