using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.AutomationClient.Models
{

    public class OutputItemProperties
    {
        //[DataMember(Name = "jobStreamId")]
        //public string JobStreamId { get; set; }
        [DataMember(Name = "summary")]
        public string Summary { get; set; }
        //public string time { get; set; }
        //public string streamType { get; set; }
    }

    public class OutputItem
    {
        public string id { get; set; }
        [DataMember(Name = "properties")]
        public OutputItemProperties Properties { get; set; }
    }


}
