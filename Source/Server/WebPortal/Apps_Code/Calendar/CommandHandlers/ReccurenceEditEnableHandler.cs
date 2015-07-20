using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Events;
using Mediachase.Ibn.Core.Business;

namespace Mediachase.Ibn.Web.UI.Calendar.CommandHandlers
{
	public class ReccurenceEditEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string uid = cp.CommandArguments["primaryKeyId"];
				string className = cp.CommandArguments["className"];
				PrimaryKeyId pKey = PrimaryKeyId.Parse(uid);
				EntityObject eo = BusinessManager.Load(className, pKey);
				if (eo != null)
				{
					CalendarEventEntity ceo = eo as CalendarEventEntity;
					if (ceo.CalendarEventExceptionId.HasValue)
						return false;
				}
				return ((VirtualEventId)pKey).IsRecurrence;
			}
			return false;
		}

		#endregion
	}
}
