using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Resources;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Projects
{
	public partial class EditSecurity : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditSecurity).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditSecurity).Assembly);

		#region ProjectId
		private int ProjectId
		{
			get
			{
				try
				{
					return int.Parse(Request["ProjectId"]);
				}
				catch
				{
					return -1;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			if (!IsPostBack)
				BindValues();

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close()");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region BindValues
		private void BindValues()
		{
			Dictionary<string, string> prop = Project.GetProperties(ProjectId);

			ToDoExecutiveCheckbox.Checked = prop.ContainsKey(Project.CanProjectExecutiveCreateTodo);
			ToDoTeamCheckbox.Checked = prop.ContainsKey(Project.CanProjectTeamCreateTodo);
			ToDoSponsorCheckbox.Checked = prop.ContainsKey(Project.CanProjectSponsorCreateTodo);
			ToDoStakeholderCheckbox.Checked = prop.ContainsKey(Project.CanProjectStakeholderCreateTodo);

			DocumentExecutiveCheckbox.Checked = prop.ContainsKey(Project.CanProjectExecutiveCreateDocument);
			DocumentTeamCheckbox.Checked = prop.ContainsKey(Project.CanProjectTeamCreateDocument);
			DocumentSponsorCheckbox.Checked = prop.ContainsKey(Project.CanProjectSponsorCreateDocument);
			DocumentStakeholderCheckbox.Checked = prop.ContainsKey(Project.CanProjectStakeholderCreateDocument);

			TaskExecutiveCheckbox.Checked = prop.ContainsKey(Project.CanProjectExecutiveCreateTask);
			TaskTeamCheckbox.Checked = prop.ContainsKey(Project.CanProjectTeamCreateTask);
			TaskSponsorCheckbox.Checked = prop.ContainsKey(Project.CanProjectSponsorCreateTask);
			TaskStakeholderCheckbox.Checked = prop.ContainsKey(Project.CanProjectStakeholderCreateTask);

			HideManagementCheckbox.Checked = prop.ContainsKey(Project.HideManagementForProjectTeam);

			ToDoManagerCheckbox.Text = DocumentManagerCheckbox.Text = TaskManagerCheckbox.Text = LocRM2.GetString("manager");
			ToDoExecutiveCheckbox.Text = DocumentExecutiveCheckbox.Text = TaskExecutiveCheckbox.Text = LocRM2.GetString("exec_manager");
			ToDoTeamCheckbox.Text = DocumentTeamCheckbox.Text = TaskTeamCheckbox.Text = LocRM2.GetString("ProjectTeam");
			ToDoSponsorCheckbox.Text = DocumentSponsorCheckbox.Text = TaskSponsorCheckbox.Text = LocRM2.GetString("ProjectSponsors");
			ToDoStakeholderCheckbox.Text = DocumentStakeholderCheckbox.Text = TaskStakeholderCheckbox.Text = LocRM2.GetString("ProjectStakeholders");
			HideManagementCheckbox.Text = LocRM2.GetString("HideManagement");

			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, EventArgs e)
		{
			Dictionary<string, bool> properties = new Dictionary<string, bool>();
			properties.Add(Project.CanProjectExecutiveCreateTodo, ToDoExecutiveCheckbox.Checked);
			properties.Add(Project.CanProjectTeamCreateTodo, ToDoTeamCheckbox.Checked);
			properties.Add(Project.CanProjectStakeholderCreateTodo, ToDoStakeholderCheckbox.Checked);
			properties.Add(Project.CanProjectSponsorCreateTodo, ToDoSponsorCheckbox.Checked);

			properties.Add(Project.CanProjectExecutiveCreateDocument, DocumentExecutiveCheckbox.Checked);
			properties.Add(Project.CanProjectTeamCreateDocument, DocumentTeamCheckbox.Checked);
			properties.Add(Project.CanProjectStakeholderCreateDocument, DocumentStakeholderCheckbox.Checked);
			properties.Add(Project.CanProjectSponsorCreateDocument, DocumentSponsorCheckbox.Checked);

			properties.Add(Project.CanProjectExecutiveCreateTask, TaskExecutiveCheckbox.Checked);
			properties.Add(Project.CanProjectTeamCreateTask, TaskTeamCheckbox.Checked);
			properties.Add(Project.CanProjectStakeholderCreateTask, TaskStakeholderCheckbox.Checked);
			properties.Add(Project.CanProjectSponsorCreateTask, TaskSponsorCheckbox.Checked);

			properties.Add(Project.HideManagementForProjectTeam, HideManagementCheckbox.Checked);

			Project.SetSecurityProperties(ProjectId, properties);
			Mediachase.Ibn.Web.UI.CHelper.CloseIt(Response);
		}
		#endregion
	}
}
