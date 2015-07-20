using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Apps.ClioSoft.Modules
{
	public partial class ProjectTaskBusReport : System.Web.UI.UserControl, Mediachase.IBN.Business.UserReport.IUserReportInfo
	{
		private const string periodAny = "0";
		private const string periodYear = "1";
		private const string periodQuarter = "2";
		private const string periodMonth = "3";
		private const string periodCustom = "4";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.ClioSoftReports.App_GlobalResources.Report", typeof(ProjectTaskBusReport).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindDefaultValues();
			}

			MainTable.Visible = false;
			PrintButton.Visible = false;

			ManagerControl.ValueChange += new MultiSelectControl.ValueChanged(ManagerControl_ValueChange);
			ClientControl.ValueChange += new MultiSelectControl.ValueChanged(ClientControl_ValueChange);
			ProjectGroupControl.ValueChange += new MultiSelectControl.ValueChanged(ProjectGroupControl_ValueChange);
			FromDate.ValueChange +=new PickerControl.ValueChanged(FromDate_ValueChange);
			ToDate.ValueChange +=new PickerControl.ValueChanged(ToDate_ValueChange);
			CompletionFromDate.ValueChange +=new PickerControl.ValueChanged(CompletionFromDate_ValueChange);
			CompletionToDate.ValueChange +=new PickerControl.ValueChanged(CompletionToDate_ValueChange);
		}

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			// Grouping
			GroupingList.Items.Add(new ListItem(LocRM.GetString("ByProject"), "1"));
			GroupingList.Items.Add(new ListItem(LocRM.GetString("ByBusinessScore"), "2"));

			// PeriodType
			PeriodType.Items.Add(new ListItem(LocRM.GetString("AnyPeriod"), periodAny));
			PeriodType.Items.Add(new ListItem(LocRM.GetString("Year"), periodYear));
			PeriodType.Items.Add(new ListItem(LocRM.GetString("Quarter"), periodQuarter));
			PeriodType.Items.Add(new ListItem(LocRM.GetString("Month"), periodMonth));
			PeriodType.Items.Add(new ListItem(LocRM.GetString("Custom"), periodCustom));

			// YearList
			YearList.Items.Add(new ListItem("2008"));
			YearList.Items.Add(new ListItem("2009"));
			YearList.Items.Add(new ListItem("2010"));
			YearList.Items.Add(new ListItem("2011"));
			YearList.Items.Add(new ListItem("2012"));
			YearList.Items.Add(new ListItem("2013"));
			YearList.Items.Add(new ListItem("2014"));
			YearList.Items.Add(new ListItem("2015"));
			CommonHelper.SafeSelect(YearList, DateTime.UtcNow.Year.ToString());

			YearList.Visible = false;

			// QuarterList
			QuarterList.Items.Add(new ListItem("1 " + LocRM.GetString("QuarterName"), "1"));
			QuarterList.Items.Add(new ListItem("2 " + LocRM.GetString("QuarterName"), "2"));
			QuarterList.Items.Add(new ListItem("3 " + LocRM.GetString("QuarterName"), "3"));
			QuarterList.Items.Add(new ListItem("4 " + LocRM.GetString("QuarterName"), "4"));

			int quarter = ((DateTime.UtcNow.Month - 1) / 3) + 1;
			CommonHelper.SafeSelect(QuarterList, quarter.ToString());

			QuarterList.Visible = false;

			// MonthList
			MonthList.Items.Add(new ListItem(LocRM.GetString("January"), "1"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("February"), "2"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("March"), "3"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("April"), "4"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("May"), "5"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("June"), "6"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("July"), "7"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("August"), "8"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("September"), "9"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("October"), "10"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("November"), "11"));
			MonthList.Items.Add(new ListItem(LocRM.GetString("December"), "12"));

			CommonHelper.SafeSelect(MonthList, DateTime.UtcNow.Month.ToString());

			MonthList.Visible = false;

			// Dates
			FromDate.SelectedDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date.AddMonths(-1);
			ToDate.SelectedDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
			FromDate.Visible = false;
			ToDate.Visible = false;
			DateDelimiter.Visible = false;

			// Completion
			CompletionList.Items.Add(new ListItem(LocRM.GetString("AnyThey"), ((int)SelectProjectType.All).ToString()));
			CompletionList.Items.Add(new ListItem(LocRM.GetString("Completed"), ((int)SelectProjectType.FinishedInCurrentPeriod).ToString()));
			CompletionList.Items.Add(new ListItem(LocRM.GetString("NotCompleted"), ((int)SelectProjectType.ActiveOnly).ToString()));

			// Completion Period
			CompletionPeriodRow.Visible = false;

			// Completion PeriodType
			CompletionPeriodType.Items.Add(new ListItem(LocRM.GetString("Year"), periodYear));
			CompletionPeriodType.Items.Add(new ListItem(LocRM.GetString("Quarter"), periodQuarter));
			CompletionPeriodType.Items.Add(new ListItem(LocRM.GetString("Month"), periodMonth));
			CompletionPeriodType.Items.Add(new ListItem(LocRM.GetString("Custom"), periodCustom));

			// Completion YearList
			CompletionYearList.Items.Add(new ListItem("2008"));
			CompletionYearList.Items.Add(new ListItem("2009"));
			CompletionYearList.Items.Add(new ListItem("2010"));
			CompletionYearList.Items.Add(new ListItem("2011"));
			CompletionYearList.Items.Add(new ListItem("2012"));
			CompletionYearList.Items.Add(new ListItem("2013"));
			CompletionYearList.Items.Add(new ListItem("2014"));
			CompletionYearList.Items.Add(new ListItem("2015"));
			CommonHelper.SafeSelect(CompletionYearList, DateTime.UtcNow.Year.ToString());

			// Completion QuarterList
			CompletionQuarterList.Items.Add(new ListItem("1 " + LocRM.GetString("QuarterName"), "1"));
			CompletionQuarterList.Items.Add(new ListItem("2 " + LocRM.GetString("QuarterName"), "2"));
			CompletionQuarterList.Items.Add(new ListItem("3 " + LocRM.GetString("QuarterName"), "3"));
			CompletionQuarterList.Items.Add(new ListItem("4 " + LocRM.GetString("QuarterName"), "4"));

			CommonHelper.SafeSelect(CompletionQuarterList, quarter.ToString());

			CompletionQuarterList.Visible = false;

			// Completion MonthList
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("January"), "1"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("February"), "2"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("March"), "3"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("April"), "4"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("May"), "5"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("June"), "6"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("July"), "7"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("August"), "8"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("September"), "9"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("October"), "10"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("November"), "11"));
			CompletionMonthList.Items.Add(new ListItem(LocRM.GetString("December"), "12"));

			CommonHelper.SafeSelect(CompletionMonthList, DateTime.UtcNow.Month.ToString());

			CompletionMonthList.Visible = false;

			// Completion Dates
			CompletionFromDate.SelectedDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow).Date.AddMonths(-1);
			CompletionToDate.SelectedDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(DateTime.UtcNow);
			CompletionFromDate.Visible = false;
			CompletionToDate.Visible = false;
			CompletionDateDelimiter.Visible = false;

			// Texts
			ShowButton.Text = LocRM.GetString("Show");
			PrintButton.Value = LocRM.GetString("Print");
			ProjectGroupControl.AnyText = LocRM.GetString("AnyShe");

			BindManagers();
		}
		#endregion

		#region IUserReportInfo Members
		public string ShowName
		{
			get
			{
				return LocRM.GetString("ManagerReportName");
			}
		}

		public string Description
		{
			get
			{
				return string.Empty;
			}
		}
		#endregion

		#region === Event Handlers ===
		#region PeriodType_SelectedIndexChanged
		protected void PeriodType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (PeriodType.SelectedValue == periodAny)
			{
				YearList.Visible = false;
				QuarterList.Visible = false;
				MonthList.Visible = false;
				FromDate.Visible = false;
				ToDate.Visible = false;
				DateDelimiter.Visible = false;
			}
			else if (PeriodType.SelectedValue == periodYear)
			{
				YearList.Visible = true;
				QuarterList.Visible = false;
				MonthList.Visible = false;
				FromDate.Visible = false;
				ToDate.Visible = false;
				DateDelimiter.Visible = false;
			}
			else if (PeriodType.SelectedValue == periodQuarter)
			{
				YearList.Visible = true;
				QuarterList.Visible = true;
				MonthList.Visible = false;
				FromDate.Visible = false;
				ToDate.Visible = false;
				DateDelimiter.Visible = false;
			}
			else if (PeriodType.SelectedValue == periodMonth)
			{
				YearList.Visible = true;
				QuarterList.Visible = false;
				MonthList.Visible = true;
				FromDate.Visible = false;
				ToDate.Visible = false;
				DateDelimiter.Visible = false;
			}
			else if (PeriodType.SelectedValue == periodCustom)
			{
				YearList.Visible = false;
				QuarterList.Visible = false;
				MonthList.Visible = false;
				FromDate.Visible = true;
				ToDate.Visible = true;
				DateDelimiter.Visible = true;
			}
			BindManagers();
		}
		#endregion

		#region YearList_SelectedIndexChanged
		protected void YearList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindManagers();
		}
		#endregion

		#region QuarterList_SelectedIndexChanged
		protected void QuarterList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindManagers();
		}
		#endregion

		#region MonthList_SelectedIndexChanged
		protected void MonthList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindManagers();
		}
		#endregion

		#region FromDate_ValueChange
		protected void FromDate_ValueChange(object sender, EventArgs e)
		{
			BindManagers();
		}
		#endregion

		#region ToDate_ValueChange
		protected void ToDate_ValueChange(object sender, EventArgs e)
		{
			BindManagers();
		}
		#endregion

		#region ManagerControl_ValueChange
		protected void ManagerControl_ValueChange(object sender, EventArgs e)
		{
			BindClients();
		}
		#endregion

		#region ClientControl_ValueChange
		protected void ClientControl_ValueChange(object sender, EventArgs e)
		{
			BindProjectGroups();
		}
		#endregion

		#region ProjectGroupControl_ValueChange
		void ProjectGroupControl_ValueChange(object sender, EventArgs e)
		{
			BindProjects();
		}
		#endregion

		#region CompletionList_SelectedIndexChanged
		protected void CompletionList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CompletionList.SelectedValue == ((int)SelectProjectType.FinishedInCurrentPeriod).ToString())
				CompletionPeriodRow.Visible = true;
			else
				CompletionPeriodRow.Visible = false;
			BindProjects();
		}
		#endregion

		#region CompletionPeriodType_SelectedIndexChanged
		protected void CompletionPeriodType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CompletionPeriodType.SelectedValue == periodAny)
			{
				CompletionYearList.Visible = false;
				CompletionQuarterList.Visible = false;
				CompletionMonthList.Visible = false;
				CompletionFromDate.Visible = false;
				CompletionToDate.Visible = false;
				CompletionDateDelimiter.Visible = false;
			}
			else if (CompletionPeriodType.SelectedValue == periodYear)
			{
				CompletionYearList.Visible = true;
				CompletionQuarterList.Visible = false;
				CompletionMonthList.Visible = false;
				CompletionFromDate.Visible = false;
				CompletionToDate.Visible = false;
				CompletionDateDelimiter.Visible = false;
			}
			else if (CompletionPeriodType.SelectedValue == periodQuarter)
			{
				CompletionYearList.Visible = true;
				CompletionQuarterList.Visible = true;
				CompletionMonthList.Visible = false;
				CompletionFromDate.Visible = false;
				CompletionToDate.Visible = false;
				CompletionDateDelimiter.Visible = false;
			}
			else if (CompletionPeriodType.SelectedValue == periodMonth)
			{
				CompletionYearList.Visible = true;
				CompletionQuarterList.Visible = false;
				CompletionMonthList.Visible = true;
				CompletionFromDate.Visible = false;
				CompletionToDate.Visible = false;
				CompletionDateDelimiter.Visible = false;
			}
			else if (CompletionPeriodType.SelectedValue == periodCustom)
			{
				CompletionYearList.Visible = false;
				CompletionQuarterList.Visible = false;
				CompletionMonthList.Visible = false;
				CompletionFromDate.Visible = true;
				CompletionToDate.Visible = true;
				CompletionDateDelimiter.Visible = true;
			}
			BindProjects();
		}
		#endregion

		#region CompletionYearList_SelectedIndexChanged
		protected void CompletionYearList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindProjects();
		}
		#endregion

		#region CompletionQuarterList_SelectedIndexChanged
		protected void CompletionQuarterList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindProjects();
		}
		#endregion

		#region CompletionMonthList_SelectedIndexChanged
		protected void CompletionMonthList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindProjects();
		}
		#endregion

		#region CompletionFromDate_ValueChange
		protected void CompletionFromDate_ValueChange(object sender, EventArgs e)
		{
			BindProjects();
		}
		#endregion

		#region CompletionToDate_ValueChange
		protected void CompletionToDate_ValueChange(object sender, EventArgs e)
		{
			BindProjects();
		}
		#endregion
		#endregion

		#region GetFromDate
		private DateTime GetFromDate()
		{
			DateTime fromDate = FromDate.SelectedDate;
			if (PeriodType.SelectedValue == periodAny)
			{
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			}
			else if (PeriodType.SelectedValue == periodYear)
			{
				int year = int.Parse(YearList.SelectedValue);
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, 1, 1));
			}
			else if (PeriodType.SelectedValue == periodQuarter)
			{
				int year = int.Parse(YearList.SelectedValue);
				int month = (int.Parse(QuarterList.SelectedValue) - 1) * 3 + 1;
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1));
			}
			else if (PeriodType.SelectedValue == periodMonth)
			{
				int year = int.Parse(YearList.SelectedValue);
				int month = int.Parse(MonthList.SelectedValue);
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1));
			}
			return fromDate.Date;
		}
		#endregion

		#region GetToDate
		private DateTime GetToDate()
		{
			DateTime toDate = ToDate.SelectedDate.AddDays(1);
			if (PeriodType.SelectedValue == periodAny)
			{
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(2100, 1, 1));
			}
			else if (PeriodType.SelectedValue == periodYear)
			{
				int year = int.Parse(YearList.SelectedValue);
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, 1, 1)).AddYears(1);
			}
			else if (PeriodType.SelectedValue == periodQuarter)
			{
				int year = int.Parse(YearList.SelectedValue);
				int month = (int.Parse(QuarterList.SelectedValue) - 1) * 3 + 1;
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1)).AddMonths(3);
			}
			else if (PeriodType.SelectedValue == periodMonth)
			{
				int year = int.Parse(YearList.SelectedValue);
				int month = int.Parse(MonthList.SelectedValue);
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1)).AddMonths(1);
			}
			return toDate.Date;
		}
		#endregion

		#region GetCompletionFromDate
		private DateTime GetCompletionFromDate()
		{
			DateTime fromDate = CompletionFromDate.SelectedDate;
			if (CompletionPeriodType.SelectedValue == periodYear)
			{
				int year = int.Parse(CompletionYearList.SelectedValue);
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, 1, 1));
			}
			else if (CompletionPeriodType.SelectedValue == periodQuarter)
			{
				int year = int.Parse(CompletionYearList.SelectedValue);
				int month = (int.Parse(CompletionQuarterList.SelectedValue) - 1) * 3 + 1;
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1));
			}
			else if (CompletionPeriodType.SelectedValue == periodMonth)
			{
				int year = int.Parse(CompletionYearList.SelectedValue);
				int month = int.Parse(CompletionMonthList.SelectedValue);
				fromDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1));
			}
			return fromDate.Date;
		}
		#endregion

		#region GetCompletionToDate
		private DateTime GetCompletionToDate()
		{
			DateTime toDate = CompletionToDate.SelectedDate.AddDays(1);
			if (CompletionPeriodType.SelectedValue == periodYear)
			{
				int year = int.Parse(CompletionYearList.SelectedValue);
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, 1, 1)).AddYears(1);
			}
			else if (CompletionPeriodType.SelectedValue == periodQuarter)
			{
				int year = int.Parse(CompletionYearList.SelectedValue);
				int month = (int.Parse(CompletionQuarterList.SelectedValue) - 1) * 3 + 1;
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1)).AddMonths(3);
			}
			else if (CompletionPeriodType.SelectedValue == periodMonth)
			{
				int year = int.Parse(CompletionYearList.SelectedValue);
				int month = int.Parse(CompletionMonthList.SelectedValue);
				toDate = Security.CurrentUser.CurrentTimeZone.ToLocalTime(new DateTime(year, month, 1)).AddMonths(1);
			}
			return toDate.Date;
		}
		#endregion

		#region BindManagers
		private void BindManagers()
		{
			ManagerControl.DataValueField = "ManagerId";
			ManagerControl.DataTextField = "ManagerName";
			ManagerControl.DataSource = BusReport.GetProjectManagers(GetFromDate(), GetToDate());
			ManagerControl.DataBind();

			if (ManagerControl.Items.Count > 0)
				ManagerControl.Value = new string[] { "0" };	// any
			else
				ManagerControl.Value = new string[] { };	// not set

			BindClients();
		}
		#endregion

		#region BindClients
		private void BindClients()
		{
			ClientControl.DataValueField = "ClientId";
			ClientControl.DataTextField = "ClientName";
			ClientControl.DataSource = BusReport.GetClients(GetFromDate(), GetToDate(), ManagerControl.Value);
			ClientControl.DataBind();

			if (ClientControl.Items.Count > 0)
				ClientControl.Value = new string[] { "0" };	// any
			else
				ClientControl.Value = new string[] { };	// not set

			BindProjectGroups();
		}
		#endregion

		#region BindProjectGroups
		private void BindProjectGroups()
		{
			ProjectGroupControl.DataValueField = "ProjectGroupId";
			ProjectGroupControl.DataTextField = "ProjectGroupName";
			ProjectGroupControl.DataSource = BusReport.GetProjectGroups(GetFromDate(), GetToDate(), ManagerControl.Value, ClientControl.Value);
			ProjectGroupControl.DataBind();

			if (ProjectGroupControl.Items.Count > 0)
				ProjectGroupControl.Value = new string[] { "0" };	// any
			else
				ProjectGroupControl.Value = new string[] { };	// not set

			BindProjects();
		}
		#endregion

		#region BindProjects
		private void BindProjects()
		{
			ProjectControl.DataValueField = "ProjectId";
			ProjectControl.DataTextField = "ProjectName";
			ProjectControl.DataSource = BusReport.GetProjects(GetFromDate(), GetToDate(), ManagerControl.Value, ClientControl.Value, ProjectGroupControl.Value, int.Parse(CompletionList.SelectedValue), GetCompletionFromDate(), GetCompletionToDate());
			ProjectControl.DataBind();

			if (ProjectControl.Items.Count > 0)
				ProjectControl.Value = new string[] { "0" };	// any
			else
				ProjectControl.Value = new string[] { };	// not set
		}
		#endregion 

		#region ShowButton_Click
		protected void ShowButton_Click(object sender, EventArgs e)
		{
			string[] projectStringArray = ProjectControl.GetAllCheckedItems();
			List<string> projectStringList = new List<string>(projectStringArray);
			List<int> projectIdList = projectStringList.ConvertAll<int>(Convert.ToInt32);

			ReportResult result;
			if (GroupingList.SelectedValue == "1")	// By project
			{
				result = BusReport.GenerateProjectTaskBusinessScoreReport(projectIdList, GetFromDate(), GetToDate(), int.Parse(CompletionList.SelectedValue), GetCompletionFromDate(), GetCompletionToDate());
			}
			else
			{
				result = BusReport.GenerateBusinessScoreProjectTaskReport(projectIdList, GetFromDate(), GetToDate(), int.Parse(CompletionList.SelectedValue), GetCompletionFromDate(), GetCompletionToDate());
			}

			GenerateTable(result, GroupingList.SelectedValue == "1");
			MainTable.Visible = true;
			PrintButton.Visible = true;
		}
		#endregion

		#region GenerateTable
		private void GenerateTable(ReportResult result, bool showTotal)
		{
			MainTable.Rows.Clear();

			ProcessChildItems(result.Items, "group_", 0, true);

			if (showTotal)
			{
				// Total row
				TableRow row = new TableRow();
				{
					TableCell cell1 = new TableCell();
					cell1.Text = LocRM.GetString("Total");
					cell1.Font.Bold = true;
					row.Cells.Add(cell1);

					TableCell cell2 = new TableCell();
					cell2.Text = result.Total.ToString("f");
					cell2.Font.Bold = true;
					cell2.HorizontalAlign = HorizontalAlign.Right;
					cell2.Width = Unit.Pixel(100);
					row.Cells.Add(cell2);
				}
				MainTable.Rows.Add(row);
			}
		}
		#endregion

		#region ProcessChildItems
		private void ProcessChildItems(List<ReportResultItem> items, string groupKey, int level, bool visibility)
		{
			if (items == null)
				return;

			for (int i = 0; i < items.Count; i++)
			{
				string newGroupKey = String.Format(CultureInfo.InvariantCulture, "{0}{1}_", groupKey, i);
				ReportResultItem item = items[i];
				bool hasChildren = (item.ChildItems != null && item.ChildItems.Count > 0);
				TableRow row = new TableRow();
				{
					TableCell cell1 = new TableCell();
					string imgUrl = ResolveClientUrl(hasChildren ? "~/layouts/images/plus.gif" : "~/layouts/images/dot.gif");
					cell1.Text = String.Format(
						CultureInfo.InvariantCulture,
						"<span style='padding-left:{0}px;'><img src='{1}' border='0' width='9px' height='9px' /> {2}</span>",
						level * 20,
						imgUrl,
						item.Name);
					row.Cells.Add(cell1);

					TableCell cell2 = new TableCell();
					cell2.Text = item.Total.ToString("f");
					cell2.HorizontalAlign = HorizontalAlign.Right;
					cell2.Width = Unit.Pixel(100);
					row.Cells.Add(cell2);
				}
				row.Style.Add(HtmlTextWriterStyle.Display, visibility ? "" : "none");
				row.Attributes.Add("key", groupKey);
				if (hasChildren)
				{
					row.Style.Add(HtmlTextWriterStyle.Cursor, "hand");
					row.Attributes.Add("onclick",
						String.Format(CultureInfo.InvariantCulture, "collapse_expand(this, '{0}');", newGroupKey));
				}
				MainTable.Rows.Add(row);

				ProcessChildItems(item.ChildItems, newGroupKey, level + 1, false);
			}
		}
		#endregion

	}
}