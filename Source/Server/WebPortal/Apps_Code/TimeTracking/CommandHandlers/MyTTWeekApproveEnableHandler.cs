using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;
using System.Globalization;

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class MyTTWeekApproveEnableHandler :ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				DateTime startDate = DateTime.Parse(cp.CommandArguments["primaryKeyId"], CultureInfo.InvariantCulture);
				WeekItemInfo[] mas = Mediachase.IBN.Business.TimeTracking.GetWeekItemsForCurrentUser(startDate, startDate.AddDays(6));
				if (mas.Length > 0)
				{
					WeekItemInfo wii = mas[0];
					if(wii.Status == WeekItemStatus.InProcess || wii.Status == WeekItemStatus.Rejected)
						return true;
				}
			}
			return retval;
		}

		#endregion
	}
}
