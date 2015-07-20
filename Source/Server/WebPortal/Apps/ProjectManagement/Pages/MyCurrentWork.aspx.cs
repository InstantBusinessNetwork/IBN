using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Pages
{
	public partial class MyCurrentWork : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			pT.Title = GetGlobalResourceObject("IbnFramework.Common", "tMyWorks").ToString();
		}
	}
}
