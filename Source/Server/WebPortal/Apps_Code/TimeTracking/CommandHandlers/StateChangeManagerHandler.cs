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
using Mediachase.UI.Web.Apps.MetaUI.Grid;
using Mediachase.Ibn.Data;
using Mediachase.IbnNext.TimeTracking;
using Mediachase.Ibn.Data.Meta;
using Mediachase.Ibn.Data.Services;
using Mediachase.Ibn.Core;

namespace Mediachase.UI.Web.Apps_Code.TimeTracking.CommandHandlers
{
	public class StateChangeManagerHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (!cp.CommandArguments.ContainsKey("parentBlockId"))
				{
					return false;
				}

				MetaObject ttb = MetaObjectActivator.CreateInstance<BusinessObject>(MetaDataWrapper.ResolveMetaClassByNameOrCardName("TimeTrackingBlock"), Convert.ToInt32(cp.CommandArguments["parentBlockId"]));
				StateMachineService sms = ((BusinessObject)ttb).GetService<StateMachineService>();
				StateTransition[] transitions = sms.GetNextAvailableTransitions(true);

				bool retval = true;
				if (transitions.Length == 0)
				{
					transitions = sms.GetPrevAvailableTransitions(true);
					retval = transitions.Length > 0;
				}
				return retval;
			}

			return false;	
		}

		#endregion
	}
}
