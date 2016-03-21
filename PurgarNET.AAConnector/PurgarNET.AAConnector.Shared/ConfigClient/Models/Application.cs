using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{
    public class Application
    {

        public string ObjectId { get; set; }
        public string AppId { get; set; }
        public string DisplayName { get; set; }
        public string homepage { get; set; }
        public List<string> identifierUris { get; set; }
        public bool availableToOtherTenants { get; set; }
            
        public List<PasswordCredential> passwordCredentials { get; set; }

    }
}
