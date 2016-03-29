using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.AutomationClient.Models
{


    public enum RunbookState { New, Published, Edit }
    public enum RunbookType { PowerShell, Script, GraphPowerShellWorkflow }

    public class Runbook
    {
        public Runbook()
        {
            Properties = new RunbookProperties();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public RunbookProperties Properties { get; set; } 
    }

    public class RunbookProperties
    {
        public RunbookProperties()
        {
            Parameters = new Dictionary<string, RunbookParameter>();
        }

        public RunbookType RunbookType { get; set; }

        public RunbookState State { get; set; }
        //public bool logVerbose { get; set; }
        //public bool logProgress { get; set; }
        //public int logActivityTrace { get; set; }
        //public string creationTime { get; set; }
        //public string lastModifiedTime { get; set; }

        public Dictionary<string, RunbookParameter> Parameters { get; set; }
    }

    public class RunbookParameter
    {
        public string Type { get; set; }

        public bool IsMandatory { get; set; }

        public object DefaultValue { get; set; }
    }
}
