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
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.UI.Web.Common
{
	/// <summary>
	/// Summary description for GetFile.
	/// </summary>
	public partial class GetUserPhoto : System.Web.UI.Page
	{
		private int UserId
		{
			get
			{
				int result;
				if (!int.TryParse(Request["UserID"], out result))
					result = -1;
				return result;
			}
		}

		private int OriginalId
		{
			get
			{
				int result;
				if (!int.TryParse(Request["OriginalId"], out result))
					result = -1;
				return result;
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.Cache.SetNoStore();
			try
			{
				int userId = UserId;
				int originalId = OriginalId;

				if (userId < 0 && originalId > 0)
				{
					using (IDataReader reader = Mediachase.IBN.Business.User.GetUserInfoByOriginalId(originalId))
					{
						if (reader.Read())
							userId = (int)reader["UserId"];
					}
				}

				string pictureUrl = string.Empty;
				using (IDataReader rdr = Mediachase.IBN.Business.User.GetUserProfile(userId))
				{
					if (rdr.Read())
					{
						if (rdr["PictureUrl"] != DBNull.Value)
							pictureUrl = (string)rdr["PictureUrl"];
					}
				}

				int fileId = -1;
				if (!string.IsNullOrEmpty(pictureUrl))
					fileId = DSFile.GetFileIDFromURL(pictureUrl);

				if (fileId > 0)
				{
					const string containerName = "FileLibrary";
					const string containerKey = "SystemPicture";
					BaseIbnContainer bic = BaseIbnContainer.Create(containerName, containerKey);
					Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
					FileInfo fi = fs.GetFile(fileId);
					Response.ContentType = "image/jpeg";
					Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", GetNameForCurrentBrowser(fi.Name)));
					Mediachase.IBN.Business.ControlSystem.FileStorage.LightLoadFile(fi, Response.OutputStream);
				}
				else
				{
					Response.ContentType = "image/jpeg";
					Response.Redirect("~/Layouts/Images/nofoto.gif");
				}
			}
			catch
			{
				Response.Redirect("~/Layouts/Images/nofoto.gif", true);
			}

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

		private string GetNameForCurrentBrowser(string sName)
		{
			if (Request.Browser.Browser.ToLower() == "gecko")
				return sName;
			else
				return HttpContext.Current.Server.UrlPathEncode(sName);
		}
	}
}
