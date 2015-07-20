using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.IBN.Business.SpreadSheet;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class TTAccountsControl : MCDataBoundControl
	{
		private const string keyBlockTypeInstance = "BlockTypeInstanceId";
		private const string keyStartDate = "StartDate";
		private const string keyOwner = "OwnerId";

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTracking", Assembly.GetExecutingAssembly());
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.TimeTracking.Resources.strTimeTrackingInfo", Assembly.GetExecutingAssembly());
		private DateTimeFormatInfo dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
		int index = -1;
		protected decimal ProjectRate = 0;

		#region block
		private TimeTrackingBlock _block = null;
		protected TimeTrackingBlock block
		{
			get
			{
				if (_block == null && blockTypeInstanceId > 0 && ownerId > 0 && startDate > DateTime.MinValue)
				{
					_block = TimeTrackingManager.GetTimeTrackingBlock(blockTypeInstanceId, startDate, ownerId);
				}
				return _block;
			}
			set
			{
				_block = value;

				if (_block != null)
				{
					blockTypeInstanceId = _block.BlockTypeInstanceId;
					startDate = _block.StartDate;
					ownerId = _block.OwnerId;
				}
			}
		}
		#endregion

		#region DataItem
		public override object DataItem
		{
			get
			{
				
				return block;
			}
			set
			{
				block = (TimeTrackingBlock)value;
			}
		}
		#endregion

		#region blockTypeInstanceId
		protected int blockTypeInstanceId
		{
			get
			{
				if (ViewState[keyBlockTypeInstance] != null)
					return (int)ViewState[keyBlockTypeInstance];
				else
					return -1;
			}
			set
			{
				ViewState[keyBlockTypeInstance] = value;
			}
		}
		#endregion

		#region ownerId
		protected int ownerId
		{
			get
			{
				if (ViewState[keyOwner] != null)
					return (int)ViewState[keyOwner];
				else
					return -1;
			}
			set
			{
				ViewState[keyOwner] = value;
			}
		}
		#endregion

		#region startDate
		protected DateTime startDate
		{
			get
			{
				if (ViewState[keyStartDate] != null)
					return (DateTime)ViewState[keyStartDate];
				else
					return DateTime.MinValue;
			}
			set
			{
				ViewState[keyStartDate] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (index < 0)
				index = PortalConfig.PortalFirstDayOfWeek;
		}

		#region DataBind
		public override void DataBind()
		{
			if (index < 0)
				index = PortalConfig.PortalFirstDayOfWeek;

			dgTimesheet.Columns[0].HeaderText = LocRM.GetString("Title");
			dgTimesheet.Columns[1].HeaderText = LocRM2.GetString("taskTime");
			dgTimesheet.Columns[2].HeaderText = LocRM2.GetString("Posted");
			dgTimesheet.Columns[3].HeaderText = LocRM2.GetString("InAWeek");
			dgTimesheet.Columns[4].HeaderText = LocRM.GetString("tTotalAppr");
			dgTimesheet.Columns[5].HeaderText = LocRM.GetString("tRate");
			dgTimesheet.Columns[6].HeaderText = LocRM.GetString("tCost");

			if (block != null)
			{
				if (block.ProjectId.HasValue)
					lblProject.Text = Mediachase.UI.Web.Util.CommonHelper.GetObjectTitle((int)Mediachase.IBN.Business.ObjectTypes.Project, block.ProjectId.Value);
				else
					lblProject.Text = block.Title;
				lblUser.Text = Mediachase.UI.Web.Util.CommonHelper.GetUserStatusPureName(block.OwnerId);

				DateTimeFormatInfo dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
				if (index < 0)
					index = PortalConfig.PortalFirstDayOfWeek;
				int WeekNumber = dtf.Calendar.GetWeekOfYear(block.StartDate, CalendarWeekRule.FirstDay, (System.DayOfWeek)(index));
				lblWeek.Text = WeekNumber.ToString() + " [ " + block.StartDate.ToShortDateString() + " - " + CHelper.GetWeekEndByDate(block.StartDate).ToShortDateString() + " ]";
			}

			BindButtons();
			BindDG();
		}
		#endregion

		#region DefineProjectRate
		private void DefineProjectRate()
		{
			if (block.ProjectId.HasValue)
			{
				int projectId = block.ProjectId.Value;
				ProjectRate = Mediachase.IBN.Business.Project.GetTeamMemberRate(projectId, block.OwnerId);
			}
		}
		#endregion

		#region BindButtons
		private void BindButtons()
		{
			btnUnregister.Visible = false;
			btnRegister.Visible = false;

			if (block != null)
			{
				SecurityService ss = block.GetService<SecurityService>();

				if ((bool)block.Properties["AreFinancesRegistered"].Value)
				{
					lblFinances.Text = GetGlobalResourceObject("IbnFramework.TimeTracking", "Registered").ToString();

					if (ss != null && ss.CheckUserRight(TimeTrackingManager.Right_UnRegFinances))
					{
						btnUnregister.Visible = true;
					}
				}
				else
				{
					lblFinances.Text = GetGlobalResourceObject("IbnFramework.TimeTracking", "NotRegistered").ToString();

					if (ss != null && ss.CheckUserRight(TimeTrackingManager.Right_RegFinances))
					{
						string url = ResolveClientUrl("~/Apps/TimeTracking/Pages/Public/RegisterFinances.aspx");

						btnRegister.Visible = true;
						btnRegister.OnClientClick = String.Format(CultureInfo.InvariantCulture,
							"ShowWizard(\"{0}?blockId={1}&btn={2}\", 300, 220);return false;",
							url, block.PrimaryKeyId, Page.ClientScript.GetPostBackEventReference(btnRegister, ""));
					}
				}
			}
		}
		#endregion

		#region BindDG
		private void BindDG()
		{
			if (blockTypeInstanceId > 0 && ownerId > 0 && startDate > DateTime.MinValue)
			{
				DefineProjectRate();

				/// TimeTrackingEntryId, UserId, ObjectTypeId, ObjectId, Title, 
				/// Day1, Day2, Day3, Day4, Day5, Day6, Day7, Total, TotalApproved, Rate, Cost, TaskTime
				dgTimesheet.DataSource = Mediachase.IBN.Business.TimeTracking.GetListTimeTrackingItemsForFinances_DataTable(blockTypeInstanceId, startDate, ownerId);
				dgTimesheet.DataBind();

				// Hide Edit button if finances are registered or there are no rights
				if ((bool)block.Properties["AreFinancesRegistered"].Value
					|| !Mediachase.Ibn.Data.Services.Security.CheckObjectRight(block, TimeTrackingManager.Right_RegFinances))
				{
					dgTimesheet.Columns[dgTimesheet.Columns.Count - 2].Visible = false;
				}
				else
				{
					dgTimesheet.Columns[dgTimesheet.Columns.Count - 2].Visible = true;
				}
			}
		}
		#endregion

		#region CheckVisibility
		public override bool CheckVisibility(object dataItem)
		{
			if (dataItem!=null && dataItem is BusinessObject)
			{
				bool isVisible = false;
				if (dataItem is TimeTrackingBlock)
				{
					TimeTrackingBlock blk = (TimeTrackingBlock)dataItem;

					// New or old (4.1) Finance schema
					bool areFinancesNew = true;
					if (blk.ProjectId.HasValue)
						areFinancesNew = ProjectSpreadSheet.IsActive(blk.ProjectId.Value);

					if (areFinancesNew)
					{
						SecurityService ss = ((TimeTrackingBlock)dataItem).GetService<SecurityService>();
						if (ss != null && ss.CheckUserRight(TimeTrackingManager.Right_ViewFinances))
							isVisible = true;
					}
				}
				return isVisible;
			}
			return base.CheckVisibility(dataItem);
		}
		#endregion

		#region GetTitle
		protected string GetTitle(bool IsGroup, int UserId, string Title)
		{
			if (IsGroup)
			{
				return Mediachase.UI.Web.Util.CommonHelper.GetUserStatus(UserId);
			}
			else
			{
				return "<span class='text' style='padding-left:25px'>" + Title + "</span>";
			}
		}
		#endregion

		#region GetTotalString
		protected string GetTotalString(object Total, object Day1, object Day2, object Day3, object Day4, object Day5, object Day6, object Day7)
		{
			string sToolTip = "";
			string[] hrs = new string[7];
			hrs[0] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day1);
			hrs[1] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day2);
			hrs[2] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day3);
			hrs[3] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day4);
			hrs[4] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day5);
			hrs[5] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day6);
			hrs[6] = Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Day7);
			string separator = "\r\n";
			if ((Request.UserAgent.IndexOf("MSIE") < 0) && (Request.UserAgent.IndexOf("Safari") < 0))
				separator = " ";

			for (int i = 0; i < 7; i++)
			{
				sToolTip += dtf.GetAbbreviatedDayName((DayOfWeek)index) + ":&nbsp;" + hrs[i] + separator;
				index++;
				if (index == 7) index = 0;
			}
			sToolTip = sToolTip.Substring(0, sToolTip.Length - separator.Length);
			string imageUrl = ResolveClientUrl("~/layouts/images/info.gif");
			return String.Format(CultureInfo.InvariantCulture,
				"<table width='100%' cellpadding='0' cellspacing='0'><tr><td width='15px'><img align='absmiddle' style='cursor:pointer' title='{0}' src='{1}' width='13px' border='0'></td><td class='text' align='right'>{2}</td></tr></table>",
				sToolTip, 
				imageUrl,
				Mediachase.UI.Web.Util.CommonHelper.GetHours((int)(double)Total));
		}
		#endregion

		#region GetPostedTime
		protected string GetPostedTime(object obj, object objectType, int timeTrackingEntryId)
		{
			string retval = "";
			if (obj != null && obj != DBNull.Value && objectType != null && objectType != DBNull.Value)
			{
				int objectId = (int)obj;
				int objectTypeId = (int)objectType;

				if (objectTypeId == (int)ObjectTypes.CalendarEntry
					|| objectTypeId == (int)ObjectTypes.Document
					|| objectTypeId == (int)ObjectTypes.Issue
					|| objectTypeId == (int)ObjectTypes.Task
					|| objectTypeId == (int)ObjectTypes.ToDo)
				{
					int minutes = TimeTrackingManager.GetPostedTimeByObject((int)objectId, (int)objectTypeId);
					retval = Mediachase.UI.Web.Util.CommonHelper.GetHours(minutes);
				}
			}
			else if ((bool)block.Properties["AreFinancesRegistered"].Value)
			{
				TimeTrackingEntry entry = MetaObjectActivator.CreateInstance<TimeTrackingEntry>(TimeTrackingEntry.GetAssignedMetaClass(), timeTrackingEntryId);
				int minutes = 0;
				if (entry.Properties["TotalApproved"].Value != null && entry.Properties["TotalApproved"].Value != DBNull.Value)
					minutes = Convert.ToInt32(entry.Properties["TotalApproved"].Value);
				retval = Mediachase.UI.Web.Util.CommonHelper.GetHours(minutes);
			}

			return retval;
		}
		#endregion

		#region GetDateTimeFromMinutes
		protected DateTime GetDateTimeFromMinutes(int Minutes)
		{
			return DateTime.MinValue.AddMinutes(Minutes);
		}
		#endregion

		#region DataGrid Events
		protected void dgTimesheet_EditCommand(object source, DataGridCommandEventArgs e)
		{
			dgTimesheet.EditItemIndex = e.Item.ItemIndex;
			BindDG();	
		}

		protected void dgTimesheet_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgTimesheet.EditItemIndex = -1;
			BindDG();	
		}

		protected void dgTimesheet_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			int entryId = int.Parse(e.Item.Cells[e.Item.Cells.Count - 1].Text);
			Mediachase.UI.Web.Modules.TimeControl dtc = (Mediachase.UI.Web.Modules.TimeControl)e.Item.FindControl("dtc");
			string hours = String.Format("{0}:{1:mm}", dtc.Value.Hour + 24 * (dtc.Value.Day - 1), dtc.Value);
			TextBox txtRate = (TextBox)e.Item.FindControl("txtRate");

			TimeTrackingManager.UpdateEntry(entryId, Mediachase.UI.Web.Util.CommonHelper.GetMinutes(hours), decimal.Parse(txtRate.Text));

			dgTimesheet.EditItemIndex = -1;
			BindDG();
		}
		#endregion

		#region Register / Unregister
		protected void btnRegister_Click(object sender, EventArgs e)
		{
			BindButtons();
			BindDG();

			// Refresh Grid
			CommandParameters cp = new CommandParameters("MC_TimeTracking_InfoFrame");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, cp.ToString());
		}

		protected void btnUnregister_Click(object sender, EventArgs e)
		{
			if (block != null)
			{
				Mediachase.IBN.Business.TimeTracking.UnRegisterFinances(block);
				BindButtons();
				BindDG();

				// Refresh Grid
				CommandParameters cp = new CommandParameters("MC_TimeTracking_InfoFrame");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterRefreshParentFromFrameScript(this.Page, cp.ToString());
			}
		}
		#endregion

	}
}