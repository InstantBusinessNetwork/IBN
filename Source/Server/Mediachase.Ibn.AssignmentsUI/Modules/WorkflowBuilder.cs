using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Workflow.Activities;
using System.Workflow.ComponentModel;
using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;

namespace Mediachase.Ibn.AssignmentsUI.Modules
{
	public class WorkflowBuilder : CompositeDataBoundControl
	{
		protected Panel divContainer;
		protected Button btnSave;

		#region prop: WorkflowId
		/// <summary>
		/// Gets or sets the workflow id.
		/// </summary>
		/// <value>The workflow id.</value>
		public PrimaryKeyId WorkflowId
		{
			get
			{
				if (ViewState["_WorkflowId"] == null)
					return PrimaryKeyId.Empty;

				return (PrimaryKeyId)ViewState["_WorkflowId"];
			}
			set
			{
				ViewState["_WorkflowId"] = value;
			}
		} 
		#endregion

		#region prop: CurrentRootActivity (todo: load from database)
		/// <summary>
		/// Gets or sets the current root activity.
		/// </summary>
		/// <value>The current root activity.</value>
		public SequentialWorkflowActivity CurrentRootActivity
		{
			get
			{
				if (ViewState["_CurrentRootActivity"] != null)	
					return  McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(ViewState["_CurrentRootActivity"].ToString());

				return null;
			}
			set
			{
				ViewState["_CurrentRootActivity"] = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(value); ;
			}
		} 
		#endregion

		#region btnSave_Click
		/// <summary>
		/// Handles the Click event of the btnSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void btnSave_Click(object sender, EventArgs e)
		{
			//WorkflowDefinitionEntity en = BusinessManager.InitializeEntity<WorkflowDefinitionEntity>(WorkflowDefinitionEntity.ClassName);//new WorkflowDefinitionEntity(); //
			//en.Xaml = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(this.CurrentRootActivity);
			//en.PrimaryKeyId = BusinessManager.Create(en);
			WorkflowDefinitionEntity en = (WorkflowDefinitionEntity)BusinessManager.Load(WorkflowDefinitionEntity.ClassName, this.WorkflowId);
			en.Xaml = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(this.CurrentRootActivity);
			BusinessManager.Update(en);
		}
		#endregion

		#region PerformControls
		/// <summary>
		/// Performs the controls.
		/// </summary>
		public void PerformControls()
		{
			this.DataBind();
		} 
		#endregion

		#region CreateChildControls
		/// <summary>
		/// When overridden in an abstract class, creates the control hierarchy that is used to render the composite data-bound control based on the values from the specified data source.
		/// </summary>
		/// <param name="dataSource">An <see cref="T:System.Collections.IEnumerable"/> that contains the values to bind to the control.</param>
		/// <param name="dataBinding">true to indicate that the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"/> is called during data binding; otherwise, false.</param>
		/// <returns>
		/// The number of items created by the <see cref="M:System.Web.UI.WebControls.CompositeDataBoundControl.CreateChildControls(System.Collections.IEnumerable,System.Boolean)"/>.
		/// </returns>
		protected override int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			if (this.CurrentRootActivity == null)
			{
				EntityObject[] arr = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new Mediachase.Ibn.Data.FilterElement[] { new Mediachase.Ibn.Data.FilterElement("WorkflowDefinitionId", FilterElementType.Equal, this.WorkflowId) });

				if (arr.Length == 1)
				{
					WorkflowDefinitionEntity wf = (WorkflowDefinitionEntity)arr[0];
					this.CurrentRootActivity = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(wf.Xaml);
				}
				else
				{
					throw new ArgumentNullException(String.Format("Cant find workflow with id: {0}", this.WorkflowId));
				}
			}

			btnSave = new Button();
			btnSave.Text = "Save workflow";
			btnSave.Click += new EventHandler(btnSave_Click);

			divContainer = new Panel();
			divContainer.CssClass = "wfMainContainer";
			divContainer.ID = this.ID + "_wfMainContainerId";

			_counter = 0;

			if (this.CurrentRootActivity != null)
				ProcessNodeCollection(this.CurrentRootActivity, divContainer);

			divContainer.Controls.Add(btnSave);
			this.Controls.Add(divContainer);

			return 1;
		}
		#endregion

		#region ProcessNodeCollection
		private int _counter = 0;
		/// <summary>
		/// Processes the node collection.
		/// </summary>
		/// <param name="activity">The activity.</param>
		private void ProcessNodeCollection(Activity activity, Control Parent)
		{
			if (activity == null)
				return;

			WorkflowItem item = new WorkflowItem(this);
			item.CurrentActivity = activity;
			_counter++;
			item.ID = String.Format("{1}_wi_{0}", _counter, this.ID);

			item.DataBind();
			Parent.Controls.Add(item);
			
			if (activity is CompositeActivity)
			{
				foreach (Activity innerActivity in ((CompositeActivity)activity).Activities)
				{
					ProcessNodeCollection(innerActivity, item);
				}
			}
		}
		#endregion
	}
}
