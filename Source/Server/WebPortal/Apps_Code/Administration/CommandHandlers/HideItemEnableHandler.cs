using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Administration.CommandHandlers
{
	public class HideItemEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				if (!cp.CommandArguments.ContainsKey("hidden"))
					throw new ArgumentException("hidden is null for HideItemEnableHandler");
				if (!cp.CommandArguments.ContainsKey("hiddenParent"))
					throw new ArgumentException("hiddenParent is null for HideItemEnableHandler");

				bool hidden = bool.Parse(cp.CommandArguments["hidden"]);
				bool hiddenParent = bool.Parse(cp.CommandArguments["hiddenParent"]);

				retval = !hidden && !hiddenParent;
			}
			return retval;
		}
		#endregion
	}
}
