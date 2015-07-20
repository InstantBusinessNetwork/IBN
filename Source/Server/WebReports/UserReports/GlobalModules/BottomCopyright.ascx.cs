namespace Mediachase.UI.Web.UserReports.GlobalModules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Threading;


	/// <summary>
	///		Summary description for BottomCopyright.
	/// </summary>
	public partial  class BottomCopyright : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.UserReports.GlobalModules.Resources.strTemplate", typeof(BottomCopyright).Assembly);

		bool _Russian = false;
	
		public bool IsRussian
		{
			get
			{
				return _Russian;
			}

			set
			{
				_Russian = value;
			}
		}

		bool _ShowTopLine = false;
		public bool ShowTopLine
		{
			get
			{
				return _ShowTopLine;
			}

			set
			{
				_ShowTopLine = value;
			} 
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			loggedincopyright.Visible = ShowTopLine;
			lblVersion.Text = LocRM.GetString("tVersion")+" " + Mediachase.IBN.Business.Configuration.GetServerVersion();
			#if (RUSSIANEDITION)
				link1.HRef = "http://www.mediachase.ru";
				lblCopyright.Text = LocRM.GetString("RussianCopyright");
				
				lblPrivacy.Visible = false;
				link2.HRef = "javascript:OpenPopUpWindow('../Public/TermsOfUseRUS.aspx', 650, 470)"; 
				lblTermsOfUse.Text = LocRM.GetString("TermsofUse");
				lblVersion.Text += "r";
			#else
			lblCopyright.Text = LocRM.GetString("Copyright");
			link2.HRef = "javascript:OpenPopUpWindow('../Public/TermsOfUse.aspx', 650, 400)"; 
			lblTermsOfUse.Text = LocRM.GetString("TermsofUse");
			link3.HRef = "javascript:OpenPopUpWindow('../Public/PrivacyStatement.aspx', 650, 400)";
			lblPrivacy.Text = LocRM.GetString("PrivacyStatement");
			#endif
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
