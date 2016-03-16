using PurgarNET.AAConnector.Shared;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurgarNET.AAConnector.Workflows
{
    public class WorkflowHandler : HandlerBase
    {

        public static WorkflowHandler Current
        {
            get
            {
                return (WorkflowHandler)GetCurrent(() => new WorkflowHandler());
            }
        }

        public bool Initialize(string clientIdStr, string clientSecret)
        {
            Guid clientId;
            try
            {
                clientId = new Guid(clientIdStr);
            }
            catch (Exception e)
            {
                throw e;
            }

            return base.Initialize("localhost", AuthenticationType.ClientSecret, null, clientId, clientSecret);
        }


        public void ProcessActivities()
        {
            CheckInitialized();

            using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"C:\Windows\Temp\TestWorkflow.txt", true))
            {
                outputFile.WriteLine(" ");
                outputFile.WriteLine("Date: " + DateTime.Now.ToString());
              

                outputFile.WriteLine(" ");
            }


            var rt = _aaClient.GetRunbooksAsync();
            var runbooks = rt.Result;
            string str = string.Empty;
            foreach (var r in runbooks)
                str += "\n\r" + r.Name;

            using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(@"C:\Windows\Temp\TestWorkflowRunbooks.txt", true))
            {
                outputFile.WriteLine(" ");
                outputFile.WriteLine("Date: " + DateTime.Now.ToString());
                outputFile.WriteLine("Runbooks:");
                
                outputFile.WriteLine(runbooks);

                //outputFile.WriteLine("Id: " + ClientId);
                //outputFile.WriteLine("Secret: " + ClientSecret);

                outputFile.WriteLine(" ");
            }
        }

        public void StartRunbook(Guid activityId)
        {
            CheckInitialized();

            var activityObj = GetActivityObject(activityId);
            try
            {


                var j = WorkflowHandler.Current.CreateStartRunbookJob(activityId);

                //activityObj[_smClient.ActivityClass, "JobId"].Value = jobId;
                activityObj[ActivityClass, "JobStatus"].Value = "Starting";
                activityObj[ActivityClass, "JobException"].Value = string.Empty;
                activityObj[ActivityClass, "JobOutput"].Value = string.Empty;
                activityObj.Overwrite();


            }
            catch (Exception error)
            {
                activityObj[ActivityClass, "JobException"].Value = error.Message;
                activityObj.Overwrite();
            }


        }

    }
}
