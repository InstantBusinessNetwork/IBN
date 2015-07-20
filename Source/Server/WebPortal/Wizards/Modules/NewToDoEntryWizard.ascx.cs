namespace Mediachase.UI.Web.Wizards.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Collections;
	using System.Resources;
	using System.Web;
	using System.Web.SessionState;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Web.Interfaces;



	/// <summary>
	///		Summary description for NewToDoEntryWizard.
	/// </summary>
	public partial class NewToDoEntryWizard : System.Web.UI.UserControl, IWizardControl
	{

		#region HTML Vars
		protected System.Web.UI.HtmlControls.HtmlTable s4TeamSelect;
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strNewTDEWd", typeof(NewToDoEntryWizard).Assembly);
		private int _stepCount = 5;
		ArrayList steps = new ArrayList();
		ArrayList subtitles = new ArrayList();
		public string sUnit;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindDefaultValues();
				s4AssignTeam_CheckedChanged(sender, e);
			}
			InitTeamStep();
		}

		private void BindDefaultValues()
		{
			BindStep1();
		}

		#region BindStep1
		private void BindStep1()
		{
			tbTitle.Text = String.Empty;

			ddlManager.Attributes.Add("onchange", "SaveManagerId();");

			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataSource = ToDo.GetListPriorities();
			ddPriority.DataBind();

			ddProject.DataTextField = "Title";
			ddProject.DataValueField = "ProjectId";
			s4OnlyForMe.Text = LocRM.GetString("s4OnlyForMe");
			chbMustBeConfirmed.Text = LocRM.GetString("s4MustBeConfirmed");

			switch (sUnit)
			{
				case "Entry":
					s2ToDoManager.Style.Add("display", "none");
					ddEntryType.DataTextField = "TypeName";
					ddEntryType.DataValueField = "TypeId";
					ddEntryType.DataSource = CalendarEntry.GetListEventTypes();
					ddEntryType.DataBind();
					CommonHelper.SafeSelect(ddEntryType, "2");
					ddProject.DataSource = CalendarEntry.GetListProjects(Security.CurrentUser.UserID);

					s4AssignTeam.Text = LocRM.GetString("s4AssignParticipants");
					s1ToDoProperties.Visible = false;
					//	DateTimeCommentForToDo.Visible = false;
					dtcStartDate.SelectedDate = UserDateTime.UserNow;
					dtcEndDate.SelectedDate = dtcStartDate.SelectedDate.AddMinutes(30);

					CommonHelper.SafeSelect(ddPriority, PortalConfig.CEntryDefaultValuePriorityField);
					divPriority.Visible = PortalConfig.CommonCEntryAllowEditPriorityField;
					break;
				case "ToDo":
					s2ToDoManager.Style.Add("display", "inline");
					ddToDoCompletionType.DataTextField = "CompletionTypeName";
					ddToDoCompletionType.DataValueField = "CompletionTypeId";
					ddToDoCompletionType.DataSource = ToDo.GetListCompletionTypes();
					ddProject.DataSource = Project.GetListActiveProjectsByManager();
					ddToDoCompletionType.DataBind();
					s4AssignTeam.Text = LocRM.GetString("s4AssignResources");
					s1EntryProperties.Visible = false;
					break;
			}
			ddProject.DataBind();
			ListItem liDefault = new ListItem(LocRM.GetString("s4NotSet"), "-1");
			ddProject.Items.Insert(0, liDefault);

			ArrayList alManagers = new ArrayList();
			using (IDataReader rManagers = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager, false))
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
			}
			else
			{
				lblManager.Visible = true;
				ddlManager.Enabled = false;
				ddlManager.Visible = false;
				lblManager.Text = CommonHelper.GetUserStatusUL((Security.CurrentUser.UserID));
				txtManagerId.Value = Security.CurrentUser.UserID.ToString();
			}

		}
		#endregion

		#region TeamStep
		private void InitTeamStep()
		{
			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnAddGroup.Attributes.Add("onclick", "DisableButtons(this);");
			if (sUnit == "ToDo" && ddToDoCompletionType.SelectedItem.Value == "1")
				btnAddGroup.Visible = false;
			else
				btnAddGroup.Visible = true;
			btnSearch.Text = LocRM.GetString("s4FindNow");
			if (!IsPostBack)
			{
				btnAdd.InnerText = LocRM.GetString("s4Add");
				btnAddGroup.InnerText = LocRM.GetString("s4AddGroup");

				CreateDataTableStructure();
				BindGroups();
				BinddgMemebers();
			}
		}

		private void CreateDataTableStructure()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("UserId", typeof(int));
			dt.Columns.Add("MustBeConfirmed", typeof(bool));
			ViewState["Team"] = dt;

		}

		private void BindGroups()
		{
			ddGroups.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Common", "FavoritesGroup").ToString(), "0"));

			using (IDataReader reader = SecureGroup.GetListGroupsAsTree())
			{
				while (reader.Read())
				{
					string GroupName = CommonHelper.GetResFileString(reader["GroupName"].ToString());
					string GroupId = reader["GroupId"].ToString();
					int Level = (int)reader["Level"];
					for (int i = 1; i < Level; i++)
						GroupName = "  " + GroupName;
					ListItem item = new ListItem(GroupName, GroupId);

					ddGroups.Items.Add(item);
				}
			}

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}

		private void BindGroupUsers(int groupId)
		{
			lbUsers.Items.Clear();
			DataTable dt = (DataTable)ViewState["Team"];
			if (groupId > 0)
			{
				using (IDataReader rdr = SecureGroup.GetListActiveUsersInGroup(groupId))
				{
					while (rdr.Read())
					{
						DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
						if (dr.Length == 0)
							lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() + " " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));

					}
				}
			}
			else // Favorites
			{
				DataTable favorites = Mediachase.IBN.Business.Common.GetListFavoritesDT(ObjectTypes.User);
				foreach (DataRow row in favorites.Rows)
				{
					DataRow[] dr = dt.Select("UserId = " + (int)row["ObjectId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem((string)row["Title"], row["ObjectId"].ToString()));
				}
			}

		}

		private void BindProjectUsers(int ProjId)
		{
			lbUsers.Items.Clear();
			DataTable dt = (DataTable)ViewState["Team"];
			using (IDataReader rdr = Project.GetListTeamMemberNames(ProjId))
			{
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() + " " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));

				}
			}
		}

		private void BinddgMemebers()
		{
			dgMembers.Columns[2].HeaderText = LocRM.GetString("s4Name");

			DataTable dt = (DataTable)ViewState["Team"];
			dgMembers.DataSource = dt.DefaultView;
			dgMembers.DataBind();

			foreach (DataGridItem dgi in dgMembers.Items)
			{
				ImageButton ib = (ImageButton)dgi.FindControl("ibDelete");
				if (ib != null)
					ib.ToolTip = LocRM.GetString("s4Delete");
			}
		}

		protected void ddGroups_ChangeGroup(object sender, System.EventArgs e)
		{
			tbSearch.Text = "";
			ViewState["SearchString"] = null;

			ListItem li = ddGroups.SelectedItem;
			if (li != null)
				BindGroupUsers(int.Parse(li.Value));
		}

		protected void btnAdd_Click(object sender, System.EventArgs e)
		{

			DataTable dt = (DataTable)ViewState["Team"];
			foreach (ListItem li in lbUsers.Items)
				if (li.Selected)
				{
					DataRow dr = dt.NewRow();
					string username = li.Text;

					string fl = String.Empty;
					string ll = String.Empty;

					try
					{
						fl = username.Substring(0, 1).ToUpper();
						ll = username.Substring(username.IndexOf(" ") + 1, 1).ToUpper();
					}
					catch
					{
					}

					dr["UserId"] = int.Parse(li.Value);
					dr["MustBeConfirmed"] = chbMustBeConfirmed.Checked;
					dt.Rows.Add(dr);
				}
			ViewState["Team"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		protected void btnAddGroup_Click(object sender, System.EventArgs e)
		{
			DataTable dt = (DataTable)ViewState["Team"];
			if (ddGroups.SelectedItem != null && dt.Select("UserId=" + ddGroups.SelectedItem.Value).Length == 0 && int.Parse(ddGroups.SelectedValue) > 0)
			{
				DataRow dr = dt.NewRow();
				dr["UserId"] = int.Parse(ddGroups.SelectedItem.Value);
				dr["MustBeConfirmed"] = false;
				dt.Rows.Add(dr);
			}
			ViewState["Team"] = dt;

			ListItem _li = ddGroups.SelectedItem;
			if (_li != null)
				BindUsers();
			BinddgMemebers();
		}

		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			int UserID = int.Parse(e.Item.Cells[1].Text);
			DataTable dt = (DataTable)ViewState["Team"];
			DataRow[] dr = dt.Select("UserId = " + UserID);
			if (dr.Length > 0)
				dt.Rows.Remove(dr[0]);
			ViewState["Team"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			if (tbSearch.Text != "")
			{
				ViewState["SearchString"] = tbSearch.Text;
				BindSearchedUsers(tbSearch.Text);
			}
		}

		private void BindSearchedUsers(string searchstr)
		{
			DataTable dt = (DataTable)ViewState["Team"];
			lbUsers.Items.Clear();
			using (IDataReader rdr = Mediachase.IBN.Business.User.GetListUsersBySubstring(searchstr))
			{
				while (rdr.Read())
				{
					DataRow[] dr = dt.Select("UserId = " + (int)rdr["UserId"]);
					if (dr.Length == 0)
						lbUsers.Items.Add(new ListItem(rdr["LastName"].ToString() + " " + rdr["FirstName"].ToString(), rdr["UserId"].ToString()));
				}
			}
		}

		private void BindUsers()
		{
			int iProjectId = int.Parse(ddProject.SelectedItem.Value);
			if ((iProjectId > 0) && (sUnit == "ToDo"))
				BindProjectUsers(iProjectId);
			else if (ViewState["SearchString"] == null)
			{
				ListItem _li = ddGroups.SelectedItem;
				if (_li != null)
					BindGroupUsers(int.Parse(_li.Value));
			}
			else
			{
				BindSearchedUsers(ViewState["SearchString"].ToString());
			}
		}
		#endregion

		#region ShowStep
		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			if (step == _stepCount)
			{
				lblToDoEntryTitle.Text = tbTitle.Text;
				if (DateTime.MinValue.AddDays(1) >= dtcStartDate.SelectedDate)
				{
					s5StartDateRow.Visible = false;
				}
				else if (DateTime.MinValue.AddDays(1) >= dtcEndDate.SelectedDate)
				{
					s5EndDateRow.Visible = false;
				}
				else
				{
					s5EndDateRow.Visible = true;
					s5StartDateRow.Visible = true;
					lblStartDate.Text = dtcStartDate.SelectedDate.ToString("g");
					lblEndDate.Text = dtcEndDate.SelectedDate.ToString("g");
				}

				switch (sUnit)
				{
					case "ToDo":
						btnAddGroup.Visible = false;
						s5RowForEntry1.Visible = false;
						s5RowForEntry2.Visible = false;
						s5RowForToDo1.Visible = true;
						s5RowForToDo2.Visible = chbToDoMustBeConfirmed.Checked;
						lblCompletionType.Text = ddToDoCompletionType.SelectedItem.Text;
						if (ddlManager.Enabled)
							lblManagerName.Text = ddlManager.SelectedItem.Text;
						else
							lblManagerName.Text = lblManager.Text;
						break;
					case "Entry":
						btnAddGroup.Visible = true;
						s5RowForEntry1.Visible = true;
						s5RowForEntry2.Visible = true;
						s5RowForToDo1.Visible = false;
						s5RowForToDo2.Visible = false;
						lblEntryType.Text = ddEntryType.SelectedItem.Text;
						trManager.Visible = false;
						lblLocation.Text = tbLocation.Text;
						break;
				}
				lblPriority.Text = ddPriority.SelectedItem.Text;
				lblDescription.Text = tbDescription.Text;
				lblProjectTitle.Text = ddProject.SelectedItem.Text;

				s5OnlyRow.Visible = s4OnlyForMe.Checked;
				s5TeamRow.Visible = s4AssignTeam.Checked;
				if (s5TeamRow.Visible)
				{
					DataTable dt = (DataTable)ViewState["Team"];
					dlTeam.DataSource = dt.DefaultView;
					dlTeam.DataBind();
				}
			}

			if (step == _stepCount + 1)
			{
				DataTable dt = (DataTable)ViewState["Team"];
				if (s4OnlyForMe.Checked)
				{
					dt.Clear();
					DataRow dr = dt.NewRow();
					dr["UserId"] = Security.CurrentUser.UserID;
					dr["MustBeConfirmed"] = false;
					dt.Rows.Add(dr);
				}
				int iManagerId;
				if (ddlManager.Enabled)
					iManagerId = int.Parse(ddlManager.SelectedItem.Value);
				else
					iManagerId = int.Parse(txtManagerId.Value);
				int iProjectId = int.Parse(ddProject.SelectedItem.Value);
				int iPriorityId = int.Parse(ddPriority.SelectedItem.Value);
				switch (sUnit)
				{
					case "ToDo":
						int iCompletionType = int.Parse(ddToDoCompletionType.SelectedItem.Value);
						ViewState["UnitID"] = ToDo.CreateFromWizard(iProjectId, iManagerId, tbTitle.Text,
							tbDescription.Text, dtcStartDate.SelectedDate, dtcEndDate.SelectedDate,
							iPriorityId, (int)ActivationTypes.AutoWithCheck, iCompletionType,
							chbToDoMustBeConfirmed.Checked, -1, dt, PrimaryKeyId.Empty, PrimaryKeyId.Empty);
						Response.Redirect("../Wizards/CommonWizard.aspx?ObjectType=6&ObjectID=" + ViewState["UnitID"].ToString());
						break;
					case "Entry":
						int iEntryTypeId = int.Parse(ddEntryType.SelectedItem.Value);
						ViewState["UnitID"] = CalendarEntry.CreateFromWizard(tbTitle.Text, tbDescription.Text,
							tbLocation.Text, iProjectId, iPriorityId, iEntryTypeId, dtcStartDate.SelectedDate,
							dtcEndDate.SelectedDate, dt, PrimaryKeyId.Empty, PrimaryKeyId.Empty);
						Response.Redirect("../Wizards/CommonWizard.aspx?ObjectType=4&ObjectID=" + ViewState["UnitID"].ToString());
						break;
				}
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			try
			{
				sUnit = Request["Unit"].ToString();
			}
			catch
			{
				sUnit = "";
			}
			InitializeComponent();
			base.OnInit(e);

			steps.Add(step1);
			subtitles.Add(LocRM.GetString("s1SubTitle") + " " + LocRM.GetString(sUnit + "PP"));
			if (Configuration.ProjectManagementEnabled)
			{
				steps.Add(step2);
				subtitles.Add(LocRM.GetString("s2SubTitle") + " " + LocRM.GetString(sUnit + "TP"));
			}
			else
			{
				_stepCount--;
				step2.Visible = false;
			}
			steps.Add(step3);
			steps.Add(step4);
			steps.Add(step5);
			steps.Add(step6);
			subtitles.Add(LocRM.GetString("s3SubTitle") + " " + LocRM.GetString(sUnit));
			subtitles.Add(LocRM.GetString("s4SubTitle"));
			subtitles.Add(LocRM.GetString("s5SubTitle") + " " + LocRM.GetString(sUnit + "PP"));
			subtitles.Add(LocRM.GetString("s6SubTitle"));

			if (sUnit == "Entry")
			{
				dtcStartDate.DateIsRequired = true;
				dtcEndDate.DateIsRequired = true;
				ddProject.AutoPostBack = false;
				//dtcStartDate.RelatedControlName = dtcEndDate.Date.ClientID;
			}
			else if (sUnit == "ToDo")
			{
				dtcStartDate.DateIsRequired = false;
				dtcEndDate.DateIsRequired = false;
				dtcStartDate.DefaultTimeString = PortalConfig.WorkTimeStart;
				dtcEndDate.DefaultTimeString = PortalConfig.WorkTimeFinish;
				//dtcStartDate.ViewStartDate = false;
				//dtcEndDate.ViewStartDate = false;
				//dtcStartDate.btnImgResetVisible = true;
				//dtcEndDate.btnImgResetVisible = true;
			}
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator1_ServerValidate);
			this.dgMembers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMembers_Delete);

		}
		#endregion

		#region ddlProject_SelectedIndexChanged
		private void ddlProject_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int iProjectId = int.Parse(((DropDownList)sender).SelectedItem.Value);
			if (iProjectId > 0)
			{
				int iManagerId = ToDo.GetProjectManager(iProjectId);
				ddlManager.Enabled = false;
				ddlManager.Visible = false;
				lblManager.Visible = true;
				lblManager.Text = CommonHelper.GetUserStatusUL(iManagerId);
				txtManagerId.Value = iManagerId.ToString();
			}
			else
			{
				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				{
					lblManager.Visible = false;
					ddlManager.Enabled = true;
					ddlManager.Visible = true;
				}
				else
				{
					lblManager.Visible = true;
					ddlManager.Enabled = false;
					ddlManager.Visible = false;
					lblManager.Text = CommonHelper.GetUserStatusUL((Security.CurrentUser.UserID));
					txtManagerId.Value = Security.CurrentUser.UserID.ToString();
				}
			}
		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("GlobalTitle") + LocRM.GetString(sUnit); } }
		public bool ShowSteps { get { return true; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
		}

		public string GenerateFinalStepScript()
		{
			if (ViewState["UnitID"] != null)
				return "try{window.opener.top.right.location.href='../Projects/ProjectView.aspx?ProjectID=" + ViewState["UnitID"].ToString() + "';} catch (e) {} window.close();";
			else
				return String.Empty;
		}
		#endregion

		public void CancelAction()
		{

		}

		protected void s4AssignTeam_CheckedChanged(object sender, System.EventArgs e)
		{
			TeamTable.Visible = s4AssignTeam.Checked;
		}


		#region ddProject_SelectedIndexChanged
		protected void ddProject_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int iProjectId = int.Parse(((DropDownList)sender).SelectedItem.Value);
			if (iProjectId > 0)
			{

				int iManagerId = ToDo.GetProjectManager(iProjectId);
				ddlManager.Enabled = false;
				ddlManager.Visible = false;
				lblManager.Visible = true;
				lblManager.Text = CommonHelper.GetUserStatusUL(iManagerId);
				txtManagerId.Value = iManagerId.ToString();
				if (sUnit == "ToDo")
				{
					s4GroupRow.Visible = false;
					s4SearchRow.Visible = false;
					btnAddGroup.Visible = false;
					BindUsers();
				}
			}
			else
			{
				if (sUnit == "ToDo")
				{
					s4GroupRow.Visible = true;
					s4SearchRow.Visible = true;
					btnAddGroup.Visible = true;
					BindUsers();
				}

				if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
				{
					lblManager.Visible = false;
					ddlManager.Enabled = true;
					ddlManager.Visible = true;
					ddlManager.ClearSelection();
					ddlManager.SelectedIndex = 0;
					txtManagerId.Value = ddlManager.SelectedItem.Value;
				}
				else
				{
					lblManager.Visible = true;
					ddlManager.Enabled = false;
					ddlManager.Visible = false;
					lblManager.Text = CommonHelper.GetUserStatusUL((Security.CurrentUser.UserID));
					txtManagerId.Value = Security.CurrentUser.UserID.ToString();
				}
			}
		}
		#endregion

		private void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{

			if (((sUnit == "ToDo" && dtcStartDate.SelectedDate > DateTime.MinValue.AddDays(1) && dtcEndDate.SelectedDate > DateTime.MinValue.AddDays(1)) || sUnit == "Entry") && dtcEndDate.SelectedDate < dtcStartDate.SelectedDate)
			{
				CustomValidator1.ErrorMessage = LocRM.GetString("EndDateError");
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}

	}
}
