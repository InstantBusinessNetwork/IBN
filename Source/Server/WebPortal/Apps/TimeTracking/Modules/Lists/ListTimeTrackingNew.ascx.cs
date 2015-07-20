using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.Lists
{
	public partial class ListTimeTrackingNew : System.Web.UI.UserControl
	{
		#region prop: CurrentView
		private MetaView _currentView;

		public MetaView CurrentView
		{
			get
			{
				if (_currentView == null)
				{
					if (this.ViewName == string.Empty)
						throw new ArgumentNullException("ViewName");

					if (this.ViewName == "TT_Report")
						_currentView = null;//new MetaView(TimeTrackingEntry.GetAssignedMetaClass(), "TT_Report", string.Empty);
					else
						_currentView = DataContext.Current.MetaModel.MetaViews[this.ViewName];

					if (_currentView == null)
						throw new ArgumentException(String.Format("Cant find meta view: {0}", this.ViewName));
				}

				return _currentView;
			}
		}
		#endregion

		#region prop: CurrentPreferences
		//private McMetaViewPreference _currentPreferences;
		public McMetaViewPreference CurrentPreferences
		{
			get
			{
				McMetaViewPreference pref = UserMetaViewPreference.Load(CurrentView, (int)DataContext.Current.CurrentUserId);

				if (pref == null || pref.Attributes.Count == 0)
				{
					McMetaViewPreference.CreateDefaultUserPreference(CurrentView);
					pref = UserMetaViewPreference.Load(CurrentView, (int)DataContext.Current.CurrentUserId);
				}

				return pref;
			}
		}
		#endregion

		#region prop: ViewName
		/// <summary>
		/// Gets the name of the view.
		/// </summary>
		/// <value>The name of the view.</value>
		public string ViewName
		{
			get
			{
				if (CHelper.GetFromContext("MetaViewName") != null)
					return CHelper.GetFromContext("MetaViewName").ToString();

				if (Request["ViewName"] != null)
					return Request["ViewName"];

				return string.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			MetaGridControl.ChangingMetaGridColumnHeader += new Mediachase.UI.Web.Apps.MetaUI.Grid.ChangingMetaGridColumnHeaderEventHandler(MetaGridControl_ChangingMetaGridColumnHeader);
			//LoadScripts();

			CHelper.AddToContext("ClassName", CurrentView.MetaClass.Name);

			#region Grouping
			lock (this.GetType())
			{
				switch (this.ViewName)
				{
					case "TT_MyGroupByWeek":
						{
							MyGroupByWeek();
							break;
						}
					case "TT_MyGroupByWeekProject":
						{
							MyGroupByWeekProject();
							break;
						}
					case "TT_MyGroupByWeekProjectAll":
						{
							MyGroupByWeekProjectAll();
							break;
						}
					case "TT_MyRejectedGroupByWeekProject":
						{
							MyRejectedGroupByWeekProject();
							break;
						}
					case "TT_CurrentProjectGroupByWeekUser":
						{
							CurrentProjectGroupByWeekUser();
							break;
						}
					case "TT_ManagerGroupByWeekUser":
						{
							ManagerGroupByWeekUser();
							break;
						}
					case "TT_ManagerGroupByWeekProject":
						{
							ManagerGroupByWeekProject();
							break;
						}
					case "TT_ManagerGroupByUserProject":
						{
							ManagerGroupByUserProject();
							break;
						}
					case "TT_ManagerGroupByProjectUser":
						{
							ManagerGroupByProjectUser();
							break;
						}
					case "TT_Report":
						{
							ReportGroupByFilter();
							break;
						}
					default:
						{
							//GroupForTimeSheet();
							break;
						}
				}
			} 
			#endregion

			//if (!IsPostBack)
			BindTotalRow();

			// MetaToolbar
			MainMetaToolbar.ClassName = CurrentView.MetaClass.Name;
			MainMetaToolbar.ViewName = ViewName;
			MainMetaToolbar.GridId = "MetaGridControl";

			MetaGridControl.ViewName = ViewName;
			ctrlGridEventUpdater.GridId = MetaGridControl.GridClientContainerId;
			//ctrlGridEventUpdater.GetCurrent.ActionList.Add(new ClientGridAction(ClientGridEvents.RowSelect, ctrlObjDet.UpdateElement.ClientID));
			//ctrlGrid.ViewName = this.Request["ViewName"];
			//ctrlGrid.ShowPaging = false;
			//ctrlGrid.AllowClientDrag = false;

			//GridPopupEdit.GridId = ctrlGrid.GridClientContainerId;

			ctrlGridEventUpdater.GridId = MetaGridControl.GridClientContainerId;
			ctrlGridEventUpdater.ClassName = CurrentView.MetaClass.Name;
			ctrlGridEventUpdater.ViewName = this.ViewName;

			//ctrlGridEventUpdater.GetCurrent.ActionList.Add(new ClientGridAction(ClientGridEvents.RowSelect, ctrlObjDet.UpdateElement.ClientID));

			if (!IsPostBack)
			{
				InitLayoutSize();
				CheckDefaultFilters();
			}
		}

		#region CheckDefaultFilters
		/// <summary>
		/// Checks the default filters.
		/// </summary>
		void CheckDefaultFilters()
		{
			#region MyFiltersDefaultValues
			DateTime date = CurrentPreferences.GetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, DateTime.MinValue);
			if (date == DateTime.MinValue)
			{
				CurrentPreferences.SetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, CHelper.GetRealWeekStartByDate(DateTime.Now));
			}
			#endregion

			#region ManagerLists
			string userId = CurrentPreferences.GetAttribute<string>(TTFilterPopupEdit.FilterUserAttr, TTFilterPopupEdit.FilterUserAttr, string.Empty);
			if (userId == string.Empty)
			{
				CurrentPreferences.SetAttribute<string>(TTFilterPopupEdit.FilterUserAttr, TTFilterPopupEdit.FilterUserAttr, Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString());
			}

			DateTime dateManager = CurrentPreferences.GetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, DateTime.MinValue);
			if (dateManager == DateTime.MinValue)
			{
				CurrentPreferences.SetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, CHelper.GetRealWeekStartByDate(DateTime.Now));
			} 
			#endregion

			UserMetaViewPreference.Save(Mediachase.IBN.Business.Security.CurrentUser.UserID, CurrentPreferences);
		} 
		#endregion

		#region BindTotalRow
		/// <summary>
		/// Binds the total row.
		/// </summary>
		void BindTotalRow()
		{
			CurrentView.Filters.Remove(new Guid(TimeTrackingEntry.SecurityFilterId));
			CurrentView.Filters.Add(TimeTrackingEntry.CreateSecurityFilterElement());

			CurrentView.TotalGroupBy = new GroupByElement();
			CurrentView.TotalGroupBy.IsPreGroupObjectVisible = false;
			CurrentView.TotalGroupBy.IsPostGroupObjectVisible = true;

			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Custom, CHelper.GetResFileString("{IbnFramework.Global:_mc_Total}")));
			//CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Title", "ParentBlock", false));
			//CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", "ParentBlockId"));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.TotalGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
		}
		#endregion

		#region ManagerGroupByProjectUser
		void ManagerGroupByProjectUser()
		{
			DockTop.DefaultSize = 105;

			if (Request.Browser.Browser.Contains("IE") && Request.Browser.MajorVersion < 8)
				DockTop.DefaultSize = 112;

			CurrentView.PrimaryGroupBy = new GroupByElement("BlockTypeInstanceId");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("OwnerId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("AreFinancesRegistered", ValueSourceType.Field, "AreFinancesRegistered", true));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			MetaGridControl.PrimaryGroupType = string.Empty;
			MetaGridControl.SecondaryGroupType = "TimeTrackingBlock";

			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "_mc_ManagerGroupByProjectUser").ToString();
		}
		#endregion

		#region ManagerGroupByUserProject
		void ManagerGroupByUserProject()
		{
			DockTop.DefaultSize = 105;

			if (Request.Browser.Browser.Contains("IE") && Request.Browser.MajorVersion < 8)
				DockTop.DefaultSize = 112;

			CurrentView.PrimaryGroupBy = new GroupByElement("OwnerId");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("AreFinancesRegistered", ValueSourceType.Field, "AreFinancesRegistered", true));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			MetaGridControl.PrimaryGroupType = string.Empty; 
			MetaGridControl.SecondaryGroupType = "TimeTrackingBlock";

			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "_mc_ManagerGroupByUserProject").ToString();
		}
		#endregion

		#region ManagerGroupByWeekProject
		void ManagerGroupByWeekProject()
		{
			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
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
		#endregion

		#region ManagerGroupByWeekUser
		void ManagerGroupByWeekUser()
		{
			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("OwnerId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
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
		#endregion

		#region CurrentProjectGroupByWeekUser
		void CurrentProjectGroupByWeekUser()
		{
			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("OwnerId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
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
		#endregion

		#region MyGroupByWeekProject
		void MyGroupByWeekProject()
		{
			//DockBottom.DefaultSize = 0;
			DockTop.DefaultSize = 75;
			if (Request.Browser.Browser.Contains("IE") && Request.Browser.MajorVersion < 8)
			    DockTop.DefaultSize = 78;
			//ExtGrid1.ShowPaging = false;
			//CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			//CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			//CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.PrimaryGroupBy = new GroupByElement("ParentBlockId");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("BlockTypeInstanceId", ValueSourceType.Field, "BlockTypeInstanceId"));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = null;

			ctrlTTFilterMain.ShowFilters = false;

			MetaGridControl.PrimaryGroupType = "TimeTrackingBlock";
			MetaGridControl.SecondaryGroupType = string.Empty;

			//CurrentPreferences.ChangeMetaFieldOrder(1, 9);
			//Mediachase.Ibn.Core.UserMetaViewPreference.Save(65, CurrentPreferences);

			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "_mc_TT_MyGroupByWeek").ToString();
		}
		#endregion

		#region MyGroupByWeekProjectAll
		void MyGroupByWeekProjectAll()
		{
			//DockBottom.DefaultSize = 0;
			DockTop.DefaultSize = 110;
			//ExtGrid1.ShowPaging = false;
			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("BlockTypeInstanceId", ValueSourceType.Field, "BlockTypeInstanceId"));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.SecondaryGroupBy = null;
		}
		#endregion

		#region MyRejectedGroupByWeekProject
		void MyRejectedGroupByWeekProject()
		{
//			DockBottom.DefaultSize = 0;
			DockTop.DefaultSize = 50;
			if (Request.Browser.Browser.Contains("IE"))
				DockTop.DefaultSize = 51;
			//ExtGrid1.ShowPaging = false;

			Guid uidToDelete = new Guid("68DA7079-91EF-4d17-AF86-EA8297670166");
			foreach (FilterElement fe in CurrentPreferences.Filters.GetListBySource("StartDate"))
			{
				uidToDelete = fe.Uid;
				break;
			}

			if (uidToDelete.ToString() != "68DA7079-91EF-4d17-AF86-EA8297670166")
			{
				CurrentPreferences.Filters.Remove(uidToDelete);
				Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.Ibn.Data.Services.Security.CurrentUserId, CurrentPreferences);
			}

			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("BlockTypeInstanceId", ValueSourceType.Field, "BlockTypeInstanceId"));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			ctrlTTFilterMain.Visible = false;

			MetaGridControl.PrimaryGroupType = string.Empty;
			MetaGridControl.SecondaryGroupType = "TimeTrackingBlock";

			BlockHeaderMain.Title = GetGlobalResourceObject("IbnFramework.TimeTracking", "_mc_TT_MyRejectedGroupByWeekProject").ToString();
		}
		#endregion

		#region MyGroupByWeek
		void MyGroupByWeek()
		{
//			DockBottom.DefaultSize = 0;
			//ExtGrid1.ShowPaging = false;
			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("StateFriendlyName", ValueSourceType.Field, "StateFriendlyName", true));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			//CurrentView.PrimaryGroupBy.PostGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));		
		}
		#endregion

		#region ReportGroupByFilter
		void ReportGroupByFilter()
		{
			CurrentView.PrimaryGroupBy = new GroupByElement("StartDate");
			CurrentView.PrimaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.PrimaryGroupBy.IsPostGroupObjectVisible = false;

			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day1", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day2", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day3", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day4", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day5", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day6", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Day7", ValueSourceType.Function, GroupByObjectField.SumFunction));
			CurrentView.PrimaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("DayT", ValueSourceType.Function, GroupByObjectField.SumFunction));

			CurrentView.SecondaryGroupBy = new GroupByElement("ParentBlockId");
			CurrentView.SecondaryGroupBy.IsPreGroupObjectVisible = true;
			CurrentView.SecondaryGroupBy.IsPostGroupObjectVisible = false;
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("Title", ValueSourceType.Field, "ParentBlock", false));
			CurrentView.SecondaryGroupBy.PreGroupObjectFields.Add(new GroupByObjectField("ParentBlockId", ValueSourceType.Field, "ParentBlockId"));
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
		#endregion

		#region InitLayoutSize
		/// <summary>
		/// Inits the size of the layout.
		/// </summary>
		void InitLayoutSize()
		{
			int detailsHSize = 0;

			if (CurrentPreferences.Attributes["MarginBottom"] != null)
				detailsHSize = (int)CurrentPreferences.Attributes["MarginBottom"];

//			DockBottom.DefaultSize = detailsHSize;
		}
		#endregion

		#region MetaGridControl_ChangingMetaGridColumnHeader
		void MetaGridControl_ChangingMetaGridColumnHeader(object sender, Mediachase.UI.Web.Apps.MetaUI.Grid.ChangingMetaGridColumnHeaderEventArgs e)
		{
			string fieldName = e.Field.Name;
			if (fieldName == "Day1" || fieldName == "Day2" || fieldName == "Day3" || fieldName == "Day4" || fieldName == "Day5" || fieldName == "Day6" || fieldName == "Day7")
			{
				bool showDate = false;
				object obj = CHelper.GetFromContext("SelectedWeek");
				DateTime dt;
				if (obj != null)
				{
					dt = (DateTime)obj;
					showDate = true;
				}
				else
				{
					// If we don't know week then we use "dt" for displaying day of week only.
					dt = CHelper.GetRealWeekStartByDate(DateTime.UtcNow);
				}

				if (fieldName == "Day2")
					dt = dt.AddDays(1);
				else if (fieldName == "Day3")
					dt = dt.AddDays(2);
				else if (fieldName == "Day4")
					dt = dt.AddDays(3);
				else if (fieldName == "Day5")
					dt = dt.AddDays(4);
				else if (fieldName == "Day6")
					dt = dt.AddDays(5);
				else if (fieldName == "Day7")
					dt = dt.AddDays(6);

				if (showDate)
					e.ControlField.HeaderText = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
				else
					e.ControlField.HeaderText = GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString();
			}
		}
		#endregion
	}
}