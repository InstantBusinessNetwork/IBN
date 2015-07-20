using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.Ibn.Assignments;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Assignments.Schemas;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class WFTemplatesEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			// O.R. [2009-07-28]: Check license and NET Framework version
			// O.R. [2009-10-28]: Check SchemaManager count
			SchemaMaster[] list = SchemaManager.GetAvailableShemaMasters();
			if (Configuration.WorkflowModule && WorkflowActivityWrapper.IsFramework35Installed() && list != null && list.Length > 0)
				retval = true;

			return retval;
		}
		#endregion
	}
}
