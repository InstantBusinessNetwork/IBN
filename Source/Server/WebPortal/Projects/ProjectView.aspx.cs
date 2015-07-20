using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for ProjectView.
	/// </summary>
	public partial class ProjectView : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
			ApplyLocalization();

			if (!String.IsNullOrEmpty(Mediachase.IBN.Business.PortalConfig.ProjectViewControl)
				&& File.Exists(Server.MapPath(Mediachase.IBN.Business.PortalConfig.ProjectViewControl)))
				pT.ControlName = Mediachase.IBN.Business.PortalConfig.ProjectViewControl;

			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			string tab = Request.QueryString["Tab"];
			if (!String.IsNullOrEmpty(tab))
			{
				pc["ProjectView_CurrentTab"] = tab;
			}
			else
			{
				tab = pc["ProjectView_CurrentTab"];
				if (!String.IsNullOrEmpty(tab))
				{
					string url = Request.RawUrl;
					if (url.Contains("?"))
						url += String.Concat("&Tab=", tab);
					else
						url += String.Concat("?Tab=", tab);
					Response.Redirect(url);
				}
			}
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPageTitles", typeof(ProjectView).Assembly);
			pT.Title = LocRM.GetString("tProjectView");
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
