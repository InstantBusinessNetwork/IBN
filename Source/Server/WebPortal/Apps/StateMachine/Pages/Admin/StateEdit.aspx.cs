using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.StateMachine.Pages.Admin
{
	public partial class StateEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["StateId"] != null)
				pT.Title = GetGlobalResourceObject("IbnFramework.Security", "EditState").ToString();
			else
				pT.Title = GetGlobalResourceObject("IbnFramework.Security", "NewState").ToString();	
		}
	}
}
