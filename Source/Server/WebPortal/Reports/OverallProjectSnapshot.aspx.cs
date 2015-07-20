using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn;
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Reports
{
	/// <summary>
	/// Summary description for OverallProjectSnapshot.
	/// </summary>
	public partial class OverallProjectSnapshot : System.Web.UI.Page
	{
		protected string sTitle = String.Format("{0} {1}", IbnConst.ProductFamily, IbnConst.VersionMajorDotMinor);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/ibn.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			string path = Mediachase.IBN.Business.PortalConfig.ProjectSnapshotControlDefaultValue;
			if (!String.IsNullOrEmpty(Mediachase.IBN.Business.PortalConfig.ProjectSnapshotControl)
				&& File.Exists(Server.MapPath(Mediachase.IBN.Business.PortalConfig.ProjectSnapshotControl)))
				path = Mediachase.IBN.Business.PortalConfig.ProjectSnapshotControl;

			System.Web.UI.Control control = LoadControl(path);
			MainPlaceHolder.Controls.Add(control);
				

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.moveTo(0,0);window.resizeTo(screen.availWidth,screen.availHeight);}" +
					  "catch (e){}", true);
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
