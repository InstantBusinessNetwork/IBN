using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for PasteHandler.
	/// </summary>
	public partial class PasteHandler : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strFileLibrary", typeof(PasteHandler).Assembly);
		UserLightPropertyCollection pc = Mediachase.IBN.Business.Security.CurrentUser.Properties;

		#region Request Variables
		private int FolderId
		{
			get
			{
				if (Request["FolderId"] != null)
					return int.Parse(Request["FolderId"]);
				else
					return -1;
			}
		}

		private string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return "";
			}
		}

		private string ContainerName
		{
			get
			{
				if (Request["ContainerName"] != null)
					return Request["ContainerName"];
				else
					return "";
			}
		}
		#endregion

		BaseIbnContainer bic;
		Mediachase.IBN.Business.ControlSystem.FileStorage fs;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/windows.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/Theme.css");
			UtilHelper.RegisterCssStyleSheet(Page, "~/Styles/IbnFramework/mcCalendClient.css");

			UtilHelper.RegisterScript(Page, "~/Scripts/browser.js");
			UtilHelper.RegisterScript(Page, "~/Scripts/buttons.js");

			Response.Cache.SetNoStore();

			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

			if (!Page.IsPostBack)
				BindDG();
		}

		private void Page_PreRender(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
			  "window.setInterval('ResizeForm()', 100);", true);
		}

		#region BindDG
		private void BindDG()
		{
			int iCount = 10;
			if (pc["ClipboardItemsCount"] != null)
				iCount = int.Parse(pc["ClipboardItemsCount"].ToString());
			string sNewFilesClip = "";
			if (pc["ClipboardFiles"] != null)
				sNewFilesClip = pc["ClipboardFiles"].ToString();
			string sCheck = sNewFilesClip;
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("FileId", typeof(int)));
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			DataRow dr;
			while (sCheck.Length > 0)
			{
				if (sCheck.IndexOf("|") >= 0)
				{
					dr = dt.NewRow();
					int elemId = int.Parse(sCheck.Substring(0, sCheck.IndexOf("|")));
					dr["FileId"] = elemId;
					FileInfo fi = fs.GetFile(elemId);
					if (fi != null && fi.ParentDirectoryId != FolderId)
					{
						dr["Name"] = fi.Name;
						dt.Rows.Add(dr);
					}
					sCheck = sCheck.Substring(sCheck.IndexOf("|") + 1);
				}
			}
			if (dt.Rows.Count == 0)
			{
				dgFiles.Visible = false;
				lblAlert.Visible = true;
				lblAlert.Text = LocRM.GetString("tNoItems");
			}
			else
			{
				dgFiles.Visible = true;
				lblAlert.Visible = false;
				dgFiles.DataSource = dt.DefaultView;
				dgFiles.DataBind();
			}
		}
		#endregion

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
			this.lbCopyFile.Click += new EventHandler(lbCopyFile_Click);
		}
		#endregion

		private void lbCopyFile_Click(object sender, EventArgs e)
		{
			int FileId = int.Parse(hdnFileId.Value);
			try
			{
				fs.CopyFile(FileId, FolderId);
			}
			catch { }

			if (Request["New"] != null)
			{
				CommandParameters cp = new CommandParameters("FL_Clipboard_Paste");
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
			{
				CHelper.CloseItAndRefresh(Response);
			}
		}
	}
}
