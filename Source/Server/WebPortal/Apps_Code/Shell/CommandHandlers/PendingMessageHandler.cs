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

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class PendingMessageHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			if ((Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Mediachase.IBN.Business.Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager)) &&
				 PortalConfig.UseAntiSpamFilter )
			{
				retval = true;
			}

			return retval;
		}
		#endregion
	}
}
