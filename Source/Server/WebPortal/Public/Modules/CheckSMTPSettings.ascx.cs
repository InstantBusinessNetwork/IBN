using System;
using System.Resources;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.EMail;
using System.Reflection;

namespace Mediachase.UI.Web.Public.Modules
{
	public partial class CheckSMTPSettings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, EventArgs e)
		{
			string uid = Request["uid"];

			if (string.IsNullOrEmpty(uid))
				Response.Redirect("~/Public/default.aspx", true);
			else
			{
				if (Security.CurrentUser != null && Security.IsUserInGroup(InternalSecureGroups.Administrator))
				{
					divUserCheckedTrue.Visible = false;
					divUserCheckedFalse.Visible = false;
				}
				else
				{
					divAdminCheckedTrue.Visible = false;
					divAdminCheckedFalse.Visible = false;
				}

				if (SmtpBox.CommitTestEmail(new Guid(uid)))
				{
					labelCheckedFalse.Visible = false;
					divAdminCheckedFalse.Visible = false;
					divUserCheckedFalse.Visible = false;
				}
				else
				{
					labelCheckedTrue.Visible = false;
					divAdminCheckedTrue.Visible = false;
					divUserCheckedTrue.Visible = false;
				}
			}
		}
	}
}
