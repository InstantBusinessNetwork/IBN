using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.Web.UI.StateMachine.Pages.Admin
{
	public partial class StateMachineEdit : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["SMId"] != null)
				pT.Title = GetGlobalResourceObject("IbnFramework.Security", "EditStateMachine").ToString();
			else
				pT.Title = GetGlobalResourceObject("IbnFramework.Security", "NewStateMachine").ToString();	
		}
	}
}
