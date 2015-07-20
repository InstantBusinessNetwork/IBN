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
using Mediachase.Ibn.Lists;
using Mediachase.Ibn.Data.Meta.Management;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class DeleteListHandler : ICommand
	{
		#region ICommand Members
		public void Invoke(object Sender, object Element)
		{
			if (Element is CommandParameters)
			{
				NameValueCollection qs = ((Control)Sender).Page.Request.QueryString;
				string className = qs["class"];
				if(String.IsNullOrEmpty(className))
					className = qs["ClassName"];
				ListInfo li = ListManager.GetListInfoByMetaClassName(className);
				int folderId = li.FolderId.Value;
				try
				{
					li.Delete();
					((Control)Sender).Page.Response.Redirect("~/Apps/ListApp/Pages/ListInfoList.aspx?ListFolderId=" + folderId.ToString());
				}
				catch (MetaFieldReferencedException ex)
				{
					ClientScript.RegisterStartupScript(((Control)Sender).Page, ((Control)Sender).Page.GetType(), Guid.NewGuid().ToString("N"),
						String.Format("alert('{0}');", String.Format(CHelper.GetResFileString("{IbnFramework.ListInfo:RefException}"), CHelper.GetResFileString(ListManager.GetListInfoByMetaClassName(ex.MetaClassName).Title))), true);
				}
			}
		}
		#endregion
	}
}
