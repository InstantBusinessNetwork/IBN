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
	public class ResetArticlesHandler : ICommand
	{
		#region ICommand Members

		public void Invoke(object Sender, object Element)
		{
			UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

			pc["ArticleListMain_Mode"] = Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules.ArticleListMain.MODE_SEARCH;
			pc["ArticleListMain_Tag"] = "";
			pc["ArticleListMain_Search"] = "";

			CHelper.RequireBindGrid();
		}

		#endregion
	}
}
