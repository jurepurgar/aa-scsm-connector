using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{
    public class AdApplication
    {

        public AdApplication()
        {
            IdentifierUris = new List<Uri>();
            PasswordCredentials = new List<PasswordCredential>();
        }

        public Guid ObjectId { get; set; }
        public Guid AppId { get; set; }
        public string DisplayName { get; set; }
        public Uri Homepage { get; set; }
        public List<Uri> IdentifierUris { get; set; }
        public bool AvailableToOtherTenants { get; set; }
        public List<PasswordCredential> PasswordCredentials { get; set; }

    }

    public class AdCreateableApplication
    {
        public AdCreateableApplication()
        {
            IdentifierUris = new List<Uri>();
        }

        public string DisplayName { get; set; }
        public Uri Homepage { get; set; }
        public List<Uri> IdentifierUris { get; set; }
    }
}
