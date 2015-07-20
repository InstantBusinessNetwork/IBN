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

namespace Mediachase.Ibn.Web.UI.Shell.CommandHandlers
{
	public class PowerExecutiveHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			if (Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.PowerProjectManager) ||
				Mediachase.IBN.Business.Security.IsUserInGroup(Mediachase.IBN.Business.InternalSecureGroups.ExecutiveManager))
			{
				retval = true;
			}

			return retval;
		}

		#endregion
	}
}
