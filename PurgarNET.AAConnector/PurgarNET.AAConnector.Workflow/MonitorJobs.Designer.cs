﻿using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace PurgarNET.AAConnector.Workflows
{
    partial class MonitorJobs
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        [System.CodeDom.Compiler.GeneratedCode("", "")]
        private void InitializeComponent()
        {
            Name = "MonitorJobs";
            CanModifyActivities = true;

            MonitorJobsCodeActivity = new System.Workflow.Activities.CodeActivity();
            MonitorJobsCodeActivity.Name = "MonitorSmaJobsCodeActivity";
            MonitorJobsCodeActivity.ExecuteCode += new System.EventHandler(MonitorJobsCodeActivity_ExecuteCode);
            Activities.Add(MonitorJobsCodeActivity);

            CanModifyActivities = false;
        }

        #endregion

        private CodeActivity MonitorJobsCodeActivity;
    }
}
