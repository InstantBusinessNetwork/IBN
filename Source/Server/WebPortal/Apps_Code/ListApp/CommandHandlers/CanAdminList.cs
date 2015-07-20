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
using Mediachase.IBN.Business;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class CanAdminList : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			if (Element is CommandParameters)
			{
				CommandParameters cp = (CommandParameters)Element;
				string sid = cp.CommandArguments["primaryKeyId"];

				string[] elem = sid.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				int id = -1;
				int.TryParse(elem[0], out id);
				retval = ListInfoBus.CanAdmin(id);
			}
			return retval;
		}

		#endregion
	}
}
