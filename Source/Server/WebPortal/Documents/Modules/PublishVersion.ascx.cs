namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.ControlSystem;
	using System.Data.SqlClient;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for PublishVersion.
	/// </summary>
	public partial class PublishVersion : System.Web.UI.UserControl, IPageTemplateTitle
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(PublishVersion).Assembly);

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

		protected int FileId
		{
			get
			{
				if (Request["FileId"] != null)
					return int.Parse(Request["FileId"]);
				else
					return -1;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			trError.Visible = false;
			lblNotValid.Visible = false;

			BindValues();
			ApplyLocalization();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			btnPublish.ServerClick += new EventHandler(btnPublish_ServerClick);
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			BaseIbnContainer bic = BaseIbnContainer.Create("FileLibrary", "Workspace");
			Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");

			ctrlDirTree.DisFolderId = fs.Root.Id;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			btnPublish.Attributes.Add("onclick", "DisableButtons(this);");
			btnPublish.CustomImage = this.Page.ResolveUrl("~/layouts/images/upload.gif");
			btnCancel.Attributes.Add("onclick", "window.close();");
			lblPublishTo.Text = LocRM.GetString("PublishTo");
			btnPublish.Text = LocRM.GetString("Publish");
			btnCancel.Text = LocRM.GetString("tbSaveCancel");
		}
		#endregion

		#region btnPublish_ServerClick
		private void btnPublish_ServerClick(object sender, EventArgs e)
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

			BaseIbnContainer sourceBic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			Mediachase.IBN.Business.ControlSystem.FileStorage sourceFS = (Mediachase.IBN.Business.ControlSystem.FileStorage)sourceBic.LoadControl("FileStorage");
			FileInfo sourceFi = sourceFS.GetFile(FileId);

			string cKey = DirectoryInfo.GetContainerKey(iDestFolder);
			if (String.IsNullOrEmpty(cKey))
				cKey = "Workspace";
			BaseIbnContainer destBic = BaseIbnContainer.Create("FileLibrary", cKey);
			Mediachase.IBN.Business.ControlSystem.FileStorage destFS = (Mediachase.IBN.Business.ControlSystem.FileStorage)destBic.LoadControl("FileStorage");
			DirectoryInfo destDi = destFS.GetDirectory(iDestFolder);

			try
			{
				destFS.CopyFile(sourceFi, destDi);
			}
			catch (SqlException exception)
			{
				if (exception.Number == 2627 || exception.Number == 50000)
				{
					lblErrorMessage.Text = LocRM.GetString("tDuplicateName");
					trError.Visible = true;
					return;
				}
			}

			Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try {window.opener.location.href=window.opener.location.href;window.close();}" +
					  "catch (e){}", true);
		}
		#endregion

		#region IPageTemplateTitle Members
		public string Modify(string oldValue)
		{
			return LocRM.GetString("Publication");
		}
		#endregion
	}
}
