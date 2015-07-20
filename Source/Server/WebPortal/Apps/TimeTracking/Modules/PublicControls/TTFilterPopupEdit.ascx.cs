using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.IbnNext.TimeTracking;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class TTFilterPopupEdit : System.Web.UI.UserControl
	{
		public const string FilterBlockAttr = "TTFilter_Block";
		public const string FilterUserAttr = "TTFilter_User";
		public const string FilterStateAttr = "TTFilter_State";
		public const string FilterWeekAttr = "TTFilter_DTCWeek";

		#region MetaViewName
		public string MetaViewName
		{
			get
			{
				if (ViewState["MetaViewName"] == null)
					ViewState["MetaViewName"] = CHelper.GetFromContext("MetaViewName");
				return ViewState["MetaViewName"].ToString();
			}
		}
		#endregion

		#region ClassName
		public string ClassName
		{
			get
			{
				return CurrentView.MetaClass.Name;
			}
		}
		#endregion

		#region CurrentView
		private MetaView currentView;
		public MetaView CurrentView
		{
			get
			{
				if (currentView == null)
				{
					if (DataContext.Current.MetaModel.MetaViews[MetaViewName] == null)
						throw new ArgumentException(String.Format("Cant find meta view: {0}", MetaViewName));
					currentView = DataContext.Current.MetaModel.MetaViews[MetaViewName];
				}
				return currentView;
			}
		}
		#endregion

		#region GetMetaViewPreference
		private McMetaViewPreference GetMetaViewPreference()
		{
			return CHelper.GetMetaViewPreference(CurrentView);
		}
		#endregion

		#region event: OnFilterChange
		public delegate void FilterChanged(Object sender, EventArgs e);

		public event FilterChanged FilterChange;

		protected virtual void OnFilterChange(Object sender, EventArgs e)
		{
			if (FilterChange != null)
			{
				FilterChange(this, e);
			}
		}
		#endregion

		#region ShowFilters
		/// <summary>
		/// Gets or sets a value indicating whether filters are visible.
		/// </summary>
		/// <value><c>true</c> if filters are visible; otherwise, <c>false</c>.</value>
		public bool ShowFilters
		{
			set
			{
				FiltersDiv.Visible = value;
			}
			get
			{
				return FiltersDiv.Visible;
			}
		}
		#endregion

		#region ShowWeeker
		/// <summary>
		/// Gets or sets a value indicating whether weeker is visible.
		/// </summary>
		/// <value><c>true</c> if weeker is visible; otherwise, <c>false</c>.</value>
		public bool ShowWeeker
		{
			set
			{
				WeekerDiv.Visible = value;
			}
			get
			{
				return WeekerDiv.Visible;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			//DV 200-12-06 change
			BindVisibility();
			//BindDD();

			if (!Page.IsPostBack)
			{
				//BindVisibility();
				BindDD();
				BindSavedValues();
				SaveValues();
				BindFilters(); //by DV 2008-04-23
			}

            DTCBeg.ValueChange += new PickerControl.ValueChanged(DTCBeg_ValueChange);
			DTCEnd.ValueChange += new PickerControl.ValueChanged(DTCEnd_ValueChange);
			DTCWeek.ValueChange += new PickerControl.ValueChanged(DTCWeek_ValueChange);
			ttBlock.SelectedIndexChanged += new EventHandler(ttBlock_SelectedIndexChanged);
			ddPeriod.SelectedIndexChanged += new EventHandler(ddPeriod_SelectedIndexChanged);
			ddUser.SelectedIndexChanged += new EventHandler(ddUser_SelectedIndexChanged);
			ddState.SelectedIndexChanged += new EventHandler(ddState_SelectedIndexChanged);

			DTCWeeker.ValueChange += new Weeker.ValueChanged(DTCWeeker_ValueChange);

			btnSave.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_Apply}");
			btnReset.Text = CHelper.GetResFileString("{IbnFramework.Global:_mc_Reset}");

			this.Page.LoadComplete += new EventHandler(Page_LoadComplete);
		}

		void Page_LoadComplete(object sender, EventArgs e)
		{
			// O.R. in Page_Load cntrlWeeker.SelectedDate doesn't have value yet
			if (this.Visible)
			{
				if (tdWeek.Visible)
					CHelper.AddToContext("SelectedWeek", DTCWeek.SelectedDate);
				if (WeekerDiv.Visible)
					CHelper.AddToContext("SelectedWeek", DTCWeeker.SelectedDate);
			}
		}

		void DTCWeeker_ValueChange(object sender, EventArgs e)
		{
			btnSave_Click(this, e);
		}

		void ddState_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ddState.AutoPostBack)
				btnSave_Click(this, e);
		}

		void ddUser_SelectedIndexChanged(object sender, EventArgs e)
		{
			CHelper.AddToContext("TTQAddBindUsersFlag", 1);
			CHelper.UpdatePanelUpdate(this.Page, "up_tt_QuickAdd");
			
			if (ddUser.AutoPostBack)
				btnSave_Click(this, e);
		}

		void DTCWeek_ValueChange(object sender, EventArgs e)
		{
			if (DTCWeek.AutoPostBack)
				btnSave_Click(this, e);
		}

		#region ddPeriod_SelectedIndexChanged
		void ddPeriod_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ddPeriod.AutoPostBack)
				btnSave_Click(this, e);
		} 
		#endregion

		#region ttBlock_SelectedIndexChanged
		void ttBlock_SelectedIndexChanged(object sender, EventArgs e)
		{
			CHelper.UpdatePanelUpdate(this.Page, "up_tt_QuickAdd");
			EnsureSelectInstance();
			if (ttBlock.AutoPostBack)
				btnSave_Click(this, e);
		} 
		#endregion

		#region DTCEnd_ValueChange
		void DTCEnd_ValueChange(object sender, EventArgs e)
		{
			if (ddPeriod.SelectedValue == "-1" && tdPeriod.Visible && DTCBeg.SelectedDate > DTCEnd.SelectedDate)
			{
				throw new ArgumentException("Invalid data period");
			}

			if (DTCEnd.AutoPostBack)
				btnSave_Click(this, e);
		} 
		#endregion

		#region DTCBeg_ValueChange
		void DTCBeg_ValueChange(object sender, EventArgs e)
		{
			if (ddPeriod.SelectedValue == "-1" && tdPeriod.Visible && DTCBeg.SelectedDate > DTCEnd.SelectedDate)
			{
				throw new ArgumentException("Invalid data period");
			}

			if (DTCBeg.AutoPostBack)
				btnSave_Click(this, e);
		} 
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			BindFilters();

			if (ddPeriod.SelectedValue == "0")
			{
				divBeg.Style.Add("display", "");
				divEnd.Style.Add("display", "none");
			}
			else if (ddPeriod.SelectedValue == "-1")
			{
				divBeg.Style.Add("display", "");
				divEnd.Style.Add("display", "");
			}
			else
			{
				divBeg.Style.Add("display", "none");
				divEnd.Style.Add("display", "none");
			}
			ddPeriod.Attributes.Add("onchange", string.Format("DateTime_WeekModify('{0}', '{1}', '{2}')", ddPeriod.ClientID, divBeg.ClientID, divEnd.ClientID));

			RegisterClientScripts();
			if (tdPeriod.Visible)
			{
				tdPeriod.ColSpan = 2;
				tdEmpty.Visible = false;
			}
			else
			{
				tdPeriod.ColSpan = 1;
				tdEmpty.Visible = true;
			}

			if (!tdPeriod.Visible && !tdWeek.Visible)
				trWeekPeriod.Visible = false;

			if (!Page.IsPostBack)
				this.OnFilterChange(this, e);
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			tdWeek.Visible = false;
			tdProjectTitle.Visible = false;
            tdButtons.Visible = true;
			tdState.Visible = false;

			//tdEmpty.Visible = false;

			switch (MetaViewName)
			{
				case "TT_MyGroupByWeek":
				case "TT_CurrentProjectGroupByWeekUser":
					tdUser.Visible = false;
					tdProject.Visible = false;
					tdProjectTitle.Visible = true;
					break;
				case "TT_MyGroupByWeekProject":
					tdProject.Visible = false;
					tdUser.Visible = false;
					break;
				case "TT_ManagerGroupByWeekUser":
					tdUser.Visible = false;
					break;
				case "TT_ManagerGroupByWeekProject":
					tdProject.Visible = false;
					break;
				case "TT_ManagerGroupByUserProject":				
					tdPeriod.Visible = false;
					tdWeek.Visible = false;
					tdUser.Visible = true;
					tdPeriod.Visible = false;
					tdButtons.Visible = false;
					tdProject.Visible = false;
					tdState.Visible = true;

					ddPeriod.AutoPostBack = true;
					ddState.AutoPostBack = true;
					ddUser.AutoPostBack = true;
					ttBlock.AutoPostBack = false;
					break;
				case "TT_ManagerGroupByProjectUser":
					tdPeriod.Visible = false;
					tdWeek.Visible = false;
					tdUser.Visible = false;
					tdProject.Visible = true;
					tdButtons.Visible = false;
					tdState.Visible = true;

					ddPeriod.AutoPostBack = true;
					ddUser.AutoPostBack = false;
					ddState.AutoPostBack = true;
					ttBlock.AutoPostBack = true;
					break;
                case "TT_MyGroupByWeekProjectAll":
                    tdProject.Visible = true;
                    tdUser.Visible = false;
                    tdWeek.Visible = false;
                    tdPeriod.Visible = true;
                    tdButtons.Visible = false;
					DTCBeg.AutoPostBack = true;
					DTCEnd.AutoPostBack = true;
					ttBlock.AutoPostBack = true;
					ddPeriod.AutoPostBack = true;
                    break;
				default:
					break;
			}
		}
		#endregion

		#region BindDD
		private void BindDD()
		{
			BindStates();
			BindDates();
			BindBlocks();
			BindUsers();
			DateTime dt = CHelper.GetWeekStartByDate(DateTime.Now);
			DTCBeg.SelectedDate = dt;
			DTCEnd.SelectedDate = dt;
			DTCWeek.SelectedDate = dt;
			if (Request["ProjectId"] != null)
				lblProjTitle.Text = Mediachase.UI.Web.Util.CommonHelper.GetProjectStatus(int.Parse(Request["ProjectId"].ToString()));
		}

		private void BindDates()
		{
			ddPeriod.Items.Clear();
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_ThisWeek}"), "[DateTimeThisWeek]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_LastWeek}"), "[DateTimeLastWeek]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_ThisMonth}"), "[DateTimeThisMonth]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_LastMonth}"), "[DateTimeLastMonth]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_ThisYear}"), "[DateTimeThisYear]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_LastYear}"), "[DateTimeLastYear]"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_CustomWeek}"), "0"));
			ddPeriod.Items.Add(new ListItem(CHelper.GetResFileString("{TimeTracking:_mc_CustomPeriod}"), "-1"));
		}

		private void BindBlocks()
		{
			string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;
			ttBlock.Items.Clear();

			// Non-project
			bool isHeaderAdded = false;
			foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetNonProjectBlockTypeInstances())
			{
				if (!isHeaderAdded)
				{
					ttBlock.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "NonProject").ToString(), "-1"));
					isHeaderAdded = true;
				}

				ListItem li = new ListItem("   " + bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
				ttBlock.Items.Add(li);
			}

			// Projects
			isHeaderAdded = false;
			foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				if (!isHeaderAdded)
				{
					ttBlock.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByProject").ToString(), "-2"));
					isHeaderAdded = true;
				}

				ListItem li = new ListItem("   " + bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
				ttBlock.Items.Add(li);
			}

			//ttBlock.DataSource = TimeTrackingManager.GetBlockTypeInstances().DefaultView;
			//ttBlock.DataTextField = "Title";
			//ttBlock.DataValueField = "PrimaryKeyId";
			//ttBlock.DataBind();

			EnsureSelectInstance();

			if (tdWeek.Visible || WeekerDiv.Visible)
				ttBlock.Items.Insert(0, new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));
		}

		private void BindUsers()
		{
			ddUser.Items.Clear();
			if (tdWeek.Visible || WeekerDiv.Visible)
				ddUser.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));
			Principal[] mas = Principal.List(new FilterElementCollection(FilterElement.EqualElement("Card", "User"), FilterElement.EqualElement("Activity", 3)), new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc)));
			foreach (Principal pl in mas)
				ddUser.Items.Add(new ListItem(pl.Name, pl.PrimaryKeyId.ToString()));
		}

		private void BindStates()
		{
			ddState.Items.Add(new ListItem(CHelper.GetResFileString("{IbnFramework.Global:_mc_All}"), "0"));

			foreach (MetaObject mo in StateMachineManager.GetAvailableStates(TimeTrackingBlock.GetAssignedMetaClass()))
			{
				ddState.Items.Add(new ListItem(CHelper.GetResFileString(mo.Properties["FriendlyName"].Value.ToString()), mo.Properties["FriendlyName"].Value.ToString()));
			}
		}
		#endregion

		#region BindFilters
		private void BindFilters()
		{
			if (!this.Visible)
				return;

			McMetaViewPreference mvPref = GetMetaViewPreference();

			// O.R.: It's expected that the TTFilterPopupEdit is the only filter control on the page, which uses MetaViewPrefrences filters
			mvPref.Filters.Clear();

			FilterElement filter = null;

			if (tdPeriod.Visible)
				switch (ddPeriod.SelectedValue)
				{
					case "[DateTimeThisWeek]":
						filter = new FilterElement("StartDate", FilterElementType.Equal, CHelper.GetWeekStartByDate(DateTime.Today));
						break;
					case "[DateTimeLastWeek]":
						filter = new FilterElement("StartDate", FilterElementType.Equal, CHelper.GetWeekStartByDate(DateTime.Today.AddDays(-7)));
						break;
					case "[DateTimeThisMonth]":
						filter = new IntervalFilterElement("StartDate", CHelper.GetWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.Day)), CHelper.GetWeekStartByDate(DateTime.Today));
						break;
					case "[DateTimeLastMonth]":
						filter = new IntervalFilterElement("StartDate", CHelper.GetWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.Day).AddMonths(-1)), CHelper.GetWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.Day)));
						break;
					case "[DateTimeThisYear]":
						filter = new IntervalFilterElement("StartDate", CHelper.GetWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear)), CHelper.GetWeekStartByDate(DateTime.Today));
						break;
					case "[DateTimeLastYear]":
						filter = new IntervalFilterElement("StartDate", CHelper.GetWeekStartByDate(DateTime.Today.AddDays(1 - DateTime.Now.DayOfYear).AddYears(-1)), CHelper.GetWeekStartByDate(DateTime.Today.AddDays(-DateTime.Now.DayOfYear)));
						break;
					case "0":
						DateTime dt = DTCBeg.SelectedDate;
						filter = new FilterElement("StartDate", FilterElementType.Equal, CHelper.GetWeekStartByDate(dt));
						break;
					case "-1":
						DateTime dt1 = DTCBeg.SelectedDate;
						DateTime dt2 = DTCEnd.SelectedDate;
						filter = new IntervalFilterElement("StartDate", CHelper.GetWeekStartByDate(dt1), CHelper.GetWeekStartByDate(dt2));
						break;
					default:
						break;
				}
			else if (tdWeek.Visible)
			{
				DateTime dt = DTCWeek.SelectedDate;
				filter = new FilterElement("StartDate", FilterElementType.Equal, CHelper.GetWeekStartByDate(dt));
			}
			else if (WeekerDiv.Visible)
			{
				DateTime dt = DTCWeeker.SelectedDate;
				filter = new FilterElement("StartDate", FilterElementType.Equal, CHelper.GetWeekStartByDate(dt));
			}

			if (filter != null)
				mvPref.Filters.Add(filter);

			if (tdUser.Visible && ddUser.SelectedItem != null && ddUser.SelectedValue != "0")
			{
				filter = new FilterElement("OwnerId", FilterElementType.Equal, int.Parse(ddUser.SelectedValue));
				mvPref.Filters.Add(filter);
			}

			if (ttBlock.Visible && ttBlock.SelectedItem != null && ttBlock.SelectedValue != "0")
			{
				filter = new FilterElement("BlockTypeInstanceId", FilterElementType.Equal, int.Parse(ttBlock.SelectedValue));
				mvPref.Filters.Add(filter);
			}

			if (tdState.Visible && ddState.SelectedValue != "0")
			{
				filter = new FilterElement("StateFriendlyName", FilterElementType.Equal, ddState.SelectedValue);
				mvPref.Filters.Add(filter);
			}

			//Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.IBN.Business.Security.CurrentUser.UserID, mvPref);
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			McMetaViewPreference pref = GetMetaViewPreference();
			if (tdProject.Visible)
				CHelper.SafeSelect(ttBlock, pref.GetAttribute<string>(FilterBlockAttr, FilterBlockAttr, "0"));
			if (tdUser.Visible)
				CHelper.SafeSelect(ddUser, pref.GetAttribute<string>(FilterUserAttr, FilterUserAttr, Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString()));
			if (tdPeriod.Visible)
			{
				CHelper.SafeSelect(ddPeriod, pref.GetAttribute<string>("TTFilter_Period", "TTFilter_Period", "[DateTimeThisWeek]"));
				if (ddPeriod.SelectedValue == "0")			//week
				{
					DateTime dt = CHelper.GetWeekStartByDate(pref.GetAttribute<DateTime>("TTFilter_DTCBeg", "TTFilter_DTCBeg", DateTime.Now));
					DTCBeg.SelectedDate = dt;
				}
				else if (ddPeriod.SelectedValue == "-1")	//period
				{
					DateTime dt = CHelper.GetWeekStartByDate(pref.GetAttribute<DateTime>("TTFilter_DTCBeg", "TTFilter_DTCBeg", DateTime.Now));
					DTCBeg.SelectedDate = dt;
					dt = CHelper.GetWeekStartByDate(pref.GetAttribute<DateTime>("TTFilter_DTCEnd", "TTFilter_DTCEnd", DateTime.Now));
					DTCEnd.SelectedDate = dt;
				}
			}
			else if (tdWeek.Visible)
			{
				DateTime dt = CHelper.GetWeekStartByDate(pref.GetAttribute<DateTime>(FilterWeekAttr, FilterWeekAttr, DateTime.Now));
				if (dt == DateTime.MinValue)
					dt = CHelper.GetWeekStartByDate(DateTime.Now);
				DTCWeek.SelectedDate = dt;

				if (this.Visible)
					CHelper.AddToContext("SelectedWeek", dt);
			}
			else if (WeekerDiv.Visible)
			{
				DateTime dt = CHelper.GetWeekStartByDate(pref.GetAttribute<DateTime>(FilterWeekAttr, FilterWeekAttr, DateTime.Now));
				if (dt == DateTime.MinValue)
					dt = CHelper.GetWeekStartByDate(DateTime.Now);
				DTCWeeker.SelectedDate = dt;

				if (this.Visible)
					CHelper.AddToContext("SelectedWeek", dt);
			}

			if (tdState.Visible)
				CHelper.SafeSelect(ddState, pref.GetAttribute<string>(FilterStateAttr, FilterStateAttr, "0"));
		}
		#endregion

		#region SaveValues
		private void SaveValues()
		{
			bool bSavePref = false;
			McMetaViewPreference pref = GetMetaViewPreference();

			if (tdProject.Visible)
				bSavePref = pref.SetAttribute<string>(FilterBlockAttr, FilterBlockAttr, ttBlock.SelectedValue);
			else
				bSavePref = pref.SetAttribute<string>(FilterBlockAttr, FilterBlockAttr, null);

			if (tdUser.Visible)
			{
				if (pref.SetAttribute<string>(FilterUserAttr, FilterUserAttr, ddUser.SelectedValue))
					bSavePref = true;
			}
			else
			{
				if(pref.SetAttribute<string>(FilterUserAttr, FilterUserAttr, null))
					bSavePref = true;
			}


			if (tdPeriod.Visible)
			{
				if(pref.SetAttribute<string>("TTFilter_Period", "TTFilter_Period", ddPeriod.SelectedValue))
					bSavePref = true;
				if (ddPeriod.SelectedValue == "0")
				{
					if(pref.SetAttribute<DateTime>("TTFilter_DTCBeg", "TTFilter_DTCBeg", DTCBeg.SelectedDate))
						bSavePref = true;
				}
				else if (ddPeriod.SelectedValue == "-1")
				{
					if(pref.SetAttribute<DateTime>("TTFilter_DTCBeg", "TTFilter_DTCBeg", DTCBeg.SelectedDate))
						bSavePref = true;
					if(pref.SetAttribute<DateTime>("TTFilter_DTCEnd", "TTFilter_DTCEnd", DTCEnd.SelectedDate))
						bSavePref = true;
				}
			}
			else if (tdWeek.Visible)
			{
				if(pref.SetAttribute<DateTime>(FilterWeekAttr, FilterWeekAttr, DTCWeek.SelectedDate))
					bSavePref = true;
			}
			else if (WeekerDiv.Visible)
			{
				if(pref.SetAttribute<DateTime>(FilterWeekAttr, FilterWeekAttr, DTCWeeker.SelectedDate))
					bSavePref = true;
			}
			if (tdState.Visible)
			{
				if (pref.SetAttribute<string>(FilterStateAttr, FilterStateAttr, ddState.SelectedValue))
					bSavePref = true;
			}
			else
			{
				if(pref.SetAttribute<string>(FilterStateAttr, FilterStateAttr, null))
					bSavePref = true;
			}

			if (bSavePref)
				Mediachase.Ibn.Core.UserMetaViewPreference.Save(Mediachase.IBN.Business.Security.CurrentUser.UserID, pref);
		}
		#endregion

		#region EnsureSelectInstance
		private void EnsureSelectInstance()
		{
			if (ttBlock.Items.Count > 0)
			{
				int selectedValue = int.Parse(ttBlock.SelectedValue, CultureInfo.InvariantCulture);
				if (selectedValue < 0)
					ttBlock.SelectedIndex = ttBlock.SelectedIndex + 1;
			}
		}
		#endregion

		#region Events
		protected void btnSave_Click(object sender, EventArgs e)
		{
			SaveValues();
			//BindFilters();
			CHelper.UpdatePanelUpdate(this.Page, "GridPanel1");
			CHelper.UpdatePanelUpdate(this.Page, "SelectorPanel");
			CHelper.UpdatePanelUpdate(this.Page, "panelGridGeneral");

			CHelper.AddToContext("TTQAddBindBlocksFlag", 1);
			CHelper.AddToContext("SelectedWeek", DTCWeeker.SelectedDate);
			CHelper.AddToContext("NeedToClearSelector", "true");
			CHelper.RequireBindGrid();
			this.OnFilterChange(this, e);
		}

		protected void btnReset_Click(object sender, EventArgs e)
		{
			BindDD();
			SaveValues();
			//BindFilters();
			CHelper.UpdatePanelUpdate(this.Page, "GridPanel1");
			CHelper.UpdatePanelUpdate(this.Page, "SelectorPanel");
			CHelper.UpdatePanelUpdate(this.Page, "panelGridGeneral");
			CHelper.AddToContext("SelectedWeek", DTCWeeker.SelectedDate);
			CHelper.AddToContext("NeedToClearSelector", "true");
			CHelper.RequireBindGrid();
			this.OnFilterChange(this, e);
		}
		#endregion

		#region RegisterClientScripts
		public bool RegisterClientScripts()
		{
			if (!Page.ClientScript.IsClientScriptBlockRegistered(this.Page.GetType(), "DateTime_WeekModify"))
			{
				Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "DateTime_WeekModify", GetClientScript());
				return true;
			}
			return false;
		}
		public string GetClientScript()
		{
			string ret = "<script language='javascript'>" +
								"\n function DateTime_WeekModify(obj,hidenEl1, hidenEl2)" +
								"\n {" +
									"\n var objToHide1 = document.getElementById(hidenEl1);" +
									"\n var objToHide2 = document.getElementById(hidenEl2);" +
									"\n var objVal = document.getElementById(obj);" +
									"\n if(objToHide1 == null || objToHide2 == null || objVal == null) return;" +
									"\n var curVal = objVal.value;" +

									"\n if(curVal == '0')" +
									"\n {" +
										"\n objToHide1.style.display = '';" +
										"\n objToHide2.style.display = 'none';" +
									"\n }" +
									"\n else if(curVal == '-1')" +
									"\n {" +
										"\n objToHide1.style.display = '';" +
										"\n objToHide2.style.display = '';" +
									"\n }" +
									"\n else" +
									"\n {" +
										"\n objToHide1.style.display = 'none';" +
										"\n objToHide2.style.display = 'none';" +
									"\n }" +
								"\n }" +
							"\n </script> \n";
			return ret;
		}
		#endregion
	}
}