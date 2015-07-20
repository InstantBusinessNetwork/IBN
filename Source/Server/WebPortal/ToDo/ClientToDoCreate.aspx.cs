using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using Mediachase.UI.Web.Util;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.ToDo
{
	/// <summary>
	/// Summary description for ClientToDoCreate.
	/// </summary>
	public partial class ClientToDoCreate : System.Web.UI.Page
	{
		private ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoEdit", typeof(ClientToDoCreate).Assembly);
		private int ToDoID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			if (!Page.IsPostBack)
			{
				ApplyLocalization();
				BindValues();
			}
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");

		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblTitleTitle.Text = LocRM.GetString("Title");
			lblProjectTitle.Text = LocRM.GetString("Project"); ;
			lblManagerTitle.Text = LocRM.GetString("Manager");
			lblStartDateTitle.Text = LocRM.GetString("StartDate");
			lblEndDateTitle.Text = LocRM.GetString("EndDate");
			lblPriorityTitle.Text = LocRM.GetString("Priority");
			lblCompletionTitle.Text = LocRM.GetString("Completion");
			lblDescriptionTitle.Text = LocRM.GetString("Description");
			chbMustBeConfirmed.Text = LocRM.GetString("MustConfirm");
			lblFileLoad.Text = LocRM.GetString("FileLoad");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			ddlPriority.DataSource = Mediachase.IBN.Business.ToDo.GetListPriorities();
			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataBind();
			ddlPriority.ClearSelection();
			
			ddlCompletionType.DataSource = Mediachase.IBN.Business.ToDo.GetListCompletionTypes();
			ddlCompletionType.DataTextField = "CompletionTypeName";
			ddlCompletionType.DataValueField = "CompletionTypeId";
			ddlCompletionType.DataBind();

			if (Configuration.ProjectManagementEnabled)
			{
				ddlProject.DataSource = Mediachase.IBN.Business.ToDo.GetListProjects();
				ddlProject.DataTextField = "Title";
				ddlProject.DataValueField = "ProjectId";
				ddlProject.DataBind();
			}
			else
				trProject.Visible = false;

			ListItem liNew = new ListItem(LocRM.GetString("ProjectNotSet"), "-1");
			ddlProject.Items.Insert(0, liNew);
			ddlProject.DataSource = null;
			ddlProject.DataBind();

			ArrayList alManagers = new ArrayList();
			using (IDataReader rManagers = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
			{
				while (rManagers.Read())
				{
					ListItem li = new ListItem(rManagers["LastName"].ToString() + " " + rManagers["FirstName"].ToString(), rManagers["UserId"].ToString());
					alManagers.Add(li);
				}
			}

			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				for (int i = 0; i < alManagers.Count; i++)
					ddlManager.Items.Add((ListItem)alManagers[i]);
				Util.CommonHelper.SafeSelect(ddlManager, Security.CurrentUser.UserID.ToString());
				txtManagerId.Value = "0";
			}
			else
			{
				lblManager.Visible = true;
				ddlManager.Visible = false;
				lblManager.Text = CommonHelper.GetUserStatus((Security.CurrentUser.UserID));
				txtManagerId.Value = Security.CurrentUser.UserID.ToString();
			}

			dtcStartDate.DefaultTimeString = PortalConfig.WorkTimeStart;
			dtcEndDate.DefaultTimeString = PortalConfig.WorkTimeFinish;

			CommonHelper.SafeSelect(ddlPriority, PortalConfig.ToDoDefaultValuePriorityField);
			CommonHelper.SafeSelect(ddlCompletionType, PortalConfig.ToDoDefaultValueCompetionTypeField);
			chbMustBeConfirmed.Checked = bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField);

			trPriority.Visible = PortalConfig.ToDoAllowEditPriorityField;
			trCompletion.Visible = PortalConfig.ToDoAllowEditCompletionTypeField;
			trMustConfirm.Visible = PortalConfig.ToDoAllowEditMustConfirmField;
			trAttach.Visible = PortalConfig.ToDoAllowEditAttachmentField;
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator1_ServerValidate);
			this.btnCancel.ServerClick += new EventHandler(btnCancel_ServerClick);
		}
		#endregion

		#region ddlProject_SelectedIndexChanged
		protected void ddlProject_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int iProjectId = int.Parse(((DropDownList)sender).SelectedValue);
			if (iProjectId > 0)
			{
				int iManagerId = CalendarEntry.GetProjectManager(iProjectId);
				ddlManager.Visible = false;
				lblManager.Visible = true;
				lblManager.Text = CommonHelper.GetUserStatus(iManagerId);
				txtManagerId.Value = iManagerId.ToString();
			}
			else
			{
				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				{
					lblManager.Visible = false;
					ddlManager.Visible = true;
					txtManagerId.Value = "0";
				}
				else
				{
					lblManager.Visible = true;
					ddlManager.Visible = false;
					lblManager.Text = CommonHelper.GetUserStatus((Security.CurrentUser.UserID));
					txtManagerId.Value = Security.CurrentUser.UserID.ToString();
				}
			}
		}
		#endregion

		#region CustomValidator1_ServerValidate
		private void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			DateTime dmv = DateTime.MinValue.AddDays(1);
			DateTime ed = dtcEndDate.SelectedDate;
			DateTime sd = dtcStartDate.SelectedDate;

			if (((ed > dmv) && (sd > dmv)) && (ed < sd))
			{
				CustomValidator1.ErrorMessage = LocRM.GetString("EndDateError") + " (" + dtcStartDate.SelectedDate.ToShortDateString() + " " + dtcStartDate.SelectedDate.ToShortTimeString() + ")";
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}
		#endregion

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();

			if (!Page.IsValid)
				return;

			int iManagerId = 0;
			iManagerId = int.Parse(txtManagerId.Value);
			if (iManagerId == 0)
				iManagerId = int.Parse(ddlManager.SelectedValue);

			ArrayList categories = Mediachase.IBN.Business.Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField);
			PrimaryKeyId org_id = PrimaryKeyId.Empty;
			PrimaryKeyId contact_id = PrimaryKeyId.Empty;
			Mediachase.IBN.Business.Common.GetDefaultClient(PortalConfig.ToDoDefaultValueClientField, out contact_id, out org_id);

			if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
				ToDoID = Mediachase.IBN.Business.ToDo.Create(int.Parse(ddlProject.SelectedItem.Value), iManagerId, txtTitle.Text, txtDescription.Text,
					dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
					int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
					int.Parse(ddlCompletionType.SelectedItem.Value), chbMustBeConfirmed.Checked, -1,
					categories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream, 
					contact_id, org_id);
			else
				ToDoID = Mediachase.IBN.Business.ToDo.Create(int.Parse(ddlProject.SelectedItem.Value), iManagerId, txtTitle.Text, txtDescription.Text,
					dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
					int.Parse(PortalConfig.ToDoDefaultValueActivationTypeField),
					int.Parse(ddlCompletionType.SelectedItem.Value), chbMustBeConfirmed.Checked, -1,
					categories, null, null, contact_id, org_id);

			DataTable dt = Mediachase.IBN.Business.ToDo.GetListResourcesDataTable(ToDoID);

			if (Request["users"] != null)
			{
				string sUsers = Request["users"].ToString();
				while (sUsers.EndsWith(","))
					sUsers = sUsers.Substring(0, sUsers.Length - 1);

				string[] _mUsers = sUsers.Split(',');
				foreach (string s_id in _mUsers)
				{
					using (IDataReader _user = Mediachase.IBN.Business.User.GetUserInfoByOriginalId(int.Parse(s_id)))
					{
						if (_user.Read())
						{
							int iUserId = (int)_user["UserId"];

							DataRow dr = dt.NewRow();
							dr["UserId"] = iUserId;
							dr["MustBeConfirmed"] = true;
							dr["ResponsePending"] = true;
							dt.Rows.Add(dr);
						}
					}
				}
				Mediachase.IBN.Business.ToDo2.UpdateResources(ToDoID, dt);
			}

			Response.Redirect("../ToDo/ToDoView.aspx?ToDoId=" + ToDoID);
		}
		#endregion

		private void btnCancel_ServerClick(object sender, EventArgs e)
		{
			Response.Redirect("~/Apps/Shell/Pages/default.aspx");
		}
	}
}
