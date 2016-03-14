using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PurgarNET.AAConnector.Shared.AutomationClient.Models
{

    public class Job
    {
        public Job()
        {
            Properties = new JobProperties();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "properties")]
        public JobProperties Properties { get; set; }
    }

    public class JobProperties
    {
        public JobProperties()
        {
            Parameters = new Dictionary<string, object>();
            Runbook = new Runbook();
        }

        [DataMember(Name = "jobId")]
        public Guid JobId { get; set; }

        [DataMember(Name = "provisioningState")]
        public string ProvisioningState { get; set; }

        [DataMember(Name = "creationTime")]
        public DateTime? CreationTime { get; set; }

        [DataMember(Name = "startTime")]
        public DateTime? StartTime { get; set; }

        [DataMember(Name = "lastModifiedTime")]
        public DateTime? LastModifiedTime { get; set; }

        [DataMember(Name = "lastStatusModifiedTime")]
        public DateTime? LastStatusModifiedTime { get; set; }

        [DataMember(Name = "endTime")]
        public DateTime? EndTime { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "statusDetails")]
        public string StatusDetails { get; set; }

        [DataMember(Name = "startedBy")]
        public object StartedBy { get; set; }

        [DataMember(Name = "exception")]
        public string Exception { get; set; }

        [DataMember(Name = "runOn")]
        public string RunOn { get; set; }

        [DataMember(Name = "runbook")]
        public Runbook Runbook { get; set; }

        [DataMember(Name = "parameters")]
        public Dictionary<string, object> Parameters { get; set; }
    }


}
