using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Client.Models
{
    public class Tenant
    {
        [DataMember(Name = "tenantId")]
        public Guid TenantId { get; set; }
    }

}
