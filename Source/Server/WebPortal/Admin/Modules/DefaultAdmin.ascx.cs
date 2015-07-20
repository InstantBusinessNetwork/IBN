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
	using System.Reflection;


	/// <summary>
	///		Summary description for DefaultAdmin.
	/// </summary>
	public partial class DefaultAdmin : System.Web.UI.UserControl
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
		}

		private void BindTabs()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strDefault", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));

			UserLightPropertyCollection pc;
			pc = Security.CurrentUser.Properties;

			if (Tab != null)
			{
				if (Tab == "PortalSetup" || Tab == "Dictionaries" || Tab == "RoutingWorkflow"
					|| Tab == "Customization" || Tab == "Reports" || Tab == "BusinessData"
					|| Tab == "CommonSettings" || Tab == "HelpDesk" || Tab == "FilesForms"
					|| Tab == "AddTools")
					pc["Admin_CurrentTab"] = Tab;
			}
			else if (pc["Admin_CurrentTab"] == null)
				pc["Admin_CurrentTab"] = "PortalSetup";

			string controlName = "~/admin/modules/default.ascx";
			if (pc["Admin_CurrentTab"] == "Reports")
			{
				controlName = "~/admin/modules/AdminReports.ascx";
				((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title = LocRM.GetString("tReport");
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
