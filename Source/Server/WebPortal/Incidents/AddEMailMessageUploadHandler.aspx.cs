using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Resources;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;

using Mediachase.FileUploader;
using Mediachase.FileUploader.Web;
using Mediachase.FileUploader.Web.UI;
using CS = Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.Incidents
{
	public partial class AddEMailMessageUploadHandler : System.Web.UI.Page
	{
		#region guid
		protected string guid
		{
			get
			{
				if (Request["guid"] != null)
					return Request["guid"];
				else
					return Guid.NewGuid().ToString();
			}
		}
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(AddEMailMessageUploadHandler).Assembly);

		protected void Page_Load(object sender, EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/outlook2003.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/emailsend.js");

			if (IsPostBack)
				Process();
		}

		private void Process()
		{
			string _containerName = "FileLibrary";
			string _containerKey = "EMailAttach";
			CS.BaseIbnContainer bic = CS.BaseIbnContainer.Create(_containerName, _containerKey);
			CS.FileStorage fs = (CS.FileStorage)bic.LoadControl("FileStorage");
			CS.DirectoryInfo di = fs.GetDirectory(fs.Root.Id, guid, true);
			
			for (int Index = 0; Index < Request.Files.Count; Index++)
			{
				if(!String.IsNullOrEmpty(Request.Files[Index].FileName))
					using (McHttpPostedFile PostedFile = new McHttpPostedFile(Request.Files[Index]))
					{
						if (PostedFile.InputStream != null)
							fs.SaveFile(di.Id, PostedFile.FileName, PostedFile.InputStream);
					}
			}
		}

	}
}