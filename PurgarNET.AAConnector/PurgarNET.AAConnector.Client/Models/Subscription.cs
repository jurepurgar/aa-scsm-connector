using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Client.Models
{
    public class Subscription
    {
        [DataMember(Name = "subscriptionId")]
        public Guid SubscriptionId { get; set; }
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
    }

}
