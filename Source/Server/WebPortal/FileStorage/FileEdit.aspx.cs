using System;
using System.Text;
using System.IO;
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
	/// Summary description for FileEdit.
	/// </summary>
	public partial class FileEdit : System.Web.UI.Page
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.FileLibrary.Resources.strAddDoc", typeof(FileEdit).Assembly);

		private int _fileId = -1;
		private string _containerKey = "";
		private string _containerName = "";
		#region Request Variables
		private int FileId
		{
			get
			{
				if (Request["FileId"] != null)
					return int.Parse(Request["FileId"]);
				else
					return _fileId;
			}
			set
			{
				_fileId = value;
			}
		}

		private string ContainerKey
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

		private string ContainerName
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

		private int LinkId
		{
			get
			{
				if (Request["LinkId"] != null)
					return int.Parse(Request["LinkId"]);
				else
					return -1;
			}
		}

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

		private bool ShowHistory
		{
			get
			{
				if (Request["history"] == "0")
					return false;
				else
					return true;
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

			ApplyValues();
			if (!Page.IsPostBack)
				BindValues();
		}

		#region ApplyValues
		private void ApplyValues()
		{
			if (Request["PrimaryKeyId"] != null)
			{
				string[] elem = Request["PrimaryKeyId"].Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				FileId = int.Parse(elem[1]);
				ContainerName = elem[2];
				ContainerKey = elem[3];
			}
			bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");

			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnCancel.Attributes.Add("onclick", "window.close()");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			rfTitle.ErrorMessage = LocRM.GetString("Required");
			cvTitle.ErrorMessage = LocRM.GetString("tDuplicateName");
			cvInvalidChars.ErrorMessage = "*";

			cbKeepHistory.Text = LocRM.GetString("tKeepHistory");

			if (LinkId == 0 && ParentFolderId >= 0)
				trLink.Visible = true;
			else
				trLink.Visible = false;

			if (!ShowHistory)
				cbKeepHistory.Visible = false;
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.GetFile(FileId);
			if (fi != null)
			{
				txtTitle.Text = fi.Name;
				textDescription.Text = fi.Description;
				cbKeepHistory.Checked = fi.AllowHistory;
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
			this.cvTitle.ServerValidate += new ServerValidateEventHandler(cvTitle_ServerValidate);
			this.cvInvalidChars.ServerValidate += new ServerValidateEventHandler(cvInvalidChars_ServerValidate);
		}
		#endregion

		#region Validator
		private void cvTitle_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (LinkId == 0 && ParentFolderId >= 0)
			{
				if (fs.FileExist(txtTitle.Text, ParentFolderId))
					args.IsValid = false;
				else
					args.IsValid = true;
			}
			else
			{
				Mediachase.IBN.Business.ControlSystem.FileInfo fi = fs.GetFile(FileId);
				if (fs.FileExist(txtTitle.Text, fi.ParentDirectoryId) && fi.Name != txtTitle.Text)
					args.IsValid = false;
				else
					args.IsValid = true;
			}
		}

		void cvInvalidChars_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (CheckInvalidPathChars(txtTitle.Text))
				args.IsValid = false;
			else
				args.IsValid = true;
		}
		#endregion

		#region Save
		private void btnSave_ServerClick(object sender, EventArgs e)
		{
			if (!Page.IsValid)
				return;
			if (FileId > 0)
			{	
				fs.RenameFile(FileId, txtTitle.Text, textDescription.Text);
				fs.AllowFileHistory(FileId, cbKeepHistory.Checked);
			}
			else if (LinkId == 0 && ParentFolderId >= 0)
			{
				string data = string.Format("[InternetShortcut]\r\nURL={0}", txtLink.Text);
				MemoryStream memStream = new MemoryStream();
				StreamWriter writer = new StreamWriter(memStream, Encoding.Unicode);
				writer.Write(data);
				writer.Flush();
				memStream.Seek(0, SeekOrigin.Begin);
				string title = txtTitle.Text;
				string html_filename = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title);
				if (html_filename.IndexOf(".url") < 0)
					html_filename += ".url";
				fs.SaveFile(ParentFolderId, html_filename, textDescription.Text, memStream);
			}

			CommandParameters cp = new CommandParameters("FL_NewLinkItem");
			if (Request["PrimaryKeyId"] != null)
			{
				cp.CommandName = "FL_Storage_EditFileItem";
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

		#region CheckInvalidPathChars
		private static bool CheckInvalidPathChars(string path)
		{
			bool retVal = false;
			for (int i = 0; i < path.Length; i++)
			{
				int num2 = path[i];
				if (((num2 == 0x22) || (num2 == 60)) || (((num2 == 0x3e) || (num2 == 0x7c)) || (num2 < 0x20)))
				{
					retVal = true;
				}
			}
			return retVal;
		} 
		#endregion
	}
}
