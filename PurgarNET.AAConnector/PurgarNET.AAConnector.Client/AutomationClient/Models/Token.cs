using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PurgarNET.AAConnector.Shared.AutomationClient.Models
{
    public class Token
    {
        //public string id_token { get; set; }
        //public string resource { get; set; }
        //public string token_type { get; set; }
        //public string expires_in { get; set; }
        //public string scope { get; set; }

        [DataMember(Name = "expires_on")]
        public DateTime ExpiresOn { get; set; }
        
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        //[DataMember(Name = "not_before")]
        //public DateTime NotBefore { get; set; }

    }
}
