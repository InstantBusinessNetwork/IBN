namespace Mediachase.UI.Web.Projects.Modules
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

	/// <summary>
	///		Summary description for Projects.
	/// </summary>
	public partial  class Projects : System.Web.UI.UserControl
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjects", typeof(Projects).Assembly);

		private string BTab
		{
			get 
			{
				return Request["BTab"];
			}
		}

		public Mediachase.UI.Web.Modules.PageViewMenu ToolBar
		{
			get 
			{
					return secHeader;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Configuration.ProjectManagementEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			BindTabs();
		}

		private void BindTabs()
		{
			UserLightPropertyCollection pc;
			pc = Security.CurrentUser.Properties;

			if (BTab!=null && (BTab== "MyProjects" || BTab == "AllProjects"))
				pc["ProjectList_CurrentTab"] = BTab;
			else 
				pc["ProjectList_CurrentTab"] = "AllProjects";

			int UserID = Security.CurrentUser.UserID;
			bool HasMyProj = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager);

			string controlName="";				
			if (HasMyProj && (pc["ProjectList_CurrentTab"] == "MyProjects" || pc["ProjectList_CurrentTab"] == null))
			{
				pc["ProjectList_CurrentTab"] = "MyProjects";
				controlName = "ProjectsListInner.ascx";
				((Mediachase.UI.Web.Modules.PageTemplateNext)this.Parent.Parent.Parent.Parent.Parent).Title=LocRM.GetString("tMyProjects");
			}
			else
			{
				pc["ProjectList_CurrentTab"] = "AllProjects";
				controlName = "ProjectsListInner.ascx";
				((Mediachase.UI.Web.Modules.PageTemplateNext)this.Parent.Parent.Parent.Parent.Parent).Title=LocRM.GetString("Projects");
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
