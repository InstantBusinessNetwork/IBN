using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Pages.Admin
{
	public partial class MetaClassEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request["class"] != null)
				pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EditTable").ToString();
			else
				pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "CreateTable").ToString();
		}		
	}
}
