using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.HelpDeskManagement.Modules
{
	public partial class MarkAsSpam : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
				BindValues();
			btnMark.Click += new EventHandler(btnMark_Click);
		}

		private void BindValues()
		{
			btnMark.Text = GetGlobalResourceObject("IbnFramework.Incident", "MarkAsSpam").ToString();
			rbAction.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "CloseIssue").ToString(), "close"));
			rbAction.Items.Add(new ListItem(GetGlobalResourceObject("IbnFramework.Incident", "DeleteIssue2").ToString(), "delete"));
			rbAction.SelectedIndex = 0;
		}

		void btnMark_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(Request["ReturnCommand"]);
			cp.CommandArguments = new Dictionary<string, string>();
			if(Request["GridId"] != null)
				cp.AddCommandArgument("GridId", Request["GridId"]);
			cp.AddCommandArgument("action", rbAction.SelectedValue);
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}