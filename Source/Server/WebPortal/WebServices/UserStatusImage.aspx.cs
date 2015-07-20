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
using System.Drawing.Imaging;

namespace Mediachase.UI.Web.WebServices
{
	/// <summary>
	/// Summary description for UserStatusImage.
	/// </summary>
	public partial class UserStatusImage : System.Web.UI.Page
	{
		enum UserStatus
		{

		};
		private int UserID
		{
			get
			{
				try
				{
					return int.Parse(Request["UserID"]);
				}
				catch
				{
					return 0;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			int _status = Mediachase.IBN.Business.User.GetUserStatus(UserID);
			string status = "";
			try
			{
				status = String.Format("layouts/images/status/status_{0}.gif", ((Mediachase.UI.Web.WebServices.UserStatus)_status).ToString());
			}
			catch(Exception){}
			status = Util.CommonHelper.EndWithSlash(Request.ApplicationPath) + status;
			
			// output image
			System.Drawing.Image icon = System.Drawing.Image.FromFile(Server.MapPath(status));
			Response.ContentType = "image/gif"; 
			icon.Save(Response.OutputStream, ImageFormat.Gif); 
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
