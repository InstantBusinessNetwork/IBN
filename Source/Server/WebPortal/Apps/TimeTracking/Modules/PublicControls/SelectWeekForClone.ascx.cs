using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.UI.TimeTracking.Modules.PublicControls
{
	public partial class SelectWeekForClone : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			DTCWeek.SelectedDate = UserDateTime.UserNow;

			DateTime startDate = DateTime.Parse(Request["uid"], CultureInfo.InvariantCulture);
			lblComment.Text = String.Format(GetGlobalResourceObject("IbnFramework.TimeTracking", "CloneComments").ToString(), 
				String.Format("<b>#{0}&nbsp;&nbsp;{1} - {2}</b>", Iso8601WeekNumber.GetWeekNumber(startDate).ToString(),
					startDate.ToShortDateString(), 
					startDate.AddDays(6).ToShortDateString()));
			if (Request["closeFramePopup"] != null)
			{
				CloseButton.OnClientClick = String.Format(CultureInfo.InvariantCulture, "javascript:try{{window.parent.{0}();}}catch(ex){{}}", Request["closeFramePopup"]);
			}
		}

		#region AddButton_Click
		protected void AddButton_Click(object sender, EventArgs e)
		{
			DateTime startDate = DateTime.Parse(Request["uid"], CultureInfo.InvariantCulture);
			DateTime destStartDate = CHelper.GetWeekStartByDate(DTCWeek.SelectedDate);

			if (startDate.Date != destStartDate.Date)
			{
				TimeTrackingEntry[] mas = TimeTrackingEntry.List(FilterElement.EqualElement("OwnerId", Mediachase.IBN.Business.Security.CurrentUser.UserID), FilterElement.EqualElement("StartDate", startDate));
				List<int> entryIds = new List<int>();
				foreach (TimeTrackingEntry tte in mas)
					entryIds.Add((int)tte.PrimaryKeyId.Value);
				if (entryIds.Count > 0)
					TimeTrackingManager.AddEntries(destStartDate, Mediachase.IBN.Business.Security.CurrentUser.UserID, entryIds);
			}

			
			// Refresh parent window
			CommandParameters cp = new CommandParameters("MC_TT_CloneWeek");
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
		#endregion
	}
}