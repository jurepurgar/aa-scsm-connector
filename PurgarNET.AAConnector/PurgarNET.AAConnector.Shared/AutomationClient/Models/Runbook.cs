using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.AutomationClient.Models
{
    
    public enum RunbookState { New, Published, Edit }
    public enum RunbookType { PowerShell, Script, Graph }

    public class Runbook
    {
        public class RunbookProperties
        {
            [DataMember(Name = "runbookType")]
            public RunbookType RunbookType { get; set; }

            [DataMember(Name = "state")]
            public RunbookState State { get; set; }
            //public bool logVerbose { get; set; }
            //public bool logProgress { get; set; }
            //public int logActivityTrace { get; set; }
            //public string creationTime { get; set; }
            //public string lastModifiedTime { get; set; }
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }
        /*[DataMember(Name = "location")]
        public string Location { get; set; }*/

        [DataMember(Name = "name")]
        public string Name { get; set; }

        /*[DataMember(Name = "type")]
        public string Type { get; set; } */

        [DataMember(Name = "properties")]
        public RunbookProperties Properties { get; set; } 
    }
}
