using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Resources;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;
using System.Reflection;

namespace Mediachase.UI.Web.Admin.Modules
{
	public partial class SmtpListSettings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
				BindSmtpSettings();

			UpdateLogSettingsButton.Text = LocRM.GetString("tSave");
			CancelButton.Text = LocRM.GetString("tCancel");
			UpdateLogSettingsButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			CancelButton.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
		}

		#region BindSmtpSettings
		private void BindSmtpSettings()
		{
			tbLogPeriod.Text = PortalConfig.SmtpRequestTimeout.ToString();
		}
		#endregion

		#region UpdateLogSettingsButton_ServerClick
		protected void UpdateLogSettingsButton_ServerClick(object sender, EventArgs e)
		{
			try
			{
				if (int.Parse(tbLogPeriod.Text) > 0)
					PortalConfig.SmtpRequestTimeout = int.Parse(tbLogPeriod.Text);
			}
			catch
			{
				PortalConfig.SmtpRequestTimeout = 30;
			}
			CommonHelper.CloseIt(Response);
		}
		#endregion
	}
}