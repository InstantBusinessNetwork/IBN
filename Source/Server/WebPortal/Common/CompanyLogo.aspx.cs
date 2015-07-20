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
using Mediachase.IBN.Business;

using System.IO;

namespace Mediachase.UI.Web.Common
{
	/// <summary>
	/// Summary description for CompanyLogo.
	/// </summary>
	public partial class CompanyLogo : System.Web.UI.Page
	{
/*
		private int CompanyID
		{
			get 
			{
				try
				{
					return int.Parse(Request["CompanyID"]);
				}
				catch
				{
					return 0;
				}
			}
		}*/


		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "image/png";
			try
			{
				byte[] data = PortalConfig.PortalCompanyLogo;
				if (data != null && data.Length > 0)
				{
					Stream stream = Response.OutputStream;
					stream.Write(data, 0, System.Convert.ToInt32(data.Length));
				}
				else
					Response.Redirect("~/Images/Shell/CompanyLogo.png", true);
			}
			catch
			{
				Response.Redirect("~/Images/Shell/CompanyLogo.png", true);
			}
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
