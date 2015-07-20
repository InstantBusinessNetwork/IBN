using System;
using System.Resources;

using Mediachase.Ibn;

namespace Mediachase.UI.Web.Public.Modules
{
	/// <summary>
	/// Summary description for LicenseExpired.
	/// </summary>
	public partial class LicenseExpired : System.Web.UI.UserControl
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strContactUs", typeof(LicenseExpired).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			lblLicExp.Text = GlobalResourceManager.Strings["LicenseExpiredText"];
			lblSendToMC.Text = GlobalResourceManager.Strings["SendToMCPurchase"];
			lblBuy.Text = GlobalResourceManager.Strings["BuyProductText"];
			lblTech.Text = GlobalResourceManager.Strings["TechText"];
			lblPhones.Text = GlobalResourceManager.Strings["PhonesText"];
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

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
