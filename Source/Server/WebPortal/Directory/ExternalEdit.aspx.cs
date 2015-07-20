using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Resources;
using System.Globalization;

namespace Mediachase.UI.Web.Directory
{
	/// <summary>
	/// Summary description for UserEdit.
	/// </summary>
	public partial class ExternalEdit : System.Web.UI.Page
	{
		private int UID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(Request["UserID"]!=null) UID = int.Parse(Request["UserID"]);
			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strPageTitles", typeof(ExternalEdit).Assembly);

			if(UID!=0)
				pT.Title = LocRM.GetString("tExternalUserEdit");
			else
				pT.Title = LocRM.GetString("tExternalUserAdd");
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
