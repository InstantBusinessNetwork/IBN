using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class UpdatesEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			DateTime startDate = User.GetLocalDate(DateTime.UtcNow).Date.AddDays(-6);
			DateTime endDate = DateTime.Today.AddDays(1);
			return SystemEvents.CheckSystemEvents(startDate, endDate);
		}
		#endregion
	}
}
