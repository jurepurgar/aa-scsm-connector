using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{

    public class PasswordCredential
    {
        public string CustomKeyIdentifier { get; set; }
        public Guid KeyId { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string Value { get; set; }
    }

}
