using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Mediachase.IBN.Business;
using Mediachase.IBN.Business.WebDAV.Common;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.FileStorage.Modules
{
	public partial class DownloadLink : System.Web.UI.UserControl
	{
		private string _id
		{
			get
			{
				if (Request[WebDavUrlBuilder.AUTH_TICKET_PARAM_NAME] != null)
					return Request[WebDavUrlBuilder.AUTH_TICKET_PARAM_NAME];
				else
					throw new ArgumentException();
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", Assembly.GetExecutingAssembly());
			secHeader.Title = LocRM.GetString("tDownloadLinkTitle");
			int lifeTime = 0;
			try
			{
				lifeTime = Convert.ToInt32(PortalConfig.WebDavSessionLifeTime);
			}
			catch (Exception)
			{
			}

			string sTime = String.Format("{0:D2}:{1:D2}", lifeTime / 60, lifeTime % 60);
			string sLink = WebDavUrlBuilder.GetWebDavUrl(_id);
			lblDownloadLink.Text = String.Format(LocRM.GetString("tDownLoadLink"), "javascript:FileDownload();", sTime);

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("var global_DownloadFlag = true;");
			sb.AppendLine("function FileDownload()");
			sb.AppendLine("{");
			sb.AppendLine("global_DownloadFlag = false;");
			sb.AppendFormat("window.location.href='{0}'", sLink);
			sb.AppendLine("}");
			sb.AppendLine("function AutoFileDownload()");
			sb.AppendLine("{");
			sb.AppendLine("if(!global_DownloadFlag) return;");
			sb.AppendFormat("window.location.href='{0}'", sLink);
			sb.AppendLine("}");
			sb.AppendLine("setTimeout('AutoFileDownload()', 3000);");
			ClientScript.RegisterStartupScript(this.Page, this.Page.GetType(), Guid.NewGuid().ToString("N"),
				sb.ToString(), true);
		}
	}
}