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

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class CloneEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			object obj = CHelper.GetFromContext("SelectedWeek");
			if (obj != null)
			{
				DateTime selectedWeek = (DateTime)obj;

				if (selectedWeek != CHelper.GetRealWeekStartByDate(DateTime.UtcNow))
					retval = true;
			}
			return retval;
		}
		#endregion
	}
}
