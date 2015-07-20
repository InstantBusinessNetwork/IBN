namespace Mediachase.UI.Web.ToDo.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using System.IO;
	using System.Text;
	using System.Web.UI;
	using System.Globalization;

	using Mediachase.UI.Web.Util;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.UI.WebControls;
	using System.Collections.Generic;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Data;

	/// <summary>
	///		Summary description for ToDoEdit.
	/// </summary>
	public partial class ToDoEdit : System.Web.UI.UserControl
	{
		#region Variables
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.ToDo.Resources.strToDoEdit", typeof(ToDoEdit).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskEdit", typeof(ToDoEdit).Assembly);
		private int ToDoID = 0;
		private int IncidentID = 0;
		private int DocumentID = 0;
		private int TaskID = 0;
		private int ProjID = 0;
		private int _successorToDoID = 0;
		private int _predecessorToDoID = 0;
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			fckEditor.Language = Security.CurrentUser.Culture;
			fckEditor.EnableSsl = Request.IsSecureConnection;
			fckEditor.SslUrl = ResolveUrl("~/Common/Empty.html");


			if (Request["ToDoID"] != null)
			{
				ToDoID = int.Parse(Request["ToDoID"]);
				btnSaveAssign.Visible = false;
				Button1.Visible = false;
			}
			if (Request["ProjectID"] != null) ProjID = int.Parse(Request["ProjectID"]);
			if (Request["IncidentID"] != null) IncidentID = int.Parse(Request["IncidentID"]);
			if (Request["DocumentID"] != null) DocumentID = int.Parse(Request["DocumentID"]);
			if (Request["TaskID"] != null) TaskID = int.Parse(Request["TaskID"]);

			//2008-10-29 AK
			if (Request["SuccID"] != null) _successorToDoID = int.Parse(Request["SuccID"]);
			if (Request["PredID"] != null) _predecessorToDoID = int.Parse(Request["PredID"]);

			if (IncidentID != 0 && !Incident.CanAddToDo(IncidentID))
				throw new Exception("Can't add ToDo for this incident");
			if (DocumentID != 0 && !Document.CanAddToDo(DocumentID))
				throw new Exception("Can't add ToDo for this document");
			if (TaskID != 0 && !Task.CanAddToDo(TaskID))
				throw new Exception("Can't add ToDo for this task");
			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ")");
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("EditDictionary"));
			btnAddGeneralCategory.Visible = Security.IsManager();

			vFile.Visible = false;
			if (!Page.IsPostBack)
			{
				BindValues();
				BindVisibility();
				ApplyLocalization();
			}
			if (ToDoID > 0)
			{
				cbOneMore.Visible = false;
				trAttachFile.Visible = false;
			}
			else
				cbOneMore.Visible = true;
			if (Request["OldToDoID"] != null && Request["Assign"] != null)
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_OldToDoResEdit");
				string cmd = cm.AddCommand("ToDo", "", "ToDoView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
					 "function OpenAssignWizard(){" + cmd + "} setTimeout('OpenAssignWizard()', 400);", true);
			}

			if (Request["Checked"] != null)
				cbOneMore.Checked = true;
		}
		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (Page.IsPostBack)
			{
				trHTML1.Style.Add("display", "");
				trHTML2.Style.Add("display", "");
				if (rbFile.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowFile()', 200);</script>");
				if (rbHTML.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowHTML()', 200);</script>");
				if (rbLink.Checked)
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>window.setTimeout('ShowLink()', 200);</script>");

				ListBox lbTmp = lbCategory;
				ArrayList alCategories = new ArrayList();
				for (int i = 0; i < lbTmp.Items.Count; i++)
					if (lbTmp.Items[i].Selected)
						alCategories.Add(int.Parse(lbTmp.Items[i].Value));
				BindCategories();
				for (int i = 0; i < alCategories.Count; i++)
				{
					ListItem liCategory = lbTmp.Items.FindByValue(alCategories[i].ToString());
					if (liCategory != null)
						liCategory.Selected = true;
				}
			}
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblTitleTitle.Text = LocRM.GetString("Title");
			lblProjectTitle.Text = LocRM.GetString("Project");
			lblManagerTitle.Text = LocRM.GetString("Manager");
			lblStartDateTitle.Text = LocRM.GetString("StartDate");
			lblEndDateTitle.Text = LocRM.GetString("EndDate");
			lblPriorityTitle.Text = LocRM.GetString("Priority");
			lblCompletionTitle.Text = LocRM.GetString("Completion");
			lblDescriptionTitle.Text = LocRM.GetString("Description");
			chbMustBeConfirmed.Text = LocRM.GetString("MustConfirm");
			lblIncidentTitle.Text = LocRM.GetString("Incident");
			lblDocumentTitle.Text = LocRM.GetString("Document");
			lblTaskTitle.Text = LocRM.GetString("Task");
			lblActivationTitle.Text = LocRM.GetString("activation");
			if (IncidentID > 0)
				cbCompleteAfterToDo.Text = LocRM.GetString("CompleteAferToDo");
			else if (DocumentID > 0)
				cbCompleteAfterToDo.Text = LocRM.GetString("CompleteDocumentAferToDo");
			else if (TaskID > 0)
				cbCompleteAfterToDo.Text = LocRM.GetString("CompleteTaskAferToDo");
			lblCategoryTitle.Text = LocRM.GetString("Category");
			cbOneMore.Text = LocRM.GetString("tOneMoreItem");
			lblTaskTimeTitle.Text = LocRM2.GetString("taskTime");
			lblClient.Text = LocRM2.GetString("tClient");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			if (ToDoID != 0)
				tbSave.Title = LocRM.GetString("tbsave_edit");
			else
				tbSave.Title = LocRM.GetString("tbsave_add");
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			btnSaveAssign.Text = LocRM.GetString("SaveAssign");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSaveAssign.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			if (ToDoID != 0)
			{
				btnSaveAssign.Attributes.Add("onclick", "DisableButtons();");
				btnSave.Attributes.Add("onclick", "DisableButtons();");
			}
			else
			{
				btnSave.Attributes.Add("onclick", "DisableButtons();ShowProgress();");
				btnSaveAssign.Attributes.Add("onclick", "DisableButtons();ShowProgress();");
			}

			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrCategoryInfo.AddText(LocRM.GetString("tCategoryInfo"));
			hdrAttach.AddText(LocRM.GetString("tAttachedFiles"));
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			trPriority.Visible = PortalConfig.CommonToDoAllowEditPriorityField;
			trTaskTime.Visible = PortalConfig.CommonToDoAllowEditTaskTimeField;
			trActivation.Visible = PortalConfig.ToDoAllowEditActivationTypeField;
			trCompletion.Visible = PortalConfig.ToDoAllowEditCompletionTypeField;
			trMustConfirm.Visible = PortalConfig.ToDoAllowEditMustConfirmField;
			trClient.Visible = PortalConfig.CommonToDoAllowEditClientField;
			trCategory.Visible = PortalConfig.CommonToDoAllowEditGeneralCategoriesField;
			trAttachFile.Visible = PortalConfig.ToDoAllowEditAttachmentField;
		} 
		#endregion

		#region BindValues

		private void BindCategories()
		{
			lbCategory.DataSource = Task.GetListCategoriesAll();
			lbCategory.DataTextField = "CategoryName";
			lbCategory.DataValueField = "CategoryId";
			lbCategory.DataBind();
		}

		private void BindValues()
		{
			ddlPriority.DataSource = ToDo.GetListPriorities();
			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataBind();
			
			ddlActivationType.DataSource = Task.GetListActivationTypes();
			ddlActivationType.DataTextField = "ActivationTypeName";
			ddlActivationType.DataValueField = "ActivationTypeId";
			ddlActivationType.DataBind();

			ddlCompletionType.DataSource = ToDo.GetListCompletionTypes();
			ddlCompletionType.DataTextField = "CompletionTypeName";
			ddlCompletionType.DataValueField = "CompletionTypeId";
			ddlCompletionType.DataBind();

			if (Configuration.ProjectManagementEnabled)
			{
				ucProject.ObjectTypeId = (int)ObjectTypes.Project;
				ucProject.ObjectId = -1;
			}

			BindCategories();

			dtcStartDate.DefaultTimeString = PortalConfig.WorkTimeStart;
			dtcEndDate.DefaultTimeString = PortalConfig.WorkTimeFinish;

			if (ToDoID == 0)	// New
			{
				ucTaskTime.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.ToDoDefaultValueTaskTimeField));
				CommonHelper.SafeSelect(ddlPriority, PortalConfig.ToDoDefaultValuePriorityField);
				CommonHelper.SafeSelect(ddlActivationType, PortalConfig.ToDoDefaultValueActivationTypeField);
				CommonHelper.SafeSelect(ddlCompletionType, PortalConfig.ToDoDefaultValueCompetionTypeField);
				chbMustBeConfirmed.Checked = bool.Parse(PortalConfig.ToDoDefaultValueMustConfirmField);
				ArrayList list = Common.StringToArrayList(PortalConfig.ToDoDefaultValueGeneralCategoriesField);
				foreach (int i in list)
					CommonHelper.SafeMultipleSelect(lbCategory, i.ToString());
				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.ToDoDefaultValueClientField, out contact_id, out org_id);
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
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>ShowFile();</script>");

				//2008-10-29 AK
				if (_predecessorToDoID > 0 || _successorToDoID > 0)
				{
					int _toDoId = (_predecessorToDoID > 0) ? _predecessorToDoID : _successorToDoID;
					using (IDataReader reader = ToDo.GetToDo(_toDoId))
					{
						if (reader.Read())
						{
							if(reader["ProjectId"] != DBNull.Value)
								ProjID = (int)reader["ProjectId"];
							if (reader["IncidentId"] != DBNull.Value)
								IncidentID = (int)reader["IncidentId"];
							if (reader["DocumentId"] != DBNull.Value)
								DocumentID = (int)reader["DocumentId"];
							if (reader["TaskId"] != DBNull.Value)
								TaskID = (int)reader["TaskId"];
						}
					}
				}

				// managers
				if (ProjID > 0)
					FillManagersByProject(ProjID);
				else
					FillManagers();

				CommonHelper.SafeSelect(ddlManager, Security.CurrentUser.UserID.ToString());
			}

			if (ToDoID != 0)
			{
				#region ToDoID != 0
				int managerId = -1;
				using (IDataReader reader = ToDo.GetToDo(ToDoID))
				{
					if (reader.Read())
					{
						if (reader["StartDate"] != DBNull.Value)
						{
							string sStartDate = reader["StartDate"].ToString();
							if (sStartDate != "")
								dtcStartDate.SelectedDate = DateTime.Parse(sStartDate);
						}
						if (reader["FinishDate"] != DBNull.Value)
						{
							string sEndDate = reader["FinishDate"].ToString();
							if (sEndDate != "")
								dtcEndDate.SelectedDate = DateTime.Parse(sEndDate);
						}
						txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());

						if (reader["Description"] != DBNull.Value)
							txtDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());

						if (reader["IncidentId"] != DBNull.Value)
						{
							IncidentID = (int)reader["IncidentId"];
							lblIncident.Text = "<a href=\"../Incidents/IncidentView.aspx?IncidentId=" + IncidentID + "\">" + reader["IncidentTitle"].ToString() + "</a>";
							cbCompleteAfterToDo.Checked = (bool)reader["CompleteIncident"];
						}
						if (reader["DocumentId"] != DBNull.Value)
						{
							DocumentID = (int)reader["DocumentId"];
							lblDocument.Text = "<a href=\"../Documents/DocumentView.aspx?DocumentId=" + DocumentID + "\">" + reader["DocumentTitle"].ToString() + "</a>";
							cbCompleteAfterToDo.Checked = (bool)reader["CompleteDocument"];
						}
						if (reader["TaskId"] != DBNull.Value)
						{
							TaskID = (int)reader["TaskId"];
							lblTask.Text = "<a href=\"../Tasks/TaskView.aspx?TaskId=" + TaskID + "\">" + reader["TaskTitle"].ToString() + "</a>";
							cbCompleteAfterToDo.Checked = (bool)reader["CompleteTask"];
						}

						if (reader["ProjectId"] != DBNull.Value)
						{
							ProjID = (int)reader["ProjectId"];
							ucProject.ObjectId = ProjID;
						}

						managerId = (int)reader["ManagerId"];

						if (reader["PriorityId"] != DBNull.Value)
						{
							ListItem lItem = ddlPriority.Items.FindByValue(reader["PriorityId"].ToString());
							if (lItem != null)
							{
								ddlPriority.ClearSelection();
								lItem.Selected = true;
							}
						}

						CommonHelper.SafeSelect(ddlActivationType, reader["ActivationTypeId"].ToString());
						CommonHelper.SafeSelect(ddlCompletionType, reader["CompletionTypeId"].ToString());

						chbMustBeConfirmed.Checked = (bool)reader["MustBeConfirmed"];

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
					}
				}

				// Managers
				if (ProjID > 0)
					FillManagersByProject(ProjID);
				else
					FillManagers();

				// Select current manager
				if (managerId > 0)
				{
					ListItem managerItem = ddlManager.Items.FindByValue(managerId.ToString());
					if (managerItem == null)
					{
						UserLight ul = UserLight.Load(managerId);
						ddlManager.Items.Add(new ListItem(ul.LastName + " " + ul.FirstName, managerId.ToString()));
					}
					CommonHelper.SafeSelect(ddlManager, managerId.ToString());
				}

				using (IDataReader reader = ToDo.GetListCategories(ToDoID))
				{
					if (reader != null)
						lbCategory.ClearSelection();
					while (reader.Read())
					{
						ListItem li = lbCategory.Items.FindByValue(reader["CategoryId"].ToString());
						if (li != null)
							li.Selected = true;
					}
				}

				if (!ToDo.CanChangeProject(ToDoID))
				{
					ProjectLink.Visible = true;
					ucProject.Visible = false;
				}
				else
				{
					ProjectLink.Visible = false;
					ucProject.Visible = true;
				}

				EditControl.ObjectId = ToDoID;
				EditControl.BindData();
				#endregion
			}
			if (IncidentID > 0)
			{
				#region IncidentID > 0
				trIncident.Visible = true;
				trStatus.Visible = true;
				trDocument.Visible = false;
				trProject.Visible = false;
				trTask.Visible = false;
				if (ToDoID == 0)
					lblIncident.Text = "<a href=\"../Incidents/IncidentView.aspx?IncidentId=" + IncidentID + "\">" + Incident.GetTitle(IncidentID) + "</a>";
				#endregion
			}
			else if (DocumentID > 0)
			{
				#region DocumentID > 0
				trDocument.Visible = true;
				trStatus.Visible = true;
				trIncident.Visible = false;
				trProject.Visible = false;
				trTask.Visible = false;
				if (ToDoID == 0)
					lblDocument.Text = "<a href=\"../Documents/DocumentView.aspx?DocumentId=" + DocumentID + "\">" + Document.GetTitle(DocumentID) + "</a>";
				#endregion
			}
			else
			{
				#region else
				trIncident.Visible = false;
				trDocument.Visible = false;
				trStatus.Visible = false;

				trProject.Visible = Configuration.ProjectManagementEnabled;

				if (TaskID == 0)
				{
					trTask.Visible = false;
				}
				else
				{
					trStatus.Visible = true;
					trProject.Visible = false;
					if (ToDoID == 0)
					{
						using (IDataReader reader = Task.GetTask(TaskID))
						{
							if (reader.Read())
								lblTask.Text = "<a href=\"../Tasks/TaskView.aspx?TaskId=" + TaskID + "\">" + reader["Title"].ToString() + "</a>";
						}
					}
				}

				if (ProjID != 0)
				{
					ucProject.ObjectId = ProjID;
					ucProject.Visible = false;
					ProjectLink.Visible = true;
					ProjectLink.Text = Project.GetProjectTitle(ProjID);
					ProjectLink.NavigateUrl = String.Format(CultureInfo.InvariantCulture, "~/Projects/ProjectView.aspx?ProjectId={0}", ProjID);
				}
				else if (ucProject.Visible)
				{
					ProjectLink.Visible = false;
				}
				#endregion
			}

			txtIncidentId.Text = IncidentID.ToString();
			txtDocumentId.Text = DocumentID.ToString();
			txtTaskId.Text = TaskID.ToString();
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
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator1_ServerValidate);

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

		#region btnSave_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();

			if (!Page.IsValid)
				return;

			bool valid = true;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);

			bool SaveAndAssign = false;
			if (sender == (object)btnSaveAssign)
				SaveAndAssign = true;

			IncidentID = int.Parse(txtIncidentId.Text);
			DocumentID = int.Parse(txtDocumentId.Text);
			TaskID = int.Parse(txtTaskId.Text);
			
			TimeSpan ts = new TimeSpan(ucTaskTime.Value.Ticks);
			int Minutes = (int)ts.TotalMinutes;

			ArrayList alCategories = new ArrayList();
			for (int i = 0; i < lbCategory.Items.Count; i++)
				if (lbCategory.Items[i].Selected)
					alCategories.Add(int.Parse(lbCategory.Items[i].Value));
			ArrayList res = new ArrayList();
			if (!SaveAndAssign) res.Add(Security.CurrentUser.UserID);

			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			int iManagerId = int.Parse(ddlManager.SelectedValue);

			if (IncidentID == 0 && TaskID == 0 && DocumentID == 0)
			{
				int iProjectId = ucProject.ObjectId;

				if (ToDoID != 0)
					Mediachase.IBN.Business.ToDo2.UpdateWithProject(ToDoID, iProjectId,
						iManagerId, txtTitle.Text, txtDescription.Text, dtcStartDate.SelectedDate, dtcEndDate.SelectedDate,
						int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
						chbMustBeConfirmed.Checked, Minutes,
						alCategories, contactUid, orgUid);
				else
				{
					if (rbFile.Checked && (fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						ToDoID = Mediachase.IBN.Business.ToDo.Create(iProjectId, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, Minutes, alCategories, fAssetFile.PostedFile.FileName,
							fAssetFile.PostedFile.InputStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.Create(iProjectId, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, Minutes, alCategories, html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.Create(iProjectId, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, Minutes, alCategories, html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					}
					else if (fAssetFile.PostedFile == null)
						ToDoID = Mediachase.IBN.Business.ToDo.Create(iProjectId, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, Minutes, alCategories, null, null, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
			}
			else if (TaskID != 0)
			{
				if (ToDoID != 0)
					Mediachase.IBN.Business.ToDo2.UpdateTaskToDo(ToDoID, TaskID, iManagerId, txtTitle.Text, txtDescription.Text,
						dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
						chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories, contactUid, orgUid);
				else
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						ToDoID = Mediachase.IBN.Business.ToDo.CreateToDoForTask(TaskID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.CreateToDoForTask(TaskID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.CreateToDoForTask(TaskID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					}
					else if (fAssetFile.PostedFile == null)
						ToDoID = Mediachase.IBN.Business.ToDo.CreateToDoForTask(TaskID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							null, null, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
			}
			else if (IncidentID != 0)
			{
				if (ToDoID != 0)
					Mediachase.IBN.Business.ToDo2.UpdateIssueToDo(ToDoID, IncidentID, iManagerId, txtTitle.Text, txtDescription.Text,
						dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
						chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories, contactUid, orgUid);
				else
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						ToDoID = Mediachase.IBN.Business.ToDo.Create(IncidentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.Create(IncidentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.Create(IncidentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					}
					else if (fAssetFile.PostedFile == null)
						ToDoID = Mediachase.IBN.Business.ToDo.Create(IncidentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							null, null, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
			}
			else if (DocumentID != 0)
			{
				if (ToDoID != 0)
					Mediachase.IBN.Business.ToDo2.UpdateDocumentToDo(ToDoID, DocumentID, iManagerId, txtTitle.Text, txtDescription.Text,
						dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
						chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories, contactUid, orgUid);
				else
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						ToDoID = Mediachase.IBN.Business.ToDo.CreateDocumentTodo(DocumentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.CreateDocumentTodo(DocumentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
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
						ToDoID = Mediachase.IBN.Business.ToDo.CreateDocumentTodo(DocumentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes, alCategories,
							html_filename, memStream, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					}
					else if (fAssetFile.PostedFile == null)
						ToDoID = Mediachase.IBN.Business.ToDo.CreateDocumentTodo(DocumentID, iManagerId, txtTitle.Text, txtDescription.Text,
							dtcStartDate.SelectedDate, dtcEndDate.SelectedDate, int.Parse(ddlPriority.SelectedItem.Value),
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, cbCompleteAfterToDo.Checked, Minutes,
							alCategories, null, null, res, contactUid, orgUid, _predecessorToDoID, _successorToDoID);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
			}

			if (ToDoID > 0 && valid)
			{
				EditControl.Save(ToDoID);

				if (!SaveAndAssign)
				{
					if (!cbOneMore.Checked)
						Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoView.aspx?ToDoID=" + ToDoID, Response);
					else
					{
						if (IncidentID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&IncidentID=" + IncidentID, Response);
						else if (DocumentID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&DocumentID=" + DocumentID, Response);
						else if (TaskID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&TaskID=" + TaskID, Response);
						else if (ProjID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&ProjectID=" + ProjID, Response);
						else
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1", Response);
					}
				}
				else
				{
					if (!cbOneMore.Checked)
						Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoView.aspx?ToDoID=" + ToDoID + "&Assign=1", Response);
					else
					{
						if (IncidentID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&OldToDoID=" + ToDoID + "&Assign=1&IncidentID=" + IncidentID, Response);
						else if (DocumentID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&OldToDoID=" + ToDoID + "&Assign=1&DocumentID=" + DocumentID, Response);
						else if (TaskID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&OldToDoID=" + ToDoID + "&Assign=1&TaskID=" + TaskID, Response);
						else if (ProjID > 0)
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&OldToDoID=" + ToDoID + "&Assign=1&ProjectID=" + ProjID, Response);
						else
							Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../ToDo/ToDoEdit.aspx?Checked=1&OldToDoID=" + ToDoID + "&Assign=1", Response);
					}
				}
			}
		}

		#endregion

		#region CustomValidator1_ServerValidate
		private void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{

			DateTime dmv = DateTime.MinValue.AddDays(1);
			DateTime ed = dtcEndDate.SelectedDate;
			DateTime sd = dtcStartDate.SelectedDate;

			if (((ed > dmv) && (sd > dmv)) && (ed < sd))
			{
				//CustomValidator1.ErrorMessage = LocRM.GetString("tWrongDate");
				CustomValidator1.ErrorMessage = LocRM.GetString("EndDateError") + " (" + dtcStartDate.SelectedDate.ToShortDateString() + " " + dtcStartDate.SelectedDate.ToShortTimeString() + ")";
				args.IsValid = false;
			}
			else
				args.IsValid = true;

		}
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			string BackUrl = "../Workspace/default.aspx";
			if (ToDoID != 0)
				BackUrl = "ToDoView.aspx?ToDoID=" + ToDoID;
			else if (IncidentID != 0)
				BackUrl = "../Incidents/IncidentView.aspx?IncidentId=" + IncidentID;
			else if (DocumentID != 0)
				BackUrl = "../Documents/DocumentView.aspx?DocumentId=" + DocumentID;
			else if (TaskID != 0)
				BackUrl = "../Tasks/TaskView.aspx?TaskId=" + TaskID;
			else if (ProjID != 0)
				BackUrl = "../Projects/ProjectView.aspx?ProjectId=" + ProjID;
			Response.Redirect(BackUrl);
		}
		#endregion

		#region Button1_Click
		protected void Button1_Click(object sender, System.EventArgs e)
		{
			dtcStartDate.SelectedDate = DateTime.MinValue;
			dtcEndDate.SelectedDate = DateTime.MinValue;
		}
		#endregion

		#region lbChangeProject_Click
		protected void lbChangeProject_Click(object sender, EventArgs e)
		{
			string savedValue = ddlManager.SelectedValue;

			ddlManager.Items.Clear();
			int projectId = ucProject.ObjectId;
			if (projectId > 0)
				FillManagersByProject(projectId);
			else
				FillManagers();

			CommonHelper.SafeSelect(ddlManager, savedValue);
		}
		#endregion
	}
}
