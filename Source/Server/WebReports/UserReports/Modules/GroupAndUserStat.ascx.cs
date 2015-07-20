using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Resources;

using ComponentArt.Web.UI;

//using Mediachase.IBN.Business;
using Mediachase.IBN.DefaultUserReports;
using Mediachase.UI.Web.UserReports.Util;
using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI.WebControls;


namespace Mediachase.UI.Web.UserReports.Modules
{
	/// <summary>
	///		Summary description for GroupAndUserStat.
	/// </summary>
	/// 

	public partial class GroupAndUserStat : System.Web.UI.UserControl
	{
		protected System.Collections.ArrayList aGroupPath = new System.Collections.ArrayList();
		IFormatProvider culture = CultureInfo.InvariantCulture;
		Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strGroupAndUserStat", typeof(GroupAndUserStat).Assembly);
		protected string ProductName = Mediachase.Ibn.IbnConst.ProductName;
		#region HTML Variables

		//protected System.Web.UI.WebControls.TextBox txtDateFrom;
		//protected System.Web.UI.WebControls.TextBox txtDateTo;
		protected System.Web.UI.WebControls.CustomValidator CustomValidator1;

		//protected System.Web.UI.WebControls.RangeValidator rvFromDate;
		//protected System.Web.UI.WebControls.RangeValidator rvToDate;

		#endregion

		#region GroupId&UserId
		protected int SGroupID
		{
			get
			{
				if (Request["SGroupID"] != null)
				{
					ViewState["SGroupID"] = Request["SGroupID"].ToString();
				}
				else if (ViewState["SGroupID"] == null)
					ViewState["SGroupID"] = "1";
				return int.Parse(ViewState["SGroupID"].ToString());
			}
			set
			{
				ViewState["SGroupID"] = value;
			}
		}

		protected int UserID
		{
			get
			{
				if (Request["UserID"] != null)
				{
					ViewState["UserID"] = Request["UserID"].ToString();
				}
				else if (ViewState["UserID"] == null)
					ViewState["UserID"] = "0";
				return int.Parse(ViewState["UserID"].ToString());
			}
			set
			{
				ViewState["UserID"] = value;
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/treeStyle.css");

			// Alex Fix
			/*
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();
			*/
			trLibrary.Visible = false;
			trLibrary1.Visible = false;

			btnAplly.Text = LocRM.GetString("tShow");
			btnExport.Text = LocRM.GetString("tExport");
			lblIMStat.Text = String.Format(LocRM.GetString("tIMStatistics"), IbnConst.ProductFamilyShort);
			if (!IsPostBack)
			{
				BindTree();
				BindData();

				DateTime StartDate = new DateTime(0);
				DateTime EndDate = new DateTime(0);

				//SetDates(ddPeriod.Value, out StartDate, out EndDate, txtDateFrom.Text, txtDateTo.Text);
				UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());
				if (UserID == 0)
				{
					tblGroupStat.Visible = true;
					tblUser1.Visible = false;
					tblUser2.Visible = false;
					tblUser3.Visible = false;
					BindGroupStat(SGroupID, StartDate, EndDate);
				}
				else
				{
					tblGroupStat.Visible = false;
					tblUser1.Visible = true;
					tblUser2.Visible = true;
					tblUser3.Visible = true;
					if (Mediachase.IBN.Business.User.IsExternal(UserID) || Mediachase.IBN.Business.User.GetUserActivity(UserID) == Mediachase.IBN.Business.User.UserActivity.Pending)
					{
						trIMGroup.Visible = false;
						tblUser2.Visible = false;
					}
					BindUserStat(UserID, StartDate, EndDate);
				}
			}
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (ddPeriod.Value == "9")
				tableDate.Style.Add("display", "block");
			else
				tableDate.Style.Add("display", "none");

			_header.ForPrintOnly = true;
			_header.Title = LocRM.GetString("tGrAndUs");

			if (Request["Export"] != null && Request["Export"] == "1")
			{
				ExportStatistics();
			}

			if (!Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
			{
				trPM.Visible = false;
				trPrjs.Visible = false;
				trTasks.Visible = false;
			}
			if (!Mediachase.IBN.Business.Configuration.HelpDeskEnabled)
			{
				trHDM.Visible = false;
				trIss.Visible = false;
			}
		}
		#endregion

		#region BindData
		private void BindData()
		{
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tAny"), "0"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tToday"), "1"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tYesterday"), "2"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisWeek"), "3"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastWeek"), "4"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisMonth"), "5"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastMonth"), "6"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tThisYear"), "7"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tLastYear"), "8"));
			ddPeriod.Items.Add(new ListItem(LocRM.GetString("tCustom"), "9"));
			if (pc["GroupAndUserStat_ddPeriod"] != null)
			{
				string sVal = pc["GroupAndUserStat_ddPeriod"];
				ListItem liItem = ddPeriod.Items.FindByValue(sVal);
				if (liItem != null)
					liItem.Selected = true;
				if (pc["GroupAndUserStat_Start"] != null)
				{
					DateTime _date = DateTime.Parse(pc["GroupAndUserStat_Start"], culture);
					//txtDateFrom.Text = _date.ToShortDateString();
					dtcStartDate.SelectedDate = _date;
				}
				else
				{
					//txtDateFrom.Text = UserDateTime.UserToday.ToShortDateString();
					dtcStartDate.SelectedDate = UserDateTime.Today;
				}
				if (pc["GroupAndUserStat_End"] != null)
				{
					DateTime _date = DateTime.Parse(pc["GroupAndUserStat_End"], culture);
					//txtDateTo.Text = _date.ToShortDateString();
					dtcEndDate.SelectedDate = _date;
				}
				else
				{
					//txtDateTo.Text = UserDateTime.UserNow.ToShortDateString();
					dtcEndDate.SelectedDate = UserDateTime.Now;
				}
			}
			else
			{
				//txtDateFrom.Text = UserDateTime.UserToday.AddMonths(-1).ToShortDateString();
				dtcStartDate.SelectedDate = UserDateTime.Today.AddMonths(-1);
				//txtDateTo.Text = UserDateTime.UserNow.ToShortDateString();
				dtcEndDate.SelectedDate = UserDateTime.Now;
			}
			if (ddPeriod.Value != "9")
				tableDate.Style.Add("display", "none");
		}
		#endregion

		#region BindStats
		private void BindGroupStat(int iSGroupId, DateTime StartDate, DateTime EndDate)
		{
			using (IDataReader reader = UserReport.GetSecGroupStats(iSGroupId, StartDate, EndDate))
			{
				if (reader.Read())
				{
					int ActAcc = (int)reader["ActiveAccounts"];
					int InActAcc = (int)reader["InactiveAccounts"];
					lblActiveAccounts.Text = ActAcc.ToString();
					lblDeActiveAccounts.Text = InActAcc.ToString();
					lblNumberAccounts.Text = (ActAcc + InActAcc).ToString();
					lblCalendarEntries.Text = reader["NewEventsCount"].ToString();
					lblProjectsCreated.Text = reader["NewProjectsCount"].ToString();
					lblIssuesCreated.Text = reader["NewIncidentsCount"].ToString();
					lblTasksCreated.Text = reader["NewTasksCount"].ToString();
					lblToDoCreated.Text = reader["NewToDosCount"].ToString();
					lblIssuesReopen.Text = reader["ReOpenIncidentsCount"].ToString();
				}
			}

			using (IDataReader reader = UserReport.GetGroup(iSGroupId))
			{
				if (reader.Read())
					lblGroupName.Text = CommonHelper.GetResFileString(reader["GroupName"].ToString());
			}

			if (ddPeriod.Value != "0")
				_header.Filter = LocRM.GetString("tPeriod") + ":<br/>&nbsp;&nbsp;" + StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
			else
				_header.Filter = "";

			if (ddPeriod.Value != "0")
				lblInterval.Text = StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
			else
				lblInterval.Text = "";
		}

		private void BindUserStat(int iUserId, DateTime StartDate, DateTime EndDate)
		{
			int iIMGroupId = 0;
			using (IDataReader reader = UserReport.GetUserInfo(iUserId, false))
			{

				if (reader.Read())
				{
					if (reader["Login"] != DBNull.Value)
						lblUserAccountName.Text = reader["Login"].ToString();
					lblUserEmail.Text = reader["Email"].ToString();
					lblUserFirstName.Text = reader["FirstName"].ToString();
					lblUserLastName.Text = reader["LastName"].ToString();
					lblUserStatus.Text = ((bool)reader["IsActive"]) ? LocRM.GetString("tActive") : LocRM.GetString("tNotActive");
					if (reader["IMGroupId"] != DBNull.Value)
						iIMGroupId = (int)reader["IMGroupId"];
				}
			}

			if (!Mediachase.IBN.Business.User.IsExternal(iUserId) &&
				Mediachase.IBN.Business.User.GetUserActivity(iUserId) != Mediachase.IBN.Business.User.UserActivity.Pending)
			{
				DataTable dt = UserReport.GetUserStats(iUserId, StartDate, EndDate);
				if (dt.Rows.Count > 0)
				{
					DataRow dr = dt.Rows[0];
					lblClientLoginsCount.Text = ((int)dr["LoginsClient"]).ToString();
					lblUserConfCreated.Text = ((int)dr["ConfCreated"]).ToString();
					lblUserConfMessagesSent.Text = ((int)dr["ConfMsgSent"]).ToString();
					lblUserMessagesReceived.Text = ((int)dr["MsgReceived"]).ToString();
					lblUserMessagesSent.Text = ((int)dr["MsgSent"]).ToString();
					lblUserTotalFilesReceived.Text = ((int)dr["FilesReceived"]).ToString();
					lblUserTotalFilesReceivedBytes.Text = FormatBytes((long)dr["FRBytes"]);
					lblUserTotalFilesSent.Text = ((int)dr["FilesSent"]).ToString();
					lblUserTotalFilesSentBytes.Text = FormatBytes((long)dr["FSBytes"]);
				}
				using (IDataReader rdr = UserReport.GetIMGroup(iIMGroupId))
				{
					if (rdr.Read() && rdr["IMGroupName"] != DBNull.Value)
					{
						lblUserGroupName.Text = rdr["IMGroupName"].ToString();
					}
				}
			}

			using (IDataReader reader = UserReport.GetListSecureGroup(iUserId))
			{
				while (reader.Read())
					lblUserSecurityLevel.Text += CommonHelper.GetResFileString(reader["GroupName"].ToString()) + "<br/>";
			}

			using (IDataReader reader = UserReport.GetQuickSnapshotReport(StartDate, EndDate, iUserId))
			{
				if (reader.Read())
				{
					lblPortalLoginsCount.Text = ((int)reader["PortalLogins"]).ToString();
					lblUserProjectTotalCreated.Text = ((int)reader["NewProjectsCount"]).ToString();
					lblUserIssuesTotalCreated.Text = ((int)reader["NewIncidentsCount"]).ToString();
					lblUserToDoTotalCreated.Text = ((int)reader["NewToDosCount"]).ToString();
					lblUserTasksTotalCreated.Text = ((int)reader["NewTasksCount"]).ToString();
					lblUserCalendarTotalEntriesCreated.Text = ((int)reader["NewEventsCount"]).ToString();
				}
			}

			if (ddPeriod.Value != "0")
				_header.Filter = LocRM.GetString("tPeriod") + ":<br/>&nbsp;&nbsp;" + StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
			else
				_header.Filter = "";

			if (ddPeriod.Value != "0")
				lblInterval.Text = StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
			else
				lblInterval.Text = "";
		}
		#endregion

		#region BindTree
		private void BindTree()
		{
			GUTree.Nodes.Clear();
			int iGroup = SGroupID;
			aGroupPath.Clear();
			while (iGroup > 1)
			{
				aGroupPath.Add(iGroup);
				iGroup = UserReport.GetParentGroup(iGroup);
			}
			aGroupPath.Add(iGroup);
			ProcessGroup2(iGroup, null);
		}

		private void ProcessGroup2(int iGroup, TreeViewNode nodeParent)
		{
			TreeViewNode nodeGroup;
			nodeGroup = new TreeViewNode();
			using (IDataReader reader = UserReport.GetGroup(iGroup))
			{
				if (reader.Read())
				{
					nodeGroup.Text =  CommonHelper.GetResFileString(reader["GroupName"].ToString());/*CommonHelper.GetGroupLinkUL(iGroup, reader["GroupName"].ToString())*/
					nodeGroup.ImageUrl = Page.ResolveUrl("~/Layouts/Images/icons/regular.gif");
					nodeGroup.Value = "Group";
					nodeGroup.NavigateUrl = String.Format("GroupAndUserStat.aspx?SGroupID={0}&UserID=0", reader["GroupId"].ToString());
				}
			}
			// Expanding
			if (aGroupPath.Contains(iGroup))
				nodeGroup.Expanded = true;

			if (nodeParent == null)
				GUTree.Nodes.Add(nodeGroup);
			else
				nodeParent.Nodes.Add(nodeGroup);

			ArrayList children = new ArrayList();

			using (IDataReader rdr = UserReport.GetListChildGroups(iGroup))
			{
				while (rdr.Read())
					children.Add((int)rdr["GroupId"]);
			}

			foreach (object obj in children)
				ProcessGroup2((int)obj, nodeGroup);

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("UserId", typeof(int)));
			dt.Columns.Add(new DataColumn("UserName", typeof(string)));
			dt.Columns.Add(new DataColumn("sortUserName", typeof(string)));
			DataRow dr;
			using (IDataReader rdr = UserReport.GetListActiveUsersInGroup(iGroup))
			{
				while (rdr.Read())
				{
					dr = dt.NewRow();
					int iUserId = (int)rdr["UserId"];
					dr["UserId"] = iUserId;
					using (IDataReader reader = UserReport.GetUserInfo(iUserId, false))
					{
						if (reader.Read())
						{
							dr["UserName"] = /*"<img src='../Layouts/Images/Status/status_online.gif ' border=0 align='absmiddle'>&nbsp;" +*/(string)reader["FirstName"] + " " + (string)reader["LastName"];
							dr["sortUserName"] = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
						}
					}
					dt.Rows.Add(dr);
				}
			}
			DataView dv = dt.DefaultView;
			dv.Sort = "sortUserName";
			foreach (DataRowView _dr in dv)
				ProcessUser2(iGroup, _dr, nodeGroup);
		}

		private void ProcessUser2(int iGroup, DataRowView dr, TreeViewNode nodeParent)
		{
			TreeViewNode nodeUser;
			nodeUser = new TreeViewNode();
			nodeUser.Text = dr["UserName"].ToString();
			nodeUser.ImageUrl = Page.ResolveUrl("~/Layouts/Images/Status/status_online.gif");
			nodeUser.Value = "User";
			nodeUser.NavigateUrl = String.Format("GroupAndUserStat.aspx?SGroupID={0}&UserID={1}", iGroup.ToString(), dr["UserId"].ToString());
			nodeParent.Nodes.Add(nodeUser);
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region Apply Click
		protected void btnAplly_Click(object sender, System.EventArgs e)
		{
			DateTime StartDate = new DateTime(0);
			DateTime EndDate = new DateTime(0);
			//SetDates(ddPeriod.Value, out StartDate, out EndDate, txtDateFrom.Text, txtDateTo.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			pc["GroupAndUserStat_ddPeriod"] = ddPeriod.Value;
			if (ddPeriod.Value == "9")
			{
				pc["GroupAndUserStat_Start"] = StartDate.ToString(culture);
				pc["GroupAndUserStat_End"] = EndDate.ToString(culture);
			}

			if (UserID == 0)
			{
				BindGroupStat(SGroupID, StartDate, EndDate);
				tblGroupStat.Visible = true;
				tblUser1.Visible = false;
				tblUser2.Visible = false;
				tblUser3.Visible = false;
			}
			else
			{
				BindUserStat(UserID, StartDate, EndDate);
				tblGroupStat.Visible = false;
				tblUser1.Visible = true;
				tblUser2.Visible = true;
				tblUser3.Visible = true;
			}
		}
		#endregion

		#region Export
		protected void btnExport_Click(object sender, System.EventArgs e)
		{
			DateTime StartDate = new DateTime(0);
			DateTime EndDate = new DateTime(0);
			//SetDates(ddPeriod.Value, out StartDate, out EndDate, txtDateFrom.Text, txtDateTo.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			pc["GroupAndUserStat_ddPeriod"] = ddPeriod.Value;
			if (ddPeriod.Value == "9")
			{
				pc["GroupAndUserStat_Start"] = StartDate.ToString(culture);
				pc["GroupAndUserStat_End"] = EndDate.ToString(culture);
			}
			Response.Redirect("~/UserReports/GroupAndUserStat.aspx?Export=1&SGroupID=" + SGroupID.ToString() + "&UserID=" + UserID.ToString());
		}

		private void ExportStatistics()
		{
			dHeader.Visible = true;
			CommonHelper.ExportExcel(exportPanel, "GroupAndUserStatistics.xls", null);
		}
		#endregion

		#region Help Strings
		string FormatBytes(long bytes)
		{
			const double ONE_KB = 1024;
			const double ONE_MB = ONE_KB * 1024;
			const double ONE_GB = ONE_MB * 1024;
			const double ONE_TB = ONE_GB * 1024;
			const double ONE_PB = ONE_TB * 1024;
			const double ONE_EB = ONE_PB * 1024;
			const double ONE_ZB = ONE_EB * 1024;
			const double ONE_YB = ONE_ZB * 1024;

			if ((double)bytes <= 999)
				return bytes.ToString() + " bytes";
			else if ((double)bytes <= ONE_KB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_KB) + " KB";
			else if ((double)bytes <= ONE_MB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_MB) + " MB";
			else if ((double)bytes <= ONE_GB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_GB) + " GB";
			else if ((double)bytes <= ONE_TB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_TB) + " TB";
			else if ((double)bytes <= ONE_PB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_PB) + " PB";
			else if ((double)bytes <= ONE_EB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_EB) + " EB";
			else if ((double)bytes <= ONE_ZB * 999)
				return ThreeNonZeroDigits((double)bytes / ONE_ZB) + " ZB";
			else
				return ThreeNonZeroDigits((double)bytes / ONE_YB) + " YB";
		}

		string ThreeNonZeroDigits(double value)
		{
			if (value >= 100)
				return ((int)value).ToString();
			else if (value >= 10)
				return value.ToString("0.0");
			else
				return value.ToString("0.00");
		}
		#endregion
	}
}
