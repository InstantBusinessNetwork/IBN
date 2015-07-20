namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI;
	using System.Text;

	/// <summary>
	///		Summary description for WorksByCategory.
	/// </summary>
	public partial class WorksByCategory : System.Web.UI.UserControl
	{

		#region HTML Vars






		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(WorksByCategory).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPageTitles", typeof(WorksByCategory).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				if (!Configuration.ProjectManagementEnabled)
				{
					pc["CV_Project"] = "0";
					pc["CV_ShowProjects"] = false.ToString();
					pc["CV_ShowTasks"] = false.ToString();
				}
				if (!Configuration.HelpDeskEnabled)
					pc["CV_ShowIssues"] = false.ToString();
				BindGroups();
				BindDefaultValues();
			}
			BindSavedValues();
			BindInfoTable();

			BindDataGrid();
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
				trManager.Visible = false;
			else
				trManager.Visible = true;

			BindToolbar();
			FilterTable.Visible = (pc["CV_ShowFilter"] != null && bool.Parse(pc["CV_ShowFilter"]));
			tblFilterInfo.Visible = !FilterTable.Visible;

			if (pc["CV_ShowFilter"] == null || !bool.Parse(pc["CV_ShowFilter"]))
				lbShowFilter.Text = string.Format("<img alt='' title='{0}' src='{1}' />", LocRM.GetString("tShowFilter"), Page.ResolveUrl("~/Layouts/Images/scrolldown_hover.GIF"));
			else
				lbShowFilter.Text = string.Format("<img alt='' title='{0}' src='{1}' />", LocRM.GetString("tHideFilter"), Page.ResolveUrl("~/Layouts/Images/scrollup_hover.GIF"));

			trProject.Visible = Configuration.ProjectManagementEnabled;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApplyF.Text = LocRM.GetString("Apply");
			btnResetF.Text = LocRM.GetString("Reset");
			btnVResetF.Value = LocRM.GetString("ResetFilter");

			cbProjects.Text = LocRM.GetString("tProjects");
			cbCalEntries.Text = LocRM.GetString("tEvents");
			cbIssues.Text = LocRM.GetString("tIssues");
			cbDocs.Text = LocRM.GetString("tDocuments");
			cbTasks.Text = LocRM.GetString("tTasks");
			cbToDo.Text = LocRM.GetString("tToDos");
			cbChkAll.Text = "&nbsp;" + LocRM.GetString("tObjects");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			//Groups Binding
			ddGroups.Items.Clear();
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

			//Saved Value
			if (pc["CV_Group"] == null)
				pc["CV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			ddGroups.ClearSelection();
			try
			{
				ddGroups.SelectedValue = pc["CV_Group"];
			}
			catch
			{
				ddGroups.SelectedIndex = 0;
				pc["CV_Group"] = ddGroups.SelectedValue;
			}

			//Users Binding
			BindUsers(int.Parse(ddGroups.SelectedValue));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupId)
		{
			ddUser.Items.Clear();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));					// 0
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));				// 1
			DataRow dr;
			using (IDataReader reader = SecureGroup.GetListAllUsersInGroup(GroupId))
			{
				while (reader.Read())
				{
					if ((byte)reader["Activity"] == (byte)Mediachase.IBN.Business.User.UserActivity.Active)
					{
						dr = dt.NewRow();
						dr[0] = reader["UserId"].ToString();
						dr[1] = reader["LastName"].ToString() + " " + reader["FirstName"].ToString();
						dt.Rows.Add(dr);
					}
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "UserName";

			ddUser.DataSource = dv;
			ddUser.DataTextField = "UserName";
			ddUser.DataValueField = "UserId";
			ddUser.DataBind();
			ddUser.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));

			//Saved Value
			if (pc["CV_User"] == null)
				pc["CV_User"] = ddUser.SelectedValue;
			else
			{
				ddUser.ClearSelection();
				try
				{
					ddUser.SelectedValue = pc["CV_User"];
				}
				catch
				{
					ddUser.SelectedIndex = 0;
					pc["CV_User"] = ddUser.SelectedValue;
				}
			}
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			ddManager.DataSource = Mediachase.IBN.Business.ToDo.GetManagers();
			ddManager.DataTextField = "FullName";
			ddManager.DataValueField = "PrincipalId";
			ddManager.DataBind();
			ddManager.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));

			ddPrjs.DataSource = Project.GetListProjects();
			ddPrjs.DataTextField = "Title";
			ddPrjs.DataValueField = "ProjectId";
			ddPrjs.DataBind();
			ddPrjs.Items.Insert(0, new ListItem(LocRM.GetString("All"), "0"));

			ddCompleted.Items.Clear();
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tDontShow"), "0"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForDay"), "-1"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForWeek"), "-7"));
			ddCompleted.Items.Add(new ListItem(LocRM.GetString("tForAnyPeriod"), "-11000"));

			ddUpcoming.Items.Clear();
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tDontShow"), "0"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInDay"), "1"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInWeek"), "7"));
			ddUpcoming.Items.Add(new ListItem(LocRM.GetString("tInAnyPeriod"), "11000"));

			ddShowActive.Items.Clear();
			ddShowActive.Items.Add(new ListItem(LocRM.GetString("tShowActive"), "True"));
			ddShowActive.Items.Add(new ListItem(LocRM.GetString("tDontShowActive"), "False"));

			cbProjects.Checked = true;
			cbCalEntries.Checked = true;
			cbIssues.Checked = true;
			cbDocs.Checked = true;
			cbTasks.Checked = true;
			cbToDo.Checked = true;
			if (!Configuration.ProjectManagementEnabled)
			{
				cbTasks.Visible = false;
				cbProjects.Visible = false;
			}
			if (!Configuration.HelpDeskEnabled)
				cbIssues.Visible = false;
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (pc["CV_User"] != null)
			{
				ddUser.ClearSelection();
				Util.CommonHelper.SafeSelect(ddUser, pc["CV_User"]);
			}

			if (!Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) &&
				!Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager))
			{
				ListItem liItem = ddManager.Items.FindByValue(Security.CurrentUser.UserID.ToString());
				if (liItem != null)
					pc["CV_Manager"] = liItem.Value;
			}
			if (pc["CV_Manager"] != null)
			{
				ddManager.ClearSelection();
				Util.CommonHelper.SafeSelect(ddManager, pc["CV_Manager"]);
			}
			if (pc["CV_Project"] != null)
			{
				ddPrjs.ClearSelection();
				Util.CommonHelper.SafeSelect(ddPrjs, pc["CV_Project"]);
			}
			if (pc["CV_Completed"] != null)
			{
				ddCompleted.ClearSelection();
				Util.CommonHelper.SafeSelect(ddCompleted, pc["CV_Completed"]);
			}
			if (pc["CV_Upcoming"] != null)
			{
				ddUpcoming.ClearSelection();
				Util.CommonHelper.SafeSelect(ddUpcoming, pc["CV_Upcoming"]);
			}
			if (pc["CV_Active"] != null)
			{
				ddShowActive.ClearSelection();
				Util.CommonHelper.SafeSelect(ddShowActive, pc["CV_Active"]);
			}

			if (pc["CV_ShowProjects"] != null)
				cbProjects.Checked = bool.Parse(pc["CV_ShowProjects"]);
			else
				cbProjects.Checked = true;

			if (pc["CV_ShowEvents"] != null)
				cbCalEntries.Checked = bool.Parse(pc["CV_ShowEvents"]);
			else
				cbCalEntries.Checked = true;

			if (pc["CV_ShowIssues"] != null)
				cbIssues.Checked = bool.Parse(pc["CV_ShowIssues"]);
			else
				cbIssues.Checked = true;

			if (pc["CV_ShowDocs"] != null)
				cbDocs.Checked = bool.Parse(pc["CV_ShowDocs"]);
			else
				cbDocs.Checked = true;

			if (pc["CV_ShowTasks"] != null)
				cbTasks.Checked = bool.Parse(pc["CV_ShowTasks"]);
			else
				cbTasks.Checked = true;

			if (pc["CV_ShowToDo"] != null)
				cbToDo.Checked = bool.Parse(pc["CV_ShowToDo"]);
			else
				cbToDo.Checked = true;

			bool Allchecked = cbCalEntries.Checked && cbDocs.Checked && cbToDo.Checked;
			if (Configuration.ProjectManagementEnabled)
				Allchecked = Allchecked && cbProjects.Checked && cbTasks.Checked;
			if (Configuration.HelpDeskEnabled)
				Allchecked = Allchecked && cbIssues.Checked;
			if (Allchecked)
				cbChkAll.Checked = true;
			else
				cbChkAll.Checked = false;
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			pc["CV_User"] = ddUser.SelectedValue;
			pc["CV_Manager"] = ddManager.SelectedValue;
			pc["CV_Project"] = ddPrjs.SelectedValue;
			pc["CV_Completed"] = ddCompleted.SelectedValue;
			pc["CV_Upcoming"] = ddUpcoming.SelectedValue;
			pc["CV_Active"] = ddShowActive.SelectedValue;

			pc["CV_ShowProjects"] = cbProjects.Checked.ToString();
			pc["CV_ShowEvents"] = cbCalEntries.Checked.ToString();
			pc["CV_ShowIssues"] = cbIssues.Checked.ToString();
			pc["CV_ShowDocs"] = cbDocs.Checked.ToString();
			pc["CV_ShowTasks"] = cbTasks.Checked.ToString();
			pc["CV_ShowToDo"] = cbToDo.Checked.ToString();
		}
		#endregion

		#region BindInfoTable
		private void BindInfoTable()
		{
			tblFilterInfoSet.Rows.Clear();

			// Group
			if (pc["CV_Group"] != null)
			{
				ListItem li = ddGroups.Items.FindByValue(pc["CV_Group"]);
				if (li != null)
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tGroup")), li.Text.TrimStart());
			}

			//User
			if (pc["CV_User"] != null)
			{
				ListItem li = ddUser.Items.FindByValue(pc["CV_User"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tUser")), li.Text);
			}

			// Project
			if (pc["CV_Project"] != null)
			{
				ListItem li = ddPrjs.Items.FindByValue(pc["CV_Project"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tProject")), li.Text);
			}

			// Manager
			if (pc["CV_Manager"] != null)
			{
				ListItem li = ddManager.Items.FindByValue(pc["CV_Manager"]);
				if (li != null && li.Value != "0")
					AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tManager")), li.Text);
			}

			//State
			string state = "";
			if (pc["CV_Completed"] != null)
			{
				ListItem li = ddCompleted.Items.FindByValue(pc["CV_Completed"]);
				if (li != null && li.Value != "0")
				{
					state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowCompleted"), li.Text);
				}
			}
			if (pc["CV_Active"] == null || bool.Parse(pc["CV_Active"]))
			{
				if (state != "")
					state += ", ";
				state += LocRM.GetString("tActive");
			}
			if (pc["CV_Upcoming"] != null)
			{
				ListItem li = ddUpcoming.Items.FindByValue(pc["CV_Upcoming"]);
				if (li != null && li.Value != "0")
				{
					if (state != "")
						state += ", ";
					state += String.Format("{0}&nbsp;{1}", LocRM.GetString("tShowUpcoming"), li.Text);
				}
			}
			if (state != "")
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("State")), state);

			// Objects
			string Objs = "";
			if (pc["CV_ShowProjects"] == null || bool.Parse(pc["CV_ShowProjects"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tProjects");
			}
			if (pc["CV_ShowEvents"] == null || bool.Parse(pc["CV_ShowEvents"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tEvents");
			}
			if (pc["CV_ShowIssues"] == null || bool.Parse(pc["CV_ShowIssues"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tIssues");
			}
			if (pc["CV_ShowDocs"] == null || bool.Parse(pc["CV_ShowDocs"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tDocuments");
			}
			if (pc["CV_ShowTasks"] == null || bool.Parse(pc["CV_ShowTasks"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tTasks");
			}
			if (pc["CV_ShowToDo"] == null || bool.Parse(pc["CV_ShowToDo"]))
			{
				if (Objs != "")
					Objs += ", ";
				Objs += LocRM.GetString("tToDos");
			}
			if (Objs != "")
				AddRow(String.Format("{0}:&nbsp; ", LocRM.GetString("tShow")), Objs);
		}

		private void AddRow(string filterName, string filterValue)
		{
			HtmlTableRow tr = new HtmlTableRow();
			HtmlTableCell td1 = new HtmlTableCell();
			td1.Align = "Right";
			td1.Width = "120px";
			td1.InnerHtml = filterName;
			HtmlTableCell td2 = new HtmlTableCell();
			td2.InnerHtml = filterValue;
			tr.Cells.Add(td1);
			tr.Cells.Add(td2);
			tblFilterInfoSet.Rows.Add(tr);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM2.GetString("tWorksByCategory");
		}
		#endregion

		#region BindDataGrid
		private void BindDataGrid()
		{
			int i = 3;
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Title");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("Type");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("tResources");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("PercentCompleted");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("StartDate");
			dgObjects.Columns[i++].HeaderText = LocRM.GetString("FinishDate2");

			int PrincipalId = (ddUser.SelectedValue == "0") ? int.Parse(ddGroups.SelectedValue) : int.Parse(ddUser.SelectedValue);
			int ProjectId = int.Parse(ddPrjs.SelectedValue);
			int ManagerId = int.Parse(ddManager.SelectedValue);
			bool ShowActive = bool.Parse(ddShowActive.SelectedValue);

			ArrayList alTypes = new ArrayList();
			if (cbProjects.Checked)
				alTypes.Add((int)ObjectTypes.Project);
			if (cbCalEntries.Checked)
				alTypes.Add((int)ObjectTypes.CalendarEntry);
			if (cbIssues.Checked)
				alTypes.Add((int)ObjectTypes.Issue);
			if (cbDocs.Checked)
				alTypes.Add((int)ObjectTypes.Document);
			if (cbTasks.Checked)
				alTypes.Add((int)ObjectTypes.Task);
			if (cbToDo.Checked)
				alTypes.Add((int)ObjectTypes.ToDo);

			DateTime dtCompleted = UserDateTime.UserNow;
			DateTime dtUpcoming = UserDateTime.UserNow;
			int iCompleted = int.Parse(ddCompleted.SelectedValue);
			if (iCompleted < 0)
				dtCompleted = UserDateTime.UserToday.AddDays(iCompleted - 1).AddSeconds(1);
			int iUpcoming = int.Parse(ddUpcoming.SelectedValue);
			if (iUpcoming > 0)
				dtUpcoming = UserDateTime.UserToday.AddDays(iUpcoming + 1).AddSeconds(-1);

			DataTable dt = Mediachase.IBN.Business.ToDo.GetGroupedItemsByCategoryDataTable(PrincipalId,
				ManagerId, ProjectId, ShowActive, alTypes, dtCompleted, dtUpcoming);

			DataView dv = dt.DefaultView;
			dgObjects.DataSource = dv;

			if (pc["CV_PageSize"] != null)
				dgObjects.PageSize = int.Parse(pc["CV_PageSize"]);

			if (pc["CV_Page"] != null)
			{
				int pageindex = int.Parse(pc["CV_Page"]);
				int ppi = dv.Count / dgObjects.PageSize;
				if (dv.Count % dgObjects.PageSize == 0)
					ppi = ppi - 1;

				if (pageindex <= ppi)
					dgObjects.CurrentPageIndex = pageindex;
				else
					dgObjects.CurrentPageIndex = 0;
			}

			dgObjects.DataBind();
			foreach (DataGridItem dgi in dgObjects.Items)
			{
				if (int.Parse(dgi.Cells[0].Text) == 0)
					dgi.BackColor = Color.FromArgb(0xBB, 0xBB, 0xBB);
				else if (int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Upcoming)
					dgi.BackColor = Color.FromArgb(0xE9, 0xFE, 0xEC);
				else if (int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Suspended || int.Parse(dgi.Cells[1].Text) == (int)ObjectStates.Completed)
					dgi.BackColor = Color.FromArgb(0xF2, 0xF2, 0xF2);
			}
		}
		#endregion

		#region Protected DG strings
		protected string GetTitle(int itemId, string title, int itemTypeId, object groupName, int stateId, bool isOverdue, string itemCode)
		{
			string result = string.Empty;

			if (itemId == 0)
			{
				result = string.Format("<b>{0}</b>", groupName);
			}
			else
			{
				string link = null;
				string icon = null;
				string iconCompleted = null;
				string iconOverdue = null;

				switch (itemTypeId)
				{
					case (int)ObjectTypes.Project:
						link = "~/Projects/ProjectView.aspx?ProjectId=";
						icon = "project.gif";
						iconCompleted = "project_completed.gif";
						iconOverdue = "project.gif";
						title += CHelper.GetProjectNumPostfix(itemId, itemCode);
						break;
					case (int)ObjectTypes.Task:
						link = "~/Tasks/TaskView.aspx?TaskId=";
						icon = "task1.gif";
						iconCompleted = "task1_completed.gif";
						iconOverdue = "task1_overdue.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.ToDo:
						link = "~/ToDo/ToDoView.aspx?ToDoId=";
						icon = "task.gif";
						iconCompleted = "task_completed.gif";
						iconOverdue = "task_overdue.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.Document:
						link = "~/Documents/DocumentView.aspx?DocumentId=";
						icon = "document.gif";
						iconCompleted = "document_completed.gif";
						iconOverdue = "document.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.Issue:
						link = "~/Incidents/IncidentView.aspx?IncidentId=";
						icon = "incident.gif";
						iconCompleted = "incident_closed.gif";
						iconOverdue = "incident.gif";
						title += " (#" + itemId + ")";
						break;
					case (int)ObjectTypes.CalendarEntry:
						link = "~/Events/EventView.aspx?EventId=";
						icon = "event.gif";
						iconCompleted = "event_completed.gif";
						iconOverdue = "event.gif";
						break;
				}

				if (!string.IsNullOrEmpty(link))
				{
					link = Page.ResolveUrl(link + itemId);

					if (!string.IsNullOrEmpty(icon))
						icon = Page.ResolveUrl("~/Layouts/Images/icons/" + icon);

					if (!string.IsNullOrEmpty(iconCompleted))
						iconCompleted = Page.ResolveUrl("~/Layouts/Images/icons/" + iconCompleted);

					if (!string.IsNullOrEmpty(iconOverdue))
						iconOverdue = Page.ResolveUrl("~/Layouts/Images/icons/" + iconOverdue);

					if (stateId == (int)ObjectStates.Completed)
						result = string.Format("<span style='color:#999999;text-decoration:line-through'><a href='{0}'><span><img alt='' src='{1}' /></span> <span>{2}</span></a></span>", link, iconCompleted, title);
					else if (stateId == (int)ObjectStates.Overdue || isOverdue)
						result = string.Format("<a href='{0}'><span style='color:red'><span><img alt='' src='{1}' /></span> <span>{2}</span></span></a>", link, iconOverdue, title);
					else
						result = string.Format("<a href='{0}'><span><img alt='' src='{1}' /></span> <span>{2}</span></a>", link, icon, title);
				}
			}

			return result;
		}

		protected string GetType(int itemType, int stateId, bool isOverdue)
		{
			string result = string.Empty;

			if (itemType > 0)
			{
				string type = null;

				switch (itemType)
				{
					case (int)ObjectTypes.Task:
						type = LocRM.GetString("tTask");
						break;
					case (int)ObjectTypes.ToDo:
						type = LocRM.GetString("tTodo");
						break;
					case (int)ObjectTypes.Document:
						type = LocRM.GetString("tDoc");
						break;
					case (int)ObjectTypes.Issue:
						type = LocRM.GetString("tIssue");
						break;
					case (int)ObjectTypes.CalendarEntry:
						type = LocRM.GetString("tCalEntry");
						break;
					case (int)ObjectTypes.Project:
						type = LocRM.GetString("tProject");
						break;
				}

				if (!string.IsNullOrEmpty(type))
				{
					if (stateId == (int)ObjectStates.Completed)
						result = string.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}</span></span>", type);
					else if (isOverdue || stateId == (int)ObjectStates.Overdue)
						result = string.Format("<span style='color:red'>{0}</span>", type);
					else
						result = type;
				}
			}

			return result;
		}

		protected string GetDate(object _date, int StateId, bool isOverdue)
		{
			if (_date != DBNull.Value)
			{
				if (StateId == (int)ObjectStates.Completed)
					return String.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}</span></span>", ((DateTime)_date).ToShortDateString());
				else if (StateId == (int)ObjectStates.Overdue || isOverdue)
					return String.Format("<span style='color:red'>{0}</span>", ((DateTime)_date).ToShortDateString());
				else
					return ((DateTime)_date).ToShortDateString();
			}
			else
				return "";
		}

		protected string GetResources(int CType, int itemType, int itemId, int stateId, bool isOverdue)
		{
			StringBuilder builder = new StringBuilder();

			if (itemType > 0)
			{
				bool hasResources = false;
				switch (itemType)
				{
					case (int)ObjectTypes.Project:
						using (IDataReader reader = Project.GetListTeamMemberNames(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""));
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""));
							}
						}
						break;
					case (int)ObjectTypes.Task:
						using (IDataReader reader = Task.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? ("<td style='padding-left:7'><span style='color:#999999;text-decoration:line-through'><span style='color:black'>" + reader["PercentCompleted"].ToString() + "%</span></span></td>") : "");
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], "red"), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'><span style='color:red'>" + reader["PercentCompleted"].ToString() + "%</span></td>" : "");
								else
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'>" + reader["PercentCompleted"].ToString() + "%</td>" : "");
							}
						}
						break;
					case (int)ObjectTypes.ToDo:
						using (IDataReader reader = Mediachase.IBN.Business.ToDo.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? ("<td style='padding-left:7'><span style='color:#999999;text-decoration:line-through'><span style='color:black'>" + reader["PercentCompleted"].ToString() + "%</span></span></td>") : "");
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], "red"), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'><span style='color:red'>" + reader["PercentCompleted"].ToString() + "%</span></td>" : "");
								else
									builder.AppendFormat("<tr><td>{0}</td>{1}</tr>", Util.CommonHelper.GetUserStatus((int)reader["UserId"], ""), (CType == (int)CompletionType.All) ? "<td style='padding-left:7'>" + reader["PercentCompleted"].ToString() + "%</td>" : "");
							}
						}
						break;
					case (int)ObjectTypes.Document:
						using (IDataReader reader = Document.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
							}
						}
						break;
					case (int)ObjectTypes.Issue:
						using (IDataReader reader = Incident.GetListIncidentResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
							}
						}
						break;
					case (int)ObjectTypes.CalendarEntry:
						using (IDataReader reader = CalendarEntry.GetListResources(itemId))
						{
							while (reader.Read())
							{
								hasResources = true;

								if (stateId == (int)ObjectStates.Completed)
									builder.AppendFormat("<tr><td><span style='color:#999999;text-decoration:line-through'>{0}</span></td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
								else if (stateId == (int)ObjectStates.Overdue || isOverdue)
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], "red"));
								else
									builder.AppendFormat("<tr><td>{0}</td></tr>", Util.CommonHelper.GetUserStatus((int)reader["PrincipalId"], ""));
							}
						}
						break;
				}

				if (hasResources)
				{
					builder.Insert(0, "<table class='text' cellspacing='0' cellpadding='1' border='0'>");
					builder.Append("</table>");
				}
			}

			return builder.ToString();
		}

		protected string GetPercentCompleted(int itemType, int percentCompleted, int stateId, bool isOverdue)
		{
			string retVal = string.Empty;

			string image = Page.ResolveUrl("~/Layouts/Images/" + ((isOverdue || stateId == (int)ObjectStates.Overdue) ? "redpoint.gif" : "point.gif"));

			if (itemType == (int)ObjectTypes.Task || itemType == (int)ObjectTypes.ToDo)
			{
				string strPercCompleted = string.Empty;
				if (stateId == (int)ObjectStates.Completed)
					strPercCompleted = string.Format("<span style='color:#999999;text-decoration:line-through'><span style='color:black'>{0}%</span></span>", percentCompleted);
				else if (stateId == (int)ObjectStates.Overdue || isOverdue)
					strPercCompleted = string.Format("<span style='color:red'>{0}%</span>", percentCompleted);
				else
					strPercCompleted = percentCompleted + "%";

				retVal = string.Format(@"<table cellspacing='0' cellpadding='0'><tr><td><div class='progress'><img alt='' src='{0}' width='{1}%' /></div></td><td>{2}</td></tr></table>",
					image, percentCompleted, strPercCompleted);
			}

			return retVal;
		}

		protected string GetPriorityIcon(int ItemType, int PriorityId, string PName)
		{
			string retVal = "";
			if (ItemType > 0)
			{
				retVal = "<img src='{0}' alt='{1}' title='{1}'/>";
				switch (PriorityId)
				{
					case (int)Priority.Low:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityLow.gif"), PName + " " + LocRM.GetString("tPriority"));
					case (int)Priority.High:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityHigh.gif"), PName + " " + LocRM.GetString("tPriority"));
					case (int)Priority.VeryHigh:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityVeryHigh.gif"), PName + " " + LocRM.GetString("tPriority"));
					case (int)Priority.Urgent:
						return String.Format(retVal, Page.ResolveUrl("~/Layouts/Images/icons/PriorityUrgent.gif"), PName + " " + LocRM.GetString("tPriority"));
					default:
						return "";
				}
			}
			return retVal;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbShowFilter.Click += new EventHandler(lbShowFilter_Click);
			this.lbHideFilter.Click += new EventHandler(lbShowFilter_Click);
			this.ddGroups.SelectedIndexChanged += new EventHandler(ddGroups_SelectedIndexChanged);
			this.btnApplyF.Click += new EventHandler(btnApplyF_Click);
			this.btnResetF.Click += new EventHandler(btnResetF_Click);
			this.btnVResetF.ServerClick += new EventHandler(btnVResetF_ServerClick);
			this.dgObjects.PageIndexChanged += new DataGridPageChangedEventHandler(dgObjects_PageIndexChanged);
			this.dgObjects.PageSizeChanged += new Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventHandler(dgObjects_PageSizeChanged);
		}
		#endregion

		#region dgEvents
		private void dgObjects_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			pc["CV_Page"] = e.NewPageIndex.ToString();
			BindDataGrid();
		}

		private void dgObjects_PageSizeChanged(object source, Mediachase.UI.Web.Modules.DGExtension.DataGridPageSizeChangedEventArgs e)
		{
			pc["CV_PageSize"] = e.NewPageSize.ToString();
			BindDataGrid();
		}
		#endregion

		#region ddGroups_Change
		private void ddGroups_SelectedIndexChanged(object sender, EventArgs e)
		{
			pc["CV_Group"] = ddGroups.SelectedValue;
			BindUsers(int.Parse(ddGroups.SelectedValue));

			BindDataGrid();
		}
		#endregion

		#region Apply-Reset Filter/Grouping
		private void btnApplyF_Click(object sender, EventArgs e)
		{
			SaveValues();
			BindSavedValues();
			BindInfoTable();
			BindDataGrid();
		}

		private void btnResetF_Click(object sender, EventArgs e)
		{
			pc["CV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			BindGroups();
			BindDefaultValues();
			SaveValues();
			ddUser.SelectedIndex = 0;
			pc["CV_User"] = ddUser.SelectedValue;
			BindInfoTable();
			BindDataGrid();
		}

		private void btnVResetF_ServerClick(object sender, EventArgs e)
		{
			pc["CV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			BindGroups();
			BindDefaultValues();
			SaveValues();
			ddUser.SelectedIndex = 0;
			pc["CV_User"] = ddUser.SelectedValue;
			BindInfoTable();
			BindDataGrid();
		}
		#endregion

		#region Show-Hide
		private void lbShowFilter_Click(object sender, EventArgs e)
		{
			if (pc["CV_ShowFilter"] == null || !bool.Parse(pc["CV_ShowFilter"]))
				pc["CV_ShowFilter"] = "True";
			else
				pc["CV_ShowFilter"] = "False";
			BindDataGrid();
		}
		#endregion

	}
}
