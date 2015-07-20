namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Modules;
	using ComponentArt.Web.UI;

	/// <summary>
	///		Summary description for Incidents.
	/// </summary>
	public partial  class Incidents : System.Web.UI.UserControl, IPageViewMenu
	{
    protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidents", typeof(Incidents).Assembly);
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;
		private int ProjID = 0;
		private string strPref = "Inc";
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
			if(!Configuration.HelpDeskEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if(Request["ProjectId"] != null)
				ProjID = int.Parse(Request["ProjectId"]);
			if(ProjID!=0)
				strPref = "Prj";
			BindTabs();
		}

		#region BindTabs
		private void BindTabs()
		{
			if (BTab != null && (BTab == "MyIncidents" || BTab == "AllIncidents" || BTab == "NotAssigned" || BTab == "MailIncidents" ))
				pc[strPref+"IncidentList_CurrentTab"] = BTab;
			else if (pc[strPref+"IncidentList_CurrentTab"] == null) 
				pc[strPref+"IncidentList_CurrentTab"] = "MyIncidents";

			int UserID = Security.CurrentUser.UserID;
			bool HasMyProj = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager) ||
				Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager) || 
				Security.IsUserInGroup(InternalSecureGroups.ProjectManager) || 
				Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager);

			bool IsHDM = Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager);
			bool IsPPM = Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager);
			bool IsPM = Security.IsUserInGroup(InternalSecureGroups.ProjectManager);
			bool IsExec = Security.IsUserInGroup(InternalSecureGroups.ExecutiveManager);

			if (IsHDM && ProjID == 0)
			{
			}
			else
				if (pc[strPref + "IncidentList_CurrentTab"] == "MailIncidents")
					pc[strPref + "IncidentList_CurrentTab"] = "MyIncidents";


			if (pc[strPref+"IncidentList_CurrentTab"] == "MyIncidents" || pc[strPref+"IncidentList_CurrentTab"] == null)
			{
				pc[strPref+"IncidentList_CurrentTab"] = "MyIncidents";
			}
			else if (pc[strPref+"IncidentList_CurrentTab"] == "AllIncidents")
			{
			}
			else if (pc[strPref + "IncidentList_CurrentTab"] == "MailIncidents")
			{
			}

			string controlName = "IncidentsList.ascx";
			if (pc[strPref+"IncidentList_CurrentTab"] == "MailIncidents")
			{
				controlName = "MailIncidentsList.ascx";
				((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title=LocRM.GetString("tMailIncidents");
				secHeader.Title = LocRM.GetString("tMailIncidents");
			}
			else if ((pc[strPref+"IncidentList_CurrentTab"] == "NotAssigned"))
			{
				controlName = "IncidentsList1.ascx";
				((Mediachase.UI.Web.Modules.PageTemplateNew)this.Parent.Parent.Parent.Parent).Title=LocRM.GetString("IncidentsNotAssigned");
				secHeader.Title = LocRM.GetString("IncidentsNotAssigned");
			}
			//else if ((pc[strPref+"IncidentList_CurrentTab"] == "Grid"))
			//controlName = "IssuesGridControl.ascx";


			System.Web.UI.UserControl control = (System.Web.UI.UserControl)LoadControl(controlName);
			phItems.Controls.Add(control);
		}
		#endregion
		
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

		#region IPageViewMenu Members
		public PageViewMenu GetToolBar()
		{
			return secHeader;
		}
		#endregion
	}
}
