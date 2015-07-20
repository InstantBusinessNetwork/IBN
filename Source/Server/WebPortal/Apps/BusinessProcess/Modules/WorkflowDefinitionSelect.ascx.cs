using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Core.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.UI.Web.Util;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.Modules
{
	public partial class WorkflowDefinitionSelect : System.Web.UI.UserControl
	{
		#region OwnerTypeId
		protected int OwnerTypeId
		{
			get
			{
				int retval = (int)ObjectTypes.UNDEFINED;
				if (!String.IsNullOrEmpty(Request.QueryString["OwnerTypeId"]))
					retval = int.Parse(Request.QueryString["OwnerTypeId"]);

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

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindGroups();
				CancelButton.Attributes.Add("onclick", Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			}
		}

		#region BindGroups
		private void BindGroups()
		{
			MetaClass mc = Mediachase.Ibn.Core.MetaDataWrapper.GetMetaClassByName("WorkflowDefinition");
			MetaField mf = mc.Fields[WorkflowDefinitionEntity.FieldTemplateGroup];
			TeplateGroupList.DataTextField = "Name";
			TeplateGroupList.DataValueField = "Handle";
			TeplateGroupList.DataSource = MetaEnum.GetItems(mf.GetMetaType());
			TeplateGroupList.DataBind();

			BindTemplates();
		} 
		#endregion

		#region BindTemplates
		private void BindTemplates()
		{
			int projectId = CommonHelper.GetProjectIdByObjectIdObjectType(OwnerId, OwnerTypeId);

			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement(WorkflowDefinitionEntity.FieldSupportedIbnObjectTypes, OwnerTypeId));
			if (projectId > 0)
			{
				// O.R. [2010-02-03]: Allow to select non-project templates for a project
				OrBlockFilterElement orBlock = new OrBlockFilterElement();
				orBlock.ChildElements.Add(FilterElement.EqualElement(WorkflowDefinitionEntity.FieldProjectId, projectId));
				orBlock.ChildElements.Add(FilterElement.IsNullElement(WorkflowDefinitionEntity.FieldProjectId));
				filters.Add(orBlock);
			}
			else
			{
				filters.Add(FilterElement.IsNullElement(WorkflowDefinitionEntity.FieldProjectId));
			}

			if (TeplateGroupList.Items.Count > 0)
				filters.Add(FilterElement.EqualElement(WorkflowDefinitionEntity.FieldTemplateGroup, int.Parse(TeplateGroupList.SelectedValue)));

			TemplateList.Items.Clear();
			foreach (WorkflowDefinitionEntity entity in BusinessManager.List(WorkflowDefinitionEntity.ClassName, filters.ToArray()))
			{
				TemplateList.Items.Add(new ListItem(entity.Name, entity.PrimaryKeyId.Value.ToString()));
			}
		} 
		#endregion

		#region SaveButton_ServerClick
		protected void SaveButton_ServerClick(object sender, EventArgs e)
		{
			WorkflowDefinitionEntity definition = (WorkflowDefinitionEntity)BusinessManager.Load(WorkflowDefinitionEntity.ClassName, PrimaryKeyId.Parse(TemplateList.SelectedValue));

			WorkflowInstanceEntity instance = BusinessManager.InitializeEntity<WorkflowInstanceEntity>(WorkflowInstanceEntity.ClassName);
			instance.Name = definition.Name;
			instance.Description = definition.Description;

			if (!String.IsNullOrEmpty(OwnerName) && OwnerId > 0)
				instance[OwnerName] = OwnerId;

			instance.SchemaId = definition.SchemaId;
			instance.Xaml = definition.Xaml;

			if (definition.PlanFinishTimeType == (int)TimeType.Duration)
			{
				instance.PlanFinishTimeType = (int)TimeType.Duration;
				instance.PlanDuration = definition.PlanDuration;
			}
			else if (definition.PlanFinishTimeType == (int)TimeType.DateTime)
			{
				instance.PlanFinishTimeType = (int)TimeType.DateTime;
				instance.PlanFinishDate = definition.PlanFinishDate;
			}

			//instance.PlanDuration = definition.PlanDuration;
			//if (instance.PlanDuration.HasValue)
			//	instance.PlanFinishTimeType = (int)TimeType.Duration;

			// Lock Library
			WorkflowParameters.SetOwnerReadOnly(instance, WorkflowParameters.GetOwnerReadOnly(definition));

			// Overdue Action
			WorkflowParameters.SetAssignmentOverdueAction(instance, WorkflowParameters.GetAssignmentOverdueAction(definition));

			BusinessManager.Create(instance);

			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, string.Empty);
		}
		#endregion

		#region TeplateGroupList_SelectedIndexChanged
		protected void TeplateGroupList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindTemplates();
		} 
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (TemplateList.Items.Count == 0)
			{
				TemplateList.Visible = false;
				NoTemplatesLabel.Visible = true;
				SaveButton.Disabled = true;
			}
			else
			{
				TemplateList.Visible = true;
				NoTemplatesLabel.Visible = false;
				SaveButton.Disabled = false;
			}
		} 
		#endregion
	}
}