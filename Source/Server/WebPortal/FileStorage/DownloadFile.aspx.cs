using System;
using System.Text;
using System.IO;
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

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for DownloadFile.
	/// </summary>
	public partial class DownloadFile : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Response.ContentType = "application/octet-stream";

			int FileId = int.Parse(Request.QueryString["Id"]);
			if (Request.QueryString["CName"] != null && Request.QueryString["CKey"] != null)
			{
				string ContainerName = Request.QueryString["CName"];
				string ContainerKey = Request.QueryString["CKey"];
				BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
				Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
			
				Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.GetFile(FileId);
				if(fi==null)
					throw new NotExistingIdException();
				FileHistoryInfo fhiLoad = null;
				if(Request["VId"]!=null)
				{
					FileHistoryInfo[] _fhi = fi.GetHistory();
					foreach(FileHistoryInfo fhi in _fhi)
					{
						if(fhi.Id == int.Parse(Request["VId"]))
						{
							fhiLoad = fhi;
							break;
						}
					}
				}
				if(fhiLoad==null)
				{
					Response.ContentType = fi.FileBinaryContentType;
					if(fi.FileBinaryContentType.ToLower().IndexOf("url")>=0)
					{
						MemoryStream memStream = new MemoryStream();
						fs.LoadFile(FileId, memStream);
						memStream.Position = 0;
						StreamReader reader = new StreamReader(memStream, Encoding.Unicode);
						string data = reader.ReadLine();
						string sLink = "";
						while(data!=null)
						{
							if(data.IndexOf("URL=")>=0)
							{
								sLink = data.Substring(data.IndexOf("URL=")+4);
								break;
							}
							data = reader.ReadLine();
						}
						if(sLink!="")
						{
							if(!sLink.StartsWith("http://"))
								sLink = "http://" + sLink;
							Response.Redirect(sLink);
							return;
						}
					}

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
				if(fi==null)
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
