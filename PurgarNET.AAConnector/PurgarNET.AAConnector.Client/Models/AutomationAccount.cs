using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Client.Models
{
    public class AutomationAccount
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }
        [DataMember(Name = "location")]
        public string Location { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }


}
