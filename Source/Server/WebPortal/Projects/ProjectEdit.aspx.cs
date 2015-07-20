using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using System.Globalization;


namespace Mediachase.UI.Web.Projects
{
	/// <summary>
	/// Summary description for ProjectAdd.
	/// </summary>
	public partial class ProjectEdit : System.Web.UI.Page
	{
		private int PID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(Request["ProjectID"]!=null) PID = int.Parse(Request["ProjectID"]);
			ApplyLocalization();

			if (!String.IsNullOrEmpty(Mediachase.IBN.Business.PortalConfig.ProjectEditControl)
				&& File.Exists(Server.MapPath(Mediachase.IBN.Business.PortalConfig.ProjectEditControl)))
				pT.ControlName = Mediachase.IBN.Business.PortalConfig.ProjectEditControl;
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPageTitles", typeof(ProjectEdit).Assembly);
			if(PID!=0)
				pT.Title = LocRM.GetString("tProjectEdit");
			else
				pT.Title = LocRM.GetString("tProjectAdd");
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
