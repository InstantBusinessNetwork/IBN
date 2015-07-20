using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.Ibn.Web.UI.WebControls;
using System.Web.UI;
using Mediachase.IBN.Business;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.CommandHandlers
{
	public class ManagementEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			if (((Control)Sender).Page.Request["ProjectId"] != null)
			{
				int projectId = int.Parse(((Control)Sender).Page.Request["ProjectId"]);
				retval = Project.CanViewManagement(projectId);
			}

			return retval;
		}

		#endregion
	}
}
