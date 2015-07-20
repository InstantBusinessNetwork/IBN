using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.Projects.Modules
{
	public partial class ResourceUtilGraph : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourceUtilGraph).Assembly);
		private UserLightPropertyCollection _pc = Security.CurrentUser.Properties;

		private readonly string allUsers = "0";

		#region StartDate
		protected DateTime StartDate
		{
			get
			{
				if (ViewState["StartDate"] == null)
					ViewState["StartDate"] = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;

				return (DateTime)ViewState["StartDate"];
			}
			set
			{
				ViewState["StartDate"] = value;
			}
		}
		#endregion

		#region CurDate
		protected DateTime CurDate
		{
			get
			{
				if (ViewState["CurDate"] == null)
					ViewState["CurDate"] = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);

				return (DateTime)ViewState["CurDate"];
			}
			set
			{
				ViewState["CurDate"] = value;
			}
		}
		#endregion

		#region IntervalDuration (days)
		protected int IntervalDuration
		{
			get
			{
				return int.Parse(_pc["MV_Weeks"], CultureInfo.InvariantCulture) * 7;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (_pc["MV_Group"] == null)
				_pc["MV_Group"] = ((int)InternalSecureGroups.Intranet).ToString();
			if (_pc["MV_Weeks"] == null)
				_pc["MV_Weeks"] = "1";

			ApplyLocalization();
			if (!Page.IsPostBack)
			{
				BindGroups();
				BindDefaultValues();
				BindSavedValues();

				BindData();
			}
		}

		#region ResetCurDate
		private void ResetCurDate()
		{
			CurDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
		}
		#endregion

		#region ResetStartDate
		private void ResetStartDate()
		{
			StartDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date;
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			BindToolbar();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ApplyButton.Text = LocRM.GetString("Apply");

			CalEntriesCheckbox.Text = LocRM.GetString("tEvents");
			IssuesCheckBox.Text = LocRM.GetString("tIssues");
			TasksCheckBox.Text = LocRM.GetString("tTasks");
			ToDoCheckBox.Text = LocRM.GetString("tToDos");
			DocumentsCheckBox.Text = LocRM.GetString("tDocuments");
		}
		#endregion

		#region BindGroups
		private void BindGroups()
		{
			//Groups Binding
			GroupsList.Items.Clear();

			// Any
//			GroupsList.Items.Add(new ListItem(LocRM.GetString("AllFem"), "1"));

			foreach (DataRow row in SecureGroup.GetListGroupsAsTreeDataTable().Rows)
			{
				string GroupName = CommonHelper.GetResFileString(row["GroupName"].ToString());
				string GroupId = row["GroupId"].ToString();
				ListItem item = new ListItem(GroupName, GroupId);
				GroupsList.Items.Add(item);
			}

			//Saved Value
			GroupsList.ClearSelection();
			try
			{
				GroupsList.SelectedValue = _pc["MV_Group"];
			}
			catch
			{
				GroupsList.SelectedIndex = 0;
				_pc["MV_Group"] = GroupsList.SelectedValue;
			}

			//Users Binding
			BindUsers(int.Parse(GroupsList.SelectedValue));
		}
		#endregion

		#region BindUsers
		private void BindUsers(int GroupId)
		{
			UsersList.Items.Clear();
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(string)));					// 0
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));				// 1
			DataRow dr;

			dr = dt.NewRow();
			dr[0] = allUsers;
			dr[1] = LocRM.GetString("All");
			dt.Rows.Add(dr);

			if (GroupId > 0)
			{
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
			}

			DataView dv = dt.DefaultView;

			UsersList.DataSource = dv;
			UsersList.DataTextField = "UserName";
			UsersList.DataValueField = "UserId";
			UsersList.DataBind();

			//Saved Value
			if (_pc["MV_User"] != null)
			{
				UsersList.ClearSelection();
				try
				{
					UsersList.SelectedValue = _pc["MV_User"];
				}
				catch
				{
					UsersList.SelectedIndex = 0;
					_pc["MV_User"] = UsersList.SelectedValue;
				}
			}
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			CalEntriesCheckbox.Checked = true;
			IssuesCheckBox.Checked = true;
			TasksCheckBox.Checked = true;
			ToDoCheckBox.Checked = true;
			DocumentsCheckBox.Checked = true;
			if (!Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
			{
				TasksCheckBox.Visible = false;
				_pc["MV_ShowTasks"] = false.ToString();
			}
			if (!Mediachase.IBN.Business.Configuration.HelpDeskEnabled)
			{
				IssuesCheckBox.Visible = false;
				_pc["MV_ShowIssues"] = false.ToString();
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			if (_pc["MV_ShowEvents"] != null)
				CalEntriesCheckbox.Checked = bool.Parse(_pc["MV_ShowEvents"]);
			else
				CalEntriesCheckbox.Checked = true;

			if (_pc["MV_ShowIssues"] != null)
				IssuesCheckBox.Checked = bool.Parse(_pc["MV_ShowIssues"]);
			else
				IssuesCheckBox.Checked = true;

			if (_pc["MV_ShowTasks"] != null)
				TasksCheckBox.Checked = bool.Parse(_pc["MV_ShowTasks"]);
			else
				TasksCheckBox.Checked = true;

			if (_pc["MV_ShowToDo"] != null)
				ToDoCheckBox.Checked = bool.Parse(_pc["MV_ShowToDo"]);
			else
				ToDoCheckBox.Checked = true;

			if (_pc["MV_ShowDocuments"] != null)
				DocumentsCheckBox.Checked = bool.Parse(_pc["MV_ShowDocuments"]);
			else
				DocumentsCheckBox.Checked = true;
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secHeader.Title = LocRM.GetString("tResUtiliz");
			RenderMenu(secHeader.ActionsMenu);
		}

		private void RenderMenu(ComponentArt.Web.UI.Menu actionsMenu)
		{
			ComponentArt.Web.UI.MenuItem topMenuItem;

			actionsMenu.Items.Clear();

			#region Legend Item
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = GetGlobalResourceObject("IbnFramework.Calendar", "tLegend").ToString();
//			topMenuItem.Look.LeftIconUrl = ResolveClientUrl("~/Layouts/Images/downbtn1.gif");
//			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
//			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";
			topMenuItem.ClientSideCommand = "ShowLegend()";
			actionsMenu.Items.Add(topMenuItem);
			#endregion

			#region View Menu Items
			topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = LocRM.GetString("tView");
			topMenuItem.Look.LeftIconUrl = ResolveClientUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;
			string graphPeriod = _pc["MV_Weeks"];

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (graphPeriod == "1")	// 1 week
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(ViewButton, "1");
			subItem.Text = LocRM.GetString("Week1");
			topMenuItem.Items.Add(subItem);

			subItem = new ComponentArt.Web.UI.MenuItem();
			if (graphPeriod == "3")	// 3 week
			{
				subItem.Look.LeftIconUrl = "~/Layouts/Images/accept.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
			}
			subItem.ClientSideCommand = "javascript:" + Page.ClientScript.GetPostBackEventReference(ViewButton, "3");
			subItem.Text = LocRM.GetString("Week3");
			topMenuItem.Items.Add(subItem);

			actionsMenu.Items.Add(topMenuItem);
			#endregion
		}
		#endregion

		#region ViewButton_Command
		protected void ViewButton_Command(object sender, CommandEventArgs e)
		{
			if (Request["__EVENTARGUMENT"] != null)
				_pc["MV_Weeks"] = Request["__EVENTARGUMENT"];

			ResetStartDate();
			ResetCurDate();

			BindData();
		}
		#endregion

		#region ApplyButton_Click
		protected void ApplyButton_Click(object sender, EventArgs e)
		{
			ResetCurDate();
			ResetStartDate();

			SaveValues();

			BindData();
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			_pc["MV_User"] = UsersList.SelectedValue;

			_pc["MV_ShowEvents"] = CalEntriesCheckbox.Checked.ToString();
			_pc["MV_ShowIssues"] = IssuesCheckBox.Checked.ToString();
			_pc["MV_ShowTasks"] = TasksCheckBox.Checked.ToString();
			_pc["MV_ShowToDo"] = ToDoCheckBox.Checked.ToString();
			_pc["MV_ShowDocuments"] = DocumentsCheckBox.Checked.ToString();
		}
		#endregion

		#region GroupsList_SelectedIndexChanged
		protected void GroupsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			_pc["MV_Group"] = GroupsList.SelectedValue;
			ResetCurDate();

			BindUsers(int.Parse(GroupsList.SelectedValue));

			BindData();
		}
		#endregion

		#region BindData
		private void BindData()
		{
			#region users
			string users = String.Empty;
			if (UsersList.SelectedValue != allUsers)
			{
				users = UsersList.SelectedValue;
			}
			else
			{
				foreach (ListItem item in UsersList.Items)
				{
					if (item.Value != allUsers)
					{
						if (!String.IsNullOrEmpty(users))
							users += ",";
						users += item.Value;
					}
				}
			}
			#endregion

			#region object types
			string objectTypes = String.Empty;
			if (CalEntriesCheckbox.Checked)
			{
				objectTypes = ((int)ObjectTypes.CalendarEntry).ToString();
			}
			if (ToDoCheckBox.Checked)
			{
				if (!String.IsNullOrEmpty(objectTypes))
					objectTypes += ",";

				objectTypes += ((int)ObjectTypes.ToDo).ToString();
			}
			if (TasksCheckBox.Checked && TasksCheckBox.Visible)
			{
				if (!String.IsNullOrEmpty(objectTypes))
					objectTypes += ",";

				objectTypes += ((int)ObjectTypes.Task).ToString();
			}
			if (IssuesCheckBox.Checked && IssuesCheckBox.Visible)
			{
				if (!String.IsNullOrEmpty(objectTypes))
					objectTypes += ",";

				objectTypes += ((int)ObjectTypes.Issue).ToString();
			}
			if (DocumentsCheckBox.Checked)
			{
				if (!String.IsNullOrEmpty(objectTypes))
					objectTypes += ",";

				objectTypes += ((int)ObjectTypes.Document).ToString();
			}
			#endregion

			GraphControlMain.Users = users;
			GraphControlMain.IntervalDuration = IntervalDuration;
			GraphControlMain.StartDate = StartDate;
			GraphControlMain.CurDate = CurDate;
			GraphControlMain.ObjectTypes = objectTypes;
			GraphControlMain.DataBind();
		}
		#endregion
	}
}