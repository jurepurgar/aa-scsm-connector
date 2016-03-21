using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{

    public class PasswordCredential
    {

        public object customKeyIdentifier { get; set; }
        public string endDate { get; set; }
        public string keyId { get; set; }
        public string startDate { get; set; }
        public object value { get; set; }
    }

}
