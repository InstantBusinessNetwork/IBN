using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Assignments;
using System.Workflow.Runtime;
using System.Workflow.ComponentModel;
using System.Workflow.Runtime.Hosting;
using Mediachase.Ibn.Core.Business.Configuration;
using System.Configuration;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Workflow.Activities;

namespace Mediachase.Ibn.AssignmentsUI
{
	public partial class _Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			Response.Redirect("~/Modules/Pages/WorkflowList.aspx");
			WorkflowDefinitionEntity en = BusinessManager.InitializeEntity<WorkflowDefinitionEntity>(WorkflowDefinitionEntity.ClassName);

			SequentialWorkflowActivity item = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(en.Xaml);

			CreateAssignmentAndWaitResultActivity act1 = new CreateAssignmentAndWaitResultActivity();
			act1.Name = "Guid1";
			//act1.Subject =" Subject Here " ;
			//a
			//item.Activities.Add(act1);
		}

		protected void ButtonRunWF_Click(object sender, EventArgs e)
		{
			WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.CreateWorkflow(typeof(SimpleWorkflow));

			instance.Start();

			//ViewState["WorkflowInstanceId"] = instance.InstanceId;

			ManualWorkflowSchedulerService service = GlobalWorkflowRuntime.WorkflowRuntime.GetService<ManualWorkflowSchedulerService>();
			if(service!=null)
				service.RunWorkflow(instance.InstanceId);
		}

		protected void ButtonRunWFApproveFirstItem_Click(object sender, EventArgs e)
		{
			string strQueueName = "28ad9324-85a5-4dd2-96a2-ace25bd14be4";

			//BusinessManager.Execute<AcceptRequest, Response>(new AcceptRequest(PrimaryKeyId.Parse(strQueueName)));

			/*WorkflowInstance instance = GlobalWorkflowRuntime.WorkflowRuntime.GetWorkflow(instanceid);

			

			instance.EnqueueItem(strQueueName, "123", null, null);

            ManualWorkflowSchedulerService service = GlobalWorkflowRuntime.WorkflowRuntime.GetService<ManualWorkflowSchedulerService>();
			if (service != null)
				service.RunWorkflow(instance.InstanceId);*/

		}
	}
}
