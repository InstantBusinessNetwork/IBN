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
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkflowDefinitionEdit : System.Web.UI.UserControl
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

		#region FromInstanceId
		protected PrimaryKeyId FromInstanceId
		{
			get
			{
				PrimaryKeyId retval = PrimaryKeyId.Empty;
				if (!String.IsNullOrEmpty(Request.QueryString["FromInstanceId"]))
					retval = PrimaryKeyId.Parse(Request.QueryString["FromInstanceId"]);

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

		#region TemplateGroupValue
		public object TemplateGroupValue
		{
			set
			{
				if (value != null)
					CHelper.SafeSelect(TemplateGroupList, value.ToString());
			}
			get
			{
				if (TemplateGroupList.Items.Count > 0)
					return int.Parse(TemplateGroupList.SelectedValue);
				else
					return null;
			}
		}
		#endregion

		WorkflowDefinitionEntity template = null;
		WorkflowInstanceEntity instance = null;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (ObjectId != PrimaryKeyId.Empty)
				template = (WorkflowDefinitionEntity)BusinessManager.Load(WorkflowDefinitionEntity.ClassName, ObjectId);
			else if (FromInstanceId != PrimaryKeyId.Empty)
				instance = (WorkflowInstanceEntity)BusinessManager.Load(WorkflowInstanceEntity.ClassName, FromInstanceId);

			if (!IsPostBack)
			{
				BindInfo();		// texts, icons
				BindData();
				BindToolbar();
			}

			LoadControlToPlaceHolder(!IsPostBack);

			if (!this.Page.ClientScript.IsStartupScriptRegistered(this.Page.GetType(), "EnumEdit"))
				this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "EnumEdit",
					@"function enumEdit_OpenWindow(query, w, h, resize){
						var l = (screen.width - w) / 2;
						var t = (screen.height - h) / 2;
						
						winprops = 'height='+h+',width='+w+',top='+t+',left='+l;
						if (scroll) winprops+=',scrollbars=1';
						if (resize) 
							winprops+=',resizable=1';
						else
							winprops+=',resizable=0';
						var f = window.open(query, '_blank', winprops);
					}", true);
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
			if (template == null)
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "NewTemplate").ToString();
			else
				MainHeader.Title = GetGlobalResourceObject("IbnFramework.BusinessProcess", "TemplateEditing").ToString();

			string link = CHelper.GetLinkEntityList(WorkflowDefinitionEntity.ClassName);
			string text = CHelper.GetIconText(GetGlobalResourceObject("IbnFramework.Common", "Back").ToString(), ResolveClientUrl("~/Images/IbnFramework/cancel.GIF"));
			if (!String.IsNullOrEmpty(link))
				MainHeader.AddLink(text, link);
		}
		#endregion

		#region BindData
		private void BindData()
		{
			RebuildTemplateGroupList();

			if (instance != null)	// create from instance
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

				ControlName = currentShemaMaster.Description.UI.EditControl;

				// Object types
				DisableUnsupportedObjectTypes(currentShemaMaster);

				// Lock Library
				LockLibraryCheckBox.Checked = WorkflowParameters.GetOwnerReadOnly(instance);

				// Overdue Action
				CHelper.SafeSelect(AssignmentOverdueActionList, ((int)WorkflowParameters.GetAssignmentOverdueAction(instance)).ToString());
			}
			else if (template != null)	// edit
			{
				NameText.Text = template.Name;
				if (template.Description != null)
					DescriptionText.Text = template.Description;

				SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(template.SchemaId);

				string title = currentShemaMaster.Description.Comment;
				if (string.IsNullOrEmpty(title))
					title = currentShemaMaster.Description.Name;

				SchemaMasterList.Items.Add(new ListItem(CHelper.GetResFileString(title), currentShemaMaster.Description.Id.ToString()));
				SchemaMasterList.Enabled = false;

				ControlName = currentShemaMaster.Description.UI.EditControl;

				if (template.ProjectId.HasValue)
					ProjectControl.ObjectId = (int)template.ProjectId.Value;

				// Object types
				DisableUnsupportedObjectTypes(currentShemaMaster);

				List<int> selectedObjectTypes = new List<int>(template.SupportedIbnObjectTypes);
				DocumentCheckBox.Checked = selectedObjectTypes.Contains((int)ObjectTypes.Document);
				IncidentCheckBox.Checked = selectedObjectTypes.Contains((int)ObjectTypes.Issue);
				TaskCheckBox.Checked = selectedObjectTypes.Contains((int)ObjectTypes.Task);
				TodoCheckBox.Checked = selectedObjectTypes.Contains((int)ObjectTypes.ToDo);

				// Lock Library
				LockLibraryCheckBox.Checked = WorkflowParameters.GetOwnerReadOnly(template);

				// Overdue Action
				CHelper.SafeSelect(AssignmentOverdueActionList, ((int)WorkflowParameters.GetAssignmentOverdueAction(template)).ToString());

				// Template Group
				TemplateGroupValue = template.TemplateGroup;
			}
			else // new
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

					SchemaMaster currentShemaMaster = null; ;
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

					DisableUnsupportedObjectTypes(currentShemaMaster);
				}
			}

			string url = String.Format(CultureInfo.InvariantCulture,
				"{0}?type=TemplateGroups&btn={1}", 
				ResolveClientUrl("~/Apps/MetaDataBase/Pages/Public/EnumView.aspx"),
				Page.ClientScript.GetPostBackEventReference(RefreshButton, ""));

			EditItemsButton.Attributes.Add("onclick",
				String.Format(CultureInfo.InvariantCulture, "enumEdit_OpenWindow(\"{0}\", 750, 500, 1)", url));
		}
		#endregion

		#region DisableUnsupportedObjectTypes
		private void DisableUnsupportedObjectTypes(SchemaMaster currentShemaMaster)
		{
			// enable all
			DocumentCheckBox.Enabled = true;
			TaskCheckBox.Enabled = true;
			TodoCheckBox.Enabled = true;
			IncidentCheckBox.Enabled = true;

			if (currentShemaMaster != null)
			{
				List<int> supportedIbnObjectTypes = currentShemaMaster.Description.SupportedIbnObjectTypes;
				if (supportedIbnObjectTypes.Count > 0) // not all supported
				{
					if (!supportedIbnObjectTypes.Contains((int)ObjectTypes.Document))
					{
						DocumentCheckBox.Enabled = false;
					}
					if (!supportedIbnObjectTypes.Contains((int)ObjectTypes.Task))
					{
						TaskCheckBox.Enabled = false;
					}
					if (!supportedIbnObjectTypes.Contains((int)ObjectTypes.ToDo))
					{
						TodoCheckBox.Enabled = false;
					}
					if (!supportedIbnObjectTypes.Contains((int)ObjectTypes.Issue))
					{
						IncidentCheckBox.Enabled = false;
					}
				}
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
					if (instance != null)	// create from instance
					{
						SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(instance.SchemaId);
						object rootActivity = McWorkflowSerializer.GetObject(instance.Xaml);

						WorkflowDescription wfDescription = new WorkflowDescription(Guid.NewGuid(), instance.Name, currentShemaMaster, rootActivity);

						wfDescription.PlanFinishTimeType = (TimeType)instance.PlanFinishTimeType;

						int duration = 60 * 24; // 1 day
						if (instance.PlanDuration.HasValue && instance.PlanDuration.Value > 0)
							duration = instance.PlanDuration.Value;
						wfDescription.PlanDuration = duration;
						wfDescription.PlanFinishDate = instance.PlanFinishDate;

						control.DataItem = wfDescription;
					}
					else if (template != null)	// edit
					{
						SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(template.SchemaId);
						object rootActivity = McWorkflowSerializer.GetObject(template.Xaml);

						WorkflowDescription wfDescription = new WorkflowDescription((Guid)template.PrimaryKeyId.Value, template.Name, currentShemaMaster, rootActivity);

						wfDescription.PlanFinishTimeType = (TimeType)template.PlanFinishTimeType;
						int duration = 60 * 24; // 1 day
						if (template.PlanDuration.HasValue && template.PlanDuration.Value > 0)
							duration = template.PlanDuration.Value;
						wfDescription.PlanDuration = duration;
						wfDescription.PlanFinishDate = template.PlanFinishDate;

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
			DisableUnsupportedObjectTypes(currentShemaMaster);

			LoadControlToPlaceHolder(true);
		}
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			if (template != null) // edit
			{
				// Step 1. Modify template
				template.Name = NameText.Text.Trim();
				template.Description = DescriptionText.Text;

				SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(template.SchemaId);

				object rootActivity = McWorkflowSerializer.GetObject(template.Xaml);

				if (ProjectControl.ObjectId > 0)
					template.ProjectId = ProjectControl.ObjectId;
				else
					template.ProjectId = null;

				// Object types
				List<int> selectedObjectTypes = new List<int>();
				if (DocumentCheckBox.Enabled && DocumentCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.Document);
				if (TaskCheckBox.Enabled && TaskCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.Task);
				if (TodoCheckBox.Enabled && TodoCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.ToDo);
				if (IncidentCheckBox.Enabled && IncidentCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.Issue);

				template.SupportedIbnObjectTypes = selectedObjectTypes.ToArray();

				// Step 2. Create WorkflowDescription
				WorkflowDescription wfDescription = new WorkflowDescription((Guid)template.PrimaryKeyId.Value, template.Name, currentShemaMaster, rootActivity);

				// Step 3. Apply Modifications
				if (MainPlaceHolder.Controls.Count > 0)
				{
					MCDataSavedControl control = (MCDataSavedControl)MainPlaceHolder.Controls[0];
					control.Save(wfDescription);
				}

				template.PlanFinishTimeType = (int)wfDescription.PlanFinishTimeType;
				if (wfDescription.PlanFinishTimeType == TimeType.Duration)
				{
					template.PlanDuration = wfDescription.PlanDuration;
					template.PlanFinishDate = null;
				}
				else if (wfDescription.PlanFinishTimeType == TimeType.DateTime)
				{
					template.PlanFinishDate = wfDescription.PlanFinishDate;
					template.PlanDuration = null;
				}

				template.Xaml = McWorkflowSerializer.GetString(wfDescription.TransientWorkflow);

				// Template Group
				if (TemplateGroupValue != null)
					template.TemplateGroup = (int)TemplateGroupValue;

				// Lock Library
				WorkflowParameters.SetOwnerReadOnly(template, LockLibraryCheckBox.Checked);

				// Overdue Action
				WorkflowParameters.SetAssignmentOverdueAction(template, (AssignmentOverdueAction)int.Parse(AssignmentOverdueActionList.SelectedValue));

				BusinessManager.Update(template);
			}
			else // new or create from instance
			{
				// Step 1. Initialize a new template
				template = BusinessManager.InitializeEntity<WorkflowDefinitionEntity>(WorkflowDefinitionEntity.ClassName);
				template.Name = NameText.Text.Trim();
				template.Description = DescriptionText.Text;

				template.SchemaId = new Guid(SchemaMasterList.SelectedValue);

				SchemaMaster currentShemaMaster = SchemaManager.GetShemaMaster(template.SchemaId);
				object rootActivity = currentShemaMaster.InstanceFactory.CreateInstance();

				if (ProjectControl.ObjectId > 0)
					template.ProjectId = ProjectControl.ObjectId;

				// Object types
				List<int> selectedObjectTypes = new List<int>();
				if (DocumentCheckBox.Enabled && DocumentCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.Document);
				if (TaskCheckBox.Enabled && TaskCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.Task);
				if (TodoCheckBox.Enabled && TodoCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.ToDo);
				if (IncidentCheckBox.Enabled && IncidentCheckBox.Checked)
					selectedObjectTypes.Add((int)ObjectTypes.Issue);

				template.SupportedIbnObjectTypes = selectedObjectTypes.ToArray();

				// Step 2. Create WorkflowDescription
				WorkflowDescription wfDescription = new WorkflowDescription(template.Name, currentShemaMaster, rootActivity);

				// Step 3. Apply Modifications
				if (MainPlaceHolder.Controls.Count > 0)
				{
					MCDataSavedControl control = (MCDataSavedControl)MainPlaceHolder.Controls[0];
					control.Save(wfDescription);
				}

				template.PlanFinishTimeType = (int)wfDescription.PlanFinishTimeType;
				if (wfDescription.PlanFinishTimeType == TimeType.Duration)
				{
					template.PlanDuration = wfDescription.PlanDuration;
					template.PlanFinishDate = null;
				}
				else if (wfDescription.PlanFinishTimeType == TimeType.DateTime)
				{
					template.PlanFinishDate = wfDescription.PlanFinishDate;
					template.PlanDuration = null;
				}

				template.Xaml = McWorkflowSerializer.GetString(wfDescription.TransientWorkflow);

				// Template Group
				if (TemplateGroupValue != null)
					template.TemplateGroup = (int)TemplateGroupValue;

				// Lock Library
				WorkflowParameters.SetOwnerReadOnly(template, LockLibraryCheckBox.Checked);

				// Overdue Action
				WorkflowParameters.SetAssignmentOverdueAction(template, (AssignmentOverdueAction)int.Parse(AssignmentOverdueActionList.SelectedValue));

				BusinessManager.Create(template);
			}

			// Step Final. Redirect
			RedirectToList();
		}
		#endregion

		#region CancelButton_ServerClick
		protected void CancelButton_ServerClick(object sender, EventArgs e)
		{
			RedirectToList();
		}
		#endregion

		#region RedirectToList
		private void RedirectToList()
		{
			string link = CHelper.GetLinkEntityList(WorkflowDefinitionEntity.ClassName);
			if (!String.IsNullOrEmpty(link))
				Response.Redirect(link);
		}
		#endregion

		#region RebuildTemplateGroupList
		private void RebuildTemplateGroupList()
		{
			object savedValue = TemplateGroupValue;

			TemplateGroupList.Items.Clear();
			foreach (MetaEnumItem item in MetaEnum.GetItems(DataContext.Current.MetaModel.RegisteredTypes["TemplateGroups"]))
			{
				string text = CHelper.GetResFileString(item.Name);
				TemplateGroupList.Items.Add(new ListItem(text, item.Handle.ToString()));
			}

			TemplateGroupValue = savedValue;
		}
		#endregion

		#region RefreshButton_Click
		protected void RefreshButton_Click(object sender, EventArgs e)
		{
			RebuildTemplateGroupList();
		}
		#endregion
	}
}