namespace Mediachase.UI.Web.Reports.Modules
{
	using System;
	using System.Data;
	using System.Text;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business; 

	/// <summary>
	///		Summary description for Reports.
	/// </summary>
	public partial  class Reports : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlTableRow trProjectStatus;

    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strReports", typeof(Reports).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			

			Page.EnableViewState = false;
			BindTabVisibility();

			int level=0;
			if(Security.IsUserInGroup(InternalSecureGroups.Administrator)) 
				level+=8;
			if(Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager)
				|| Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager)) 
				level+=4;
			else if(Security.IsUserInGroup(InternalSecureGroups.ProjectManager))
				level+=2;
			if(level==8 || level==10)
			{
				//Tr5.Visible = false;
			}
			if(level<8)
			{
			}
			if(level<4)
			{
				//Tr5.Visible = false;
			}
			if(level<2)
			{
			}
		}

		private void BindTabVisibility()
		{
			UserLightPropertyCollection pc;
			pc = Security.CurrentUser.Properties;

			String tab = pc["Reports_CurrentTab"];

			bool projects = tab == "Projects";
			tblProjects.Visible = projects;

			bool admin = tab == "Administrative";
			tblAdminReports.Visible = admin;
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
