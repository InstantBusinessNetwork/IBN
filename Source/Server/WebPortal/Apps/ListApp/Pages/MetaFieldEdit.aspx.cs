using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Pages
{
	public partial class MetaFieldEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			pT.SetControlProperties("ReturnUrl", CHelper.ListAdminPage);
			pT.SetControlProperties("ClassLabelText", GetGlobalResourceObject("IbnFramework.ListInfo", "List").ToString());
			pT.SetControlProperties("Place", "ListInfo");
			pT.SetControlProperties("AutogenerateSystemNames", true);

			if (Request.QueryString["field"] != null)
				pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EditField").ToString();
			else
				pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewField").ToString();
		}
	}
}
