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
using System.Collections.Specialized;
using Mediachase.Ibn.Lists;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class ListFolderRedirect : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
				string className = qs["ClassName"];
				ListInfo li = ListManager.GetListInfoByMetaClassName(className);
				if (li != null)
				{
					int folderId = li.FolderId.Value;
					((Control)Sender).Page.Response.Redirect("~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + folderId.ToString());
				}
			}
		}

		#endregion
	}
}
