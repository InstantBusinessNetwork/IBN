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
using Mediachase.Ibn.Web.UI;
using Mediachase.Ibn.Web.UI.WebControls;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for MoveObject.
	/// </summary>
	public partial class MoveObject : System.Web.UI.Page
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strLibraryAdvanced", typeof(MoveObject).Assembly);

		ArrayList alFolders = new ArrayList();
		ArrayList alFiles = new ArrayList();

		private int ParentFolderId = -1;

		#region Request Variables
		protected string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return "";
			}
		}

		protected string ContainerName
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

			GetCurrentFolderAndMoveObjects();
			BindElements();
		}

		#region GetCurrentFolderAndMoveObjects
		private void GetCurrentFolderAndMoveObjects()
		{
			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

			string sFolders = Request["Folders"].ToString();
			string sFiles = Request["Files"].ToString();
			while (sFolders.Length > 0)
			{
				alFolders.Add(sFolders.Substring(0, sFolders.IndexOf(",")));
				sFolders = sFolders.Remove(0, sFolders.IndexOf(",") + 1);
			}
			while (sFiles.Length > 0)
			{
				alFiles.Add(sFiles.Substring(0, sFiles.IndexOf(",")));
				sFiles = sFiles.Remove(0, sFiles.IndexOf(",") + 1);
			}
			if (alFolders.Count > 0)
				ParentFolderId = fs.GetDirectory(int.Parse(alFolders[0].ToString())).ParentDirectoryId;
			else if (alFiles.Count > 0)
			{
				int i = int.Parse(alFiles[0].ToString());
				FileInfo fi = fs.GetFile(i);
				ParentFolderId = fi.ParentDirectoryId;
			}
			ctrlDirTree.DisFolderId = ParentFolderId;
		}
		#endregion

		#region BindElements
		private void BindElements()
		{
			btnMove.Attributes.Add("onclick", "DisableButtons(this);");
			btnMove.CustomImage = this.Page.ResolveUrl("~/layouts/images/upload.gif");
			btnCancel.Attributes.Add("onclick", "window.close();");
			lblMoveTo.Text = LocRM.GetString("MoveTo");
			btnMove.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");
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
			this.btnMove.ServerClick += new EventHandler(btnMove_ServerClick);
		}
		#endregion

		#region MoveEvent
		private void btnMove_ServerClick(object sender, EventArgs e)
		{
			int iDestFolder = -1;
			try
			{
				iDestFolder = ctrlDirTree.FolderId;
			}
			catch { }
			if (iDestFolder == -1)
			{
				lblNotValid.Visible = true;
				return;
			}

			foreach (string sFld in alFolders)
			{
				int iFld = int.Parse(sFld);
				if (iFld != iDestFolder)
					fs.MoveDirectory(iFld, iDestFolder);
			}

			foreach (string sFls in alFiles)
			{
				int iFls = int.Parse(sFls);
				fs.MoveFile(iFls, iDestFolder);
			}
			CHelper.CloseItAndRefresh(Response);
		}
		#endregion

	}
}
