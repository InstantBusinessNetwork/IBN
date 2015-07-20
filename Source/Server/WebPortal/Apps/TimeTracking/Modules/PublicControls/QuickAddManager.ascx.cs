using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Core;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class QuickAddManager : System.Web.UI.UserControl
	{
		#region ViewName
		public string ViewName
		{
			get
			{
				string retval = String.Empty;
				if (Request.QueryString["ViewName"] != null)
					retval = Request.QueryString["ViewName"];

				return retval;
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
					if (DataContext.Current.MetaModel.MetaViews[ViewName] == null)
						throw new ArgumentException(String.Format("Cant find meta view: {0}", ViewName));
					currentView = DataContext.Current.MetaModel.MetaViews[ViewName];
				}
				return currentView;
			}
		}
		#endregion

		#region startDate
		private DateTime? _startDate;
		private DateTime startDate
		{
			get
			{
				if (!_startDate.HasValue)
				{
					McMetaViewPreference pref = CHelper.GetMetaViewPreference(CurrentView);
					_startDate = CHelper.GetRealWeekStartByDate(pref.GetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, DateTime.MinValue));
					if (_startDate == DateTime.MinValue)
						_startDate = CHelper.GetRealWeekStartByDate(DateTime.Now);
				}

				return _startDate.Value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			lblError2.Style.Add("display", "none");

			if (!Page.IsPostBack)
			{
				BindProjects();
				BindNullValues();
			}
		}

		#region AddButton_Click
		protected void AddButton_Click(object sender, EventArgs e)
		{
			if (Process())
			{
				CommandParameters cp = new CommandParameters("MC_TimeTracking_QuickAddManagerFrame");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion

		#region AddCloseButton_Click
		protected void AddCloseButton_Click(object sender, EventArgs e)
		{
			if (Process())
			{
				CommandParameters cp = new CommandParameters("MC_TimeTracking_QuickAddManagerFrame");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion

		#region Process
		private bool Process()
		{
			Page.Validate();
			if (!Page.IsValid)
				return false;

			if (startDate == DateTime.MinValue)
				throw new NotSupportedException("Start date is not specified");

			int blockTypeInstanceId = int.Parse(ProjectList.SelectedValue);
			if (blockTypeInstanceId < 0)
			{
				lblError2.Style.Add("display", "");
				return false;
			}

			McMetaViewPreference pref = CHelper.GetMetaViewPreference(CurrentView);
			int ownerId = int.Parse(UserList.SelectedValue, CultureInfo.InvariantCulture);

			double maxMinutes = (double)(24 * 60);
			TimeTrackingManager.AddEntryWithData(blockTypeInstanceId,
				startDate,
				ownerId,
				txtEntry.Text,
				Math.Min((new TimeSpan(Day1Time.Value.Ticks)).TotalMinutes, maxMinutes),
				Math.Min((new TimeSpan(Day2Time.Value.Ticks)).TotalMinutes, maxMinutes),
				Math.Min((new TimeSpan(Day3Time.Value.Ticks)).TotalMinutes, maxMinutes),
				Math.Min((new TimeSpan(Day4Time.Value.Ticks)).TotalMinutes, maxMinutes),
				Math.Min((new TimeSpan(Day5Time.Value.Ticks)).TotalMinutes, maxMinutes),
				Math.Min((new TimeSpan(Day6Time.Value.Ticks)).TotalMinutes, maxMinutes),
				Math.Min((new TimeSpan(Day7Time.Value.Ticks)).TotalMinutes, maxMinutes));

			BindNullValues();

			return true;
		}
		#endregion

		#region BindNullValues
		private void BindNullValues()
		{
			txtEntry.Text = "";

			Day1Time.Value = DateTime.MinValue;
			Day2Time.Value = DateTime.MinValue;
			Day3Time.Value = DateTime.MinValue;
			Day4Time.Value = DateTime.MinValue;
			Day5Time.Value = DateTime.MinValue;
			Day6Time.Value = DateTime.MinValue;
			Day7Time.Value = DateTime.MinValue;
		}
		#endregion

		#region BindProjects
		private void BindProjects()
		{
			ProjectList.Items.Clear();

			if (startDate == DateTime.MinValue)
				return;

			string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;

			int blockTypeInstanceFromFilter = GetProjectFromFilter();
			if (blockTypeInstanceFromFilter > 0)
			{
				TimeTrackingBlockTypeInstance inst = MetaObjectActivator.CreateInstance<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), blockTypeInstanceFromFilter);
				if (inst != null)
				{
					ProjectList.Items.Add(new ListItem(inst.Properties[titledFieldName].Value.ToString(), blockTypeInstanceFromFilter.ToString()));
				}
			}
			else	// all instances
			{
				// Non-project
				bool isHeaderAdded = false;
				foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetNonProjectBlockTypeInstances())
				{
					if (!isHeaderAdded)
					{
						ProjectList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByActivity").ToString(), "-1"));
						isHeaderAdded = true;
					}

					ListItem li = new ListItem("   " + bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
					ProjectList.Items.Add(li);
				}

				// Projects
				isHeaderAdded = false;
				foreach (TimeTrackingBlockTypeInstance bo in TimeTrackingManager.GetProjectBlockTypeInstances())
				{
					if (!isHeaderAdded)
					{
						ProjectList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByProject").ToString(), "-2"));
						isHeaderAdded = true;
					}

					ListItem li = new ListItem("   " + bo.Properties[titledFieldName].Value.ToString(), bo.PrimaryKeyId.ToString());
					ProjectList.Items.Add(li);
				}
			}

			EnsureSelectInstance();

			BindUsers();
		}
		#endregion

		#region BindUsers
		private void BindUsers()
		{
			string savedValue;
			if (!IsPostBack)
				savedValue = Mediachase.IBN.Business.Security.CurrentUser.UserID.ToString();
			else
				savedValue = UserList.SelectedValue;

			UserList.Items.Clear();

			if (ProjectList.SelectedValue == null || startDate == DateTime.MinValue)
				return;

			int blockTypeInstanceId = int.Parse(ProjectList.SelectedValue, CultureInfo.InvariantCulture);

			TimeTrackingBlockTypeInstance inst = MetaObjectActivator.CreateInstance<TimeTrackingBlockTypeInstance>(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), blockTypeInstanceId);

			int userFromFilter = GetUserFromFilter();
			if (userFromFilter > 0)
			{
				if (Mediachase.Ibn.Data.Services.Security.CheckObjectRight(inst, TimeTrackingManager.Right_AddAnyTTBlock)
					|| (Mediachase.Ibn.Data.Services.Security.CheckObjectRight(inst, TimeTrackingManager.Right_AddMyTTBlock) && userFromFilter == Mediachase.Ibn.Data.Services.Security.CurrentUserId))
				{
					TimeTrackingBlock block = TimeTrackingManager.GetTimeTrackingBlock(blockTypeInstanceId, startDate, userFromFilter);
					if (block == null || Mediachase.Ibn.Data.Services.Security.CanWrite(block))
					{
						Principal pl = MetaObjectActivator.CreateInstance<Principal>(Principal.GetAssignedMetaClass(), userFromFilter);
						if (pl != null)
						{
							UserList.Items.Add(new ListItem(pl.Name, userFromFilter.ToString()));
						}
					}
				}
			}
			else	// all users
			{
				if (Mediachase.Ibn.Data.Services.Security.CheckObjectRight(inst, TimeTrackingManager.Right_AddAnyTTBlock))
				{
					#region 1. Make the Dictionary of Principal
					Dictionary<int, string> allUsers = new Dictionary<int, string>();
					Principal[] principals = Principal.List(new FilterElementCollection(FilterElement.EqualElement("Card", "User"), FilterElement.EqualElement("Activity", 3)), new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc)));
					foreach (Principal p in principals)
					{
						allUsers.Add(p.PrimaryKeyId.Value, p.Name);
					}
					#endregion

					#region 2. Make the list of the Id (to pass it as array) and the Dictionary of TimeTrackingBlock
					List<int> idList = new List<int>();
					Dictionary<int, TimeTrackingBlock> allblocks = new Dictionary<int, TimeTrackingBlock>();
					TimeTrackingBlock[] blocks = TimeTrackingBlock.List(FilterElement.EqualElement("StartDate", startDate), FilterElement.EqualElement("BlockTypeInstanceId", blockTypeInstanceId));
					foreach (TimeTrackingBlock block in blocks)
					{
						idList.Add(block.PrimaryKeyId.Value);
						allblocks.Add(block.PrimaryKeyId.Value, block);
					}
					#endregion

					#region 3. Get the list of the existing blocks with rights and remove the forbidden items from allUsers
					SerializableDictionary<int, Collection<string>> objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlock.GetAssignedMetaClass(), idList.ToArray());
					foreach (KeyValuePair<int, Collection<string>> item in objectRights)
					{
						int id = item.Key;
						Collection<string> allowedRights = item.Value;

						TimeTrackingBlock block = allblocks[id];
						int ownerId = block.OwnerId;

						if (!allowedRights.Contains(Mediachase.Ibn.Data.Services.Security.RightWrite))
						{
							allUsers.Remove(ownerId);
						}
					}
					#endregion

					#region 4. Fill in the dropdown
					foreach (KeyValuePair<int, string> item in allUsers)
					{
						UserList.Items.Add(new ListItem(item.Value, item.Key.ToString()));
					}
					#endregion
				}
				else if (Mediachase.Ibn.Data.Services.Security.CheckObjectRight(inst, TimeTrackingManager.Right_AddMyTTBlock))
				{
					// eliminate the block for which we don't have the "Write" right
					TimeTrackingBlock block = TimeTrackingManager.GetTimeTrackingBlock(blockTypeInstanceId, startDate, Mediachase.Ibn.Data.Services.Security.CurrentUserId);
					if (block == null || Mediachase.Ibn.Data.Services.Security.CanWrite(block))
					{
						Principal pl = MetaObjectActivator.CreateInstance<Principal>(Principal.GetAssignedMetaClass().Name, Mediachase.Ibn.Data.Services.Security.CurrentUserId);
						UserList.Items.Add(new ListItem(pl.Name, Mediachase.Ibn.Data.Services.Security.CurrentUserId.ToString()));
					}
				}
			}

			if (savedValue != null)
				CHelper.SafeSelect(UserList, savedValue);
		}
		#endregion

		#region EnsureSelectInstance
		private void EnsureSelectInstance()
		{
			if (ProjectList.Items.Count > 0)
			{
				int selectedValue = int.Parse(ProjectList.SelectedValue, CultureInfo.InvariantCulture);
				if (selectedValue < 0)
					ProjectList.SelectedIndex = ProjectList.SelectedIndex + 1;
			}
		}
		#endregion

		#region GetProjectFromFilter
		private int GetProjectFromFilter()
		{
			int retval = -1;
			McMetaViewPreference prefs = CHelper.GetMetaViewPreference(CurrentView);
			string blockId = prefs.GetAttribute<string>(TTFilterPopupEdit.FilterBlockAttr, TTFilterPopupEdit.FilterBlockAttr, "0");
			if (blockId != string.Empty)
				retval = Convert.ToInt32(blockId, CultureInfo.InvariantCulture);
			return retval;
		}
		#endregion

		#region GetUserFromFilter
		private int GetUserFromFilter()
		{
			int retval = -1;

			McMetaViewPreference prefs = CHelper.GetMetaViewPreference(CurrentView);
			string ownerId = prefs.GetAttribute<string>(TTFilterPopupEdit.FilterUserAttr, TTFilterPopupEdit.FilterUserAttr, "0");
			if (ownerId != string.Empty)
				retval = Convert.ToInt32(ownerId, CultureInfo.InvariantCulture);
			return retval;
		}
		#endregion

		#region ProjectList_SelectedIndexChanged
		protected void ProjectList_SelectedIndexChanged(object sender, EventArgs e)
		{
			EnsureSelectInstance();

			BindUsers();
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (Request["closeFramePopup"] != null)
			{
				CancelButton.OnClientClick = String.Format("javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
			}

			BindDayHeaders(startDate);

			// Project
			if (ProjectList.Items.Count == 0)
			{
				labelNoItems.Visible = true;
				ProjectList.Visible = false;
				AddButton.Enabled = false;
				AddCloseButton.Enabled = false;
			}
			else
			{
				labelNoItems.Visible = false;
				ProjectList.Visible = true;
				AddButton.Enabled = true;
				AddCloseButton.Enabled = true;
			}

			// User
			if (UserList.Items.Count == 0)
			{
				labelNoUsers.Visible = true;
				UserList.Visible = false;
				AddButton.Enabled = false;
				AddCloseButton.Enabled = false;
			}
			else
			{
				labelNoUsers.Visible = false;
				UserList.Visible = true;
			}
		}
		#endregion

		#region BindDayHeaders
		private void BindDayHeaders(DateTime startDate)
		{
			DateTime dt;
			DateTime curDate = Mediachase.IBN.Business.User.GetLocalDate(DateTime.UtcNow).Date;

			dt = startDate;
			Day1Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day1Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day2Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day2Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day3Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day3Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day4Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day4Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day5Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day5Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day6Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day6Label.CssClass = (curDate == dt) ? "smallSelected" : "small";

			dt = dt.AddDays(1);
			Day7Label.Text = String.Format(CultureInfo.InvariantCulture, "{0}, {1}", GetGlobalResourceObject("IbnFramework.TimeTracking", dt.DayOfWeek.ToString()).ToString(), dt.ToString("d MMM"));
			Day7Label.CssClass = (curDate == dt) ? "smallSelected" : "small";
		}
		#endregion
	}
}