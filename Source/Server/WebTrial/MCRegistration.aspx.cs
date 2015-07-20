using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Mediachase.Ibn.WebTrial
{
	public partial class MCRegistration : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebTrial.App_GlobalResources.Resources.Resources", typeof(defaultpage).Assembly);

		private string GetReseller()
		{
			string retVal = string.Empty;

			retVal = CManage.GetReseller(Request);

			if (retVal == Settings.UnknownResellerGuid)
			{
				retVal = Settings.EnResellerGuid;
			}

			return retVal;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			lblErrorDomainMessage.Text = "";
			lblParentDomain.Text = TrialHelper.GetParentDomain();
		}

		public void Register_Click(object sender, EventArgs e)
		{
			if (!Page.IsValid || portalName2.Text.Length > 0)
				return;

			localhost.TrialResult tr = localhost.TrialResult.Failed;
			string message = String.Empty;

			int requestId;
			string requestGuid;

			Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
			Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");

			try
			{
				tr = TrialHelper.Request(
					portalDomain.Text
					, portalDomain.Text
					, firstName.Text
					, secondName.Text
					, portalEmail.Text
					, portalPhone.Text
					, "46"
					, portalLogin.Text
					, portalPassword.Text
					, GetReseller()
					, Thread.CurrentThread.CurrentUICulture.Name
					, 54
					, CManage.GetReferrer(Request)
					, out requestId
					, out requestGuid
					/*
					 , cbSendMe.Checked
					 , cbCallMe.Checked
					 */
				);

				switch (tr)
				{
					case localhost.TrialResult.AlreadyActivated:
						message = LocRM.GetString("TrialActivated");
						lblErrorDomainMessage.Text = LocRM.GetString("TrialActivated");
						break;
					case localhost.TrialResult.DomainExists:
						message = LocRM.GetString("DomainExists");
						lblErrorDomainMessage.Text = LocRM.GetString("DomainExists");
						break;
					case localhost.TrialResult.Failed:
						message = LocRM.GetString("UnknownReason");
						lblErrorDomainMessage.Text = LocRM.GetString("UnknownReason");
						break;
					case localhost.TrialResult.InvalidRequest:
						message = LocRM.GetString("InvalidRequest");
						lblErrorDomainMessage.Text = LocRM.GetString("InvalidRequest");
						break;
					case localhost.TrialResult.RequestPending:
						message = String.Format("Request Pending.");
						lblErrorDomainMessage.Text = String.Format("Request Pending.");
						break;
					case localhost.TrialResult.Success:
						message = String.Format(LocRM.GetString("Congratulations"), portalDomain.Text, portalDomain.Text);
						Response.Redirect(String.Format("http://{0}.{1}", portalDomain.Text, lblParentDomain.Text));
						break;
					case localhost.TrialResult.UserRegistered:
						message = LocRM.GetString("UserRegistered");
						lblErrorDomainMessage.Text = LocRM.GetString("UserRegistered");
						break;
					case localhost.TrialResult.WaitingForActivation:
						Response.Redirect(String.Format("MCActivate.aspx?rid={0}&guid={1}&locale={2}", requestId, requestGuid, Thread.CurrentThread.CurrentUICulture.Name), true);
						return;
					default:
						break;
				}
			}
			catch
			{
				try
				{
					string conStr = Settings.ConnectionString;
					if (conStr != null && conStr.Length > 0) //Save request in local database.
						CManage.CreateRequest(
							portalDomain.Text
							, "1 - 20"
							, ""
							, portalDomain.Text
							, firstName.Text
							, secondName.Text
							, portalEmail.Text
							, "46"
							, portalPhone.Text
							, "54"
							, portalLogin.Text
							, portalPassword.Text
							, new Guid(Settings.UnknownResellerGuid)
							, (int)tr
							, message
						);
				}
				catch
				{
				}
			}
			//lblErrorDomainMessage.Text = "Доменное имя занято";
		}
	}
}
