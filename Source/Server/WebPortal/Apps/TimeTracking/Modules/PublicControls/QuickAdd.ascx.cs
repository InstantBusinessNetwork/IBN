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
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class QuickAdd : System.Web.UI.UserControl
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

		#region BlockId
		public int BlockId
		{
			get
			{
				int retval = -1;
				if (Request.QueryString["BlockId"] != null)
				{
					int.TryParse(Request.QueryString["BlockId"], NumberStyles.Integer, CultureInfo.InvariantCulture, out retval);
				}
				return retval;
			}
		}
		#endregion

		#region blockInstanceId
		protected int blockInstanceId
		{
			get
			{
				int retval = -1;
				if (ViewState["blockInstanceId"] != null)
					retval = (int)ViewState["blockInstanceId"];
				return retval;
			}
			set
			{
				ViewState["blockInstanceId"] = value;
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

		protected void Page_Load(object sender, EventArgs e)
		{
			lblError2.Style.Add("display", "none");

			if (!Page.IsPostBack)
			{
				ApplyStartValues();
				BindBlocks();
			}
		}

		#region ApplyStartValues
		private void ApplyStartValues()
		{
			if (BlockId > 0)
			{
				TimeTrackingBlock block = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(TimeTrackingBlock.GetAssignedMetaClass(), BlockId);
				if (block != null)
				{
					blockInstanceId = block.BlockTypeInstanceId;
				}
			}

			BindNullValues();
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

		#region BindBlocks
		private void BindBlocks()
		{
			ProjectList.Items.Clear();

			string titledFieldName = TimeTrackingManager.GetBlockTypeInstanceMetaClass().TitleFieldName;

			McMetaViewPreference pref = CHelper.GetMetaViewPreference(CurrentView);
			DateTime startDate = CHelper.GetRealWeekStartByDate(pref.GetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, DateTime.MinValue));
			if (startDate == DateTime.MinValue)
				startDate = CHelper.GetRealWeekStartByDate(DateTime.Now);
			int ownerId = Mediachase.IBN.Business.Security.CurrentUser.UserID;

			BindDayHeaders(startDate);

			// Non-project
			#region 1. Make the list of all BlockTypeInstances
			List<int> idList = new List<int>();
			Dictionary<int, string> allList = new Dictionary<int, string>();
			foreach (TimeTrackingBlockTypeInstance item in TimeTrackingManager.GetNonProjectBlockTypeInstances())
			{
				idList.Add(item.PrimaryKeyId.Value);
				allList.Add(item.PrimaryKeyId.Value, item.Title);
			}
			#endregion

			#region 2. Check the rights AddMyTTBlock and AddAnyTTBlock and remove from the allList the forbidden items
			SerializableDictionary<int, Collection<string>> objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				if (!((allowedRights.Contains(TimeTrackingManager.Right_AddMyTTBlock) && ownerId == Mediachase.Ibn.Data.Services.Security.CurrentUserId) || allowedRights.Contains(TimeTrackingManager.Right_AddAnyTTBlock)))
				{
					allList.Remove(id);
				}
			}
			#endregion

			#region 3. Make the list of the TimeTrackingBlocks by OwnerId, StartDate and BlockTypeInstanceId[]
			List<string> blockTypeInstanceIdList = new List<string>();
			foreach (int id in allList.Keys)
				blockTypeInstanceIdList.Add(id.ToString(CultureInfo.InvariantCulture));

			List<int> blockIdList = new List<int>();
			Dictionary<int, int> blockInstanceList = new Dictionary<int, int>();

			TimeTrackingBlock[] blocks = TimeTrackingBlock.List(
				FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.EqualElement("StartDate", startDate),
				new FilterElement("BlockTypeInstanceId", FilterElementType.In, blockTypeInstanceIdList.ToArray())
				);
			foreach (TimeTrackingBlock block in blocks)
			{
				blockIdList.Add(block.PrimaryKeyId.Value);
				blockInstanceList.Add(block.PrimaryKeyId.Value, block.BlockTypeInstanceId);
			}
			#endregion

			#region 4. Check the right Write and remove from the allList the forbidden items
			objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlock.GetAssignedMetaClass(), blockIdList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				if (!allowedRights.Contains(Mediachase.Ibn.Data.Services.Security.RightWrite))
				{
					allList.Remove(blockInstanceList[id]);
				}
			}
			#endregion

			#region 5. Fill in the dropdown
			if (allList.Count > 0)
			{
				ProjectList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByActivity").ToString(), "-1"));

				foreach (KeyValuePair<int, string> item in allList)
				{
					ListItem li = new ListItem("   " + item.Value, item.Key.ToString());
					ProjectList.Items.Add(li);
				}
			}
			#endregion


			// Projects
			#region 1. Make the list of all BlockTypeInstances
			idList = new List<int>();
			allList = new Dictionary<int, string>();
			foreach (TimeTrackingBlockTypeInstance item in TimeTrackingManager.GetProjectBlockTypeInstances())
			{
				idList.Add(item.PrimaryKeyId.Value);
				allList.Add(item.PrimaryKeyId.Value, item.Title);
			}
			#endregion

			#region 2. Check the rights AddMyTTBlock and AddAnyTTBlock and remove from the allList the forbidden items
			objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlockTypeInstance.GetAssignedMetaClass(), idList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				if (!((allowedRights.Contains(TimeTrackingManager.Right_AddMyTTBlock) && ownerId == Mediachase.Ibn.Data.Services.Security.CurrentUserId) || allowedRights.Contains(TimeTrackingManager.Right_AddAnyTTBlock)))
				{
					allList.Remove(id);
				}
			}
			#endregion

			#region 3. Make the list of the TimeTrackingBlocks by OwnerId, StartDate and BlockTypeInstanceId[]
			blockTypeInstanceIdList = new List<string>();
			foreach (int id in allList.Keys)
				blockTypeInstanceIdList.Add(id.ToString(CultureInfo.InvariantCulture));

			blockIdList = new List<int>();
			blockInstanceList = new Dictionary<int, int>();

			blocks = TimeTrackingBlock.List(
				FilterElement.EqualElement("OwnerId", ownerId),
				FilterElement.EqualElement("StartDate", startDate),
				new FilterElement("BlockTypeInstanceId", FilterElementType.In, blockTypeInstanceIdList.ToArray())
				);
			foreach (TimeTrackingBlock block in blocks)
			{
				blockIdList.Add(block.PrimaryKeyId.Value);
				blockInstanceList.Add(block.PrimaryKeyId.Value, block.BlockTypeInstanceId);
			}
			#endregion

			#region 4. Check the right Write and remove from the allList the forbidden items
			objectRights = Mediachase.Ibn.Data.Services.Security.GetAllowedRights(TimeTrackingBlock.GetAssignedMetaClass(), blockIdList.ToArray());
			foreach (KeyValuePair<int, Collection<string>> item in objectRights)
			{
				int id = item.Key;
				Collection<string> allowedRights = item.Value;

				if (!allowedRights.Contains(Mediachase.Ibn.Data.Services.Security.RightWrite))
				{
					allList.Remove(blockInstanceList[id]);
				}
			}
			#endregion

			#region 5. Fill in the dropdown
			if (allList.Count > 0)
			{
				ProjectList.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.TimeTracking", "ByProject").ToString(), "-2"));

				foreach (KeyValuePair<int, string> item in allList)
				{
					ListItem li = new ListItem("   " + item.Value, item.Key.ToString());
					ProjectList.Items.Add(li);
				}
			}
			#endregion

			if (blockInstanceId > 0)
				CHelper.SafeSelect(ProjectList, blockInstanceId.ToString());

			EnsureSelectInstance();
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

		#region AddButton_Click
		protected void AddButton_Click(object sender, EventArgs e)
		{
			if (Process())
			{
				CommandParameters cp = new CommandParameters("MC_TimeTracking_QuickAddFrame");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion

		#region AddCloseButton_Click
		protected void AddCloseButton_Click(object sender, EventArgs e)
		{
			if (Process())
			{
				CommandParameters cp = new CommandParameters("MC_TimeTracking_QuickAddFrame");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion

		#region CancelButton_Click
		protected void CancelButton_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters("MC_TimeTracking_QuickAddFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion

		#region Process
		private bool Process()
		{
			Page.Validate();
			if (!Page.IsValid)
				return false;

			int blockTypeInstanceId = int.Parse(ProjectList.SelectedValue);
			if (blockTypeInstanceId < 0)
			{
				lblError2.Style.Add("display", "");
				return false;
			}

			McMetaViewPreference pref = CHelper.GetMetaViewPreference(CurrentView);
			DateTime startDate = CHelper.GetRealWeekStartByDate(pref.GetAttribute<DateTime>(TTFilterPopupEdit.FilterWeekAttr, TTFilterPopupEdit.FilterWeekAttr, DateTime.Today));
			if (startDate == DateTime.MinValue)
				startDate = CHelper.GetRealWeekStartByDate(DateTime.Today);

			double maxMinutes = (double)(24 * 60);
			TimeTrackingManager.AddEntryWithData(blockTypeInstanceId,
				startDate,
				Mediachase.IBN.Business.Security.CurrentUser.UserID,
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

		#region Page_PreRender
		protected void Page_PreRender(object sender, EventArgs e)
		{
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
		}
		#endregion
	}
}