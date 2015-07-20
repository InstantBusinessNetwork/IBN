namespace Mediachase.UI.Web.Admin.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Globalization;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using System.IO;


	/// <summary>
	///		Summary description for DefaultAdmin.
	/// </summary>
	public partial  class DefaultReports : System.Web.UI.UserControl
	{

		protected Mediachase.UI.Web.Modules.BlockHeader secHeader;

		private string Tab
		{
			get 
			{
				return Request["Tab"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{ 
			BindTabs();
			ctrlTopTab.Visible = false;
		}

		private void BindTabs()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Reports.Resources.strReports", typeof(DefaultReports).Assembly);

			UserLightPropertyCollection pc;
			pc = Security.CurrentUser.Properties;
			
			if (Tab!=null)
			{
				if (Tab == "Dashboard" || Tab == "Administrative")
					pc["Reports_CurrentTab"] = Tab;
			}
			else if ( pc["Reports_CurrentTab"] == null )
				pc["Reports_CurrentTab"] = "Dashboard";

			ctrlTopTab.AddTab(LocRM.GetString("Dashboard"),"Dashboard");

			if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
				ctrlTopTab.AddTab(LocRM.GetString("Administrative"),"Administrative");
			else if (pc["Reports_CurrentTab"] == "Administrative")
				pc["Reports_CurrentTab"] = "Dashboard";

			ctrlTopTab.TabWidth = "130px";
			ctrlTopTab.SelectItem(pc["Reports_CurrentTab"]);

			string controlName = String.Empty;

			if (pc["Reports_CurrentTab"] == "Administrative")
				controlName = "~/Admin/modules/AdminReports.ascx";
			else
			{
				controlName = "~/reports/modules/dashboard.ascx";
				if (!String.IsNullOrEmpty(Mediachase.IBN.Business.PortalConfig.ManagementCenterDashboardControl)
					&& File.Exists(Server.MapPath(Mediachase.IBN.Business.PortalConfig.ManagementCenterDashboardControl)))
					controlName = Mediachase.IBN.Business.PortalConfig.ManagementCenterDashboardControl;
			}
			
			String tab = pc["Reports_CurrentTab"];

			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
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
