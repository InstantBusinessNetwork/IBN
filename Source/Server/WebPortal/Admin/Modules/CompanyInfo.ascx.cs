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
	///		Summary description for CompanyInfo.
	/// </summary>
	public partial class CompanyInfo : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strCompanyInfo", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (PortalConfig.PortalCompanyLogo != null)
				resetLogoBlock.Visible = true;
			checkboxResetLogo.Text = LocRM.GetString("Reset");

			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			secHeader.Title = LocRM.GetString("CompanyProfile");
			secHeader.AddLink("<img alt='' src='" + this.Page.ResolveUrl("~/Layouts/Images/cancel.gif") + "'/> " + LocRM.GetString("InformationAppearance"), ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
			bhl3.AddText(LocRM.GetString("SupportInformation"));
			bhl1.AddText(LocRM.GetString("GeneralInformation"));
			if (!Page.IsPostBack)
				BindValues();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		private void BindValues()
		{
			txtSupportName.Text = PortalConfig.PortalSupportName;
			txtSupportEmail.Text = PortalConfig.PortalSupportEmail;
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


		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			btnCancel.Disabled = true;
			Response.Redirect(this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
		}

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			byte[] companyLogo = null;

			if (!checkboxResetLogo.Checked && clogo.PostedFile != null && clogo.PostedFile.ContentLength > 0)
			{
				companyLogo = new byte[clogo.PostedFile.ContentLength];
				clogo.PostedFile.InputStream.Read(companyLogo, 0, clogo.PostedFile.ContentLength);
			}

			string shpText1 = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText1);
			string shpText2 = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText2);
			string stxtTextTitle1 = PortalConfig.PortalHomepageTitle1;
			string stxtTextTitle2 = PortalConfig.PortalHomepageTitle2;

			btnSave.Disabled = true;

			Company.UpdateCompanyInfo(txtSupportName.Text, txtSupportEmail.Text
				, stxtTextTitle1, stxtTextTitle2, shpText1, shpText2
				, checkboxResetLogo.Checked, companyLogo, false, null);

			Response.Redirect(this.Page.ResolveUrl("~/Apps/Administration/Pages/default.aspx?NodeId=MAdmin1"));
		}
	}
}
