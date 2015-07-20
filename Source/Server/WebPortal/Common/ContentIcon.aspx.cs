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
	/// Summary description for ContentIcon.
	/// </summary>
	public partial class ContentIcon : System.Web.UI.Page
  {
    #region CTypeId
    protected int CTypeId
		{
			get
			{
				try
				{
					return int.Parse(Request["IconID"].ToString());
				}
				catch
				{
					return -1;
				}
			}
    }
    #endregion

    #region Big
    protected bool Big
		{
			get
			{
				if(Request["Big"]!=null && Request["Big"]=="1")
					return true;
				else
					return false;
			}
    }
    #endregion

    protected void Page_Load(object sender, System.EventArgs e)
		{
			int iIconId = -1;
      if (CTypeId > 0)
      {
        using (IDataReader reader = Mediachase.IBN.Business.ContentType.GetContentType(CTypeId))
        {
          if (reader.Read())
          {
            if (Big && reader["BigIconFileId"] != DBNull.Value)
              iIconId = (int)reader["BigIconFileId"];
            if (!Big && reader["IconFileId"] != DBNull.Value)
              iIconId = (int)reader["IconFileId"];
          }
        }
      }

			Response.ContentType = "image/jpeg";
			if(Big && Request["ContainerKey"]!=null && Request["FileId"]!=null)
			{
				string CKey = Request["ContainerKey"].ToString();
				int FId = int.Parse(Request["FileId"].ToString());
				Size sz = new Size(90, 90);
				if(Mediachase.IBN.Business.ImageThumbnail.Create(ImageThumbnailMode.SaveProportion | ImageThumbnailMode.SkipSmallImage, CKey, FId, sz, System.Drawing.Imaging.ImageFormat.Jpeg, Response.OutputStream))
				{
					return;
				}
			}
			if(iIconId>0)
			{
				string ContainerName = "FileLibrary";
				string ContainerKey = "ContentType";
				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
				FileInfo fi = fs.GetFile(iIconId);
        Response.AddHeader("content-disposition", String.Format("inline; filename=\"{0}\"", GetNameForCurrentBrowser(fi.Name)));
				Mediachase.IBN.Business.ControlSystem.FileStorage.LightLoadFile(fi, Response.OutputStream);
			}
      else
      {
        if (Big)
          Response.Redirect("~/Layouts/Images/filetypes/unknown48.gif");
        else
          Response.Redirect("~/Layouts/Images/filetypes/unknown16.gif");
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

    #region GetNameForCurrentBrowser
    private string GetNameForCurrentBrowser(string sName)
		{
			if(Request.Browser.Browser.ToLower()=="gecko")
				return sName;
			else
				return HttpContext.Current.Server.UrlPathEncode(sName);
    }
    #endregion
  }
}
