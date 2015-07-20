using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class CheckHistoryHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
			if (!String.IsNullOrEmpty(qs["ViewName"]))
			{
				retval = ListManager.IsHistoryActivated(qs["ViewName"]);
			}
			else if (!String.IsNullOrEmpty(qs["ClassName"]))
			{
				retval = ListManager.IsHistoryActivated(qs["ClassName"]);
			}
			return retval;
		}
		#endregion
	}
}
