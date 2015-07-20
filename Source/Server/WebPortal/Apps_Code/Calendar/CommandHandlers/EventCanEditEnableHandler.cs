using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Calendar.CommandHandlers
{
	public class EventCanEditEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			//bool retval = false;
			//if (Element is CommandParameters)
			//{
			//    CommandParameters cp = (CommandParameters)Element;

			//    if (!cp.CommandArguments.ContainsKey("added"))
			//        throw new ArgumentException("added is null for DeleteLeftMenuItemEnableHandler");

			//    bool added = bool.Parse(cp.CommandArguments["added"]);

			//    retval = added;
			//}
			//return retval;

			return true;
		}

		#endregion
	}
}
