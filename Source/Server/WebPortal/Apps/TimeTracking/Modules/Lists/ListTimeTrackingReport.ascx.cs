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
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Data.Meta;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists
{
	public partial class ListTimeTrackingReport : System.Web.UI.UserControl
	{
		#region prop: CurrentView
		private MetaView _currentView;

		public MetaView CurrentView
		{
			get
			{
				if (_currentView == null)
				{
					_currentView = new MetaView(TimeTrackingBlock.GetAssignedMetaClass(), "TT_Report", string.Empty);

					foreach (MetaField mf in _currentView.MetaClass.Fields)
					{
						_currentView.AvailableFields.Add(mf);
					}

					_currentView.TotalGroupBy = new GroupByElement();
					_currentView.TotalGroupBy.IsPreGroupObjectVisible = false;
					_currentView.TotalGroupBy.IsPostGroupObjectVisible = true;

					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Custom, CHelper.GetResFileString("{IbnFramework.Global:_mc_Total}")));
					//_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Title", "ParentBlock", false));
					//_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", "ParentBlockId"));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
					_currentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
				}

				return _currentView;
			}
		}
		#endregion

		#region prop: CurrentPreferences
		private McMetaViewPreference _currentPreferences;
		public McMetaViewPreference CurrentPreferences
		{
			get
			{
				if (_currentPreferences == null)
				{
					_currentPreferences = Mediachase.UI.Web.Util.CommonHelper.CreateDefaultReportPreferenceTimeTracking(CurrentView);
				}
				return _currentPreferences;
			}
		}
		#endregion

		#region LoadScripts
		void LoadScripts()
		{
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/extpackage/ext8.js"));
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/ext-lang-ru.js"));
			ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("~/Scripts/IbnFramework/CSJSRequestObject.js"));
			ScriptManager.GetCurrent(this.Page).ScriptMode = ScriptMode.Release;

			ScriptManager.GetCurrent(this.Page).EnablePageMethods = true;
		}
		#endregion

		#region prop: ThirdGroupById
		private int ThirdGroupById
		{
			get
			{
				if (ViewState[this.ID + "_ThirdGroupById"] != null)
					return Convert.ToInt32(ViewState[this.ID + "_ThirdGroupById"].ToString(), CultureInfo.InvariantCulture);

				return -1;
			}
			set
			{
				ViewState[this.ID + "_ThirdGroupById"] = value;
			}
		} 
		#endregion

		#region prop: FilterUser
		private int FilterUser
		{
			get
			{
				if (ViewState[this.ID + "FilterUser"] != null)
					return Convert.ToInt32(ViewState[this.ID + "FilterUser"].ToString(), CultureInfo.InvariantCulture);

				return -1;
			}
			set
			{
				ViewState[this.ID + "FilterUser"] = value;
			}
		}
		#endregion

		#region prop: FilterProject
		private int FilterProject
		{
			get
			{
				if (ViewState[this.ID + "_FilterProject"] != null)
					return Convert.ToInt32(ViewState[this.ID + "_FilterProject"].ToString(), CultureInfo.InvariantCulture);

				return 0;
			}
			set
			{
				ViewState[this.ID + "_FilterProject"] = value;
			}
		}
		#endregion

		#region prop: FilterStates
		private string FilterStates
		{
			get
			{
				if (ViewState[this.ID + "_FilterStates"] != null)
					return ViewState[this.ID + "_FilterStates"].ToString();

				return "0";
			}
			set
			{
				ViewState[this.ID + "_FilterStates"] = value;
			}
		}
		#endregion

		#region prop: FilterRejected
		private bool FilterRejected
		{
			get
			{
				if (ViewState[this.ID + "_FilterRejected"] != null)
					return Convert.ToBoolean(ViewState[this.ID + "_FilterRejected"].ToString(), CultureInfo.InvariantCulture);

				return false;
			}
			set
			{
				ViewState[this.ID + "_FilterRejected"] = value;
			}
		}
		#endregion

		#region prop: FilterData1
		private DateTime FilterData1
		{
			get
			{
				if (ViewState[this.ID + "FilterData1"] != null)
					return CHelper.GetRealWeekStartByDate((DateTime)ViewState[this.ID + "FilterData1"]);

				return DateTime.MinValue;
			}
			set
			{
				ViewState[this.ID + "FilterData1"] = value;
			}
		}
		#endregion

		#region prop: FilterData2
		private DateTime FilterData2
		{
			get
			{
				if (ViewState[this.ID + "FilterData2"] != null)
					return CHelper.GetRealWeekStartByDate((DateTime)ViewState[this.ID + "FilterData2"]);

				return DateTime.MinValue;
			}
			set
			{
				ViewState[this.ID + "FilterData2"] = value;
			}
		}
		#endregion

		#region _reportType
		private string _reportType
		{
			get
			{
				if (Request["ReportType"] != null && Request["ReportType"] == "Admin")
					return "Admin";
				else
					return "User";
			}
		} 
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BindDD();
				ChangeThirdGroupBy();
			}

			ddPrimary.SelectedIndexChanged += new EventHandler(ddPrimary_SelectedIndexChanged);
			ddSecondary.SelectedIndexChanged += new EventHandler(ddSecondary_SelectedIndexChanged);

			ddProjects.SelectedIndexChanged += new EventHandler(ddProjects_SelectedIndexChanged);
			ddUsers.SelectedIndexChanged += new EventHandler(ddUsers_SelectedIndexChanged);
			ddState.SelectedIndexChanged += new EventHandler(ddState_SelectedIndexChanged);
			tbRejected.CheckedChanged += new EventHandler(tbRejected_CheckedChanged);
			ddPeriod.SelectedIndexChanged += new EventHandler(ddPeriod_SelectedIndexChanged);
			cbShowWeekNumber.CheckedChanged += new EventHandler(cbShowWeekNumber_CheckedChanged);

			Dtc1.ValueChange += new PickerControl.ValueChanged(Dtc1_ValueChange);
			Dtc2.ValueChange += new PickerControl.ValueChanged(Dtc2_ValueChange);

			trUsers.Visible = (_reportType == "Admin");
			trSecondGroup.Visible = (_reportType == "Admin");

			cbShowWeekNumber.Text = CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ShowWeekNumbers}");
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

		protected void btnExport_Click(object sender, EventArgs e)
		{
			ReportGroupByFilter();
			BindGrid();
			Mediachase.UI.Web.Util.CommonHelper.ExportExcel(MainGrid, "ListViewReport.xls", null);
		}

		#region Dtc1_ValueChange
		void Dtc1_ValueChange(object sender, EventArgs e)
		{
			this.FilterData1 = Dtc1.SelectedDate;
		} 
		#endregion

		#region Dtc2_ValueChange
		void Dtc2_ValueChange(object sender, EventArgs e)
		{
			this.FilterData2 = Dtc2.SelectedDate;
		} 
		#endregion

		#region ddPeriod_SelectedIndexChanged
		void ddPeriod_SelectedIndexChanged(object sender, EventArgs e)
		{
			tdDate1.Style.Add("visibility", "hidden");
			tdDate2.Style.Add("visibility", "hidden");

			switch (ddPeriod.SelectedValue)
			{
				case "[DateTimeThisAny]":
					this.FilterData1 = DateTime.MinValue;
					this.FilterData2 = DateTime.MinValue;
					break;
				case "[DateTimeThisWeek]":
					this.FilterData1 = CHelper.GetRealWeekStartByDate(DateTime.Today);
					this.FilterData2 = DateTime.MinValue;
					break;
				case "[DateTimeLastWeek]":
					this.FilterData1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-7));
					this.FilterData2 = DateTime.MinValue;
					break;
				case "[DateTimeThisMonth]":
					this.FilterData1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.Day));
					this.FilterData2 = CHelper.GetRealWeekStartByDate(DateTime.Today);
					break;
				case "[DateTimeLastMonth]":
					this.FilterData1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.Day).AddMonths(-1));
					this.FilterData2 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.Day));
					break;
				case "[DateTimeThisYear]":
					this.FilterData1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear));
					// OR: why should we lose the current week?
					//this.FilterData2 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.Day));
					this.FilterData2 = CHelper.GetRealWeekStartByDate(DateTime.Today);
					break;
				case "[DateTimeLastYear]":
					this.FilterData1 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1));
					this.FilterData2 = CHelper.GetRealWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.DayOfYear));
					break;
				case "0":
					tdDate1.Style.Add("visibility", "visible");

					this.FilterData1 = Dtc1.SelectedDate;
					this.FilterData2 = DateTime.MinValue;
					break;
				case "-1":
					tdDate1.Style.Add("visibility", "visible");
					tdDate2.Style.Add("visibility", "visible");

					this.FilterData1 = Dtc1.SelectedDate;
					this.FilterData2 = Dtc2.SelectedDate;
					break;
				default:
					break;
			}
		} 
		#endregion

		#region DropDowns SelectedIndexChanged
		void tbRejected_CheckedChanged(object sender, EventArgs e)
		{
			this.FilterRejected = tbRejected.Checked;
		}

		void ddState_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FilterStates = ddState.SelectedValue;
		}

		void ddUsers_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FilterUser = Convert.ToInt32(ddUsers.SelectedValue, CultureInfo.InvariantCulture);
		}

		void ddProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.FilterProject = Convert.ToInt32(ddProjects.SelectedValue, CultureInfo.InvariantCulture);
		} 
		#endregion

		protected void Page_PreRender(object sender, EventArgs e)
		{
			_header.ForPrintOnly = true;
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Workspace.Resources.strWorkspace", Assembly.GetExecutingAssembly());
			if (_reportType == "Admin")
			{
				_header.Title = LocRM.GetString("tTTReportTitleAdmin");
				lblGroupText.Text = CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_PrimaryGroup}");
			}
			else
			{
				_header.Title = LocRM.GetString("tTTReportTitleAdminMy");
				lblGroupText.Text = CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Group}");
			}

			string sFilter = "";
			if (ddProjects.SelectedValue != "0")
				sFilter += "&nbsp;&nbsp;" + LocRM.GetString("tProject") + ":&nbsp;" + ddProjects.SelectedItem.Text + "<br />";
			if (_reportType == "Admin" && ddUsers.SelectedValue != "0")
				sFilter += "&nbsp;&nbsp;" + LocRM.GetString("tUser") + ":&nbsp;" + ddUsers.SelectedItem.Text + "<br />";
			if (ddState.SelectedValue != "0")
				sFilter += "&nbsp;&nbsp;" + LocRM.GetString("tState") + ":&nbsp;" + ddState.SelectedItem.Text + "<br />";
			if(tbRejected.Checked)
				sFilter += "&nbsp;&nbsp;" + LocRM.GetString("tOnlyRej");
			_header.Filter = sFilter;

			ReportGroupByFilter();
			BindGrid();
			btnPrint.Value = LocRM.GetString("tPrint");
			btnExport2.Value = LocRM.GetString("tExport");
			btnExport.Style.Add("display", "none");
		}


		#region ddSecondary_SelectedIndexChanged
		void ddSecondary_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selected = ddPrimary.SelectedValue;
			ddPrimary.DataSource = GenerateDDSource(Convert.ToInt32(ddSecondary.SelectedValue, CultureInfo.InvariantCulture));
			ddPrimary.DataBind();

			CHelper.SafeSelect(ddPrimary, selected);

			ChangeThirdGroupBy();
		}
		#endregion

		#region ddPrimary_SelectedIndexChanged
		void ddPrimary_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selected = ddSecondary.SelectedValue;
			ddSecondary.DataSource = GenerateDDSource(Convert.ToInt32(ddPrimary.SelectedValue, CultureInfo.InvariantCulture));
			ddSecondary.DataBind();

			CHelper.SafeSelect(ddSecondary, selected);

			ChangeThirdGroupBy();
		}
		#endregion

		#region ChangeThirdGroupBy
		void ChangeThirdGroupBy()
		{

			DataTable dt = GenerateDDSource();
			if (_reportType == "Admin")
			{
				dt.DefaultView.RowFilter = String.Format("Id NOT IN ({0},{1})", Convert.ToInt32(ddSecondary.SelectedValue, CultureInfo.InvariantCulture), Convert.ToInt32(ddPrimary.SelectedValue, CultureInfo.InvariantCulture));
				if (dt.DefaultView.Count > 0)
					this.ThirdGroupById = Convert.ToInt32(dt.DefaultView[0]["Id"].ToString(), CultureInfo.InvariantCulture);
			}
			else
			{
				dt.DefaultView.RowFilter = String.Format("Id NOT IN ({0})", Convert.ToInt32(ddPrimary.SelectedValue, CultureInfo.InvariantCulture));
				if (dt.DefaultView.Count > 0)
					this.ThirdGroupById = Convert.ToInt32(dt.DefaultView[0]["Id"].ToString(), CultureInfo.InvariantCulture);
			}
		} 
		#endregion

		#region ReportGroupByFilter
		void ReportGroupByFilter()
		{

			switch (Convert.ToInt32(ddPrimary.SelectedValue, CultureInfo.InvariantCulture))
			{
				case 0:
					{
						CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
						CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "StartDate", false));
						break;
					}
				case 1:
					{
						CurrentView.PrimaryGroupBy = new GroupByElement("Owner");
						CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "Owner", false));
						break;
					}
				case 2:
					{
						CurrentView.PrimaryGroupBy = new GroupByElement("BlockTypeInstance");
						CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "BlockTypeInstance", false));
						break;
					}
			}
			
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			if (_reportType == "Admin")
			{
				switch (Convert.ToInt32(ddSecondary.SelectedValue, CultureInfo.InvariantCulture))
				{
					case 0:
						{
							CurrentView.SecondaryGroupBy = new GroupByElement("StartDate");
							CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "StartDate", false));
							break;
						}
					case 1:
						{
							CurrentView.SecondaryGroupBy = new GroupByElement("Owner");
							CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "Owner", false));
							break;
						}
					case 2:
						{
							CurrentView.SecondaryGroupBy = new GroupByElement("BlockTypeInstance");
							CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "BlockTypeInstance", false));
							break;
						}
				}

				//CurrentView.SecondaryGroupBy = new GroupByElement("Project");
				CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
				CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;


				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("TimeTrackingBlockId", ValueSourceType.Field, "TimeTrackingBlockId"));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
				CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
			}

			//CurrentView.Filters.Add(new FilterElement("ProjectId", FilterElementType.IsNotNull, null));

			if (this.FilterProject > 0)
			{
				CurrentView.Filters.Add(FilterElement.EqualElement("BlockTypeInstanceId", this.FilterProject));
			}
			else if (this.FilterProject == -1)
			{
				CurrentView.Filters.Add(new FilterElement("ProjectId", FilterElementType.IsNull, null));
			}
			else if (this.FilterProject == -2)
			{
				CurrentView.Filters.Add(new FilterElement("ProjectId", FilterElementType.IsNotNull, null));
			}

			if (this.FilterUser > 0)
			{
				CurrentView.Filters.Add(new FilterElement("OwnerId", FilterElementType.Equal, this.FilterUser));
			}

			if (this.FilterStates != "0")
			{
				CurrentView.Filters.Add(new FilterElement("StateFriendlyName", FilterElementType.Equal, this.FilterStates));
			}

			if (this.FilterRejected)
			{
				CurrentView.Filters.Add(new FilterElement("IsRejected", FilterElementType.Equal, "1"));
			}

			if (this.FilterData1 != DateTime.MinValue && this.FilterData2 != DateTime.MinValue)
			{
				CurrentView.Filters.Add(new IntervalFilterElement("StartDate", this.FilterData1, this.FilterData2));
			}
			else if (this.FilterData1 != DateTime.MinValue && this.FilterData2 == DateTime.MinValue)
			{
				CurrentView.Filters.Add(new FilterElement("StartDate", FilterElementType.Equal, this.FilterData1));
			}
		}
		#endregion

		#region BindGrid
		void BindGrid()
		{
			MetaObject[] list = this.CurrentView.List(CurrentPreferences);

			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Title", typeof(string)));
			dt.Columns.Add(new DataColumn("Day1", typeof(float)));
			dt.Columns.Add(new DataColumn("Day2", typeof(float)));
			dt.Columns.Add(new DataColumn("Day3", typeof(float)));
			dt.Columns.Add(new DataColumn("Day4", typeof(float)));
			dt.Columns.Add(new DataColumn("Day5", typeof(float)));
			dt.Columns.Add(new DataColumn("Day6", typeof(float)));
			dt.Columns.Add(new DataColumn("Day7", typeof(float)));
			dt.Columns.Add(new DataColumn("DayT", typeof(float)));
			dt.Columns.Add(new DataColumn("StateFriendlyName", typeof(string)));

			foreach (MetaObject mo in list)
			{
				DataRow row = dt.NewRow();
				string additionalTitle = string.Empty;

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
					if (Convert.ToInt32(ddPrimary.SelectedValue, CultureInfo.InvariantCulture) != 0)
						row["Title"] = String.Format("<b>{0} {1}</b>", mo.Properties["Title"].Value.ToString(), additionalTitle);
					else
						row["Title"] = String.Format("<b>{0} - {1} {2}</b>", ((DateTime)mo.Properties["Title"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["Title"].Value).AddDays(6).ToString("d MMM yyyy"), additionalTitle);
				}
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Secondary.ToString())
				{
					if (Convert.ToInt32(ddSecondary.SelectedValue, CultureInfo.InvariantCulture) != 0)
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0} {1}</div>", mo.Properties["Title"].Value.ToString(), additionalTitle);
					else
						row["Title"] = String.Format("<div style='padding-left: 25px;'>{0} - {1} {2}</div>", ((DateTime)mo.Properties["Title"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["Title"].Value).AddDays(6).ToString("d MMM yyyy"), additionalTitle);
				}
				else if (mo.Properties["MetaViewGroupByType"] != null && mo.Properties["MetaViewGroupByType"].OriginalValue.ToString() == MetaViewGroupByType.Total.ToString())
				{
					row["Title"] = String.Format("<div style='padding-left: 25px;'>{0} {1}</div>", mo.Properties["Title"].Value.ToString(), additionalTitle);
				}
				else
				{
					//TO DO: switch
					switch (this.ThirdGroupById)
					{
						case 0:
							{
								row["Title"] = String.Format("<div style='padding-left: 45px;'>{0}-{1} {2}</div>", ((DateTime)mo.Properties["StartDate"].Value).ToString("d MMM yyyy"), ((DateTime)mo.Properties["StartDate"].Value).AddDays(6).ToString("d MMM yyyy"), additionalTitle);
								break;
							}
						case 1:
							{
								row["Title"] = String.Format("<div style='padding-left: 45px;'>{0} {1}</div>", mo.Properties["Owner"].Value.ToString(), additionalTitle);
								break;
							}
						case 2:
							{
								string sTitle = (mo.Properties["Project"].Value != null) ? mo.Properties["Project"].Value.ToString() : mo.Properties["Title"].Value.ToString();
								row["Title"] = String.Format("<div style='padding-left: 45px;'>{0} {1}</div>", sTitle, additionalTitle);
								break;
							}
					}
				}

				row["Day1"] = Convert.ToDouble(mo.Properties["Day1"].Value);
				row["Day2"] = Convert.ToDouble(mo.Properties["Day2"].Value);
				row["Day3"] = Convert.ToDouble(mo.Properties["Day3"].Value);
				row["Day4"] = Convert.ToDouble(mo.Properties["Day4"].Value);
				row["Day5"] = Convert.ToDouble(mo.Properties["Day5"].Value);
				row["Day6"] = Convert.ToDouble(mo.Properties["Day6"].Value);
				row["Day7"] = Convert.ToDouble(mo.Properties["Day7"].Value);
				row["DayT"] = Convert.ToDouble(mo.Properties["DayT"].Value);

				if (mo.Properties["StateFriendlyName"].Value != null)
					row["StateFriendlyName"] = CHelper.GetResFileString(mo.Properties["StateFriendlyName"].Value.ToString());
				else
					row["StateFriendlyName"] = "";

				dt.Rows.Add(row);
			}

			// Header Text
			for (int i = 0; i < MainGrid.Columns.Count; i++)
			{
				MetaField field = CurrentPreferences.GetVisibleMetaField()[i];
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

		#region BindDD

		#region BindUsers
		private void BindUsers()
		{
			ddUsers.Items.Clear();
			ddUsers.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));
			Principal[] mas = Principal.List(new FilterElementCollection(FilterElement.EqualElement("Card", "User"), FilterElement.EqualElement("Activity", 3)), new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc)));
			foreach (Principal pl in mas)
				ddUsers.Items.Add(new ListItem(pl.Name, pl.PrimaryKeyId.ToString()));

			if (_reportType == "User")
			{
				CHelper.SafeSelect(ddUsers, Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString());
				this.FilterUser = Mediachase.IBN.Business.Security.CurrentUser.UserID;
			}
		} 
		#endregion

		#region BindBlocks
		private void BindBlocks()
		{
			string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;
			ddProjects.Items.Clear();

			ddProjects.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));

			ddProjects.Items.Add(new ListItem(" " + CHelper.GetResFileString("{IbnFramework.TimeTracking:NonProject}"), "-1"));

			foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetNonProjectBlockTypeInstances())
			{
				ListItem li = new ListItem("   " + bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
				ddProjects.Items.Add(li);
			}

			ddProjects.Items.Add(new ListItem(" " + CHelper.GetResFileString("{IbnFramework.TimeTracking:ByProject}"), "-2"));

			// Projects
			foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				ListItem li = new ListItem("   " + bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
				ddProjects.Items.Add(li);
			}

			
		} 
		#endregion

		#region BindStates
		private void BindStates()
		{
			ddState.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));

			foreach (MetaObject mo in StateMachineManager.GetAvailableStates(TimeTrackingBlock.GetAssignedMetaClass()))
			{
				ddState.Items.Add(new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), mo.Properties["FriendlyName"].Value.ToString()));
			}
		}
		#endregion

		#region BindDates
		void BindDates()
		{
			ddPeriod.Items.Clear();
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_Any}"), "[DateTimeThisAny]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisWeek}"), "[DateTimeThisWeek]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastWeek}"), "[DateTimeLastWeek]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisMonth}"), "[DateTimeThisMonth]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastMonth}"), "[DateTimeLastMonth]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_ThisYear}"), "[DateTimeThisYear]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_LastYear}"), "[DateTimeLastYear]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_CustomWeek}"), "0"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.TimeTracking:_mc_CustomPeriod}"), "-1"));

			Dtc1.SelectedDate = CHelper.GetRealWeekStartByDate(DateTime.Today);
			Dtc2.SelectedDate = CHelper.GetRealWeekStartByDate(DateTime.Today);
		} 
		#endregion

		void BindDD()
		{
			ddPrimary.DataSource = GenerateDDSource();
			ddPrimary.DataTextField = "Name";
			ddPrimary.DataValueField = "Id";
			ddPrimary.DataBind();

			if (_reportType == "Admin")
			{
				ddSecondary.DataSource = GenerateDDSource(Convert.ToInt32(ddPrimary.SelectedValue, CultureInfo.InvariantCulture));
				ddSecondary.DataTextField = "Name";
				ddSecondary.DataValueField = "Id";
				ddSecondary.DataBind();
			}
			BindUsers();
			BindBlocks();
			BindStates();
			BindDates();
		} 
		#endregion

		#region GenerateDDSource
		DataTable GenerateDDSource()
		{
			return GenerateDDSource(-1);
		}

		DataTable GenerateDDSource(int excludeId)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("Id", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));

			DataRow row = dt.NewRow();

			if (excludeId != 0)
			{
				row["Id"] = 0;
				row["Name"] = CHelper.GetResFileString("{IbnFramework.TimeTracking:Week}");
				dt.Rows.Add(row);
			}

			if (excludeId != 1 && _reportType=="Admin")
			{
				row = dt.NewRow();
				row["Id"] = 1;
				row["Name"] = CHelper.GetResFileString("{IbnFramework.TimeTracking:User}");
				dt.Rows.Add(row);
			}


			if (excludeId != 2)
			{
				row = dt.NewRow();
				row["Id"] = 2;
				row["Name"] = CHelper.GetResFileString("{IbnFramework.TimeTracking:ProjectGrouping}");
				dt.Rows.Add(row);
			}

			return dt;
		}
		#endregion
	}
}