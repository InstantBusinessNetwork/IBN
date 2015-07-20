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

using Mediachase.Ibn;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;

namespace Mediachase.UI.Web.External
{
	/// <summary>
	/// Summary description for DownloadFile.
	/// </summary>
	public partial class DownloadFile : System.Web.UI.Page
	{
		#region ExternalID
		private string ExternalID
		{
			get
			{
				return Request.QueryString["guid"];
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Check Security
      if (Security.CurrentUser == null)
      {
        int iObjectTypeId = -1;
        int iObjectId = -1;

        using (IDataReader reader = Mediachase.IBN.Business.User.GetGateByGuid(ExternalID))
        {
          if (reader.Read())
          {
            iObjectTypeId = (int)reader["ObjectTypeId"];
            iObjectId = (int)reader["ObjectId"];
          }
        }

        if (iObjectTypeId != (int)ObjectTypes.Issue)
          throw new AccessDeniedException();
      }
      
			// Download
      Response.ContentType = "application/octet-stream";
			int FileId = int.Parse(Request.QueryString["Id"]);
			string ContainerName = Request.QueryString["CName"];
			string ContainerKey = Request.QueryString["CKey"];

      if (!String.IsNullOrEmpty(ContainerName) && !String.IsNullOrEmpty(ContainerKey))
      {
        BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
        Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

        FileInfo fi = fs.GetFile(FileId);
        if (fi == null)
          throw new NotExistingIdException();

        FileHistoryInfo fhiLoad = null;
        if (Request["VId"] != null)
        {
          FileHistoryInfo[] _fhi = fi.GetHistory();
          foreach (FileHistoryInfo fhi in _fhi)
          {
            if (fhi.Id == int.Parse(Request["VId"]))
            {
              fhiLoad = fhi;
              break;
            }
          }
        }

        if (fhiLoad == null)
        {
          Response.ContentType = fi.FileBinaryContentType;

          if (Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType))
            Response.AddHeader("content-disposition", String.Format("inline; filename=\"{0}\"", GetNameForCurrentBrowser(fi.Name)));
          else
            Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", GetNameForCurrentBrowser(fi.Name)));
          fs.LoadFile(FileId, Response.OutputStream);
        }
        else
        {
          Response.ContentType = fhiLoad.FileBinaryContentType;
          if (Mediachase.IBN.Business.Common.OpenInNewWindow(fhiLoad.FileBinaryContentType))
            Response.AddHeader("content-disposition", String.Format("inline; filename=\"{0}\"", GetNameForCurrentBrowser(fhiLoad.Name)));
          else
            Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", GetNameForCurrentBrowser(fhiLoad.Name)));
          fs.LoadFile(fhiLoad, Response.OutputStream);
        }
      }
      else
      {
        Mediachase.IBN.Business.ControlSystem.FileInfo fi = Mediachase.IBN.Business.ControlSystem.FileStorage.GetFile(Mediachase.IBN.Business.Security.CurrentUser.UserID, "Read", FileId);
        if (fi == null)
          throw new NotExistingIdException();
        Response.ContentType = fi.FileBinaryContentType;
        if (Mediachase.IBN.Business.Common.OpenInNewWindow(fi.FileBinaryContentType))
          Response.AddHeader("content-disposition", String.Format("inline; filename=\"{0}\"", GetNameForCurrentBrowser(fi.Name)));
        else
          Response.AddHeader("content-disposition", String.Format("attachment; filename=\"{0}\"", GetNameForCurrentBrowser(fi.Name)));
        Mediachase.IBN.Business.ControlSystem.FileStorage.LightLoadFile(fi, Response.OutputStream);
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
			if(Request.Browser.Browser.ToLower()=="gecko")
				return sName;
			else
				return HttpContext.Current.Server.UrlPathEncode(sName);
		}
	}
}
