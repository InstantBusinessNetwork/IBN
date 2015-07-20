using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

// Alex Fix
//using Mediachase.IBN.Business;
using Mediachase.IBN.DefaultUserReports;
using Mediachase.UI.Web.UserReports.Util;


namespace Mediachase.UI.Web.UserReports.Modules
{
	/// <summary>
	///		Summary description for PortalQuickSnapshot.
	/// </summary>
	/// 

	public partial class PortalQuickSnapshot : System.Web.UI.UserControl
	{


		/*
		protected System.Web.UI.WebControls.TextBox fromDate;
		protected System.Web.UI.WebControls.TextBox toDate;
		protected System.Web.UI.WebControls.RangeValidator rvFromDate;
		protected System.Web.UI.WebControls.RangeValidator rvToDate;
		*/
		protected System.Web.UI.WebControls.CustomValidator CustomValidator1;


		IFormatProvider culture = CultureInfo.InvariantCulture;
		Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strPortalQuickSnapshot", typeof(PortalQuickSnapshot).Assembly);
		protected string ProductName = Mediachase.Ibn.IbnConst.ProductName;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (!Page.IsPostBack)
				BindData();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (Page.IsPostBack)
			{
				DateTime StartDate = new DateTime(0);
				DateTime EndDate = new DateTime(0);
				//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
				UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

				pc["PortalQuickSnapshot_ddPeriod"] = ddPeriod.Value;
				if (ddPeriod.Value == "9")
				{
					pc["PortalQuickSnapshot_Start"] = StartDate.ToString(culture);
					pc["PortalQuickSnapshot_End"] = EndDate.ToString(culture);
				}
			}
			int nImMessages = UserReport.GetCountIMMessages(false);
			int nChatMessages = UserReport.GetCountChatMessages();

			lblTotalIMMessages.Text = nImMessages.ToString();
			lblTotalConfMessages.Text = nChatMessages.ToString();
			lblTotalMessages.Text = (nImMessages + nChatMessages).ToString();
			lblTotalFilesTransferred.Text = UserReport.GetFilesTransferred().ToString();

			using (IDataReader reader = Mediachase.IBN.Business.User.GetUserStatistic())
			{
				if (reader.Read())
				{
					try
					{
						lblTotalGroups.Text = ((int)reader["SecureGroupCount"] +
							(int)reader["PartnerGroupCount"]).ToString();
					}
					catch { }
					lblTotalUsers.Text = reader["TotalUserCount"].ToString();
					lblTotalActiveUsers.Text = reader["ActiveUserCount"].ToString();
					lblTotalInactiveUsers.Text = reader["InactiveUserCount"].ToString();
					lblTotalExternalUsers.Text = reader["ExternalCount"].ToString();
					lblTotalPendingUsers.Text = reader["PendingCount"].ToString();
				}
			}

			BindVarData();
			if (ddPeriod.Value == "9")
				tableDate.Style.Add("display", "block");
			else
				tableDate.Style.Add("display", "none");

			_header.ForPrintOnly = true;
			_header.Title = LocRM.GetString("tPrtQSnap");

			if (Request["Export"] != null && Request["Export"] == "1")
			{
				ExportStatistics();
			}
			if (!Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
			{
				trProjects.Visible = false;
				trTasks.Visible = false;
				trTotProjects.Visible = false;
				trTotTasks.Visible = false;
			}
			if (!Mediachase.IBN.Business.Configuration.HelpDeskEnabled)
			{
				trIssues.Visible = false;
				trTotIssues.Visible = false;
			}
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApplySelectPeriod.Text = LocRM.GetString("tShow");
			btnExport.Text = LocRM.GetString("tExport");
			dgTop10.Columns[0].HeaderText = LocRM.GetString("tPlace");
			dgTop10.Columns[1].HeaderText = LocRM.GetString("tGroup");
			dgTop10.Columns[1].Visible = false;
			dgTop10.Columns[2].HeaderText = LocRM.GetString("tName");
			dgTop10.Columns[3].HeaderText = LocRM.GetString("tTotMes");
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
			if (pc["PortalQuickSnapshot_ddPeriod"] != null)
			{
				string sVal = pc["PortalQuickSnapshot_ddPeriod"];
				ListItem liItem = ddPeriod.Items.FindByValue(sVal);
				if (liItem != null)
					liItem.Selected = true;
				if (pc["PortalQuickSnapshot_Start"] != null)
				{
					DateTime _date = DateTime.Parse(pc["PortalQuickSnapshot_Start"], culture);
					//fromDate.Text = _date.ToShortDateString();
					dtcStartDate.SelectedDate = _date;
				}
				else
				{
					//fromDate.Text = UserDateTime.UserToday.ToShortDateString();
					dtcStartDate.SelectedDate = UserDateTime.Today;
				}
				if (pc["PortalQuickSnapshot_End"] != null)
				{
					DateTime _date = DateTime.Parse(pc["PortalQuickSnapshot_End"], culture);
					//toDate.Text = _date.ToShortDateString();
					dtcEndDate.SelectedDate = _date;
				}
				else
				{
					//toDate.Text = UserDateTime.UserNow.ToShortDateString();
					dtcEndDate.SelectedDate = UserDateTime.Now;
				}
			}
			else
			{
				//fromDate.Text = UserDateTime.UserToday.AddMonths(-1).ToShortDateString();
				dtcStartDate.SelectedDate = UserDateTime.Today.AddMonths(-1);
				//toDate.Text = UserDateTime.UserNow.ToShortDateString();
				dtcEndDate.SelectedDate = UserDateTime.Now;
			}
			if (ddPeriod.Value != "9")
				tableDate.Style.Add("display", "none");
		}

		private void BindVarData()
		{
			DateTime StartDate = new DateTime(0);
			DateTime EndDate = new DateTime(0);

			//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			using (IDataReader reader = UserReport.GetQuickSnapshotReport(StartDate, EndDate, 0))
			{
				if (reader.Read())
				{
					lblTotalProjects.Text = reader["ProjectsCount"].ToString();
					lblTotalCalendarEntries.Text = reader["EventsCount"].ToString();
					lblTotalIssues.Text = reader["IncidentsCount"].ToString();
					lblTotalToDo.Text = reader["ToDosCount"].ToString();
					lblTotalTasks.Text = reader["TasksCount"].ToString();
					//lblTotalLibraryFiles.Text = reader["AssetsCount"].ToString();
					//lblTotalLibraryFileVersions.Text = reader["AssetVersionsCount"].ToString();
					lblNewProjectsCreated.Text = reader["NewProjectsCount"].ToString();
					lblNewCalendarEntries.Text = reader["NewEventsCount"].ToString();
					//lblNewFiles.Text = reader["NewAssetsCount"].ToString();
					//lblNewFileVersions.Text = reader["NewAssetVersionsCount"].ToString();
					lblNewIssuesCreated.Text = reader["NewIncidentsCount"].ToString();
					lblNewToDo.Text = reader["NewToDosCount"].ToString();
					lblNewTask.Text = reader["NewTasksCount"].ToString();
				}
			}

			lblAuthenticatedUsers.Text = UserReport.GetAuthenticatedUsers(StartDate, EndDate).ToString();
			lblPerTotalIMMaessages.Text = UserReport.GetCountIMMessages(StartDate, EndDate, false).ToString();
			lblPerTotalChatMessages.Text = UserReport.GetCountChatMessages(StartDate, EndDate).ToString();
			lblPerTotalFilesTransferred.Text = UserReport.GetFilesTransferred(StartDate, EndDate).ToString();

			dgTop10.DataSource = UserReport.GetTop10Users(StartDate, EndDate);
			dgTop10.DataBind();

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

		protected void btnExport_Click(object sender, System.EventArgs e)
		{
			DateTime StartDate = new DateTime(0);
			DateTime EndDate = new DateTime(0);
			//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			pc["PortalQuickSnapshot_ddPeriod"] = ddPeriod.Value;
			if (ddPeriod.Value == "9")
			{
				pc["PortalQuickSnapshot_Start"] = StartDate.ToString(culture);
				pc["PortalQuickSnapshot_End"] = EndDate.ToString(culture);
			}
			/*if(ddPeriod.Value!="9")
				Response.Redirect("~/UserReports/PortalQuickSnapshot.aspx?Export=1&Period="+ddPeriod.Value);
			else
				Response.Redirect("~/UserReports/PortalQuickSnapshot.aspx?Export=1&Start="+fromDate.Text+"&End="+toDate.Text);*/
			Response.Redirect("~/UserReports/PortalQuickSnapshot.aspx?Export=1");
		}

		private void ExportStatistics()
		{
			exportHeader.Visible = true;
			CommonHelper.ExportExcel(exportPanel, "PortalQuickSnapshot.xls", null);
		}
	}
}
