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
using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists
{
	public partial class FinanceProjectReport : System.Web.UI.UserControl
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
			bool isPpmExec = Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) || Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager);
			string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;
			foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				// Условия для того, чтобы BlockTypeInstances попал в список:
				//	- он должен быть связан с проектом
				//	- у него должны быть активны финансы
				//	- текущий пользователь - либо PPM/Exec, либо менеджер/исп.менеджер проекта 

				if (bo.ProjectId.HasValue)
				{
					int projectId = bo.ProjectId.Value;

					if (ProjectSpreadSheet.IsActive(projectId))
					{
						bool add = false;
						if (isPpmExec)
						{
							add = true;
						}
						else
						{
							Project.ProjectSecurity ps = Project.GetSecurity(projectId);
							if (ps.IsManager || ps.IsExecutiveManager)
								add = true;
						}

						if (add)
						{
							ListItem li = new ListItem(bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
							ProjectList.Items.Add(li);
						}
					}
				}
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
			dt.Columns.Add(new DataColumn("Total", typeof(string)));
			dt.Columns.Add(new DataColumn("TotalApproved", typeof(string)));
			dt.Columns.Add(new DataColumn("Rate", typeof(string)));
			dt.Columns.Add(new DataColumn("Cost", typeof(string)));

			MetaView currentView = GetMetaView();
			currentView.Filters = GetFilters();

			McMetaViewPreference currentPreferences = CreateDefaultReportPreferenceTimeTracking(currentView);
			MetaObject[] list = currentView.List(currentPreferences);

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

				// Primary Grouping
				if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Primary.ToString())
				{
					prefix = "<b>";
					postfix = "</b>";
					if (GroupingList.SelectedValue == GroupingWeekUser)
						row["Title"] = String.Format("{0}{1} - {2}{3} {4}", prefix, ((DateTime)mo.Properties["Title"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["Title"].Value).AddDays(6).ToString("d MMM yyyy"), prefix, additionalTitle);
					else
						row["Title"] = String.Format("{0}{1}{2} {3}", prefix, mo.Properties["Title"].Value.ToString(), postfix, additionalTitle);
				}
				// Secondary Grouping
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
				{
					prefix = "<b>";
					postfix = "</b>";
					if (GroupingList.SelectedValue == GroupingUserWeek)
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0}{1} - {2}{3} {4}</div>", prefix, ((DateTime)mo.Properties["Title"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["Title"].Value).AddDays(6).ToString("d MMM yyyy"), prefix, additionalTitle);
					else
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0}{1}{2} {3}</div>", prefix, mo.Properties["Title"].Value.ToString(), postfix, additionalTitle);
				}
				// Total
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Total.ToString())
				{
					prefix = "<b>";
					postfix = "</b>";
					row["Title"] = String.Format("{0}{1}{2} {3}", prefix, mo.Properties["Title"].Value.ToString(), postfix, additionalTitle);
				}
				// Other
				else
				{
					row["Title"] = String.Format("<div style='padding-left: 50px;'>{0} {1}</div>", mo.Properties["Title"].Value.ToString(), additionalTitle);
					row["Rate"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToDecimal(mo.Properties["Rate"].Value), postfix);
				}

				row["Total"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["DayT"].Value) / 60, Convert.ToInt32(mo.Properties["DayT"].Value) % 60, postfix);
				row["TotalApproved"] = String.Format("{0}{1:D2}:{2:D2}{3}", prefix, Convert.ToInt32(mo.Properties["TotalApproved"].Value) / 60, Convert.ToInt32(mo.Properties["TotalApproved"].Value) % 60, postfix);
				row["Cost"] = String.Format("{0}{1:F2}{2}", prefix, Convert.ToDecimal(mo.Properties["Cost"].Value), postfix);

				dt.Rows.Add(row);
			}

			// Header Text
			for (int i = 0; i < MainGrid.Columns.Count; i++)
			{
				MetaField field = currentPreferences.GetVisibleMetaField()[i];

				MainGrid.Columns[i].HeaderText = CHelper.GetResFileString(field.FriendlyName);
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
			filters.Add(FilterElement.EqualElement("AreFinancesRegistered", true));

			// exclude zero values
			filters.Add(new FilterElement("Cost", FilterElementType.Greater, 0));

			return filters;
		}
		#endregion

		#region GetMetaView
		private MetaView GetMetaView()
		{
			MetaView currentView = new MetaView(TimeTrackingEntry.GetAssignedMetaClass(), "FinanceProjectReport", string.Empty);
			foreach (MetaField mf in currentView.MetaClass.Fields)
			{
				currentView.AvailableFields.Add(mf);
			}

			// Total
			currentView.TotalGroupBy = new GroupByElement();
			currentView.TotalGroupBy.IsPreGroupObjectVisible = false;
			currentView.TotalGroupBy.IsPostGroupObjectVisible = true;

			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Custom, CHelper.GetResFileString("{IbnFramework.Global:_mc_Total}")));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("TotalApproved", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Cost", ValueSourceType.Function, GroupByObjectField.SumFunction));

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

			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("TotalApproved", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Cost", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));

			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("TotalApproved", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Cost", ValueSourceType.Function, GroupByObjectField.SumFunction));
			currentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));

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
			Mediachase.UI.Web.Util.CommonHelper.ExportExcel(MainGrid, "FinanceReport.xls", null, "0\\.00");
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

		#region CreateDefaultReportPreferenceTimeTracking
		private McMetaViewPreference CreateDefaultReportPreferenceTimeTracking(MetaView View)
		{
			McMetaViewPreference pref = new McMetaViewPreference();
			pref.MetaView = View;

			pref.ShowAllMetaField();

			foreach (MetaField field in pref.GetVisibleMetaField())
			{
				if (field.Name != "Title" && field.Name != "TotalApproved" && field.Name != "Rate" && field.Name != "Cost" && field.Name != "DayT")
				{
					pref.HideMetaField(field.Name);
				}
				else
				{
					if (field.Name == "Title")
					{
						pref.SetAttribute<int>(field.Name, McMetaViewPreference.AttrIndex, 0);
					}
					else if (field.Name == "DayT")
					{
						pref.SetAttribute<int>(field.Name, McMetaViewPreference.AttrIndex, 1);
					}
					else if (field.Name == "TotalApproved")
					{
						pref.SetAttribute<int>(field.Name, McMetaViewPreference.AttrIndex, 2);
					}
					else if (field.Name == "Rate")
					{
						pref.SetAttribute<int>(field.Name, McMetaViewPreference.AttrIndex, 3);
					}
					else
					{
						pref.SetAttribute<int>(field.Name, McMetaViewPreference.AttrIndex, 4);
					}
				}
 			}

			pref.Attributes.Set("PageSize", -1);

			MetaViewGroupUtil.CollapseAll(MetaViewGroupByType.Secondary, pref);

			return pref;
		}
		#endregion
	}
}