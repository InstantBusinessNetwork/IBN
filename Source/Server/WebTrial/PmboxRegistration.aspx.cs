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
using System.Resources;
using System.Threading;
using System.Globalization;

namespace Mediachase.Ibn.WebTrial
{
	public partial class PmboxRegistration : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebTrial.App_GlobalResources.Resources.Resources", typeof(defaultpage).Assembly);

		private string GetReseller()
		{
			string retVal = string.Empty;
			
			retVal = CManage.GetReseller(Request);

			if (retVal == Settings.UnknownResellerGuid)
			{
				retVal = Settings.RuResellerGuid;
			}

			return retVal;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			lblErrorDomainMessage.Text = "";

			if (Request["TestMode"] != null)
			{
				CManage.AppendDebugInfo(string.Empty);
			}

			try
			{
				lblParentDomain.Text = TrialHelper.GetParentDomain();
			}
			catch
			{
				lblErrorDomainMessage.Text = "Ошибка при вызове веб сервиса";
			}
		}

		public void Register_Click(object sender, EventArgs e)
		{
			if (!Page.IsValid || portalName2.Text.Length > 0)
				return;

			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

			localhost.TrialResult tr = localhost.TrialResult.Failed;
			string message = String.Empty;

			int requestId;
			string requestGuid;

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
						//lblErrorDomainMessage.Text = String.Format("Request Pending.");
						lblPendingMessage.Text = String.Format("Запрос на активацию принят.");
						Register_Btn.Style.Add(HtmlTextWriterStyle.Display, "none");
						break;
					case localhost.TrialResult.Success:
						Response.Redirect(String.Format("http://{0}.{1}", portalDomain.Text, lblParentDomain.Text));
						break;
					case localhost.TrialResult.UserRegistered:
						message = LocRM.GetString("UserRegistered");
						lblErrorDomainMessage.Text = LocRM.GetString("UserRegistered");
						break;
					case localhost.TrialResult.WaitingForActivation:
						Response.Redirect(String.Format("PmBoxActivate.aspx?rid={0}&guid={1}&locale={2}", requestId, requestGuid, Thread.CurrentThread.CurrentUICulture.Name), true);
						return;
					default:
						break;
				}
			}
			catch(Exception)
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
