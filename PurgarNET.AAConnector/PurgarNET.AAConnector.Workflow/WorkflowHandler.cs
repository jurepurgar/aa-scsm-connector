using Microsoft.EnterpriseManagement.Common;
using PurgarNET.AAConnector.Shared;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.AutomationClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Workflows
{
    public class WorkflowHandler : AAHandlerBase
    {
        private static WorkflowHandler _current = null; 
        private static object _lck = new object();
        public static WorkflowHandler Current
        {
            get
            {
                lock (_lck)
                {
                    if (_current == null)
                        _current = new WorkflowHandler();
                }
                return _current;
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

            return base.Initialize("localhost", Shared.Client.AuthenticationType.ClientSecret, null, clientId, clientSecret);
        }

        public IEnumerable<EnterpriseManagementObject> GetInProgressAutomationActivities()
        {
            var criteria = new EnterpriseManagementObjectCriteria("Status = '" + ActivityInProgressEnum.Id + "'", ActivityClass);
            return _emg.EntityObjects.GetObjectReader<EnterpriseManagementObject>(criteria, ObjectQueryOptions.Default);
        }

        public void ProcessActivities()
        {
            CheckInitialized();

            var activities = GetInProgressAutomationActivities();
            var tasks = new List<Task>();

            foreach (var activityObj in activities) //retry so it will do web calls in parallel
                tasks.Add(ProcessActivity(activityObj));
            Task.WaitAll(tasks.ToArray()); 
        }


          
        public void StartRunbook(Guid activityId)
        {
            CheckInitialized();

            var activityObj = GetActivityObject(activityId);
            try
            {
                var job = CreateStartRunbookJob(activityId);
                var t = _aaClient.StartJob(job);
                var startedJob = t.Result;
                activityObj[ActivityClass, "JobId"].Value = startedJob.Properties.JobId;
                activityObj[ActivityClass, "JobStatus"].Value = startedJob.Properties.Status;
                activityObj[ActivityClass, "JobException"].Value = string.Empty;
                activityObj[ActivityClass, "JobOutput"].Value = string.Empty;
                activityObj.Overwrite();
            }
            catch (Exception error)
            {
                activityObj[ActivityClass, "JobException"].Value = error.ToString();
                activityObj.Overwrite();
            }
        }
        
    }
}
