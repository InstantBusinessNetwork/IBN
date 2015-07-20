using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.UI.Web.Util;
using System.IO;

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for IncidentEdit.
	/// </summary>
	public partial class IncidentEdit : System.Web.UI.Page
	{
		private int IncidentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "IncidentId", 0);
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
			if (Mediachase.IBN.Business.PortalConfig.UseIncidentEditAddon)
				pT.ControlName = "../Incidents/Modules/IncidentEdit1.ascx";
			else if (!String.IsNullOrEmpty(Mediachase.IBN.Business.PortalConfig.IncidentEditControl)
				&& File.Exists(Server.MapPath(Mediachase.IBN.Business.PortalConfig.IncidentEditControl)))
				pT.ControlName = Mediachase.IBN.Business.PortalConfig.IncidentEditControl;
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strPageTitles", typeof(IncidentEdit).Assembly);
			if (IncidentId != 0)
				pT.Title = LocRM.GetString("tIncidentEdit");
			else
				pT.Title = LocRM.GetString("tIncidentAdd");

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
