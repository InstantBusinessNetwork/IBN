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
	public partial class CompanyIdentity : System.Web.UI.Page
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
			Response.ContentType = "image/jpeg";

			try
			{
				if (PortalConfig.PortalHomepageImage != null)
				{
					Stream stream = Response.OutputStream;
					stream.Write(PortalConfig.PortalHomepageImage, 0, System.Convert.ToInt32(PortalConfig.PortalHomepageImage.Length));
				}
				else
					Response.Redirect("~/layouts/images/transparentpoint.gif", true);
			}
			catch
			{
				Response.Redirect("~/layouts/images/transparentpoint.gif", true);
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
