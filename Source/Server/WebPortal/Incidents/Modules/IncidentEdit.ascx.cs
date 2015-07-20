namespace Mediachase.UI.Web.Incidents.Modules
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Globalization;
	using System.IO;
	using System.Resources;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business;
	using Mediachase.IBN.Business.EMail;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Data;
	using System.Reflection;

	/// <summary>
	///		Summary description for IncidentEdit
	/// </summary>
	public partial class IncidentEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentEdit", typeof(IncidentEdit).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskEdit", typeof(IncidentEdit).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.Ibn.WebResources.App_GlobalResources.Admin.Resources.strMailIncidents", Assembly.Load(new AssemblyName("Mediachase.Ibn.WebResources")));
		protected ResourceManager LocRM4 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Incidents.Resources.strIncidentGeneral", typeof(IncidentEdit).Assembly);
		private int ProjectId = 0;
		private int IncidentId = 0;
		private int EMailMessageId = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.HelpDeskEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.ToolbarBackgroundImage = false;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");
			//fckEditor.Text = "test";

			if (Request["IncidentId"] != null)
				IncidentId = int.Parse(Request["IncidentId"]);
			if (Request["ProjectId"] != null)
				ProjectId = int.Parse(Request["ProjectId"]);
			if (Request["EMailMessageId"] != null)
				EMailMessageId = int.Parse(Request["EMailMessageId"]);

			vFile.Visible = false;

			if (!Page.IsPostBack)
			{
				BindVisibility();
				BindDefaultValues();
				if (IncidentId != 0)
					BindSavedValues();
				else if (EMailMessageId > 0)
					BindEmailValues();
			}
			if (IncidentId > 0 || EMailMessageId > 0)
				cbOneMore.Visible = false;
			else
				cbOneMore.Visible = true;
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ",'" + btnRefreshGen.ClientID + "')");
			btnAddIncidentCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.IncidentCategories + ",'" + btnRefreshInc.ClientID + "')");
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("EditDictionary"));
			btnAddIncidentCategory.Attributes.Add("title", LocRM.GetString("EditDictionary"));
			btnAddGeneralCategory.Visible = Security.IsManager();
			btnAddIncidentCategory.Visible = Security.IsManager();

			if (IncidentId != 0)
			{
				trHtmlAttach.Visible = false;
				btnSave.Attributes.Add("onclick", "DisableButtons(this);"); 
			}
			else if (EMailMessageId <= 0)
			{
				btnSave.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");
			}
			else
			{
				btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			}
 			BindToolbar();

			if (Request["Checked"] != null)
				cbOneMore.Checked = true;
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (Page.IsPostBack)
			{
				trHTML1.Style.Add("display", "");
				trHTML2.Style.Add("display", "");
				if (rbFile.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>ShowFile();</script>");
				if (rbHTML.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>ShowHTML();</script>");
				if (rbLink.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>ShowLink();</script>");

				ListBox lbTmp = lbCategory;
				ArrayList alCategories = new ArrayList();
				for (int i = 0; i < lbTmp.Items.Count; i++)
					if (lbTmp.Items[i].Selected)
						alCategories.Add(int.Parse(lbTmp.Items[i].Value));
				BindGeneralCategories();
				for (int i = 0; i < alCategories.Count; i++)
				{
					ListItem liCategory = lbTmp.Items.FindByValue(alCategories[i].ToString());
					if (liCategory != null)
						liCategory.Selected = true;
				}

				ListBox lbTmp1 = lbIncidentCategory;
				ArrayList alCategories1 = new ArrayList();
				for (int i = 0; i < lbTmp1.Items.Count; i++)
					if (lbTmp1.Items[i].Selected)
						alCategories1.Add(int.Parse(lbTmp1.Items[i].Value));
				BindIncidentCategories();
				for (int i = 0; i < alCategories1.Count; i++)
				{
					ListItem liCategory = lbTmp1.Items.FindByValue(alCategories1[i].ToString());
					if (liCategory != null)
						liCategory.Selected = true;
				}
			}
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			trPriority.Visible = PortalConfig.CommonIncidentAllowEditPriorityField;
			trTaskTime.Visible = PortalConfig.CommonIncidentAllowEditTaskTimeField;
			trExpAssTime.Visible = PortalConfig.IncidentAllowEditExpAssDeadlineField;
			trExpReplyTime.Visible = PortalConfig.IncidentAllowEditExpReplyDeadlineField;
			trExpDurationTime.Visible = PortalConfig.IncidentAllowEditExpResolutionDeadlineField;
			trClient.Visible = PortalConfig.CommonIncidentAllowEditClientField;
			trType.Visible = PortalConfig.IncidentAllowEditTypeField;
			trSeverity.Visible = PortalConfig.IncidentAllowEditSeverityField;
			trCategories.Visible = PortalConfig.CommonIncidentAllowEditGeneralCategoriesField;
			trIssCategories.Visible = PortalConfig.IncidentAllowEditIncidentCategoriesField;
			trHtmlAttach.Visible = (IncidentId <= 0) && PortalConfig.IncidentAllowEditAttachmentField;

			bool showProject = false;
			if (Configuration.ProjectManagementEnabled)
			{
				using (IDataReader reader = Project.GetListProjectsByKeyword(""))
				{
					if (reader.Read())
						showProject = true;
				}
			}
			trProject.Visible = showProject;
		} 
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (IncidentId != 0)
				tbSave.Title = LocRM.GetString("tbSaveEdit");
			else
				tbSave.Title = LocRM.GetString("tbSaveCreate");

			string backlink = ResolveClientUrl("~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbSaveGoToList"), backlink);
			btnSave.Text = LocRM.GetString("tbSaveSave");
			btnCancel.Text = LocRM.GetString("tbSaveCancel");
 
			cbOneMore.Text = LocRM.GetString("tOneMoreItem");
			//lgdBasicInfo.InnerText = LocRM.GetString("tBasicInfo");
			//lgdCategoryInfo.InnerText = LocRM.GetString("tCategoryInfo");
			//lgdAttach.InnerText = LocRM.GetString("tAttachedFiles");
			lgdEmail.InnerText = LocRM.GetString("tEmailInfo");
			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrCategoryInfo.AddText(LocRM.GetString("tCategoryInfo"));
			hdrAttach.AddText(LocRM.GetString("tAttachedFiles"));
		}
		#endregion

		#region BindDictionaries
		private void BindGeneralCategories()
		{
			lbCategory.DataSource = Incident.GetListCategoriesAll();
			lbCategory.DataTextField = "CategoryName";
			lbCategory.DataValueField = "CategoryId";
			lbCategory.DataBind();
		}

		private void BindIncidentCategories()
		{
			lbIncidentCategory.DataSource = Incident.GetListIncidentCategories();
			lbIncidentCategory.DataTextField = "CategoryName";
			lbIncidentCategory.DataValueField = "CategoryId";
			lbIncidentCategory.DataBind();
		}
		#endregion

		#region BindDefaultValues
		private void BindDefaultValues()
		{
			ucTaskTime.Value = DateTime.MinValue.AddHours(3);
			ucExpectedDuration.Value = DateTime.MinValue.AddDays(7);
			ucExpectedResponseTime.Value = DateTime.MinValue.AddDays(1);
			ucExpectedAssignTime.Value = DateTime.MinValue.AddHours(8);

			ddlPriority.DataSource = Incident.GetListPriorities();
			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataBind();
			
			ddlType.DataSource = Incident.GetListIncidentTypes();
			ddlType.DataTextField = "TypeName";
			ddlType.DataValueField = "TypeId";
			ddlType.DataBind();
			
			BindGeneralCategories();
			BindIncidentCategories();
			
			ddlSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddlSeverity.DataTextField = "SeverityName";
			ddlSeverity.DataValueField = "SeverityId";
			ddlSeverity.DataBind();

			if (IncidentId == 0 && ProjectId > 0)
			{
				ddProject.ObjectTypeId = (int)ObjectTypes.Project;
				ddProject.ObjectId = ProjectId;
				ddProject.Visible = false;
				lblProject.Text = Project.GetProjectTitle(ProjectId);
			}

			if (IncidentId == 0 && EMailMessageId <= 0)
			{
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

			if (EMailMessageId <= 0 && IncidentId <= 0)
				ddlFolder.Items.Add(new ListItem(LocRM.GetString("AutoDetection"), "-1"));
			foreach (IncidentBox folder in IncidentBox.List())
				ddlFolder.Items.Add(new ListItem(folder.Name, folder.IncidentBoxId.ToString()));

			if (IncidentId == 0)
			{
				ucTaskTime.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.IncidentDefaultValueTaskTimeField));
				ucExpectedDuration.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.IncidentDefaultValueExpResolutionDeadlineField));
				ucExpectedResponseTime.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.IncidentDefaultValueExpReplyDeadlineField));
				ucExpectedAssignTime.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.IncidentDefaultValueExpAssDeadlineField));

				CommonHelper.SafeSelect(ddlPriority, PortalConfig.IncidentDefaultValuePriorityField);
				CommonHelper.SafeSelect(ddlSeverity, PortalConfig.IncidentDefaultValueSeverityField);
				CommonHelper.SafeSelect(ddlType, PortalConfig.IncidentDefaultValueTypeField);

				ArrayList list = Common.StringToArrayList(PortalConfig.IncidentDefaultValueGeneralCategoriesField);
				foreach(int i in list)
					CommonHelper.SafeMultipleSelect(lbCategory, i.ToString());
				list = Common.GetDefaultIncidentCategories();
				foreach(int i in list)
					CommonHelper.SafeMultipleSelect(lbIncidentCategory, i.ToString());

				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.IncidentDefaultValueClientField, out contact_id, out org_id);
				if(contact_id != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = contact_id;
				}
				else if(org_id != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = org_id;
				}
			}
		}
		#endregion

		#region BindSavedValues
		private void BindSavedValues()
		{

			using (IDataReader reader = Incident.GetIncident(IncidentId))
			{
				if (reader.Read())
				{
					if (reader["ProjectId"] != DBNull.Value)
					{
						lblProject.Text = reader["ProjectTitle"].ToString();
						ddProject.ObjectTypeId = (int)ObjectTypes.Project;
						ddProject.ObjectId = (int)reader["ProjectId"];
					}
					else
					{
						lblProject.Text = LocRM.GetString("ProjectNotSet");
						ddProject.ObjectTypeId = (int)ObjectTypes.Project;
						ddProject.ObjectId = -1;
					}

					CommonHelper.SafeSelect(ddlPriority, reader["PriorityId"].ToString());
					CommonHelper.SafeSelect(ddlType, reader["TypeId"].ToString());
					CommonHelper.SafeSelect(ddlSeverity, reader["SeverityId"].ToString());
					if (reader["IncidentBoxId"] != DBNull.Value)
						CommonHelper.SafeSelect(ddlFolder, reader["IncidentBoxId"].ToString());

					txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
					if (reader["Description"] != DBNull.Value)
						ftbDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());

					ucTaskTime.Value = DateTime.MinValue.AddMinutes((int)reader["TaskTime"]);
					ucExpectedDuration.Value = DateTime.MinValue.AddMinutes((int)reader["ExpectedDuration"]);
					ucExpectedResponseTime.Value = DateTime.MinValue.AddMinutes((int)reader["ExpectedResponseTime"]);
					ucExpectedAssignTime.Value = DateTime.MinValue.AddMinutes((int)reader["ExpectedAssignTime"]);

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
				}
			}

			using (IDataReader reader = Incident.GetListCategories(IncidentId))
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbCategory, reader["CategoryId"].ToString());
			}

			using (IDataReader reader = Incident.GetListIncidentCategoriesByIncident(IncidentId))
			{
				while (reader.Read())
					CommonHelper.SafeMultipleSelect(lbIncidentCategory, reader["CategoryId"].ToString());
			}

			if (trProject.Visible && !Incident.CanChangeProject(IncidentId))
			{
				lblProject.Visible = true;
				ddProject.Visible = false;
			}
			else
			{
				lblProject.Visible = false;
				ddProject.Visible = true;
			}

			EditControl.ObjectId = IncidentId;
			EditControl.BindData();
		}
		#endregion

		#region BindEmailValues
		private void BindEmailValues()
		{
			EMailMessageInfo mi = EMailMessageInfo.Load(EMailMessageId);

			IncidentInfo incidentInfo = EMailIncidentMappingHandler.CreateMapping(EMailMessageId);

			// Eval IncidentBox
			ddlFolder.SelectedValue = IncidentBoxRule.Evaluate(incidentInfo).IncidentBoxId.ToString();

			txtTitle.Text = incidentInfo.Title;
			ftbDescription.Text = incidentInfo.Description;
			CommonHelper.SafeSelect(ddlPriority, incidentInfo.PriorityId.ToString());
			CommonHelper.SafeSelect(ddlSeverity, incidentInfo.SeverityId.ToString());
			CommonHelper.SafeSelect(ddlType, incidentInfo.TypeId.ToString());
			ddProject.ObjectTypeId = (int)ObjectTypes.Project;
			ddProject.ObjectId = incidentInfo.ProjectId;

			if (incidentInfo.GeneralCategories != null)
				foreach (int CatId in incidentInfo.GeneralCategories)
					CommonHelper.SafeMultipleSelect(lbCategory, CatId.ToString());
			if (incidentInfo.IncidentCategories != null)
				foreach (int CatId in incidentInfo.IncidentCategories)
					CommonHelper.SafeMultipleSelect(lbIncidentCategory, CatId.ToString());

			//try from MailSenderEmail
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			Client client = Common.GetClient(incidentInfo.MailSenderEmail);
			if (client != null)
			{
				if (client.IsContact)
				{
					contactUid = client.Id;

					ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = contactUid;
				}
				else
				{
					orgUid = client.Id;

					ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = orgUid;
				}
			}

			
			//from incidentinfo

			if (orgUid == PrimaryKeyId.Empty && contactUid == PrimaryKeyId.Empty)
			{
				if (incidentInfo.OrgUid != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = OrganizationEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = incidentInfo.OrgUid;
				}
				else if (incidentInfo.ContactUid != PrimaryKeyId.Empty)
				{
					ClientControl.ObjectType = ContactEntity.GetAssignedMetaClassName();
					ClientControl.ObjectId = incidentInfo.ContactUid;
				}
			}

			trHtmlAttach.Visible = false;
			trEmail.Visible = true;

			string sBody = "";
			if (mi.HtmlBody != null)
				sBody = EMailMessageInfo.CutHtmlBody(mi.HtmlBody, 256, "...");

			if (sBody.Trim() != "")
				lblEmail.Text = String.Format("{0}<p align=right class='text'><a href=\"javascript:OpenSizableWindow('EMailView.aspx?EMailId={2}', 750, 550)\"><b>{1}</b></a>", sBody, LocRM4.GetString("More"), EMailMessageId);
			else
				lblEmail.Text = sBody;
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

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
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

			TimeSpan ts;

			ArrayList alCategories = new ArrayList();
			for (int i = 0; i < lbCategory.Items.Count; i++)
				if (lbCategory.Items[i].Selected)
					alCategories.Add(int.Parse(lbCategory.Items[i].Value));

			ArrayList alIncidentCategories = new ArrayList();
			for (int i = 0; i < lbIncidentCategory.Items.Count; i++)
				if (lbIncidentCategory.Items[i].Selected)
					alIncidentCategories.Add(int.Parse(lbIncidentCategory.Items[i].Value));

			ts = new TimeSpan(ucTaskTime.Value.Ticks);
			int Minutes = (int)ts.TotalMinutes;

			ts = new TimeSpan(ucExpectedResponseTime.Value.Ticks);
			int ExpectedResponseTime = (int)ts.TotalMinutes;

			ts = new TimeSpan(ucExpectedAssignTime.Value.Ticks);
			int ExpectedAssignTime = (int)ts.TotalMinutes;

			ts = new TimeSpan(ucExpectedDuration.Value.Ticks);
			int ExpectedDuration = (int)ts.TotalMinutes;

			int IncidentBoxId = int.Parse(ddlFolder.SelectedValue);

			int CreatorId = Security.CurrentUser.UserID;
			if (ViewState["CreatorId"] != null)
				CreatorId = (int)ViewState["CreatorId"];

			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else
				contactUid = ClientControl.ObjectId;
			int iProjectId = ProjectId;
			if (!Configuration.ProjectManagementEnabled || ddProject.Visible)
				iProjectId = ddProject.ObjectId;
			if (IncidentId != 0)
			{
				Issue2.Update(IncidentId, txtTitle.Text, ftbDescription.Text,
					iProjectId,
					int.Parse(ddlType.SelectedValue), int.Parse(ddlPriority.SelectedValue),
					int.Parse(ddlSeverity.SelectedValue), Minutes, IncidentBoxId, contactUid, orgUid,
					ExpectedResponseTime, ExpectedDuration, ExpectedAssignTime, alCategories, alIncidentCategories);
			}
			else
			{
				if (EMailMessageId > 0)
				{
					IncidentId = Incident.CreateFromEmail(txtTitle.Text, ftbDescription.Text,
						iProjectId,
						int.Parse(ddlType.SelectedValue), int.Parse(ddlPriority.SelectedValue),
						int.Parse(ddlSeverity.SelectedValue), Minutes, 
						ExpectedDuration, ExpectedResponseTime, ExpectedAssignTime,
						alCategories, alIncidentCategories, IncidentBoxId, EMailMessageId, orgUid, contactUid);
				}
				else if (rbFile.Checked && fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0)
				{

					IncidentId = Incident.Create(txtTitle.Text, ftbDescription.Text,
						iProjectId,
						int.Parse(ddlType.SelectedValue), int.Parse(ddlPriority.SelectedValue),
						int.Parse(ddlSeverity.SelectedValue), Minutes, 
						ExpectedDuration, ExpectedResponseTime, ExpectedAssignTime,
						CreatorId, alCategories, alIncidentCategories,
						fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream, false,
						DateTime.UtcNow, IncidentBoxId, contactUid, orgUid);
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

					IncidentId = Incident.Create(txtTitle.Text, ftbDescription.Text,
						iProjectId,
						int.Parse(ddlType.SelectedValue), int.Parse(ddlPriority.SelectedValue),
						int.Parse(ddlSeverity.SelectedValue), Minutes, 
						ExpectedDuration, ExpectedResponseTime, ExpectedAssignTime,
						CreatorId, alCategories, alIncidentCategories, html_filename, memStream, false, DateTime.UtcNow,
						IncidentBoxId, contactUid, orgUid);
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
					IncidentId = Incident.Create(txtTitle.Text, ftbDescription.Text,
						iProjectId,
						int.Parse(ddlType.SelectedValue), int.Parse(ddlPriority.SelectedValue),
						int.Parse(ddlSeverity.SelectedValue), Minutes, 
						ExpectedDuration, ExpectedResponseTime, ExpectedAssignTime,
						CreatorId, alCategories, alIncidentCategories, html_filename, memStream, false, DateTime.UtcNow,
						IncidentBoxId, contactUid, orgUid);
				}
				else if (fAssetFile.PostedFile == null)
					IncidentId = Incident.Create(txtTitle.Text, ftbDescription.Text,
						iProjectId,
						int.Parse(ddlType.SelectedValue), int.Parse(ddlPriority.SelectedValue),
						int.Parse(ddlSeverity.SelectedValue), Minutes, 
						ExpectedDuration, ExpectedResponseTime, ExpectedAssignTime,
						CreatorId, alCategories, alIncidentCategories, null, null, false, DateTime.UtcNow,
						IncidentBoxId, contactUid, orgUid);
				else
				{
					vFile.Visible = true;
					valid = false;
				}
			}

			EditControl.Save(IncidentId);

			if (IncidentId != 0 && valid)
			{
				if (cbOneMore.Checked)
				{
					if (ProjectId > 0)
						Response.Redirect("../Incidents/IncidentEdit.aspx?Checked=1&ProjectId=" + ProjectId);
					else
						Response.Redirect("../Incidents/IncidentEdit.aspx?Checked=1");
				}
				else
					Response.Redirect("../Incidents/IncidentView.aspx?IncidentId=" + IncidentId + "&Tab=Forum");
			}
		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			if (ProjectId != 0)
				Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjectId);
			else if (IncidentId != 0)
				Response.Redirect("../Incidents/IncidentView.aspx?IncidentId=" + IncidentId);
			else if (EMailMessageId != 0)
				Response.Redirect("../Incidents/EMailPendingMessages.aspx");
			else
			{
				string backlink = "~/Apps/HelpDeskManagement/Pages/IncidentListNew.aspx";
				Response.Redirect(backlink, true);
			}
		}
		#endregion

		#region ddlFolder_SelectedIndexChanged
		protected void ddlFolder_SelectedIndexChanged(object sender, EventArgs e)
		{
			int incidentBoxId = int.Parse(ddlFolder.SelectedValue, CultureInfo.InvariantCulture);
			if (incidentBoxId > 0)
			{
				IncidentBox box = IncidentBox.Load(incidentBoxId);
				if (box != null)
				{
					ucExpectedResponseTime.Value = DateTime.MinValue.AddMinutes(box.Document.GeneralBlock.ExpectedResponseTime);
					ucExpectedDuration.Value = DateTime.MinValue.AddMinutes(box.Document.GeneralBlock.ExpectedDuration);
					ucExpectedAssignTime.Value = DateTime.MinValue.AddMinutes(box.Document.GeneralBlock.ExpectedAssignTime);
					ucTaskTime.Value = DateTime.MinValue.AddMinutes(box.Document.GeneralBlock.TaskTime);
				}
			}
			else
			{
				ucExpectedDuration.Value = DateTime.MinValue.AddDays(7);
				ucExpectedResponseTime.Value = DateTime.MinValue.AddDays(1);
				ucExpectedAssignTime.Value = DateTime.MinValue.AddHours(8);
				ucTaskTime.Value = DateTime.MinValue.AddHours(3);
			}
		}
		#endregion
	}
}
