using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Pages.Admin
{
	public partial class MetaBridgeEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["class"] != null)
				pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BridgeEdit").ToString();
			else
				pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "BridgeCreate").ToString();
		}
	}
}
