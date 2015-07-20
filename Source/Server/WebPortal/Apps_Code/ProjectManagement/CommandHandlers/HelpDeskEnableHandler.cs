using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class HelpDeskEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			return Configuration.HelpDeskEnabled;
		}

		#endregion
	}
}
