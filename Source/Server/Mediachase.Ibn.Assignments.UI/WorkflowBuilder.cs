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
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Assignments.UI
{
	
	public class WorkflowBuilder 
	#if (DEBUG)
		: CompositeDataBoundControl
	{
		#region --- const ---
		private string __EditTextResource = "Edit";
		private string __AddTextResource = "Create";
		private string __DeleteTextResource = "Delete?";
		#endregion

		protected Panel divContainer;

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

		#region prop: CurrentSchemaMaster
		/// <summary>
		/// Gets the current schema master.
		/// </summary>
		/// <value>The current schema master.</value>
		public SchemaMaster CurrentSchemaMaster
		{
			get
			{
				if (this.WorkflowId != PrimaryKeyId.Empty)
				{
					EntityObject obj = null;
					try
					{
						obj = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, this.WorkflowId);
					}
					catch
					{
					}

					if (obj == null)
					{
						obj = BusinessManager.Load(WorkflowInstanceEntity.ClassName, this.WorkflowId);
						if (obj == null)
						{
							throw new ArgumentNullException(String.Format("Cant find workflow with id: {0} @ get CurrentSchemaMaster", this.WorkflowId));
						}
						else
						{
							WorkflowInstanceEntity wfe = (WorkflowInstanceEntity)obj;
							return SchemaManager.GetShemaMaster(wfe.SchemaId);
						}
					}
					else
					{
						WorkflowDefinitionEntity wf = (WorkflowDefinitionEntity)obj;
						return SchemaManager.GetShemaMaster(wf.SchemaId);
					}

				}

				return null;
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
				EntityObject eo = GetEntityWorkflow();
				if (eo != null)
				{
					return McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(eo.Properties["Xaml"].Value.ToString());
				}

				//if (ViewState["_CurrentRootActivity"] != null)	
				//    return McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(ViewState["_CurrentRootActivity"].ToString());

				return null;
			}
			set
			{
				if (this.CurrentRootActivity != null) // store to database
				{
					EntityObject eo = GetEntityWorkflow();
					if (eo != null)
					{
						eo.Properties["Xaml"].Value = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(value);
						BusinessManager.Update(eo);
					} // else thro Exception (?)
				}
				ViewState["_CurrentRootActivity"] = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(value);
			}
		} 
		#endregion

		#region prop: NewActivityScript
		/// <summary>
		/// Gets or sets the new activity script.
		/// </summary>
		/// <value>The new activity script.</value>
		public string NewActivityScript
		{
			get
			{
				if (ViewState["_NewActivityScript"] == null)
					return string.Empty;

				return ViewState["_NewActivityScript"].ToString();
			}
			set
			{
				ViewState["_NewActivityScript"] = value;
			}
		} 
		#endregion

		#region prop: EditActivityScript

		/// <summary>
		/// Gets or sets the edit activity script.
		/// </summary>
		/// <value>The edit activity script.</value>
		public string EditActivityScript
		{
			get
			{
				if (ViewState["_EditActivityScript"] == null)
					return string.Empty;

				return ViewState["_EditActivityScript"].ToString();
			}
			set
			{
				ViewState["_EditActivityScript"] = value;
			}
		}
		#endregion

		#region prop: AddText
		/// <summary>
		/// Gets or sets the add text resource.
		/// </summary>
		/// <value>The add text resource.</value>
		public string AddText
		{
			get
			{
				if (ViewState["_AddTextResource"] == null)
					return __AddTextResource;

				return ViewState["_AddTextResource"].ToString();
			}
			set
			{
				ViewState["_AddTextResource"] = value;
			}
		}
		#endregion

		#region prop: EditText
		/// <summary>
		/// Gets or sets the edit text resource.
		/// </summary>
		/// <value>The edit text resource.</value>
		public string EditText
		{
			get
			{
				if (ViewState["_EditTextResource"] == null)
					return __EditTextResource;

				return ViewState["_EditTextResource"].ToString();
			}
			set
			{
				ViewState["_EditTextResource"] = value;
			}
		}
		#endregion

		#region prop: DeleteText
		/// <summary>
		/// Gets or sets the delete text resource.
		/// </summary>
		/// <value>The delete text resource.</value>
		public string DeleteText
		{
			get
			{
				if (ViewState["_DeleteTextResource"] == null)
					return __DeleteTextResource;

				return ViewState["_DeleteTextResource"].ToString();
			}
			set
			{
				ViewState["_DeleteTextResource"] = value;
			}
		}
		#endregion

		#region GetEntityWorkflow
		/// <summary>
		/// Gets the entity workflow.
		/// </summary>
		/// <returns></returns>
		private EntityObject GetEntityWorkflow()
		{
			EntityObject obj = null;
			try
			{
				obj = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, this.WorkflowId);
			}
			catch
			{
				if (obj == null)
				{
					obj = BusinessManager.Load(WorkflowInstanceEntity.ClassName, this.WorkflowId);
				}
			}

			return obj;
		}
		#endregion

		#region btnSave_Click
		/// <summary>
		/// Handles the Click event of the btnSave control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		//void btnSave_Click(object sender, EventArgs e)
		//{
		//    EntityObject eo = null;
		//    try
		//    {
		//        eo = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, this.WorkflowId);
		//    }
		//    catch
		//    {
		//    }
		//    if (eo == null)
		//    {
		//        eo = BusinessManager.Load(WorkflowInstanceEntity.ClassName, this.WorkflowId);
		//    }

		//    eo.Properties["Xaml"].Value = McWorkflowSerializer.GetString<SequentialWorkflowActivity>(this.CurrentRootActivity);
		//    BusinessManager.Update(eo);
		//}
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
				//EntityObject[] arr = BusinessManager.List(WorkflowDefinitionEntity.ClassName, new Mediachase.Ibn.Data.FilterElement[] { new Mediachase.Ibn.Data.FilterElement("WorkflowDefinitionId", FilterElementType.Equal, this.WorkflowId) });
				EntityObject obj = null;
				try
				{
					obj = BusinessManager.Load(WorkflowDefinitionEntity.ClassName, this.WorkflowId);
				}
				catch
				{
				}

				if (obj == null)
				{
					obj = BusinessManager.Load(WorkflowInstanceEntity.ClassName, this.WorkflowId);
					if (obj == null)
					{
						throw new ArgumentNullException(String.Format("Cant find workflow with id: {0}", this.WorkflowId));
					}
					else
					{
						WorkflowInstanceEntity wfe = (WorkflowInstanceEntity)obj;
						this.CurrentRootActivity = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(wfe.Xaml);
					}
				}
				else
				{
					WorkflowDefinitionEntity wf = (WorkflowDefinitionEntity)obj;
					this.CurrentRootActivity = McWorkflowSerializer.GetObject<SequentialWorkflowActivity>(wf.Xaml);
				}
			}

			divContainer = new Panel();
			divContainer.CssClass = "wfMainContainer";
			divContainer.ID = this.ID + "_wfMainContainerId";

			_counter = 0;

			if (this.CurrentRootActivity != null)
				ProcessNodeCollection(this.CurrentRootActivity, divContainer);

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
		#endif
#if (!DEBUG)
			: Control {
#endif
	}
	
}
