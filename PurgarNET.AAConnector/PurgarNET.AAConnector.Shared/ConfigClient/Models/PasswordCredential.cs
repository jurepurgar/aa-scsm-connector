using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{

    public class PasswordCredential
    {
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public string CustomKeyIdentifier { get; set; }

        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public Guid KeyId { get; set; }

        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public DateTime EndDate { get; set; }

        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public DateTime StartDate { get; set; }

        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public string Value { get; set; }
    }

}
