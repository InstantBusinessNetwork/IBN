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

using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Documents
{
	/// <summary>
	/// Summary description for DocumentView.
	/// </summary>
	public partial class DocumentView : System.Web.UI.Page
	{
		private UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		// O.R. [2009-04-06]: Workflow hack
		protected void Page_Init(object sender, System.EventArgs e)
		{
			string tab = Request.QueryString["Tab"];

			if (String.IsNullOrEmpty(tab) && pc["DocumentView_CurrentTab"] == "Workflow"
				|| tab == "Workflow")
				pT.LoadExtJs = true;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{

			ApplyLocalization();
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentView).Assembly);
			pT.Title = LocRM.GetString("tDocumentView");
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
