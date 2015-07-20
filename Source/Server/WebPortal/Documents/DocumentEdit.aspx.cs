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

using Mediachase.UI.Web.Util;

namespace Mediachase.UI.Web.Documents
{
	/// <summary>
	/// Summary description for DocumentEdit.
	/// </summary>
	public partial class DocumentEdit : System.Web.UI.Page
	{

		#region DocumentId
		private int DocumentId
		{
			get
			{
				return CommonHelper.GetRequestInteger(Request, "DocumentId", 0);
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			ApplyLocalization();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
      ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentEdit).Assembly);
			if(DocumentId != 0)
				pT.Title = LocRM.GetString("tDocumentEdit");
			else
				pT.Title = LocRM.GetString("tDocumentAdd");
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
