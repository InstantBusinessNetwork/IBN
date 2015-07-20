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
using Mediachase.MetaDataPlus;
using Mediachase.IBN.Business;

namespace Mediachase.UI.Web.Modules
{
	/// <summary>
	/// Summary description for GetMetaImageFile.
	/// </summary>
	public partial class GetMetaImageFile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			int ObjectId = int.Parse(Request.QueryString["Id"]);
			string MetaClassName = Request.QueryString["Class"];
			string FieldName = Request.QueryString["Field"];
			MetaObject obj = MetaDataWrapper.LoadMetaObject(ObjectId, MetaClassName);
			Mediachase.MetaDataPlus.MetaFile mf = (Mediachase.MetaDataPlus.MetaFile)obj[FieldName];
			Response.ContentType = "image/jpeg";
			Response.BinaryWrite(mf.Buffer);
			Response.End();
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
