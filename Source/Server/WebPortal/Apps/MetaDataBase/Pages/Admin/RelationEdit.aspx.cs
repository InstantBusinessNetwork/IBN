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

namespace Mediachase.Ibn.Web.UI.MetaDataBase.Pages.Admin
{
	public partial class RelationEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["mode"] == "1N")
			{
				if (Request.QueryString["reffield"] != null)
					pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EditRelation1N").ToString();
				else
					pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewRelation1N").ToString();
			}
			else
			{
				if (Request.QueryString["field"] != null)
					pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "EditRelationN1").ToString();
				else
					pT.Title = GetGlobalResourceObject("IbnFramework.GlobalMetaInfo", "NewRelationN1").ToString();
			}
		}
	}
}
