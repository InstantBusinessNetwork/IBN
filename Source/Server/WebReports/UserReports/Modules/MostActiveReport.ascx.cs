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

using Mediachase.IBN.DefaultUserReports;
using Mediachase.UI.Web.UserReports.Util;
using Mediachase.Ibn;
//using Mediachase.IBN.Business;


namespace Mediachase.UI.Web.UserReports.Modules
{

	/// <summary>
	///		Summary description for MostActiveReport.
	/// </summary>
	/// 

	public partial class MostActiveReport : System.Web.UI.UserControl
	{

		/*
		protected System.Web.UI.WebControls.TextBox fromDate;
		protected System.Web.UI.WebControls.TextBox toDate;
		protected System.Web.UI.WebControls.RangeValidator rvFromDate;
		protected System.Web.UI.WebControls.RangeValidator rvToDate;
		*/
		protected System.Web.UI.WebControls.CustomValidator CustomValidator1;


		private const int topxx = 20;

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.Resources.strMostActiveReport", typeof(MostActiveReport).Assembly);
		IFormatProvider culture = CultureInfo.InvariantCulture;
		Mediachase.IBN.Business.UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Alex Fix
			/*
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();
			*/
			ApplyLocalization();

			if (!IsPostBack)
				BindSelects();
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindTable();
			if (ddPeriod.Value == "9")
				tableDate.Style.Add("display", "block");
			else
				tableDate.Style.Add("display", "none");

			_header.ForPrintOnly = true;
			_header.Title = LocRM.GetString("tMostActGU");
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnApplySelectPeriod.Text = LocRM.GetString("tShow");
			lgdMain.InnerText = LocRM.GetString("tType");
		}
		#endregion

		#region BindSelects
		private void BindSelects()
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
			if (pc["MostActive_ddPeriod"] != null)
			{
				string sVal = pc["MostActive_ddPeriod"];
				ListItem liItem = ddPeriod.Items.FindByValue(sVal);
				if (liItem != null)
					liItem.Selected = true;
				if (pc["MostActive_Start"] != null)
				{
					DateTime _date = DateTime.Parse(pc["MostActive_Start"], culture);
					//fromDate.Text = _date.ToShortDateString();
					dtcStartDate.SelectedDate = _date;
				}
				else
				{
					//fromDate.Text = UserDateTime.UserToday.ToShortDateString();
					dtcStartDate.SelectedDate = UserDateTime.Today;
				}
				if (pc["MostActive_End"] != null)
				{
					DateTime _date = DateTime.Parse(pc["MostActive_End"], culture);
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

			rblSelectType.Items.Add(new ListItem(LocRM.GetString("tSecGroup"), "SecGroup"));
			rblSelectType.Items.Add(new ListItem(LocRM.GetString("tGroup"), "Group"));
			rblSelectType.Items.Add(new ListItem(LocRM.GetString("tUser"), "User"));
			rblSelectType.SelectedIndex = 0;

			BindSelectView();
		}
		#endregion

		#region BindSelectView
		private void BindSelectView()
		{
			ddSelectView.Items.Clear();
			if (rblSelectType.SelectedItem.Value != "SecGroup")
			{
				ddSelectView.Items.Add(new ListItem(LocRM.GetString("tMessagesSent"), ActivityReportType.Messages.ToString()));
				ddSelectView.Items.Add(new ListItem(LocRM.GetString("tFilesExchanged"), ActivityReportType.FilesExchanged.ToString()));
				ddSelectView.Items.Add(new ListItem(String.Format(LocRM.GetString("tIMLogins"), IbnConst.ProductFamilyShort), ActivityReportType.IMLogins.ToString()));
			}
			ddSelectView.Items.Add(new ListItem(LocRM.GetString("tPortalLogins"), ActivityReportType.PortalLogins.ToString()));
			if (Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
				ddSelectView.Items.Add(new ListItem(LocRM.GetString("tActiveProjects"), ActivityReportType.ActiveProjects.ToString()));
			if (Mediachase.IBN.Business.Configuration.HelpDeskEnabled)
				ddSelectView.Items.Add(new ListItem(LocRM.GetString("tIssuesCreated"), ActivityReportType.NewIssues.ToString()));
			ddSelectView.Items.Add(new ListItem(LocRM.GetString("tCalendarEntries"), ActivityReportType.CalendarEntries.ToString()));
			if (Mediachase.IBN.Business.Configuration.ProjectManagementEnabled)
				ddSelectView.Items.Add(new ListItem(LocRM.GetString("tTasksCreated"), ActivityReportType.NewTasks.ToString()));
			ddSelectView.Items.Add(new ListItem(LocRM.GetString("tToDosCreated"), ActivityReportType.NewToDos.ToString()));
			if (pc["MostActive_ViewBy"] != null)
			{
				ListItem liItem = ddSelectView.Items.FindByValue(pc["MostActive_ViewBy"]);
				if (liItem != null)
				{
					ddSelectView.ClearSelection();
					liItem.Selected = true;
				}
			}
		}
		#endregion

		private void BindTable()
		{
			DateTime StartDate, EndDate;
			//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			ActivityReportType art = (ActivityReportType)Enum.Parse(typeof(ActivityReportType), ddSelectView.SelectedItem.Value);

			DataTable at = null;
			switch (rblSelectType.SelectedItem.Value)
			{
				case "Group":
					at = UserReport.GetGroupsActivityTable(StartDate, EndDate, art, topxx);
					break;
				case "User":
					at = UserReport.GetUsersActivityTable(StartDate, EndDate, art, topxx);
					break;
				case "SecGroup":
					switch (art)
					{
						case ActivityReportType.PortalLogins:
							at = UserReport.GetMostActiveGroupsByPortalLoginsReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.ActiveProjects:
							at = UserReport.GetMostActiveGroupsByProjectReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.NewIssues:
							at = UserReport.GetMostActiveGroupsByIncidentReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.CalendarEntries:
							at = UserReport.GetMostActiveGroupsByEventReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.NewTasks:
							at = UserReport.GetMostActiveGroupsByTaskReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.NewToDos:
							at = UserReport.GetMostActiveGroupsByToDoReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.FilesPublished:
							at = UserReport.GetMostActiveGroupsByAssetReport(StartDate, EndDate, topxx);
							break;
						case ActivityReportType.FileVersionsPublished:
							at = UserReport.GetMostActiveGroupsByAssetVersionReport(StartDate, EndDate, topxx);
							break;
					}
					break;
			}

			if (at.Rows.Count > 0)
				BuildTable(at);
			else
			{
				lblTable.Text = "<td colspan='4'>" + LocRM.GetString("tNoRec") + "</td>";
				trLegend.Visible = false;
			}

			string sFilter = "&nbsp;&nbsp;" + LocRM.GetString("tViewBy") + ":&nbsp;" + ddSelectView.SelectedItem.Text;
			if (ddPeriod.Value != "0")
				sFilter += "<br/>&nbsp;&nbsp;" + LocRM.GetString("tPeriod") + ":&nbsp;" + StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
			_header.Filter = sFilter;
			lblRepType.Text = ddSelectView.SelectedItem.Text;
		}

		#region BuildTable
		private void BuildTable(DataTable at)
		{
			StringBuilder sb = new StringBuilder();

			int index = 1;
			int hvalue = 0;

			int maxvalue = (int)at.Rows[0]["Count"];
			if (maxvalue <= 10)
				hvalue = 10;
			else
			{
				int div = (int)Math.Pow(10, (int)Math.Log10(maxvalue));
				hvalue = (maxvalue / div) * div + div;
			}

			foreach (DataRow dr in at.Rows)
			{
				int val = (int)dr["Count"];

				sb.Append("<tr height='14px'><td width='30%'>");
				sb.Append(index.ToString() + ". " + CommonHelper.GetResFileString(dr["DisplayName"].ToString()));
				sb.Append("</td><td colspan='3' ");

				if (index == 1 && at.Rows.Count > 1)
					sb.Append("class='top' ");

				if (index == at.Rows.Count && at.Rows.Count > 1)
					sb.Append("class='bottom' ");

				if (index == 1 && at.Rows.Count == 1)
					sb.Append("class='all' ");

				else
					sb.Append("class='leftright' ");

				sb.Append("valign='center'>");
				sb.Append("<img alt='' src='../layouts/images/point.gif' height='10' width='" + ((val * 100) / (hvalue) - 4) + "%' border='0' align='absMiddle'/>");
				sb.Append("&nbsp;" + val.ToString());
				sb.Append("</td></tr>");

				index++;
			}
			lblTable.Text = sb.ToString();
			lblMaxValue.Text = hvalue.ToString();
			trLegend.Visible = true;
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

		protected void btnApplySelectPeriod_Click(object sender, System.EventArgs e)
		{
			DateTime StartDate = new DateTime(0);
			DateTime EndDate = new DateTime(0);
			//SetDates(ddPeriod.Value, out StartDate, out EndDate, fromDate.Text, toDate.Text);
			UserReport.GetDates(ddPeriod.Value, out StartDate, out EndDate, dtcStartDate.SelectedDate.ToShortDateString(), dtcEndDate.SelectedDate.ToShortDateString());

			pc["MostActive_ddPeriod"] = ddPeriod.Value;
			if (ddPeriod.Value == "9")
			{
				pc["MostActive_Start"] = StartDate.ToString(culture);
				pc["MostActive_End"] = EndDate.ToString(culture);
			}
			pc["MostActive_ViewBy"] = ddSelectView.SelectedItem.Value;
		}

		protected void rblSelectType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			BindSelectView();
			pc["MostActive_ViewBy"] = ddSelectView.SelectedItem.Value;
		}
	}
}
