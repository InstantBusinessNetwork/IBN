using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Assignments;
using System.Workflow.Activities;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.AssignmentsUI
{
	public partial class TestWF : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				//WorkflowDefinitionEntity en =  BusinessManager.InitializeEntity<WorkflowDefinitionEntity>(WorkflowDefinitionEntity.ClassName);
				//SequentialWorkflowActivity item = new SimpleWorkflow();// McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(en.Xaml);

				//EntityObject[] arr = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new Mediachase.Ibn.Data.FilterElement[] { });
				//WorkflowDefinitionEntity en;
				//if (arr.Length == 0)
				//{
				//    en = BusinessManager.InitializeEntity<WorkflowDefinitionEntity>(WorkflowDefinitionEntity.ClassName);
				//}
				//else
				//{
				//    en = (WorkflowDefinitionEntity)arr[0];
				//}

				//SequentialWorkflowActivity item = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(en.Xaml);
				if (Request["WfId"] == null)
					ctrlWF.WorkflowId = Mediachase.Ibn.Data.PrimaryKeyId.Parse("601f9a36-5536-472a-934a-3025e0e49b10");
				else
					ctrlWF.WorkflowId = Mediachase.Ibn.Data.PrimaryKeyId.Parse(Request["WfId"]);
				//ctrlWF.CurrentRootActivity = item;
				ctrlWF.DataBind();
			}

		}
	}
}
