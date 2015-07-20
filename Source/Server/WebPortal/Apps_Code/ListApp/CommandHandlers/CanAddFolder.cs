using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.Ibn.Web.UI.WebControls;
using System.Collections.Specialized;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class CanAddFolder : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retval = true;
			object id = CHelper.GetFromContext("ListFolderId");
			if (id != null)
			{
				int iid = -1;
				int.TryParse(id.ToString(), out iid);
				if (iid == -1)
					retval = false;
			}
			return retval;
		}

		#endregion
	}
}
