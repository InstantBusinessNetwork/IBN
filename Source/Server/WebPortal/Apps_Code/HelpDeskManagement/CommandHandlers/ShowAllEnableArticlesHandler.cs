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

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.CommandHandlers
{
	public class ShowAllEnableArticlesHandler :ICommandEnableHandler
	{
		#region ICommandEnableHandler Members

		public bool IsEnable(object Sender, object Element)
		{
			bool retVal = false;

			UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;
			if (pc["ArticleListMain_Mode"] == null)
				pc["ArticleListMain_Mode"] = Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.ArticleListMain.MODE_SEARCH;
			if (pc["ArticleListMain_Search"] == null)
				pc["ArticleListMain_Search"] = string.Empty;

			switch (pc["ArticleListMain_Mode"])
			{
				case Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.ArticleListMain.MODE_SEARCH:
					if (!string.IsNullOrEmpty(pc["ArticleListMain_Search"]))
						retVal = true;
					break;
				case Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.ArticleListMain.MODE_TAGS:
					retVal = true;
					break;
			}

			return retVal;
		}

		#endregion
	}
}
