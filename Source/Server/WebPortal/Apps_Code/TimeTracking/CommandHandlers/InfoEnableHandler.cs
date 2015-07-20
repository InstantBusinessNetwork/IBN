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

namespace Mediachase.Ibn.Web.UI.TimeTracking.CommandHandlers
{
	public class InfoEnableHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;

				return cp.CommandArguments.ContainsKey("primaryKeyId");
			}

			return false;
		}
		#endregion
	}
}
