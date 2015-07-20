namespace Mediachase.UI.Web.Documents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.IO;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FreeTextBoxControls;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Clients;

	/// <summary>
	///		Summary description for DocumentEdit.
	/// </summary>
	public partial class DocumentEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Documents.Resources.strDocuments", typeof(DocumentEdit).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskEdit", typeof(DocumentEdit).Assembly);
		private int ProjectId = 0;
		private int DocumentId = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			trStatus.Visible = false;

			if (!Configuration.ProjectManagementEnabled)
				trProject.Visible = false;

			LoadRequestVariables();
			BindAttributes();

			if (!Page.IsPostBack)
			{
				BindVisibility();
				BindDictionaries();
				BindSavedValues();
			}

			BindToolbar();

			if (Request["Checked"] != null)
				cbOneMore.Checked = true;
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

		#region LoadRequestVariables
		private void LoadRequestVariables()
		{
			if (Request["DocumentId"] != null)
				DocumentId = int.Parse(Request.QueryString["DocumentId"]);

			if (Request["ProjectId"] != null)
				ProjectId = int.Parse(Request.QueryString["ProjectId"]);
		}
		#endregion

		#region BindAttributes
		private void BindAttributes()
		{
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ")");
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("EditDictionary"));
			btnAddGeneralCategory.Visible = Security.IsManager();

			if (DocumentId != 0)
				btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			else
				btnSave.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");

			if (!Page.IsPostBack)
			{
				trCategory.Visible = PortalConfig.CommonDocumentAllowEditGeneralCategoriesField;
				trClient.Visible = PortalConfig.CommonDocumentAllowEditClientField;
				trTaskTime.Visible = PortalConfig.CommonDocumentAllowEditTaskTimeField;
				trPriority.Visible = PortalConfig.CommonDocumentAllowEditPriorityField;
				tdRight.Visible = (trCategory.Visible || trClient.Visible || trTaskTime.Visible || trPriority.Visible);

				trHtmlAttach.Visible = PortalConfig.DocumentAllowEditAttachmentField;
			}
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			vFile.Visible = false;

			if (DocumentId > 0)
			{
				trHtmlAttach.Visible = false;
				cbOneMore.Visible = false;
			}
			else
			{
				cbOneMore.Visible = true;
			}

			if (DocumentId <= 0 && ProjectId <= 0)
			{
				ucProject.Visible = true;
				lblProject.Visible = false;
			}
			else
			{
				ucProject.Visible = false;
				lblProject.Visible = true;
			}
		}
		#endregion

		#region BindDictionaries
		private void BindDictionaries()
		{
			lbCategory.DataSource = Document.GetListCategoriesAll();
			lbCategory.DataTextField = "CategoryName";
			lbCategory.DataValueField = "CategoryId";
			lbCategory.DataBind();

			// Priority
			ddlPriority.DataSource = Document.GetListPriorities();
			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataBind();

			if (ProjectId > 0)
				lblProject.Text = Project.GetProjectTitle(ProjectId);

			if (DocumentId <= 0)
			{
				ListItem liPriority = ddlPriority.Items.FindByValue(PortalConfig.DocumentDefaultValuePriorityField);
				if (liPriority != null)
					liPriority.Selected = true;

				ucTaskTime.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.DocumentDefaultValueTaskTimeField));

				ArrayList list = Common.StringToArrayList(PortalConfig.DocumentDefaultValueGeneralCategoriesField);
				foreach (int i in list)
					CommonHelper.SafeMultipleSelect(lbCategory, i.ToString());

				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.DocumentDefaultValueClientField, out contact_id, out org_id);
				if (contact_id != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = contact_id;
				}
				else if (org_id != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = org_id;
				}
				
				rbFile.Checked = true;
				rbFile.Attributes.Add("onclick", "ShowFile()");
				rbHTML.Attributes.Add("onclick", "ShowHTML()");
				rbLink.Attributes.Add("onclick", "ShowLink()");
				rbFile.Text = LocRM.GetString("File");
				rbHTML.Text = LocRM.GetString("HTMLText");
				rbLink.Text = LocRM.GetString("Link");
				rbHTML.Checked = false;
				rbLink.Checked = false;
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowFile()', 200);</script>");
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{
			int managerId = -1;
			int projectId = ProjectId;

			if (DocumentId > 0)
			{
				using (IDataReader reader = Document.GetDocument(DocumentId))
				{
					if (reader.Read())
					{
						if (reader["ProjectTitle"] != DBNull.Value)
							lblProject.Text = reader["ProjectTitle"].ToString();
						else
							lblProject.Text = LocRM.GetString("ProjectNotSet");

						Util.CommonHelper.SafeSelect(ddlPriority, reader["PriorityId"].ToString());
						//					Util.CommonHelper.SafeSelect(ddlStatus, reader["StatusId"].ToString());

						txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
						ftbDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());

						ucTaskTime.Value = DateTime.MinValue.AddMinutes((int)reader["TaskTime"]);

						if (reader["OrgUid"] != DBNull.Value)
						{
							ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
							ClientControl.ObjectId = PrimaryKeyId.Parse(reader["OrgUid"].ToString());
						}
						else if (reader["ContactUid"] != DBNull.Value)
						{
							ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
							ClientControl.ObjectId = PrimaryKeyId.Parse(reader["ContactUid"].ToString());
						}

						managerId = (int)reader["ManagerId"];

						if (reader["ProjectId"] != DBNull.Value)
							projectId = (int)reader["ProjectId"];
					}
				}

				using (IDataReader reader = Document.GetListCategories(DocumentId))
				{
					while (reader.Read())
					{
						for (int i = 0; i < lbCategory.Items.Count; i++)
							Util.CommonHelper.SafeMultipleSelect(lbCategory, reader["CategoryId"].ToString());
					}
				}

				EditControl.ObjectId = DocumentId;
				EditControl.BindData();
			}

			if (projectId > 0)
				FillManagersByProject(projectId);
			else
				FillManagers();

			// Select current manager
			if (managerId <= 0)
				managerId = Security.CurrentUser.UserID;

			ListItem managerItem = ddlManager.Items.FindByValue(managerId.ToString());
			if (managerItem == null)
			{
				UserLight ul = UserLight.Load(managerId);
				ddlManager.Items.Add(new ListItem(ul.LastName + " " + ul.FirstName, managerId.ToString()));
			}
			CommonHelper.SafeSelect(ddlManager, managerId.ToString());
		}
		#endregion

		#region FillManagersByProject
		private void FillManagersByProject(int projectId)
		{
			bool addCurrent = true;
			using (IDataReader reader = Project.GetListTeamMemberNamesWithManager(projectId))
			{
				while (reader.Read())
				{
					if (!(bool)reader["IsExternal"])
					{
						ddlManager.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["UserId"].ToString()));
						if ((int)reader["UserId"] == Security.CurrentUser.UserID)
							addCurrent = false;
					}
				}
			}

			if (addCurrent)
				ddlManager.Items.Add(new ListItem(Security.CurrentUser.LastName + " " + Security.CurrentUser.FirstName, Security.CurrentUser.UserID.ToString()));
		}
		#endregion

		#region FillManagers
		private void FillManagers()
		{
			bool addCurrent = true;
			if (!Security.IsUserInGroup(InternalSecureGroups.Partner))
			{
				using (IDataReader reader = User.GetListActive())
				{
					while (reader.Read())
					{
						ddlManager.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["PrincipalId"].ToString()));
						if ((int)reader["PrincipalId"] == Security.CurrentUser.UserID)
							addCurrent = false;
					}
				}
			}
			else
			{
				// UserId, Login, FirstName, LastName, Email, IsExternal, IsPending
				using (IDataReader reader = User.GetListActiveUsersForPartnerUser(Security.CurrentUser.UserID))
				{
					while (reader.Read())
					{
						ddlManager.Items.Add(new ListItem(reader["LastName"].ToString() + " " + reader["FirstName"].ToString(), reader["UserId"].ToString()));
						if ((int)reader["UserId"] == Security.CurrentUser.UserID)
							addCurrent = false;
					}
				}
			}

			if (addCurrent)
				ddlManager.Items.Add(new ListItem(Security.CurrentUser.LastName + " " + Security.CurrentUser.FirstName, Security.CurrentUser.UserID.ToString()));
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (DocumentId != 0)
				tbSave.Title = LocRM.GetString("tbSaveEdit");
			else
				tbSave.Title = LocRM.GetString("tbSaveCreate");


			if (ProjectId > 0)
				tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbSaveGoToList"), "../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
			else
				tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbSaveGoToList"), "../Documents/default.aspx");
			btnSave.Text = LocRM.GetString("tbSaveSave");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnCancel.Text = LocRM.GetString("tbSaveCancel");
			cbOneMore.Text = LocRM.GetString("tOneMoreItem");
			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrCategoryInfo.AddText(LocRM.GetString("tCategoryInfo"));
			hdrAttach.AddText(LocRM.GetString("tAttachedFiles"));
		}
		#endregion

		#region Page_PreRender
		private void Page_PreRender(object sender, EventArgs e)
		{
			if (Page.IsPostBack)
			{
				if (rbFile.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowFile()', 200);</script>");
				if (rbHTML.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowHTML()', 200);</script>");
				if (rbLink.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowLink()', 200);</script>");
			}
		}
		#endregion

		#region btnSave_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			bool valid = true;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			ftbDescription.Text = HttpUtility.HtmlEncode(ftbDescription.Text);

			int iProjectId = ProjectId;
			if (ucProject.Visible)
				iProjectId = ucProject.ObjectId;

			TimeSpan ts = new TimeSpan(ucTaskTime.Value.Ticks);
			int Minutes = (int)ts.TotalMinutes;

			ArrayList alCategories = new ArrayList();
			for (int i = 0; i < lbCategory.Items.Count; i++)
				if (lbCategory.Items[i].Selected)
					alCategories.Add(int.Parse(lbCategory.Items[i].Value));

			PrimaryKeyId OrgUid = PrimaryKeyId.Empty;
			PrimaryKeyId ContactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				OrgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				ContactUid = ClientControl.ObjectId;

			if (DocumentId > 0)
			{
				Mediachase.IBN.Business.Document2.Update(DocumentId, txtTitle.Text, ftbDescription.Text,
					int.Parse(ddlPriority.SelectedItem.Value),
					int.Parse(ddlManager.SelectedItem.Value),
					Minutes, alCategories, 
					ContactUid, OrgUid);
			}
			else
			{
				if (rbFile.Checked && fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0)
				{
					DocumentId = Document.Create(txtTitle.Text, ftbDescription.Text, iProjectId,
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlManager.SelectedItem.Value),
						1, Minutes,
						alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream,
						ContactUid, OrgUid);
				}
				else if (rbHTML.Checked && fckEditor.Text != null && fckEditor.Text != String.Empty)
				{
					MemoryStream memStream = new MemoryStream();
					StreamWriter writer = new StreamWriter(memStream);
					writer.Write(fckEditor.Text);
					writer.Flush();
					memStream.Seek(0, SeekOrigin.Begin);
					String title = String.Empty;
					if (tbHtmlFileTitle.Text != String.Empty) title = tbHtmlFileTitle.Text;
					else title = "HTML Attachment";
					string html_filename = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title);
					html_filename = String.Concat(html_filename, ".html");

					DocumentId = Document.Create(txtTitle.Text, ftbDescription.Text, iProjectId,
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlManager.SelectedItem.Value),
						1, Minutes,
						alCategories, html_filename, memStream, ContactUid, OrgUid);
				}
				else if (rbLink.Checked && tbLink.Text != String.Empty)
				{
					string data = string.Format("[InternetShortcut]\r\nURL={0}", tbLink.Text);
					MemoryStream memStream = new MemoryStream();
					StreamWriter writer = new StreamWriter(memStream, Encoding.Unicode);
					writer.Write(data);
					writer.Flush();
					memStream.Seek(0, SeekOrigin.Begin);
					string title = tbLinkTitle.Text;
					string html_filename = Mediachase.UI.Web.Util.CommonHelper.GetHtmlFileTitle(title);
					if (html_filename.IndexOf(".url") < 0)
						html_filename += ".url";
					DocumentId = Document.Create(txtTitle.Text, ftbDescription.Text, iProjectId,
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlManager.SelectedItem.Value),
						1, Minutes,
						alCategories, html_filename, memStream, ContactUid, OrgUid);
				}
				else if (fAssetFile.PostedFile == null)
				{
					DocumentId = Document.Create(txtTitle.Text, ftbDescription.Text, iProjectId,
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlManager.SelectedItem.Value),
						1, Minutes,
						alCategories, null, null, ContactUid, OrgUid);
				}
				else
				{
					vFile.Visible = true;
					valid = false;
				}
			}
			if (DocumentId != 0 && valid)
			{
				EditControl.Save(DocumentId);

				if (cbOneMore.Checked)
				{
					if (ProjectId > 0)
						Response.Redirect("~/Documents/DocumentEdit.aspx?Checked=1&ProjectId=" + ProjectId);
					else
						Response.Redirect("~/Documents/DocumentEdit.aspx?Checked=1");
				}
				else
					Response.Redirect("~/Documents/DocumentView.aspx?DocumentId=" + DocumentId);
			}
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (ProjectId > 0)
				Response.Redirect("~/Projects/ProjectView.aspx?ProjectId=" + ProjectId);
			else if (DocumentId > 0)
				Response.Redirect("~/Documents/DocumentView.aspx?DocumentId=" + DocumentId);
			else
				Response.Redirect("~/Documents/default.aspx");
		}
		#endregion

	}
}
