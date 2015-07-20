using System;
using System.Collections.Generic;
using System.Web;

using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.BusinessProcess.CommandHandlers
{
	public class WFInstanceSecurityEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;

			if (HttpContext.Current.Items.Contains("OwnerTypeId") && HttpContext.Current.Items.Contains("OwnerId"))
			{
				int ownerTypeId = (int)HttpContext.Current.Items["OwnerTypeId"];
				int ownerId = (int)HttpContext.Current.Items["OwnerId"];

				if (ownerTypeId == (int)ObjectTypes.Document)
					retval = Document.CanUpdate(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.Task)
					retval = Task.CanUpdate(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.ToDo)
					retval = ToDo.CanUpdate(ownerId);
				else if (ownerTypeId == (int)ObjectTypes.Issue)
					retval = Incident.CanUpdate(ownerId);
			}

			return retval;
		}
		#endregion
	}
}
