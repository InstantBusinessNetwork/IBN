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
using System.Globalization;

namespace Mediachase.UI.Web.Apps_Code.TimeTracking.CommandHandlers
{
	public class SendToApproveGridHandler : ICommandEnableHandler
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

				TimeTrackingBlock ttb = MetaObjectActivator.CreateInstance<TimeTrackingBlock>(MetaDataWrapper.ResolveMetaClassByNameOrCardName("TimeTrackingBlock"), Convert.ToInt32(cp.CommandArguments["parentBlockId"].ToString(), CultureInfo.InvariantCulture));
				StateMachineService sms = ttb.GetService<StateMachineService>();
				StateTransition[] transitions = sms.GetNextAvailableTransitions(true);
				
				bool isInitialState = sms.StateMachine.GetStateIndex(sms.CurrentState.Name) == 0;

				return (transitions.Length > 0 && isInitialState);
			}

			return false;
		}

		#endregion
	}
}
