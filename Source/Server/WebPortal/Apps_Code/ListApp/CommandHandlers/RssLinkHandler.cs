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
using Mediachase.IBN.Business.Rss;
using Mediachase.IBN.Business;
using Mediachase.Ibn.Web.UI.MetaUIEntity.Modules;

namespace Mediachase.Ibn.Web.UI.ListApp.CommandHandlers
{
	public class RssLinkHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object sender, object element)
		{
			if (element is CommandParameters)
			{
				NameValueCollection qs = ((Control)sender).Page.Request.QueryString;
				string className = qs["ClassName"];
				//ListInfo li = ListManager.GetListInfoByMetaClassName(className);
				UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
				ClientScript.RegisterStartupScript(((Control)sender).Page, ((Control)sender).Page.GetType(), Guid.NewGuid().ToString("N"),
					string.Format("OpenPopUpWindow('{0}', 640, 480);", RssGenerator.CreateRssLink(((Control)sender).Page, className, null, EntityList.GetProfileName(pc, className))), true);
				//((Control)sender).Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString("N"),
				//    string.Format("OpenPopUpWindow('{0}', 640, 480);", RssGenerator.CreateRssLink(((Control)sender).Page, Guid.NewGuid(), className, null, EntityList.GetProfileName(pc, className))), true);
				//((Control)sender).Page.Response.Redirect(RssGenerator.CreateRssLink(((Control)sender).Page, Guid.NewGuid(), className, null, EntityList.GetProfileName(pc, className)));
			}
		}

		#endregion

	}
}
