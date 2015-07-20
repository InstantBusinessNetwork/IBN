using System;
using System.Resources;
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
using Mediachase.Ibn.Web.UI.WebControls;
using Mediachase.Ibn.Web.UI;

namespace Mediachase.UI.Web.FileStorage
{
	/// <summary>
	/// Summary description for DirectoryEdit.
	/// </summary>
	public partial class DirectoryEdit : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddFolder", typeof(DirectoryEdit).Assembly);

		private int _folderId = -1;
		private string _containerKey = "";
		private string _containerName = "";
		#region Request Variables
		private int ParentFolderId
		{
			get
			{
				if (Request["ParentFolderId"] != null)
					return int.Parse(Request["ParentFolderId"]);
				else
					return -1;
			}
		}

		protected int FolderId
		{
			get
			{
				if (Request["FolderId"] != null)
					return int.Parse(Request["FolderId"]);
				else
					return _folderId;
			}
			set
			{
				_folderId = value;
			}
		}

		protected string ContainerKey
		{
			get
			{
				if (Request["ContainerKey"] != null)
					return Request["ContainerKey"];
				else
					return _containerKey;
			}
			set
			{
				_containerKey = value;
			}
		}

		protected string ContainerName
		{
			get
			{
				if (Request["ContainerName"] != null)
					return Request["ContainerName"];
				else
					return _containerName;
			}
			set
			{
				_containerName = value;
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

			if (Request["PrimaryKeyId"] != null)
			{
				string[] elem = Request["PrimaryKeyId"].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				FolderId = int.Parse(elem[1]);
				ContainerName = elem[2];
				ContainerKey = elem[3];
			}
			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
			rfTitle.ErrorMessage = LocRM.GetString("Required");
			cvTitle.ErrorMessage = LocRM.GetString("tDuplicateName");
			if (!IsPostBack)
				BindValues();

			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close()");

			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
		}

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				"<script type=\"text/javascript\">" +
				"setTimeout(\"LoginFocusElement('" + txtTitle.ClientID + "')\",0);</script>");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			if (FolderId > 0)
			{
				DirectoryInfo diCur = fs.GetDirectory(FolderId);
				txtTitle.Text = diCur.Name;
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
			this.btnSave.ServerClick += new EventHandler(btnSave_ServerClick);
			cvTitle.ServerValidate += new ServerValidateEventHandler(cvTitle_ServerValidate);
		}
		#endregion

		#region btnSave_ServerClick
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			if (!Page.IsValid)
				return;

			if (FolderId > 0) //update
			{
				fs.RenameDirectory(FolderId, txtTitle.Text);
			}
			else if (ParentFolderId > 0)
			{
				fs.CreateDirectory(ParentFolderId, txtTitle.Text);
			}

			CommandParameters cp = new CommandParameters("FL_NewFolderItem");

			if (Request["PrimaryKeyId"] != null)
			{
				cp.CommandName = "FL_Storage_EditFolderItem";
				Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
			}
			else
			{
				if (Request["New"] != null)
					Mediachase.Ibn.Web.UI.WebControls.CommandHandler.GetCloseOpenedFrameScript(this.Page, cp.ToString());
				else
					CHelper.CloseItAndRefresh(Response);
			}
		}
		#endregion

		#region cvTitle_ServerValidate
		private void cvTitle_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = true;
			if (FolderId > 0)
			{
				DirectoryInfo diCur = fs.GetDirectory(FolderId);
				if (diCur.ParentDirectoryId > 0)
				{
					DirectoryInfo di = fs.GetDirectory(diCur.Parent, txtTitle.Text);
					if(di != null && di.Id != FolderId)
						args.IsValid = false;
				}
			}
			if (ParentFolderId > 0)
			{
				if (fs.DirectoryExist(txtTitle.Text, ParentFolderId))
					args.IsValid = false;
				else
					args.IsValid = true;
			}
		}
		#endregion
	}
}
