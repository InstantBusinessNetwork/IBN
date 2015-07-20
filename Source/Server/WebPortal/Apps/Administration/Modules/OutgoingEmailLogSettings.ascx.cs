using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.Ibn.Web.UI.Administration.Modules
{
	public partial class OutgoingEmailLogSettings : System.Web.UI.UserControl
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			btnSave.Text = GetGlobalResourceObject("IbnFramework.Common", "tSave").ToString();
			btnCancel.Text = GetGlobalResourceObject("IbnFramework.Common", "tClose").ToString();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.CustomImage = this.Page.ResolveUrl("~/layouts/images/cancel.gif");
			btnCancel.Attributes.Add("onclick", CommandHandler.GetCloseOpenedFrameScript(this.Page, String.Empty, false, true));
			btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			if (!Page.IsPostBack)
			{
				txtDeliveryTimeout.Text = (Mediachase.IBN.Business.PortalConfig.MdsDeliveryTimeout / (60 * 24)).ToString();
				txtAttempts.Text = Mediachase.IBN.Business.PortalConfig.MdsMaxDeliveryAttempts.ToString();
				txtLogPeriod.Text = (Mediachase.IBN.Business.PortalConfig.MdsDeleteOlderMoreThan / (60 * 24)).ToString();
			}
		}

		void btnSave_ServerClick(object sender, EventArgs e)
		{
			Mediachase.IBN.Business.PortalConfig.MdsDeliveryTimeout = int.Parse(txtDeliveryTimeout.Text) * 24 * 60;
			Mediachase.IBN.Business.PortalConfig.MdsMaxDeliveryAttempts = int.Parse(txtAttempts.Text);
			Mediachase.IBN.Business.PortalConfig.MdsDeleteOlderMoreThan = int.Parse(txtLogPeriod.Text) * 24 * 60;

			CommandParameters cp = new CommandParameters("MC_MUI_ChangeSettings");
			CommandHandler.RegisterCloseOpenedFrameScript(this.Page, cp.ToString());
		}
	}
}