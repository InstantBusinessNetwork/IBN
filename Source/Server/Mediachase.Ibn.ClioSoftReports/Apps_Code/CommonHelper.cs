using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Apps.ClioSoft
{
	public class CommonHelper
	{
		#region GetWeekStartByDate
		public static DateTime GetWeekStartByDate(DateTime start)
		{
			start = start.Date;
			int dow = (int)start.DayOfWeek;
			int fdow = PortalConfig.PortalFirstDayOfWeek;
			fdow = (fdow == 7) ? 0 : fdow;

			int diff = dow - fdow;
			DateTime result;
			if (diff < 0)
				result = start.AddDays(-(7 + diff));
			else
				result = start.AddDays(-diff);

			if (result.Year < start.Year)
				result = new DateTime(start.Year, 1, 1);

			return result;
		}
		#endregion

		#region SafeSelect
		public static void SafeSelect(ListControl ddl, string val)
		{
			ListItem li = ddl.Items.FindByValue(val);
			if (li != null)
			{
				ddl.ClearSelection();
				li.Selected = true;
			}
		}
		#endregion

		#region GetHours
		public static string GetHours(int iTotalMinutes)
		{
			int iMinutes = 0;
			int iHours = Math.DivRem(iTotalMinutes, 60, out iMinutes);
			return String.Format("{0}:{1}{2}", iHours, (iMinutes < 10) ? "0" : "", iMinutes);
		}
		#endregion
	}
}
