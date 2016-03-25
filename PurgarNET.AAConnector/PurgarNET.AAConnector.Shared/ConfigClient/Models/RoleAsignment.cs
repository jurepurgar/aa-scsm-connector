using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{
    public class RoleAsignment
    {
        public RoleAsignment()
        {
            Properties = new RoleAsignmentProperties();
        }

        public RoleAsignmentProperties Properties { get; set; }
    }

    public class RoleAsignmentProperties
    {
        public string RoleDefinitionId { get; set; }
        public Guid PrincipalId { get; set; }
    }
}
