namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;

	/// <summary>
	///		Summary description for ResourcesTracking.
	/// </summary>
	public partial class ResourcesTracking : System.Web.UI.UserControl
	{
		protected UserLightPropertyCollection pc = Security.CurrentUser.Properties;
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(ResourcesTracking).Assembly);

		private string Tab
		{
			get 
			{
				return Request["Tab"];
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
		}

    private void Page_PreRender(object sender, EventArgs e)
		{
			BindTabs();
		}

		private void BindTabs()
		{
			if (Tab!=null && (Tab == "ResRole"))
				pc["ResourcesTracking_CurrentTab"] = Tab;
			else  if ( pc["ResourcesTracking_CurrentTab"] == null )
				pc["ResourcesTracking_CurrentTab"] = "ResRole";

			string controlName="";	
			if (pc["ResourcesTracking_CurrentTab"] == "ResRole")
			{
				secHeader.Title = LocRM.GetString("tResTrackUsrByRole");
				controlName = "~/Projects/Modules/ResourcesByRoles.ascx";
			}

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
