using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.Configuration;
using PurgarNET.AAConnector.Shared.AutomationClient;
using PurgarNET.AAConnector.Shared.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurgarNET.AAConnector.Shared
{
    public abstract class AAHandlerBase : HandlerBase
    {
        protected AAClient _aaClient = null;
        
        private string _clientSecret = null;
        private Guid _clientId = default(Guid);

        protected ConnectorSettings _settings = null;

        private static object _lck = new object();

        
        protected bool Initialize(EnterpriseManagementGroup emg, AuthenticationType authType, EventHandler<AuthorizationCodeRequiredEventArgs> codeRequiredHandler, Guid clientId = default(Guid), string clientSecret = null)
        {
            base.Initialize(emg);
            return InitializeClient(authType, codeRequiredHandler, clientId, clientSecret);
        }

        protected bool Initialize(string ServerName, AuthenticationType authType, EventHandler<AuthorizationCodeRequiredEventArgs> codeRequiredHandler, Guid clientId = default(Guid), string clientSecret = null)
        {
            base.Initialize(ServerName);
            return InitializeClient(authType, codeRequiredHandler, clientId, clientSecret);
        }

        protected bool InitializeClient(AuthenticationType authType, EventHandler<AuthorizationCodeRequiredEventArgs> codeRequiredHandler, Guid clientId = default(Guid), string clientSecret = null)
        {
            var s = GetSettings();

            if (!s.IsConfigured) return false;

            lock (_lck)
            { 
                if (_aaClient == null || _settings == null || _clientSecret != clientSecret || _clientId != clientId || _settings.LastChanged < s.LastChanged )
                {
                    if (authType == AuthenticationType.Code)
                    {
                        clientId = s.UserAppId;
                        if (clientId == default(Guid))
                            clientId = Parameters.CLIENT_ID;
                    }

                    _aaClient = new AAClient(s.TenantId, s.SubscriptionId, s.ResourceGroupName, s.AutomationAccountName, authType, clientId, clientSecret);

                    if (authType == AuthenticationType.Code)
                        _aaClient.AuthorizationCodeRequired += codeRequiredHandler;

                    _clientSecret = clientSecret;
                    _settings = s;
                    _isInitialized = true;
                }
            }
            return true;
        }

        public IEnumerable<ManagementPackProperty> GetActivityPropertyDefinitions(ManagementPackClass c)
        {
            if (c.Id != ActivityClass.Id && c.GetBaseTypes().FirstOrDefault(x => x.Id == ActivityClass.Id) == default(ManagementPackType))
                throw new InvalidOperationException("Activity type is not deriven from 'PurgarNET.AAConnector.RunbookActivity'");
            return c.GetProperties();
        }

        public EnterpriseManagementObject GetActivityObject(Guid activityId)
        {
            return _emg.EntityObjects.GetObject<EnterpriseManagementObject>(activityId, ObjectQueryOptions.Default);
        }

        public EnterpriseManagementObject GetActivityParentObject(Guid activityId)
        {
            EnterpriseManagementObject parent = null;
            EnterpriseManagementRelationshipObject<EnterpriseManagementObject> rel = null;
            Guid id = activityId;
            do
            {
                rel = _emg.EntityObjects.GetRelationshipObjectsWhereTarget<EnterpriseManagementObject>(id, WorkItemContainsActivityRelationship, DerivedClassTraversalDepth.None, TraversalDepth.OneLevel, ObjectQueryOptions.Default).FirstOrDefault();
                if (rel != null)
                {
                    parent = rel.SourceObject;
                    id = parent.Id;
                }

            } while (rel != null);

            if (parent == null)
                throw new ObjectNotFoundException($"Parent WorkItem could not be found for Activity with id {activityId}");
            return parent;
        }

        private List<ConnectedObject> GetRelatedObjects(Guid objectId, ManagementPackRelationship rel)
        {
            var l = new List<ConnectedObject>();
            var items = _emg.EntityObjects.GetRelatedObjects<EnterpriseManagementObject>(objectId, AffectedItemRelationship, TraversalDepth.OneLevel, ObjectQueryOptions.Default);


            foreach (var item in items)
            {
                var obj = new ConnectedObject();

                obj.Add("_Id", item.Id);

                var classes = item.GetMostDerivedClasses();
                var c = classes.Where(x => !x.Extension).FirstOrDefault();
                if (c == null)
                {
                    c = classes.FirstOrDefault();
                    while (c.Extension)
                    {
                        c = (ManagementPackClass)c.GetBaseType();
                    }
                }

                obj.Add("_Type", c.Name);
                obj.Add("_BaseType", item.GetLeastDerivedNonAbstractClass().Name);

                foreach (var prop in item.GetProperties())
                {
                    var name = prop.Name;
                    if (obj.ContainsKey(name))
                        name = name + "_" + prop.Id;
                    var val = item[prop].Value;
                    if (val != null)
                    {
                        if (prop.Type == ManagementPackEntityPropertyTypes.@enum)
                            val = val.ToString();
                        obj.Add(name, val);
                    }
                }
                l.Add(obj);
            }
            return l;
        }


        public AutomationClient.Models.Job CreateStartRunbookJob(Guid activityId)
        {
            var activityObj = GetActivityObject(activityId);
            EnterpriseManagementObject parentWorkItem = null;
            object relatedItems = null;
            object affectedItems = null;

            var parameters = ParameterMappings.CreateFromString(activityObj[ActivityClass, "ParameterMappings"].Value.ToString());

            var job = new AutomationClient.Models.Job();

            job.Properties.Runbook.Name = activityObj[ActivityClass, "RunbookName"].Value.ToString();
            var runOn = activityObj[ActivityClass, "RunOn"].Value.ToString();
            if (runOn.ToLower() == "azure")
                runOn = null;
            job.Properties.RunOn = runOn;

            foreach (var p in parameters)
            {
                bool mustSerialize = false;
                object value = null;
                if (string.IsNullOrEmpty(p.PropertyMapping)) continue;
                var arr = p.PropertyMapping.Split(':');

                var propType = (PropertyDefinitionType)Enum.Parse(typeof(PropertyDefinitionType), arr[0]);

                switch (propType)
                {
                    case PropertyDefinitionType.Property:
                    case PropertyDefinitionType.EnumDisplayName:
                    case PropertyDefinitionType.EnumName:
                        var propId = new Guid(arr[1]);

                        var prop = activityObj[propId];
                        if (prop.Value != null)
                        {
                            value = prop.Value;
                            if (propType == PropertyDefinitionType.EnumName)
                                value = ((ManagementPackEnumeration)prop.Value).Name;
                            else if (propType == PropertyDefinitionType.EnumDisplayName)
                                value = ((ManagementPackEnumeration)prop.Value).DisplayName;

                        }
                        break;
                    case PropertyDefinitionType.AffectedItems:
                    case PropertyDefinitionType.ReletedItems:
                    case PropertyDefinitionType.ParentWorkItemGuid:
                    case PropertyDefinitionType.ParentWorkItemId:
                        if (parentWorkItem == null)
                            parentWorkItem = GetActivityParentObject(activityObj.Id);

                        if (propType == PropertyDefinitionType.ParentWorkItemGuid)
                            value = parentWorkItem.Id;
                        else if (propType == PropertyDefinitionType.ParentWorkItemId)
                            value = parentWorkItem[WorkItemClass, "Id"].Value;
                        else if (propType == PropertyDefinitionType.AffectedItems)
                        {
                            if (affectedItems == null)
                                affectedItems = GetRelatedObjects(parentWorkItem.Id, AffectedItemRelationship);
                            value = affectedItems;
                        }
                        else if (propType == PropertyDefinitionType.ReletedItems)
                        {
                            if (relatedItems == null)
                                relatedItems = GetRelatedObjects(parentWorkItem.Id, RelatedItemRelationship);
                            value = relatedItems;
                            //get related
                            mustSerialize = true;
                        }
                        break;

                    case PropertyDefinitionType.ActivityGuid:
                        value = activityObj.Id;
                        break;
                    case PropertyDefinitionType.ActivityId:
                        value = activityObj[WorkItemClass, "Id"].Value;
                        break;
                }

                if (value is bool)
                    value = ((bool)value) ? "true" : "false";

                if (value is Guid)
                    mustSerialize = true;

                if (p.Type.EndsWith("[]")) //move this elsewere
                {
                    mustSerialize = true;
                    if (!(value is IEnumerable))
                        value = new object[] { value };
                }

                if (mustSerialize)
                    value = Serializer.ToJson(value); //mustSerialize to JSON

                job.Properties.Parameters.Add(p.Name, value);
            }

            return job;
        }

        public async Task ProcessActivity(EnterpriseManagementObject activityObj)
        {
            try
            {
                if (activityObj[ActivityClass, "JobId"].Value != null)
                {
                    var jobId = (Guid)activityObj[ActivityClass, "JobId"].Value;

                    var jt = _aaClient.GetJobAsync(jobId);
                    var ot = _aaClient.GetJobOutput(jobId);

                    var j = await jt;
                    activityObj[ActivityClass, "JobStatus"].Value = j.Properties.Status;
                    activityObj[ActivityClass, "JobException"].Value = j.Properties.Exception;

                    var s = j.Properties.Status.ToLower();
                    if (s == "completed")
                        activityObj[ActivityClass, "Status"].Value = ActivityCompletedEnum;
                    else if (s == "stopped" || s == "failed")
                        activityObj[ActivityClass, "Status"].Value = ActivityFailedEnum;

                    activityObj[ActivityClass, "JobOutput"].Value = await ot;

                    activityObj.Overwrite();
                }
            }
            catch (Exception error)
            {
                activityObj[ActivityClass, "JobException"].Value = error.Message;
                activityObj.Overwrite();
            }
        }



    }
}
