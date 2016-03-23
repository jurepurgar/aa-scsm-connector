using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{
    public class ServicePrincipal
    {
        public Guid ObjectId { get; set; }
        public Guid AppId { get; set; }
        public bool AccountEnabled { get; set; }
        //public string displayName { get; set; }
    }
}
