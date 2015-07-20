using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.Ibn.Assignments.Schemas;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkFlowInstanceEdit : System.Web.UI.UserControl
	{
		#region ObjectId
		protected PrimaryKeyId ObjectId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["Id"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["Id"]);

				return retval;
			}
		}
		#endregion

		#region FromId
		protected PrimaryKeyId FromId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["FromId"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["FromId"]);

				return retval;
			}
		}
		#endregion

		#region OwnerName
		protected string OwnerName
		{
			get
			{
				string retval = string.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["OwnerName"]))
					retval = Request.QueryString["OwnerName"];

				return retval;
			}
		} 
		#endregion

		#region OwnerId
		protected int OwnerId
		{
			get
			{
				int retval = -1;
				if (!String.IsNullOrEmpty(Request.QueryString["OwnerId"]))
					retval = int.Parse(Request.QueryString["OwnerId"]);

				return retval;
			}
		}
		#endregion

		#region ControlName
		protected string ControlName
		{
			get
			{
				string retval = string.Empty;
				if (ViewState["ControlName"] != null)
					retval = (string)ViewState["ControlName"];
				return retval;
			}
			set
			{
				ViewState["ControlName"] = value;
			}
		}
		#endregion

		WorkflowInstanceEntity instance = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ObjectId != PrimaryKeyId.Empty)
				instance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, ObjectId);
			else if (FromId != PrimaryKeyId.Empty)
				instance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, FromId);

			if (!IsPostBack)
			{
				BindInfo();
				BindData();

				// disable Save button for completed business-process
				if (ObjectId != PrimaryKeyId.Empty && instance.State == (int)BusinessProcessState.Closed)
					SaveButton.Disabled = true;
			}

			LoadControlToPlaceHolder(!IsPostBack);

			BindToolbar();
		}

		#region BindInfo
		private void BindInfo()
		{
			AssignmentOverdueActionList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.BusinessProcess", "AssignmentOverdueNoAction").ToString(), ((int)AssignmentOverdueAction.NoAction).ToString()));
			AssignmentOverdueActionList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.BusinessProcess", "AssignmentOverdueAutoCompleteWithAccept").ToString(), ((int)AssignmentOverdueAction.AutoCompleteWithAccept).ToString()));
			AssignmentOverdueActionList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.BusinessProcess", "AssignmentOverdueAutoCompleteWithDecline").ToString(), ((int)AssignmentOverdueAction.AutoCompleteWithDecline).ToString()));

			SaveButton.CustomImage = ResolveUrl("~/layouts/images/saveitem.gif");
			SaveButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Save").ToString();
			CancelButton.Text = GetGlobalResourceObject("IbnFramework.Global", "_mc_Cancel").ToString();
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (ObjectId != PrimaryKeyId.Empty)
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "BusinessProcessEditing").ToString();
			else if (FromId != PrimaryKeyId.Empty)
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "BusinessProcessDuplication").ToString();
			else
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "New").ToString();

			string link = GetOwnerLink();
			string text = CHelper.GetIconText(GetGlobalResourceObject("IbnFramework.Common", "Back").ToString(), ResolveClientUrl("~/Images/IbnFramework/cancel.GIF"));
			if (!String.IsNullOrEmpty(link))
				MainHeader.AddLink(text, link);

		} 
		#endregion

		#region BindData
		private void BindData()
		{
			if (instance == null)	// new
			{
				SchemaMaster[] list = SchemaManager.GetAvailableShemaMasters();
				if (list != null && list.Length > 0)
				{
					List<ListItem> listItems = new List<ListItem>();
					foreach (SchemaMaster item in list)
					{
						string title = item.Description.Comment;
						if (string.IsNullOrEmpty(title))
							title = item.Description.Name;

						listItems.Add(new ListItem(CHelper.GetResFileString(title), item.Description.Id.ToString()));
					}

					listItems.Sort(delegate(ListItem x, ListItem y) { return x.Text.CompareTo(y.Text); });
					SchemaMasterList.Items.AddRange(listItems.ToArray());
					SchemaMasterList.SelectedIndex = 0;

					SchemaMaster currentShemaMaster = null;;
					string selectedItem = SchemaMasterList.SelectedValue;
					foreach (SchemaMaster item in list)
					{
						if (item.Description.Id.ToString() == selectedItem)
						{
							currentShemaMaster = item;
							break;
						}
					}
					ControlName = currentShemaMaster.Description.UI.CreateControl;
				}
			}
			else // edit
			{
				NameText.Text = instance.Name;
				if (instance.Description != null)
					DescriptionText.Text = instance.Description;

				SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);

				string title = currentShemaMaster.Description.Comment;
				if (string.IsNullOrEmpty(title))
					title = currentShemaMaster.Description.Name;

				SchemaMasterList.Items.Add(new ListItem(CHelper.GetResFileString(title), currentShemaMaster.Description.Id.ToString()));
				SchemaMasterList.Enabled = false;

				// Lock Library
				LockLibraryCheckBox.Checked = WorkflowParameters.GetOwnerReadOnly(instance);

				// Overdue Action
				CHelper.SafeSelect(AssignmentOverdueActionList, ((int)WorkflowParameters.GetAssignmentOverdueAction(instance)).ToString());

				ControlName = currentShemaMaster.Description.UI.EditControl;
			}
		} 
		#endregion

		#region LoadControlToPlaceHolder
		private void LoadControlToPlaceHolder(bool bindData)
		{
			if (MainPlaceHolder.Controls.Count > 0)
				MainPlaceHolder.Controls.Clear();

			if (!String.IsNullOrEmpty(ControlName))
			{
				MCDataSavedControl control = (MCDataSavedControl)LoadControl(ControlName);
				control.ID = "MCControl";
				MainPlaceHolder.Controls.Add(control);

				if (bindData)
				{
					if (instance != null)
					{
						SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);
						object rootActivity = McWorkflowSerializer.GetObject(instance.Xaml);

						WorkflowDescription wfDescription = null;

						if (ObjectId != PrimaryKeyId.Empty)
							wfDescription = new WorkflowDescription((Guid)instance.PrimaryKeyId.Value, instance.Name, currentShemaMaster, rootActivity);
						else if (FromId != PrimaryKeyId.Empty)
							wfDescription = new WorkflowDescription(Guid.NewGuid(), instance.Name, currentShemaMaster, rootActivity);

						if (wfDescription != null)
						{
							wfDescription.PlanFinishTimeType = (TimeType)instance.PlanFinishTimeType;

							int duration = 60 * 24; // 1 day
							if (instance.PlanDuration.HasValue && instance.PlanDuration.Value > 0)
								duration = instance.PlanDuration.Value;
							wfDescription.PlanDuration = duration;

							wfDescription.PlanFinishDate = instance.PlanFinishDate;
						}

						control.DataItem = wfDescription;
					}
					else
					{
						control.DataItem = null;
					}

					control.DataBind();
				}
			}
		}
		#endregion

		#region SchemaMasterList_SelectedIndexChanged
		protected void SchemaMasterList_SelectedIndexChanged(object sender, EventArgs e)
		{
			SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(new Guid(SchemaMasterList.SelectedValue));
			ControlName = currentShemaMaster.Description.UI.CreateControl;

			LoadControlToPlaceHolder(true);
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			if (ObjectId != PrimaryKeyId.Empty)	// edit
			{
				// Step 1. Modify instance
				instance.Name = NameText.Text.Trim();
				instance.Description = DescriptionText.Text;

				// Step 2. Create WorkflowChangesScope
				using (WorkflowChangesScope scope = WorkflowChangesScope.Create(instance))
				{
					SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);

					WorkflowDescription wfDescription = new WorkflowDescription((Guid)instance.PrimaryKeyId.Value, instance.Name, currentShemaMaster, scope.TransientWorkflow);

					// Step 3. Apply Modifications
					if (MainPlaceHolder.Controls.Count > 0)
					{
						MCDataSavedControl control = (MCDataSavedControl)MainPlaceHolder.Controls[0];
						control.Save(wfDescription);
					}

					instance.PlanFinishTimeType = (int)wfDescription.PlanFinishTimeType;
					if (wfDescription.PlanFinishTimeType == TimeType.Duration)
						instance.PlanDuration = wfDescription.PlanDuration;
					else if (wfDescription.PlanFinishTimeType == TimeType.DateTime)
						instance.PlanFinishDate = wfDescription.PlanFinishDate;

					// Lock Library
					WorkflowParameters.SetOwnerReadOnly(instance, LockLibraryCheckBox.Checked);

					// Overdue Action
					WorkflowParameters.SetAssignmentOverdueAction(instance, (AssignmentOverdueAction)int.Parse(AssignmentOverdueActionList.SelectedValue));

					// Step 4. Accept Changes
					scope.ApplyWorkflowChanges();
				}
			}
			else // new or duplicate
			{
				// Step 1. Initialize a new instance
				instance = BusinessManager.InitializeEntity<WorkflowInstanceEntity>(WorkflowInstanceEntity.ClassName);
				instance.Name = NameText.Text.Trim();
				instance.Description = DescriptionText.Text;

				if (!String.IsNullOrEmpty(OwnerName) && OwnerId > 0)
					instance[OwnerName] = OwnerId;

				instance.SchemaId = new Guid(SchemaMasterList.SelectedValue);

				SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);
				object rootActivity = currentShemaMaster.InstanceFactory.CreateInstance();

				instance.Xaml = McWorkflowSerializer.GetString(rootActivity);

				// Step 2. Create WorkflowChangesScope
				WorkflowDescription wfDescription = new WorkflowDescription(instance.Name, currentShemaMaster, rootActivity);

				// Step 3. Apply Modifications
				if (MainPlaceHolder.Controls.Count > 0)
				{
					MCDataSavedControl control = (MCDataSavedControl)MainPlaceHolder.Controls[0];
					control.Save(wfDescription);
				}

				instance.PlanFinishTimeType = (int)wfDescription.PlanFinishTimeType;
				if (wfDescription.PlanFinishTimeType == TimeType.Duration)
					instance.PlanDuration = wfDescription.PlanDuration;
				else if (wfDescription.PlanFinishTimeType == TimeType.DateTime)
					instance.PlanFinishDate = wfDescription.PlanFinishDate;

				// Lock Library
				WorkflowParameters.SetOwnerReadOnly(instance, LockLibraryCheckBox.Checked);

				// Overdue Action
				WorkflowParameters.SetAssignmentOverdueAction(instance, (AssignmentOverdueAction)int.Parse(AssignmentOverdueActionList.SelectedValue));

				// Step 4. Accept Changes
				instance.Xaml = McWorkflowSerializer.GetString(wfDescription.TransientWorkflow);
				BusinessManager.Create(instance);
			}

			// Final Redirect
			RedirectToOwner();
		} 
		#endregion

		#region CancelButton_ServerClick
		protected void CancelButton_ServerClick(object sender, EventArgs e)
		{
			RedirectToOwner();
		} 
		#endregion

		#region RedirectToOwner
		private void RedirectToOwner()
		{
			string link = GetOwnerLink();
			if (!String.IsNullOrEmpty(link))
				Response.Redirect(link);
		} 
		#endregion

		#region GetOwnerLink
		private string GetOwnerLink()
		{
			string retval = String.Empty;

			if (instance == null) // new
			{
				if (!String.IsNullOrEmpty(OwnerName) && OwnerId > 0)
				{
					retval = CHelper.GetLinkObjectViewByOwnerName(OwnerName, OwnerId.ToString());
				}
			}
			else
			{
				string ownerName = string.Empty;
				if (instance.OwnerDocumentId != null)
					ownerName = WorkflowInstanceEntity.FieldOwnerDocumentId;

				if (!String.IsNullOrEmpty(ownerName))
				{
					retval = CHelper.GetLinkObjectViewByOwnerName(ownerName, instance.OwnerDocumentId.ToString());
				}
			}

			return retval;
		}
		#endregion
	}
}