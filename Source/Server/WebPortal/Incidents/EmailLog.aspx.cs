using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Resources;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using System.Reflection;

namespace Mediachase.UI.Web.Incidents
{
	/// <summary>
	/// Summary description for EmailLog.
	/// </summary>
	public partial class EmailLog : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();
			ApplyLocalization();
		}
		
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
			pT.Title = LocRM.GetString("tEmailLog");
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
