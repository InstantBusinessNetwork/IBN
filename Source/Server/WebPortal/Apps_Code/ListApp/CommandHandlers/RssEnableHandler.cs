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
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class RssEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object sender, object element)
		{
			return PortalConfig.IsListRssEnabled;
		}

		#endregion
	}
}
