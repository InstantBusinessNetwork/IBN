using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists
{
	public partial class ProjectReport : System.Web.UI.UserControl
	{
		ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", Assembly.GetExecutingAssembly());
		private const string GroupingWeekUser = "0";
		private const string GroupingUserWeek = "1";

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				Dtc1.Visible = false;
				Dtc2.Visible = false;

				BindDD();
				if (PanelFilters.Visible)
					BindGrid();
			}

			PrintButton.Value = LocRM.GetString("tPrint");
			ExportButton.Value = LocRM.GetString("tExport");
			
			HeaderControl.ForPrintOnly = true;
			HeaderControl.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "ProjectReport").ToString();

			cbShowWeekNumber.CheckedChanged += new EventHandler(cbShowWeekNumber_CheckedChanged);
		}

		#region cbShowWeekNumber_CheckedChanged
		/// <summary>
		/// Handles the CheckedChanged event of the cbShowWeekNumber control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void cbShowWeekNumber_CheckedChanged(object sender, EventArgs e)
		{
			BindGrid();
		} 
		#endregion

		#region BindDD
		private void BindDD()
		{
			// Group
			GroupingList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "GroupingWeekUser").ToString(), GroupingWeekUser));
			GroupingList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "GroupingUserWeek").ToString(), GroupingUserWeek));

			// Projects
			string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;
			foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				ListItem li = new ListItem(bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
				ProjectList.Items.Add(li);
			}

			if (ProjectList.Items.Count <= 0)
			{
				NoProjectsDiv.Visible = true;
				PanelFilters.Visible = false;
				return;
			}
			else
			{
				NoProjectsDiv.Visible = false;
			}

			// Users
			UserList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));
			Principal[] mas = Principal.List(new FilterElementCollection(FilterElement.EqualElement("Card", "User"), FilterElement.EqualElement("Activity", 3)), new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc)));
			foreach (Principal pl in mas)
			{
				UserList.Items.Add(new ListItem(pl.Name, pl.PrimaryKeyId.ToString()));
			}

			// Dates
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Any}"), "[DateTimeThisAny]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisWeek}"), "[DateTimeThisWeek]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastWeek}"), "[DateTimeLastWeek]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisMonth}"), "[DateTimeThisMonth]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastMonth}"), "[DateTimeLastMonth]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisYear}"), "[DateTimeThisYear]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastYear}"), "[DateTimeLastYear]"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_CustomWeek}"), "0"));
			PeriodList.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_CustomPeriod}"), "-1"));

			Dtc1.SelectedDate = CHelper.GetRealWeekStartByDate(DateTime.Today);
			Dtc2.SelectedDate = CHelper.GetRealWeekStartByDate(DateTime.Today);
		}
		#endregion

		#region BindGrid
		private void BindGrid()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Day1", typeof(string)));
			dt.Columns.Add(new DataColumn("Day2", typeof(string)));
			dt.Columns.Add(new DataColumn("Day3", typeof(string)));
			dt.Columns.Add(new DataColumn("Day4", typeof(string)));
			dt.Columns.Add(new DataColumn("Day5", typeof(string)));
			dt.Columns.Add(new DataColumn("Day6", typeof(string)));
			dt.Columns.Add(new DataColumn("Day7", typeof(string)));
			dt.Columns.Add(new DataColumn("DayT", typeof(string)));
			dt.Columns.Add(new DataColumn("StateFriendlyName", typeof(string)));

			MetaView currentView = GetMetaView();
			currentView.Filters = GetFilters();

			McMetaViewPreference currentPreferences = Mediachase.UI.Web.Util.CommonHelper.CreateDefaultReportPreferenceTimeTracking(currentView);
			MetaObject[] list = null;
			if (String.Compare(Mediachase.IBN.Business.Configuration.Domain, "ibn47.mediachase.net", true) == 0)
			{
				list = currentView.List(currentPreferences, McRoundValues);

				// For Excel 
				for (int i = 1; i <= 8; i++)
					MainGrid.Columns[i].ItemStyle.CssClass = "TdTextClass";
			}
			else
			{
				list = currentView.List(currentPreferences);
			}

			foreach (MetaObject mo in list)
			{
				DataRow row = dt.NewRow();

				string additionalTitle = string.Empty;
				string prefix = "";
				string postfix = "";

				if (cbShowWeekNumber.Checked && mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() != MetaViewGroupByType.Total.ToString())
				{
					DateTime dtNew = DateTime.MinValue;

					try
					{
						dtNew = (DateTime)mo.Properties["Title"].Value;
					}
					catch
					{

					}

					if (dtNew != DateTime.MinValue)
						additionalTitle = string.Format("(#{0})", Iso8601WeekNumber.GetWeekNumber((DateTime)mo.Properties["Title"].Value));
				}

				if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
				{
					prefix = "<b>";
					postfix = "</b>";
					if (GroupingList.SelectedValue == GroupingWeekUser)
						row["Title"] = String.Format("{0}{1} - {2}{3} {4}", prefix, ((DateTime)mo.Properties["Title"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["Title"].Value).AddDays(6).ToString("d MMM yyyy"), prefix, additionalTitle);
					else
						row["Title"] = String.Format("{0}{1}{2} {3}", prefix, mo.Properties["Title"].Value.ToString(), postfix, additionalTitle);
				}
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
				{
					prefix = "<b>";
					postfix = "</b>";
					if (GroupingList.SelectedValue == GroupingUserWeek)
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0}{1} - {2}{3} {4}</div>", prefix, ((DateTime)mo.Properties["Title"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["Title"].Value).AddDays(6).ToString("d MMM yyyy"), prefix, additionalTitle);
					else
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0}{1}{2} {3}</div>", prefix, mo.Properties["Title"].Value.ToString(), postfix, additionalTitle);
				}
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Total.ToString())
				{
					prefix = "<b>";
					postfix = "</b>";
					row["Title"] = String.Format("{0}{1}{2} {3}", prefix, mo.Properties["Title"].Value.ToString(), postfix, additionalTitle);
				}
				else
				{
					row["Title"] = String.Format("<div style='padding-left: 50px;'>{0} {1}</div>", mo.Properties["Title"].Value.ToString(),additionalTitle);
				}

				if (String.Compare(Mediachase.IBN.Business.Configuration.Domain, "ibn47.mediachase.net", true) == 0)
				{
					if (mo.Properties["MetaViewGroupByType"] == null || (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Total.ToString()) || (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString()))
					{
						row["Day1"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day1"].Value) / 60.0, postfix);
						row["Day2"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day2"].Value) / 60.0, postfix);
						row["Day3"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day3"].Value) / 60.0, postfix);
						row["Day4"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day4"].Value) / 60.0, postfix);
						row["Day5"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day5"].Value) / 60.0, postfix);
						row["Day6"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day6"].Value) / 60.0, postfix);
						row["Day7"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["Day7"].Value) / 60.0, postfix);
						row["DayT"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToInt32(mo.Properties["DayT"].Value) / 60.0, postfix);
					}
				}
				else
				{
					row["Day1"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day1"].Value) / 60, Convert.ToInt32(mo.Properties["Day1"].Value) % 60, postfix);
					row["Day2"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day2"].Value) / 60, Convert.ToInt32(mo.Properties["Day2"].Value) % 60, postfix);
					row["Day3"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day3"].Value) / 60, Convert.ToInt32(mo.Properties["Day3"].Value) % 60, postfix);
					row["Day4"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day4"].Value) / 60, Convert.ToInt32(mo.Properties["Day4"].Value) % 60, postfix);
					row["Day5"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day5"].Value) / 60, Convert.ToInt32(mo.Properties["Day5"].Value) % 60, postfix);
					row["Day6"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day6"].Value) / 60, Convert.ToInt32(mo.Properties["Day6"].Value) % 60, postfix);
					row["Day7"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["Day7"].Value) / 60, Convert.ToInt32(mo.Properties["Day7"].Value) % 60, postfix);
					row["DayT"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["DayT"].Value) / 60, Convert.ToInt32(mo.Properties["DayT"].Value) % 60, postfix);
				}

				if (mo.Properties["StateFriendlyName"].Value != null)
					row["StateFriendlyName"] = CHelper.GetResFileString(mo.Properties["StateFriendlyName"].Value.ToString());
				else
					row["StateFriendlyName"] = "";

				dt.Rows.Add(row);
			}

			// Header Text
			for (int i = 0; i < MainGrid.Columns.Count; i++)
			{
				MetaField field = currentPreferences.GetVisibleMetaField()[i];
				string fieldName = field.Name;

				// First day of week can be different, so we should specify it.
				if (fieldName == "Day1" || fieldName == "Day2" || fieldName == "Day3" || fieldName == "Day4" || fieldName == "Day5" || fieldName == "Day6" || fieldName == "Day7")
				{
					DateTime curDate = CHelper.GetRealWeekStartByDate(DateTime.UtcNow);
					if (fieldName == "Day2")
						curDate = curDate.AddDays(1);
					else if (fieldName == "Day3")
						curDate = curDate.AddDays(2);
					else if (fieldName == "Day4")
						curDate = curDate.AddDays(3);
					else if (fieldName == "Day5")
						curDate = curDate.AddDays(4);
					else if (fieldName == "Day6")
						curDate = curDate.AddDays(5);
					else if (fieldName == "Day7")
						curDate = curDate.AddDays(6);

					MainGrid.Columns[i].HeaderText = GetGlobalResourceObject("IbnFramework.TimeTracking", curDate.DayOfWeek.ToString()).ToString();
				}
				else
				{
					MainGrid.Columns[i].HeaderText = CHelper.GetResFileString(field.FriendlyName);
				}
			}

			MainGrid.DataSource = dt;
			MainGrid.DataBind();
		}
		#endregion

		#region GetFilters
		private FilterElementCollection GetFilters()
		{
			FilterElementCollection filters = new FilterElementCollection();
			filters.Add(FilterElement.EqualElement("BlockTypeInstanceId", int.Parse(ProjectList.SelectedValue)));

			if (UserList.SelectedValue != "0")
				filters.Add(FilterElement.EqualElement("OwnerId", int.Parse(UserList.SelectedValue)));

			DateTime dt1, dt2;
			switch (PeriodList.SelectedValue)
			{
				case "[DateTimeThisAny]":
					break;
				case "[DateTimeThisWeek]":
					dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today);
					filters.Add(FilterElement.EqualElement("StartDate", dt1));
					break;
				case "[DateTimeLastWeek]":
					dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-7));
					filters.Add(FilterElement.EqualElement("StartDate", dt1));
					break;
				case "[DateTimeThisMonth]":
					dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.Day));
					dt2 = CHelper.GetRealWeekStartByDate(DateTime.Today);
					filters.Add(new IntervalFilterElement("StartDate", dt1, dt2));
					break;
				case "[DateTimeLastMonth]":
					dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.Day).AddMonths(-1));
					dt2 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.Day));
					filters.Add(new IntervalFilterElement("StartDate", dt1, dt2));
					break;
				case "[DateTimeThisYear]":
					dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear));
					dt2 = CHelper.GetRealWeekStartByDate(DateTime.Today);
					filters.Add(new IntervalFilterElement("StartDate", dt1, dt2));
					break;
				case "[DateTimeLastYear]":
					dt1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1));
					dt2 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.DayOfYear));
					filters.Add(new IntervalFilterElement("StartDate", dt1, dt2));
					break;
				case "0":
					filters.Add(FilterElement.EqualElement("StartDate", Dtc1.SelectedDate));
					break;
				case "-1":
					filters.Add(new IntervalFilterElement("StartDate", Dtc1.SelectedDate, Dtc2.SelectedDate));
					break;
				default:
					break;
			}
			return filters;
		}
		#endregion

		#region GetMetaView
		private MetaView GetMetaView()
		{
			MetaView currentView = new MetaView(TimeTrackingEntry.GetAssignedMetaClass(), "ProjectReport", string.Empty);
			foreach (MetaField mf in currentView.MetaClass.Fields)
			{
				currentView.AvailableFields.Add(mf);
			}

			// Total
			currentView.TotalGroupBy = new GroupByElement();
			currentView.TotalGroupBy.IsPreGroupObjectVisible = false;
			currentView.TotalGroupBy.IsPostGroupObjectVisible = true;

			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Custom, CHelper.GetResFileString("{IbnFramework.Global:_mc_Total}")));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			// PrimaryGroupBy, SecondaryGroupBy
			if (GroupingList.SelectedValue == GroupingWeekUser)
			{
				currentView.PrimaryGroupBy = new GroupByElement("StartDate");
				currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "StartDate", false));
				currentView.SecondaryGroupBy = new GroupByElement("Owner");
				currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "Owner", false));
			}
			else
			{
				currentView.PrimaryGroupBy = new GroupByElement("Owner");
				currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "Owner", false));
				currentView.SecondaryGroupBy = new GroupByElement("StartDate");
				currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "StartDate", false));
			}
			currentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			currentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;
			currentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			currentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;

			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			//currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("TimeTrackingBlockId", ValueSourceType.Field, "TimeTrackingBlockId"));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			return currentView;
		}
		#endregion

		#region Page Events
		protected void GroupingList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}

		protected void ProjectList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}

		protected void UserList_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindGrid();
		}

		protected void PeriodList_SelectedIndexChanged(object sender, EventArgs e)
		{
			Dtc1.Visible = false;
			Dtc2.Visible = false;

			switch (PeriodList.SelectedValue)
			{
				case "0":
					Dtc1.Visible = true;
					break;
				case "-1":
					Dtc1.Visible = true;
					Dtc2.Visible = true;
					break;
				default:
					break;
			}
			BindGrid();
		}

		protected void Dtc1_ValueChange(object sender, EventArgs e)
		{
			BindGrid();
		}

		protected void Dtc2_ValueChange(object sender, EventArgs e)
		{
			BindGrid();
		}

		protected void btnExport_Click(object sender, EventArgs e)
		{
			BindGrid();
			Mediachase.UI.Web.Util.CommonHelper.ExportExcel(MainGrid, "ProjectReport.xls", null, "0\\.00");
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (PanelFilters.Visible)
			{
				string sFilter = "";
				sFilter += String.Format("{0}:&nbsp;{1}<br />", GetGlobalResourceObject("IbnFramework.Global", "_mc_Project").ToString(), ProjectList.SelectedItem.Text);
				if (UserList.SelectedValue != "0")
					sFilter += String.Format("{0}:&nbsp;{1}<br />", GetGlobalResourceObject("IbnFramework.Global", "_mc_User").ToString(), UserList.SelectedItem.Text);

				switch (PeriodList.SelectedValue)
				{
					case "[DateTimeThisAny]":
					case "[DateTimeThisWeek]":
					case "[DateTimeLastWeek]":
					case "[DateTimeThisMonth]":
					case "[DateTimeLastMonth]":
					case "[DateTimeThisYear]":
					case "[DateTimeLastYear]":
						sFilter += String.Format("{0}:&nbsp;{1}<br />", GetGlobalResourceObject("IbnFramework.Global", "_mc_TimePeriod").ToString(), PeriodList.SelectedItem.Text);
						break;
					case "0":
						sFilter += String.Format("{0}:&nbsp;{1} - {2}<br />", GetGlobalResourceObject("IbnFramework.Global", "_mc_TimePeriod").ToString(), Dtc1.SelectedDate.ToString("d MMM yyyy"), Dtc1.SelectedDate.AddDays(6).ToString("d MMM yyyy"));
						break;
					case "-1":
						sFilter += String.Format("{0}:&nbsp;{1} - {2}<br />", GetGlobalResourceObject("IbnFramework.Global", "_mc_TimePeriod").ToString(), Dtc1.SelectedDate.ToString("d MMM yyyy"), Dtc2.SelectedDate.AddDays(6).ToString("d MMM yyyy"));
						break;
					default:
						break;
				}

				HeaderControl.Filter = sFilter;
			}
		}
		#endregion

		#region McRoundValues - hack for ibn47.mediachase.ru
		private static void McRoundValues(MetaObjectProperty prop)
		{
			switch (prop.Name)
			{
				case "Day1":
				case "Day2":
				case "Day3":
				case "Day4":
				case "Day5":
				case "Day6":
				case "Day7":
					int intVal = Convert.ToInt32(prop.Value);
					prop.Value = (intVal % 15 == 0) ? intVal : ((intVal / 15) + 1) * 15;
					break;
				case "DayT":
					MetaObject obj = prop.OwnerMetaObject;

					int intVal1 = Convert.ToInt32(obj["Day1"]);
					int intVal2 = Convert.ToInt32(obj["Day2"]);
					int intVal3 = Convert.ToInt32(obj["Day3"]);
					int intVal4 = Convert.ToInt32(obj["Day4"]);
					int intVal5 = Convert.ToInt32(obj["Day5"]);
					int intVal6 = Convert.ToInt32(obj["Day6"]);
					int intVal7 = Convert.ToInt32(obj["Day7"]);

					int sumValue = 0;
					sumValue += (intVal1 % 15 == 0) ? intVal1 : ((intVal1 / 15) + 1) * 15;
					sumValue += (intVal2 % 15 == 0) ? intVal2 : ((intVal2 / 15) + 1) * 15;
					sumValue += (intVal3 % 15 == 0) ? intVal3 : ((intVal3 / 15) + 1) * 15;
					sumValue += (intVal4 % 15 == 0) ? intVal4 : ((intVal4 / 15) + 1) * 15;
					sumValue += (intVal5 % 15 == 0) ? intVal5 : ((intVal5 / 15) + 1) * 15;
					sumValue += (intVal6 % 15 == 0) ? intVal6 : ((intVal6 / 15) + 1) * 15;
					sumValue += (intVal7 % 15 == 0) ? intVal7 : ((intVal7 / 15) + 1) * 15;

					prop.SetInnerValue(sumValue);
					break;
				default:
					break;
			}
		}
		#endregion
	}
}