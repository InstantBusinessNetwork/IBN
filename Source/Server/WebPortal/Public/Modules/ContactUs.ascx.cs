using System;
using System.Resources;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Public.Modules
{
	/// <summary>
	/// Summary description for ContactUs.
	/// </summary>
	public partial class ContactUs : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strContactUs", typeof(ContactUs).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			header.Title = LocRM.GetString("SupportContacts");

			if (Request["Expired"] != null)
			{
				lblexp.Text = GlobalResourceManager.Strings["LicenseExpiredText"];
				lblexp.Visible = true;
			}

			string portalSupportLink = CommonHelper.EmailLink(PortalConfig.PortalSupportEmail, PortalConfig.PortalSupportName);
			string supportEmail = GlobalResourceManager.Strings["SupportEmail"];
			string salesEmail = GlobalResourceManager.Strings["SalesEmail"];

			if (!Configuration.IsASP)
			{
				string str = LocRM.GetString(GlobalResourceManager.Strings["SingleCompanyContactUsResourceKey"]);
				message.InnerHtml = string.Format(str, portalSupportLink, supportEmail, salesEmail);
			}
			else
			{
				string str = LocRM.GetString("Asp");

				// O.R. [2008-12-24]: In 4.7 the provider_support_email was taken from GlobalCompany
				string providerSupportEmail = string.Empty;
				message.InnerHtml = string.Format(str, portalSupportLink, providerSupportEmail, supportEmail, salesEmail);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
