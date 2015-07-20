using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Lists;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class CanWriteHandler : ICommandEnableHandler
	{
		#region ICommandEnableHandler Members
		public bool IsEnable(object Sender, object Element)
		{
			bool retval = false;
			NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
			if (!String.IsNullOrEmpty(qs["ViewName"]))
			{
				if (ListManager.MetaClassIsList(qs["ViewName"]))
				{
					int listId = ListManager.GetListIdByClassName(qs["ViewName"]);
					return ListInfoBus.CanWrite(listId);
				}
			}
			if (!String.IsNullOrEmpty(qs["ClassName"]))
			{
				if (ListManager.MetaClassIsList(qs["ClassName"]))
				{
					int listId = ListManager.GetListIdByClassName(qs["ClassName"]);
					return ListInfoBus.CanWrite(listId);
				}
			}
			object className = CHelper.GetFromContext("Reference1N_ClassName");
			if (className != null && ListManager.MetaClassIsList(className.ToString()))
			{
				int listId = ListManager.GetListIdByClassName(className.ToString());
				return ListInfoBus.CanWrite(listId);
			}
			className = CHelper.GetFromContext("ReferenceNN_ClassName");
			if (className != null && ListManager.MetaClassIsList(className.ToString()))
			{
				int listId = ListManager.GetListIdByClassName(className.ToString());
				return ListInfoBus.CanWrite(listId);
			}
			return retval;
		}
		#endregion
	}
}
