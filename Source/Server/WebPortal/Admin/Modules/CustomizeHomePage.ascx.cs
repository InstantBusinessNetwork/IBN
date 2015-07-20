namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Reflection;
	using System.Resources;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;
	
	using Mediachase.IBN.Business;

	/// <summary>
	///		Summary description for CustomizeHomePage.
	/// </summary>
	public partial class CustomizeHomePage : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCompanyInfo", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			hpText1.Language = Security.CurrentUser.Culture;
			hpText1.ToolbarBackgroundImage = false;
			hpText1.EnableSsl = Request.IsSecureConnection;
			hpText1.SslUrl = this.Page.ResolveUrl("~/Common/Empty.html");

			hpText2.Language = Security.CurrentUser.Culture;
			hpText2.ToolbarBackgroundImage = false;
			hpText2.EnableSsl = Request.IsSecureConnection;
			hpText2.SslUrl = this.Page.ResolveUrl("~/Common/Empty.html");

			btnReset.Text = LocRM.GetString("Reset");
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			secHeader.Title = LocRM.GetString("HomePage");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("InformationAppearance"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			if (!Page.IsPostBack)
				BindValues();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		private void BindValues()
		{

			hpText1.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText1);
			hpText2.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText2);
			txtTextTitle1.Text = PortalConfig.PortalHomepageTitle1;
			txtTextTitle2.Text = PortalConfig.PortalHomepageTitle2;
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

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			btnCancel.Disabled = true;
			Response.Redirect(this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
		}

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			byte[] HomeImageLogo = null;
			if (cidentity.PostedFile != null && cidentity.PostedFile.ContentLength > 0)
			{
				HomeImageLogo = new byte[cidentity.PostedFile.ContentLength];
				cidentity.PostedFile.InputStream.Read(HomeImageLogo, 0, cidentity.PostedFile.ContentLength);
			}
			btnSave.Disabled = true;
			Company.UpdateCompanyInfo(txtTextTitle1.Text, txtTextTitle2.Text,
				hpText1.Text, hpText2.Text, null, HomeImageLogo);
			Response.Redirect(this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
		}

		protected void btnReset_Click(object sender, System.EventArgs e)
		{
			PortalConfig.PortalHomepageImage = null;
			Response.Redirect(this.Page.ResolveUrl("~/Admin/CustomizeHomePage.aspx"));
		}
	}
}
