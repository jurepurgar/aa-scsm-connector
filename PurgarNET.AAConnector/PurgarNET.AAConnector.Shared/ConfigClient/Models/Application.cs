using RestSharp.Deserializers;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.ConfigClient.Models
{
    public class Application
    {

        public Application()
        {
            IdentifierUris = new List<Uri>();
            PasswordCredentials = new List<PasswordCredential>();
        }

        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public Guid ObjectId { get; set; }
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public Guid AppId { get; set; }
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public string DisplayName { get; set; }
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public Uri Homepage { get; set; }
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public List<Uri> IdentifierUris { get; set; }
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public bool AvailableToOtherTenants { get; set; }
        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public List<PasswordCredential> PasswordCredentials { get; set; }

    }

    public class CreateableApplication
    {
        public CreateableApplication()
        {
            IdentifierUris = new List<Uri>();
            //PasswordCredentials = new List<PasswordCredential>();
        }


        [SerializeAs(Name = "displayName")]
        public string DisplayName { get; set; }
        [SerializeAs(Name = "homepage")]
        public Uri Homepage { get; set; }
        [SerializeAs(Name = "identifierUris")]
        public List<Uri> IdentifierUris { get; set; }
        //public bool AvailableToOtherTenants { get; set; }

        //public List<PasswordCredential> PasswordCredentials { get; set; }


    }

    public class CredApplication
    {
        public CredApplication()
        {
            PasswordCredentials = new List<PasswordCredential>();
        }


        [SerializeAs(NameStyle = NameStyle.CamelCase)]
        public List<PasswordCredential> PasswordCredentials { get; set; }


    }
}
