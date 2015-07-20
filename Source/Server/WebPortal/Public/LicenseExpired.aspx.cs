using System;
using System.Resources;

using Mediachase.Ibn;

namespace Mediachase.UI.Web.Public
{
	/// <summary>
	/// Summary description for LicenseExpired.
	/// </summary>
	public partial class LicenseExpired : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.Global", typeof(LicenseExpired).Assembly);
			pT.Title = LocRM.GetString("LicenseExpiredTitle");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
