using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace PurgarNET.AAConnector.Workflows
{
    public sealed partial class MonitorJobs : SequentialWorkflowActivity
    {
        public MonitorJobs()
        {
            InitializeComponent();
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public void MonitorJobsCodeActivity_ExecuteCode(object sender, EventArgs e)
        {
            using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"C:\Windows\Temp\TestWorkflowPass.txt", true))
            {
                outputFile.WriteLine("");
                outputFile.WriteLine("Date: " + DateTime.Now.ToString());

                outputFile.WriteLine("Id: " + ClientId);
                outputFile.WriteLine("Secret: " + ClientSecret);

                outputFile.WriteLine(" ");
            }

            if (WorkflowHandler.Initialize(ClientId, ClientSecret))
            {
                WorkflowHandler.ProcessActivities();
            }


            /*var activities = ScsmHelpers.GetInProgressAutomationActivities();

            foreach (var activityObj in activities)
            {
                try
                {
                    var webServiceUrl = ScsmHelpers.GetSmaUrlFromActivity(activityObj);

                    if (activityObj[ScsmHelpers.SmaActivityBaseClass, "JobId"].Value != null)
                    {
                        var jobId = activityObj[ScsmHelpers.SmaActivityBaseClass, "JobId"].Value.ToString();

                        var client = GetSmaClient(webServiceUrl);
                        var jobInfo = client.GetJobInfo(jobId);

                        activityObj[ScsmHelpers.SmaActivityBaseClass, "JobStatus"].Value = jobInfo.JobStatus;
                        activityObj[ScsmHelpers.SmaActivityBaseClass, "JobException"].Value = jobInfo.JobException;
                        activityObj[ScsmHelpers.SmaActivityBaseClass, "JobOutput"].Value = jobInfo.JobOutput;

                        var s = jobInfo.JobStatus.ToLower();
                        if (s == "completed")
                        {
                            activityObj[ScsmHelpers.SmaActivityBaseClass, "Status"].Value = ScsmHelpers.ActivityCompletedEnum;
                        }
                        else if (s == "stopped" || s == "failed")
                        {
                            activityObj[ScsmHelpers.SmaActivityBaseClass, "Status"].Value = ScsmHelpers.ActivityFailedEnum;
                        }

                        activityObj.Overwrite();
                    }
                }
                catch (Exception error)
                {
                    activityObj[ScsmHelpers.SmaActivityBaseClass, "JobException"].Value = error.Message;
                    activityObj.Overwrite();
                }

            }*/

        }

    }

}
