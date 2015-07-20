using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class OutActivities : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			secHeader.Title = GetGlobalResourceObject("IbnFramework.Global", "_mc_OutActivities").ToString();
		}
	}
}