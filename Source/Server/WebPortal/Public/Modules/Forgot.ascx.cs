namespace Mediachase.UI.Web.Public.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;

	using Mediachase.IBN.Business;
	

	/// <summary>
	///		Summary description for forgot.
	/// </summary>
	public partial  class forgot : System.Web.UI.UserControl
	{
    public ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strforgot", typeof(forgot).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
		}

		public void ApplyLocalization()
		{
			lBlank.Text = LocRM.GetString("tIfYouAreReg");

			string supportEmail = PortalConfig.PortalSupportEmail;
			string supportName = PortalConfig.PortalSupportName;

			lWrong.Text = String.Format(CultureInfo.InvariantCulture, 
				"{0} {1}: <br /><a href='mailto:{2}'>{3}</a>.",
				LocRM.GetString("tThereAreNoInf"),
				LocRM.GetString("tSupCent"), 
				supportEmail,
				supportName);

			lComplete.Text = LocRM.GetString("tYourPassWasSent")+" <a href='../Default.aspx'>"+LocRM.GetString("tPressToLog")+"</a>.";
			btnSend.Text = LocRM.GetString("tSend");
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

		protected void btnSend_Click(object sender, System.EventArgs e)
		{
			string txt = tbLogin.Text;
			if(txt.Length<=0)
				return;
			if (!User.SendForgottenPassword(tbLogin.Text))
			{
				lBlank.Visible=false;
				lWrong.Visible=true;
				lComplete.Visible=false;
			}
			else
			{
				lBlank.Visible=false;
				lWrong.Visible=false;
				tbLogin.Text="";
				lComplete.Visible=true;

			}
		}
	}
}
