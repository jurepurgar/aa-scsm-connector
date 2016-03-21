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
       /*     if (WorkflowHandler.Current.Initialize(ClientId, ClientSecret))
                WorkflowHandler.Current.ProcessActivities(); */
        }

    }

}
