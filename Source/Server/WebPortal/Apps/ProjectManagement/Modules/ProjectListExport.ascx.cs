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
using Mediachase.Ibn.Web.UI.WebControls;
using System.Collections.Generic;

namespace Mediachase.Ibn.Web.UI.ProjectManagement.Modules
{
	public partial class ProjectListExport : System.Web.UI.UserControl
	{
		#region _type
		private string _type
		{
			get
			{
				if (Request["Type"] != null)
					return Request["Type"];
				else
					return "1";
			}
		}
		#endregion

		#region _refreshCommand
		private string _refreshCommand
		{
			get
			{
				if (Request["refreshCommand"] != null)
					return Request["refreshCommand"];
				else
					return String.Empty;
			}
		}
		#endregion

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindValues();
				CHelper.SafeSelect(rbList, "0");
			}
		}

		private void BindValues()
		{
			rbList.Items.Clear();
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Project", "AllProjectsExport").ToString(), "0"));
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Project", "ViewProjectsExport").ToString(), "1"));
			rbList.Items.Add(new ListItem("  " + GetGlobalResourceObject("IbnFramework.Project", "VisibleProjectsExport").ToString(), "2"));

			btnExport.Text = GetGlobalResourceObject("IbnFramework.Incident", "Export").ToString();
		}

		protected void btnExport_Click(object sender, EventArgs e)
		{
			CommandParameters cp = new CommandParameters(_refreshCommand);
			cp.CommandArguments = new Dictionary<string, string>();
			cp.AddCommandArgument("Type", _type);
			cp.AddCommandArgument("Variant", rbList.SelectedValue);
			Mediachase.Ibn.Web.UI.WebControls.CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}