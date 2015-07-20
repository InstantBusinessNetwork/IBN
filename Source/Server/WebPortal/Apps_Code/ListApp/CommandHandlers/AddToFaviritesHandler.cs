using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn.Web.UI.Controls.Util;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class AddToFaviritesHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
			if (!String.IsNullOrEmpty(qs["ViewName"]))
			{
				int listId = ListManager.GetListIdByClassName(qs["ViewName"]);
				ListInfoBus.AddFavorites(listId);
			}
			else if (!String.IsNullOrEmpty(qs["ClassName"]))
			{
				int listId = ListManager.GetListIdByClassName(qs["ClassName"]);
				ListInfoBus.AddFavorites(listId);
			}
		}
		#endregion
	}
}
