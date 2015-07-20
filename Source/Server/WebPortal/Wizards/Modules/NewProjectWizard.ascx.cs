namespace Mediachase.UI.Web.Wizards.Modules
{
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
	using System.Globalization;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for NewProjectWizard.
	/// </summary>
	public partial class NewProjectWizard : System.Web.UI.UserControl, IWizardControl
	{
		#region HTML Vars
		protected System.Web.UI.WebControls.Label lblDescriptionTitle;
		protected System.Web.UI.WebControls.Label lblScopeTitle;
		protected System.Web.UI.WebControls.Label lblGoalsTitle;
		protected System.Web.UI.WebControls.Label lblDeliverablesTitle;
		protected System.Web.UI.WebControls.Label lblGoals;
		protected System.Web.UI.WebControls.Label lblScope;
		protected System.Web.UI.WebControls.Label lblDeliverables;
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strNewPrjWd", typeof(NewProjectWizard).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(NewProjectWizard).Assembly);

		const int _stepCount = 6;
		ArrayList steps = new ArrayList();
		ArrayList subtitles = new ArrayList();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.ProjectManagementEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if (!IsPostBack)
				BindStep1();
			InitTeamStep();
		}

		#region BindStep1
		private void BindStep1()
		{
			tbTitle.Text = String.Empty;

			ddType.DataValueField = "TypeId";
			ddType.DataTextField = "TypeName";
			ddType.DataSource = Project.GetListProjectTypes();
			ddType.DataBind();

			ddCurrency.DataTextField = "CurrencySymbol";
			ddCurrency.DataValueField = "CurrencyId";
			ddCurrency.DataSource = Project.GetListCurrency();
			ddCurrency.DataBind();

			ddPhase.DataTextField = "PhaseName";
			ddPhase.DataValueField = "PhaseId";
			ddPhase.DataSource = Project.GetListProjectPhases();
			ddPhase.DataBind();

			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataSource = Project.GetListPriorities();
			ddPriority.DataBind();
			ddPriority.SelectedValue = ((int)Priority.Normal).ToString();

			dtcStartDate.SelectedDate = User.GetLocalDate(Security.CurrentUser.TimeZoneId, DateTime.UtcNow.Date);
			dtcEndDate.SelectedDate = dtcStartDate.SelectedDate.AddMonths(1);

			ListItem li = new ListItem(LocRM.GetString("s4NotSet"), "0");
			ddExecutive.Items.Add(li);

			ArrayList alManagers = new ArrayList();
			using (IDataReader iManagers = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
			{
				while (iManagers.Read())
				{
					li = new ListItem(iManagers["LastName"].ToString() + " " + iManagers["FirstName"].ToString(), iManagers["UserId"].ToString());
					alManagers.Add(li);
				}
			}

			for (int i = 0; i < alManagers.Count; i++)
			{
				ListItem liExecItem = new ListItem(((ListItem)alManagers[i]).Text, ((ListItem)alManagers[i]).Value);
				ddExecutive.Items.Add(liExecItem);
			}

			lbCategories.DataTextField = "CategoryName";
			lbCategories.DataValueField = "CategoryId";
			lbCategories.DataSource = Project.GetListProjectCategories();
			lbCategories.DataBind();

			lbPortfolios.DataTextField = "Title";
			lbPortfolios.DataValueField = "ProjectGroupId";
			lbPortfolios.DataSource = ProjectGroup.GetProjectGroups();
			lbPortfolios.DataBind();

			ddlBlockType.DataTextField = "Title";
			ddlBlockType.DataValueField = "primaryKeyId";
			ddlBlockType.DataSource = Mediachase.IbnNext.TimeTracking.TimeTrackingBlockType.List(Mediachase.Ibn.Data.FilterElement.EqualElement("IsProject", "1"));
			ddlBlockType.DataBind();

			ddlCalendar.DataTextField = "CalendarName";
			ddlCalendar.DataValueField = "CalendarId";
			ddlCalendar.DataSource = Project.GetListCalendars(0);
			ddlCalendar.DataBind();

			cbNotify.Text = LocRM.GetString("s6Notify");
		}
		#endregion

		#region TeamStep
		private void InitTeamStep()
		{
			lbUsers.Attributes.Add("ondblclick", "DisableButtons(this);" + Page.ClientScript.GetPostBackEventReference(btnAdd, ""));
			btnAdd.Attributes.Add("onclick", "DisableButtons(this);");
			btnSearch.Text = LocRM.GetString("s4FindNow");
			if (!IsPostBack)
			{
				btnAdd.InnerText = LocRM.GetString("s4Add");
				CreateDataTableStructure();
				BindGroups();
				BinddgMemebers();
			}
		}

		private void CreateDataTableStructure()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("UserId", typeof(int));
			dt.Columns.Add("Code");
			dt.Columns.Add("Rate", typeof(decimal));
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

		private void BinddgMemebers()
		{
			dgMembers.Columns[1].HeaderText = LocRM.GetString("s4Name");
			dgMembers.Columns[2].HeaderText = LocRM.GetString("s4Code");
			dgMembers.Columns[3].HeaderText = LocRM.GetString("s4Rate");

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
			SynchronizeDT();
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
					dr["Code"] = fl + ll;
					dr["Rate"] = 0;
					dt.Rows.Add(dr);
				}
			ViewState["Team"] = dt;

			BindUsers();
			BinddgMemebers();
		}

		private void SynchronizeDT()
		{
			DataTable dt = (DataTable)ViewState["Team"];
			foreach (DataGridItem dgi in dgMembers.Items)
			{
				int UserID = int.Parse(dgi.Cells[0].Text);
				HtmlInputText hitc = (HtmlInputText)dgi.FindControl("tCode");
				HtmlInputText hitr = (HtmlInputText)dgi.FindControl("tRate");

				DataRow[] dr = dt.Select("UserId = " + UserID);
				if (dr.Length > 0)
				{
					try
					{
						dr[0]["Rate"] = decimal.Parse(hitr.Value);
					}
					catch
					{
					}
					dr[0]["Code"] = hitc.Value;
				}
			}
			ViewState["Team"] = dt;
		}

		private void dgMembers_Delete(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			SynchronizeDT();
			int UserID = int.Parse(e.Item.Cells[0].Text);
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
			if (ViewState["SearchString"] == null)
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

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			if (step == 6)
			{
				lblProjectTitle.Text = tbTitle.Text;
				lblStartDate.Text = dtcStartDate.SelectedDate.ToString("d");
				lblEndDate.Text = dtcEndDate.SelectedDate.ToString("d");
				lblType.Text = ddType.SelectedItem.Text;
				lblExecutive.Text = ddExecutive.SelectedItem.Text;
				lblDescription.Text = txtDescription.Text;


				DataTable dt = (DataTable)ViewState["Team"];
				dlTeam.DataSource = dt.DefaultView;
				dlTeam.DataBind();
			}

			if (step == 7)
			{
				ArrayList alCategories = new ArrayList();
				ArrayList alPort = new ArrayList();

				for (int i = 0; i < lbCategories.Items.Count; i++)
					if (lbCategories.Items[i].Selected)
						alCategories.Add(int.Parse(lbCategories.Items[i].Value));

				for (int i = 0; i < lbPortfolios.Items.Count; i++)
					if (lbPortfolios.Items[i].Selected)
						alPort.Add(int.Parse(lbPortfolios.Items[i].Value));

				SynchronizeDT();
				DataTable dt = (DataTable)ViewState["Team"];

				ViewState["ProjectID"] = Project.Create(tbTitle.Text, txtDescription.Text, txtGoals.Text,
					txtScope.Text, txtDeliverables.Text, Security.CurrentUser.UserID,
					int.Parse(ddExecutive.SelectedValue), dtcStartDate.SelectedDate, dtcEndDate.SelectedDate,
					DateTime.MinValue, DateTime.MinValue, (int)Project.ProjectStatus.Active,
					int.Parse(ddType.SelectedValue), int.Parse(ddlCalendar.SelectedValue),
					PrimaryKeyId.Empty, PrimaryKeyId.Empty, int.Parse(ddCurrency.SelectedValue),
					int.Parse(ddPriority.SelectedValue),
					int.Parse(ddPhase.SelectedValue), int.Parse(ddPhase.SelectedValue),
					0, 1, int.Parse(ddlBlockType.SelectedValue),
					new ArrayList(), alCategories, alPort, dt, cbNotify.Checked);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

			steps.Add(step1); steps.Add(step2); steps.Add(step3); steps.Add(step4); steps.Add(step5);
			steps.Add(step6); steps.Add(step7);

			subtitles.Add(LocRM.GetString("s1SubTitle")); subtitles.Add(LocRM.GetString("s2SubTitle")); subtitles.Add(LocRM.GetString("s3SubTitle")); subtitles.Add(LocRM.GetString("s4SubTitle")); subtitles.Add(LocRM.GetString("s5SubTitle")); subtitles.Add(LocRM.GetString("s6SubTitle")); subtitles.Add(LocRM.GetString("s7SubTitle"));
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.dgMembers.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgMembers_Delete);

		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("GlobalTitle"); } }
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
			if (ViewState["ProjectID"] != null)
				return "try{window.opener.top.right.location.href='../Projects/ProjectView.aspx?Tab=1&ProjectID=" + ViewState["ProjectID"].ToString() + "';} catch (e) {} window.close();";
			else
				return String.Empty;
		}
		#endregion

		public void CancelAction()
		{

		}

	}
}