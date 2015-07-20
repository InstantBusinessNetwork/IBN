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
	public class DeleteTemplateListHandler : ICommand
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
				ListManager.DeleteList(li);
				((Control)Sender).Page.Response.Redirect("~/Apps/ListApp/Pages/ListTemplates.aspx");
			}
		}
		#endregion
	}
}
