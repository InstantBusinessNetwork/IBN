namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business.ControlSystem;
	using ComponentArt.Web.UI;
	using Mediachase.IBN.Business.WebDAV.Common;

	/// <summary>
	///		Summary description for ToDoShortInfoDoc.
	/// </summary>
	public partial class ToDoShortInfoDoc : System.Web.UI.UserControl
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoGeneral", typeof(ToDoShortInfoDoc).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(ToDoShortInfoDoc).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(ToDoShortInfoDoc).Assembly);
		
		#region ToDoID
		private int ToDoID
		{
			get
			{
				try
				{
					return int.Parse(Request["ToDoId"]);
				}
				catch
				{
					throw new Exception("Invalid ToDO Id!!!");
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindValues();
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
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			int DocumentID = -1;
			using (IDataReader reader = ToDo.GetToDo(ToDoID))
			{
				if (reader.Read())
				{
					if (reader["DocumentID"] != DBNull.Value)
						DocumentID = (int)reader["DocumentID"];
				}
			}

			if (DocumentID < 0)
			{
				this.Visible = false;
				return;
			}

			using (IDataReader reader = Document.GetDocument(DocumentID))
			{
				if (reader.Read())
				{
					///  DocumentId, ProjectId, ProjectTitle, CreatorId, ManagerId, 
					///  Title, Description, CreationDate, PriorityId, PriorityName, 
					///  StatusId, StatusName, StateId

					bool IsExternal = Security.CurrentUser.IsExternal;
					if (!IsExternal)
						lblTitle.Text = "<a href='../Documents/DocumentView.aspx?DocumentID=" + ((int)reader["DocumentID"]).ToString() + "'>" + reader["Title"].ToString() + "</a>";
					else
						lblTitle.Text = reader["Title"].ToString();

					//					lblState.ForeColor = Util.CommonHelper.GetStateColor((int)reader["StateId"]);
					lblState.Text = reader["StateName"].ToString();
					if ((int)reader["StateId"] == (int)ObjectStates.Active || (int)reader["StateId"] == (int)ObjectStates.Overdue)
						lblState.Text += String.Format(" ({0})", reader["StatusName"].ToString());

					lblPriority.Text = reader["PriorityName"].ToString();
					//					lblPriority.ForeColor = Util.CommonHelper.GetPriorityColor((int)reader["PriorityId"]);
					lblPriority.Visible = PortalConfig.CommonDocumentAllowViewPriorityField;

					if (reader["Description"] != DBNull.Value)
					{
						string txt = CommonHelper.parsetext(reader["Description"].ToString(), false);
						if (PortalConfig.ShortInfoDescriptionLength > 0 && txt.Length > PortalConfig.ShortInfoDescriptionLength)
							txt = txt.Substring(0, PortalConfig.ShortInfoDescriptionLength) + "...";
						lblDescription.Text = txt;
					}
				}
			}

			string ContainerKey = "DocumentVers_" + DocumentID.ToString();
			string ContainerName = "FileLibrary";
			BaseIbnContainer bic = BaseIbnContainer.Create(ContainerName, ContainerKey);
			Mediachase.IBN.Business.ControlSystem.FileStorage fs = (Mediachase.IBN.Business.ControlSystem.FileStorage)bic.LoadControl("FileStorage");
			FileInfo[] _fi = fs.GetFiles(fs.Root.Id);
			FileInfo LatestVersion = null;
			foreach (FileInfo fi in _fi)
			{
				if (LatestVersion == null || LatestVersion.Created < fi.Created)
					LatestVersion = fi;
			}

			if (LatestVersion == null)
				lblLatestVersion.Text = LocRM.GetString("None");
			else
			{
				string Icon = String.Format("<img src='{0}' width='16' height='16' align='absmiddle' border='0'>", ResolveUrl("~/Common/ContentIcon.aspx?IconID=" + LatestVersion.FileBinaryContentTypeId));
				//string Link = String.Format("{0}?Id={1}&CName={2}&CKey={3}", ResolveUrl("~/FileStorage/DownloadFile.aspx"), LatestVersion.Id, ContainerName, ContainerKey);
				string Link = Util.CommonHelper.GetAbsoluteDownloadFilePath(LatestVersion.Id, LatestVersion.Name, ContainerName, ContainerKey);
				string sNameLocked = CommonHelper.GetLockerText(Link);

				lblLatestVersion.Text = String.Format("<a href=\"{0}\">{1} {2}</a> (#{3}) {4}", 
					Link, Icon, 
					LatestVersion.Name, _fi.Length,
					sNameLocked);
			}


			// Toolbar
			tbView.AddText(LocRM.GetString("tbDocView"));

			ComponentArt.Web.UI.MenuItem topMenuItem = new ComponentArt.Web.UI.MenuItem();
			topMenuItem.Text = /*"<img border='0' src='../Layouts/Images/downbtn.gif' width='9px' height='5px' align='absmiddle'/>&nbsp;" + */LocRM2.GetString("Actions");
			topMenuItem.Look.LeftIconUrl = ResolveUrl("~/Layouts/Images/downbtn1.gif");
			topMenuItem.Look.LeftIconHeight = Unit.Pixel(5);
			topMenuItem.Look.LeftIconWidth = Unit.Pixel(16);
			topMenuItem.LookId = "TopItemLook";

			ComponentArt.Web.UI.MenuItem subItem;

			if (!Security.CurrentUser.IsExternal)
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/icon-search.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				subItem.NavigateUrl = String.Format("~/Documents/DocumentView.aspx?DocumentID={0}", DocumentID);
				subItem.Text = LocRM.GetString("View");
				topMenuItem.Items.Add(subItem);
			}

			if (Document.CanAddVersion(DocumentID))
			{
				subItem = new ComponentArt.Web.UI.MenuItem();
				subItem.Look.LeftIconUrl = "~/Layouts/Images/FileTypes/common.gif";
				subItem.Look.LeftIconWidth = Unit.Pixel(16);
				subItem.Look.LeftIconHeight = Unit.Pixel(16);
				string commandLink = (Security.CurrentUser.IsExternal) ? "~/External/FileUpload.aspx" : "~/FileStorage/FileUpload.aspx";
				subItem.ClientSideCommand = String.Format("javascript:ShowWizard('{0}?ParentFolderId={1}&ContainerKey={2}&ContainerName={3}{4}', 470, 270);",
				  ResolveUrl(commandLink), fs.Root.Id, ContainerKey, ContainerName,
				  (Security.CurrentUser.IsExternal) ? ("&ExternalId=" + Security.CurrentUser.UserID) : "");
				subItem.Text = LocRM3.GetString("tAddVersion");
				topMenuItem.Items.Add(subItem);
			}

			if (topMenuItem.Items.Count > 0)
				tbView.ActionsMenu.Items.Add(topMenuItem);
		}
		#endregion
	}
}
