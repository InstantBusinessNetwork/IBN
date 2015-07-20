using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.ListApp.Pages
{
	public partial class ListFolderEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if(Request["FolderId"] != null)
				pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListFolderEdit}");
			else
				pT.Title = CHelper.GetResFileString("{IbnFramework.ListInfo:ListFolderCreate}");
		}
	}
}
