using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Assignments;
using System.Workflow.Activities;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using System.Workflow.ComponentModel;


namespace Mediachase.Ibn.AssignmentsUI.Modules.Pages
{
	public partial class PrototypeEdit : System.Web.UI.Page
	{
		#region prop: ActivityName
		/// <summary>
		/// Gets the name of the activity.
		/// </summary>
		/// <value>The name of the activity.</value>
		private string ActivityName
		{
			get
			{
				return Request["ActivityId"];
			}
		} 
		#endregion

		#region prop: WorkflowId
		/// <summary>
		/// Gets the workflow id.
		/// </summary>
		/// <value>The workflow id.</value>
		private string WorkflowId
		{
			get
			{
				return Request["WfId"];
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindValue();
			}

			btnOk.Click += new EventHandler(btnOk_Click);
		}

		#region btnOk_Click
		/// <summary>
		/// Handles the Click event of the btnOk control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnOk_Click(object sender, EventArgs e)
		{
			EntityObject eo = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, PrimaryKeyId.Parse(WorkflowId));
			SequentialWorkflowActivity root = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(((WorkflowDefinitionEntity)eo).Xaml);
			Activity ac = root.GetActivityByName(ActivityName);

			if (ac is CreateAssignmentAndWaitResultActivity)
			{
				AssignmentPrototypePropertyEntityCollection props = ((CreateAssignmentAndWaitResultActivity)ac).PrototypeUserProperties;
				props.SetValue("Assignment.Properties.Subject", tbSubject.Text);
			}

			string xaml = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(root);
			((WorkflowDefinitionEntity)eo).Xaml = xaml;
			BusinessManager.Update(eo);

			Page.ClientScript.RegisterClientScriptBlock(this.GetType(), Guid.NewGuid().ToString(), "window.opener.__doPostBack('', ''); window.close();", true);
			//BusinessManager.Update(((CreateAssignmentAndWaitResultActivity)ac);
		}
		#endregion

		#region BindValue
		/// <summary>
		/// Binds the value.
		/// </summary>
		private void BindValue()
		{
			EntityObject eo = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, PrimaryKeyId.Parse(WorkflowId));
			SequentialWorkflowActivity root = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(((WorkflowDefinitionEntity)eo).Xaml);
			Activity ac = root.GetActivityByName(ActivityName);

			if (ac is CreateAssignmentAndWaitResultActivity)
			{

				 AssignmentPrototypePropertyEntityCollection props = ((CreateAssignmentAndWaitResultActivity)ac).PrototypeUserProperties;
				if (props.Contains("Assignment.Properties.Subject"))
					tbSubject.Text = props.GetValue("Assignment.Properties.Subject").ToString();
				else
					props.SetValue("Assignment.Properties.Subject", string.Empty);
			}
		}
		#endregion
	}
}
