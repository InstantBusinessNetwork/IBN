namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Collections;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using System.Threading;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Clients;
	using Mediachase.Ibn.Data;


	/// <summary>
	///		Summary description for ProjectAdd.
	/// </summary>
	public partial class ProjectEdit : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(ProjectEdit).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(ProjectEdit).Assembly);
		private int PID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.ProjectManagementEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ",'" + btnRefreshGen.ClientID + "')");
			btnAddProjectCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.ProjectCategories + ",'" + btnRefreshProj.ClientID + "')");
			btnAddPhase.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.ProjectPhases + ",'" + btnRefreshPhase.ClientID + "');");
			btnAddPhase2.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.ProjectPhases + ",'" + btnRefreshPhase.ClientID + "');");
			btnAddPhase.Attributes.Add("title", LocRM.GetString("AddCategory"));
			btnAddPhase2.Attributes.Add("title", LocRM.GetString("AddCategory"));
			btnAddProjectCategory.Attributes.Add("title", LocRM.GetString("AddCategory"));
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("AddCategory"));

			if (Request["ProjectID"] != null) PID = int.Parse(Request["ProjectID"]);
			BindToolbar();
			ApplyLocalization();

			if (!Page.IsPostBack)
			{
				BindValues();
				if (Request["TemplateId"] != null)
					BindFromTemplate(int.Parse(Request["TemplateId"]));

				if (Request["OldProjectId"] != null && Request["TemplateId"] != null)
				{
					int PrjId = int.Parse(Request["OldProjectId"].ToString());
					int TemplateId = int.Parse(Request["TemplateId"].ToString());
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
						@"<script language=javascript>
					OpenAssignWizard(" + PrjId + "," + TemplateId + @");
					function OpenAssignWizard(ProjectId, TemplateId)
					{
						var w = 550;
						var h = 300;
						var l = (screen.width - w) / 2;
						var t = (screen.height - h) / 2;
						winprops = 'resizable=0, height='+h+',width='+w+',top='+t+',left='+l;
						var f = window.open('GenerateProjectByTemplate.aspx?ProjectId=' + ProjectId+'&TemplateId='+TemplateId, 'GenerateProject', winprops);
						if (f == null)
						{
							tW = document.getElementById('tblWarning');
							aW = document.getElementById('aAssignLink');
							if (tW != null && aW !=null)
							{
								tW.style.display = 'block';
								aW.href = 'javascript:OpenAssignWizard(" + PrjId + "," + TemplateId + @");';
								aW.innerHTML = '" + LocRM.GetString("AssignWizard1") + @"';
							}
						}
					}
				</script>");
				}
			}
			if (PID > 0)
			{
				trTemplate.Visible = false;
				cbOneMore.Visible = false;
			}
			else
			{
				cbOneMore.Visible = true;
			}

			
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblTitleTitle.Text = LocRM.GetString("title");
			lblManagerTitle.Text = LocRM.GetString("manager");
			lblDescriptionTitle.Text = LocRM.GetString("description");
			lblTypeTitle.Text = LocRM.GetString("type");
			lblGoalsTitle.Text = LocRM.GetString("goals");
			lblCategoryTitle.Text = LocRM.GetString("category");
			lblCalendarTitle.Text = LocRM.GetString("calendar");
			lblTargetStartDateTitle.Text = LocRM.GetString("target_start_date");
			lblActualStartDateTitle.Text = LocRM.GetString("actual_start_date");
			lblTargetEndDateTitle.Text = LocRM.GetString("target_finish_date");
			lblScopeTitle.Text = LocRM.GetString("scope");
			lblExecManagerTitle.Text = LocRM.GetString("exec_manager");
			lblDeliverablesTitle.Text = LocRM.GetString("deliverables");
			lblStatusTitle.Text = LocRM.GetString("status");
			lblActualFinishDateTitle.Text = LocRM.GetString("actual_finish_date");
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			lblProjectCategory.Text = LocRM.GetString("ProjectCategory");
			lblClientCustomer.Text = LocRM.GetString("Client");
			lblCurrencySymbol.Text = LocRM.GetString("ProjectCurrency");
			lblTemplateLabel.Text = LocRM.GetString("tTemplate");
			lblPriority.Text = LocRM.GetString("Priority");
			lblRiskLevel.Text = LocRM.GetString("risk_level");
			lblPortfolios.Text = LocRM.GetString("tPortfolio");
			cbOneMore.Text = LocRM.GetString("tCreateAnotherOne");
			lblPrjPhaseTitle.Text = LocRM.GetString("tPrjPhase");
			lblInitialPhase.Text = LocRM.GetString("tInitialPhase");
			lblOverallStatus.Text = LocRM.GetString("tOverallStatus");
			lblBlockType.Text = GetGlobalResourceObject("IbnFramework.TimeTracking", "TimeTrackingBlockType").ToString();
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{

			if (PID != 0)
				tbSave.Title = LocRM.GetString("tbsave_edit");
			else
				tbSave.Title = LocRM.GetString("tbsave_add");
			//tbSave.AddSeparator();
			//tbSave.AddLink("<img alt='' src='../Layouts/Images/SaveItem.gif'/> " + LocRM.GetString("tbsave_save"),"javascript: document.forms[0]." + btnSave.ClientID + ".click();");
			//tbSave.AddSeparator();

			string backlink = ResolveClientUrl("~/Apps/ProjectManagement/Pages/ProjectList.aspx");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbsave_gotolist"), backlink);

			hdrBasicInfo.AddText(LocRM.GetString("tBaseInfo"));
			hdrStatusInfo.AddText(LocRM.GetString("tStatusInfo"));
			hdrCategoryInfo.AddText(LocRM.GetString("tCategoryInfo"));
			hdrAdditionalInfo.AddText(LocRM.GetString("tAdditInfo"));

			hdrTimeline.AddText(LocRM2.GetString("timeline_info"));
		}
		#endregion

		#region BindValues

		private void BindGeneralCategories()
		{
			lbCategory.DataTextField = "CategoryName";
			lbCategory.DataValueField = "CategoryId";
			lbCategory.DataSource = Project.GetListCategoriesAll();
			lbCategory.DataBind();
		}

		private void BindProjectCategories()
		{
			lbProjectCategory.DataTextField = "CategoryName";
			lbProjectCategory.DataValueField = "CategoryId";
			lbProjectCategory.DataSource = Project.GetListProjectCategories();
			lbProjectCategory.DataBind();
		}

		private void BindPortfolios()
		{
			lbPortfolios.DataTextField = "Title";
			lbPortfolios.DataValueField = "ProjectGroupId";
			lbPortfolios.DataSource = ProjectGroup.GetProjectGroups();
			lbPortfolios.DataBind();
		}

		private void BindValues()
		{
			ddlTemplate.DataSource = ProjectTemplate.GetListProjectTemplate();
			ddlTemplate.DataTextField = "TemplateName";
			ddlTemplate.DataValueField = "TemplateId";
			ddlTemplate.DataBind();
			ddlTemplate.Items.Insert(0, new ListItem(LocRM.GetString("NotSet"), "0"));
			if (Request["TemplateId"] != null)
				CommonHelper.SafeSelect(ddlTemplate, Request["TemplateId"].ToString());

			ddlStatus.DataSource = Project.GetListProjectStatus();
			ddlStatus.DataTextField = "StatusName";
			ddlStatus.DataValueField = "StatusId";
			ddlStatus.DataBind();

			ddPrjPhases.DataSource = Project.GetListProjectPhases();
			ddPrjPhases.DataTextField = "PhaseName";
			ddPrjPhases.DataValueField = "PhaseId";
			ddPrjPhases.DataBind();

			ddInitialPhase.DataSource = Project.GetListProjectPhases();
			ddInitialPhase.DataTextField = "PhaseName";
			ddInitialPhase.DataValueField = "PhaseId";
			ddInitialPhase.DataBind();

			ddlType.DataValueField = "TypeId";
			ddlType.DataTextField = "TypeName";
			ddlType.DataSource = Project.GetListProjectTypes();
			ddlType.DataBind();

			ddlBlockType.DataTextField = "Title";
			ddlBlockType.DataValueField = "primaryKeyId";
			ddlBlockType.DataSource = Mediachase.IbnNext.TimeTracking.TimeTrackingBlockType.List(Mediachase.Ibn.Data.FilterElement.EqualElement("IsProject", "1"));
			ddlBlockType.DataBind();

			ddlOverallStatus.Items.Clear();
			for (int i = 0; i <= 100; i++)
				ddlOverallStatus.Items.Add(new ListItem(i.ToString() + " %", i.ToString()));

			BindGeneralCategories();
			BindProjectCategories();
			BindPortfolios();

			ddlCalendar.DataTextField = "CalendarName";
			ddlCalendar.DataValueField = "CalendarId";
			ddlCalendar.DataSource = Project.GetListCalendars(PID);
			ddlCalendar.DataBind();

			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataSource = Project.GetListPriorities();
			ddlPriority.DataBind();

			ddlRiskLevel.DataTextField = "RiskLevelName";
			ddlRiskLevel.DataValueField = "RiskLevelId";
			ddlRiskLevel.DataSource = Project.GetListRiskLevels();
			ddlRiskLevel.DataBind();

			dtcTargetStartDate.SelectedDate = UserDateTime.UserToday;
			dtcTargetEndDate.SelectedDate = UserDateTime.UserToday.AddMonths(1);

			ddCurrency.DataTextField = "CurrencySymbol";
			ddCurrency.DataValueField = "CurrencyId";
			ddCurrency.DataSource = Project.GetListCurrency();
			ddCurrency.DataBind();

			// Managers
			ListItem li;
			ArrayList alManagers = new ArrayList();
			using (IDataReader iManagers = SecureGroup.GetListAllUsersInGroup((int)InternalSecureGroups.ProjectManager))
			{
				while (iManagers.Read())
				{
					li = new ListItem(iManagers["LastName"].ToString() + " " + iManagers["FirstName"].ToString(), iManagers["UserId"].ToString());
					alManagers.Add(li);
				}
			}

			if (Security.IsUserInGroup(InternalSecureGroups.PowerProjectManager))
			{
				int CurrentUserID = Security.UserID;
				for (int i = 0; i < alManagers.Count; i++)
					ddlManager.Items.Add((ListItem)alManagers[i]);

				li = ddlManager.Items.FindByValue(CurrentUserID.ToString());
				if (li != null) li.Selected = true;
			}
			else
			{
				lblManager.Visible = true;
				ddlManager.Visible = false;
				lblManager.Text = CommonHelper.GetUserStatus((Security.CurrentUser.UserID));
				iManagerId.Value = Security.CurrentUser.UserID.ToString();
			}

			// Exec Managers
			li = new ListItem(LocRM.GetString("NotSet"), "0");
			ddlExecManager.Items.Add(li);

			using (IDataReader iManagers = SecureGroup.GetListAllUsersIn2Group((int)InternalSecureGroups.ProjectManager, (int)InternalSecureGroups.ExecutiveManager))
			{
				while (iManagers.Read())
				{
					ddlExecManager.Items.Add(new ListItem(iManagers["LastName"].ToString() + " " + iManagers["FirstName"].ToString(), iManagers["UserId"].ToString()));
				}
			}


			if (PID != 0)
			{
				//				ddlBlockType.Enabled = false;

				using (IDataReader reader = Project.GetProject(PID))
				{
					if (reader.Read())
					{
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

						txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
						txtGoals.Text = HttpUtility.HtmlDecode(reader["Goals"].ToString());
						txtScope.Text = HttpUtility.HtmlDecode(reader["Scope"].ToString());
						txtDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());
						txtDeliverables.Text = HttpUtility.HtmlDecode(reader["Deliverables"].ToString());

						CommonHelper.SafeSelect(ddlType, reader["TypeId"].ToString());
						CommonHelper.SafeSelect(ddlCalendar, reader["CalendarId"].ToString());
						CommonHelper.SafeSelect(ddCurrency, reader["CurrencyId"].ToString());
						CommonHelper.SafeSelect(ddlPriority, reader["PriorityId"].ToString());
						CommonHelper.SafeSelect(ddlRiskLevel, reader["RiskLevelId"].ToString());
						CommonHelper.SafeSelect(ddlOverallStatus, reader["PercentCompleted"].ToString());
						if (reader["BlockTypeId"] != DBNull.Value)
							CommonHelper.SafeSelect(ddlBlockType, reader["BlockTypeId"].ToString());

						ddlExecManager.Items.FindByText(LocRM.GetString("NotSet")).Selected = true;
						if (reader["ExecutiveManagerId"] != DBNull.Value)
						{
							string str = "";
							str = reader["ExecutiveManagerId"].ToString();
							ListItem liExec = ddlExecManager.Items.FindByValue(reader["ExecutiveManagerId"].ToString());
							if (liExec != null)
							{
								ddlExecManager.ClearSelection();
								liExec.Selected = true;
							}
						}

						if (reader["ManagerId"] != DBNull.Value)
						{
							if (ddlManager.Visible)
							{
								ListItem liClient = ddlManager.Items.FindByValue(reader["ManagerId"].ToString());
								if (liClient != null)
								{
									ddlManager.ClearSelection();
									liClient.Selected = true;
								}
							}
							else
							{
								int iManager = (int)reader["ManagerId"];
								iManagerId.Value = iManager.ToString();
								lblManager.Text = CommonHelper.GetUserStatus(iManager);
							}
						}
						if (reader["StatusId"] != DBNull.Value)
						{
							ListItem liStatus = ddlStatus.Items.FindByValue(reader["StatusId"].ToString());
							if (liStatus != null)
							{
								ddlStatus.ClearSelection();
								liStatus.Selected = true;
							}
						}

						if (reader["PhaseId"] != DBNull.Value)
						{
							ListItem liPhase = ddPrjPhases.Items.FindByValue(reader["PhaseId"].ToString());
							if (liPhase != null)
							{
								ddPrjPhases.ClearSelection();
								liPhase.Selected = true;
							}
						}

						if (reader["InitialPhaseId"] != DBNull.Value)
						{
							ListItem liPhase = ddInitialPhase.Items.FindByValue(reader["InitialPhaseId"].ToString());
							if (liPhase != null)
							{
								ddInitialPhase.ClearSelection();
								liPhase.Selected = true;
							}
						}

						dtcTargetStartDate.SelectedDate = (DateTime)reader["TargetStartDate"];
						dtcTargetEndDate.SelectedDate = (DateTime)reader["TargetFinishDate"];

						if (reader["ActualStartDate"] != DBNull.Value && reader["ActualStartDate"].ToString() != "")
							dtcActualStartDate.SelectedDate = (DateTime)reader["ActualStartDate"];
						if (reader["ActualFinishDate"] != DBNull.Value && reader["ActualFinishDate"].ToString() != "")
							dtcActualFinishDate.SelectedDate = (DateTime)reader["ActualFinishDate"];
					}
				}

				using (IDataReader reader = Project.GetListCategories(PID))
				{
					while (reader.Read())
					{
						for (int i = 0; i < lbCategory.Items.Count; i++)
						{
							ListItem lItem = lbCategory.Items.FindByText(reader["CategoryName"].ToString());
							if (lItem != null) lItem.Selected = true;
						}
					}
				}

				using (IDataReader reader = Project.GetListProjectCategoriesByProject(PID))
				{
					while (reader.Read())
					{
						for (int i = 0; i < lbProjectCategory.Items.Count; i++)
						{
							ListItem lItem = lbProjectCategory.Items.FindByText(reader["CategoryName"].ToString());
							if (lItem != null) lItem.Selected = true;
						}
					}
				}

				using (IDataReader reader = ProjectGroup.ProjectGroupsGetByProject(PID))
				{
					while (reader.Read())
					{
						for (int i = 0; i < lbPortfolios.Items.Count; i++)
						{
							ListItem lItem = lbPortfolios.Items.FindByValue(reader["ProjectGroupId"].ToString());
							if (lItem != null) lItem.Selected = true;
						}
					}
				}
			}
			else
			{
				// Client
				PrimaryKeyId org_id = PrimaryKeyId.Empty;
				PrimaryKeyId contact_id = PrimaryKeyId.Empty;
				Common.GetDefaultClient(PortalConfig.ProjectDefaultValueClientField, out contact_id, out org_id);
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

				// Priority
				Util.CommonHelper.SafeSelect(ddlPriority, PortalConfig.ProjectDefaultValuePriorityField);

				// Currency
				Util.CommonHelper.SafeSelect(ddCurrency, PortalConfig.ProjectDefaultValueCurrencyField);

				// Categories
				ArrayList list = Common.StringToArrayList(PortalConfig.ProjectDefaultValueGeneralCategoriesField);
				foreach (int i in list)
					CommonHelper.SafeMultipleSelect(lbCategory, i.ToString());
				list = Common.StringToArrayList(PortalConfig.ProjectDefaultValueProjectCategoriesField);
				foreach (int i in list)
					CommonHelper.SafeMultipleSelect(lbProjectCategory, i.ToString());

				// texts
				txtGoals.Text = PortalConfig.ProjectDefaultValueGoalsField;
				txtDeliverables.Text = PortalConfig.ProjectDefaultValueDeliverablesField;
				txtScope.Text = PortalConfig.ProjectDefaultValueScopeField;
			}

			if (PID > 0)
				EditControl.ObjectId = PID;
			EditControl.MetaClassName = String.Concat("ProjectsEx_", ddlType.SelectedValue);
			EditControl.BindData();

			trPriority.Visible = PortalConfig.CommonProjectAllowEditPriorityField;
			trClient.Visible = PortalConfig.CommonProjectAllowEditClientField;
			trCategories.Visible = PortalConfig.CommonProjectAllowEditGeneralCategoriesField;
			trProjectCategories.Visible = PortalConfig.ProjectAllowEditProjectCategoriesField;
			GoalsRow.Visible = PortalConfig.ProjectAllowEditGoalsField;
			DeleverablesRow.Visible = PortalConfig.ProjectAllowEditDeliverablesField;
			ScopeRow.Visible = PortalConfig.ProjectAllowEditScopeField;
			LeftTextCell.Visible = GoalsRow.Visible || DeleverablesRow.Visible;
		}
		private void BindFromTemplate(int TemplateId)
		{
			DataTable dt = new DataTable();
			ArrayList GeneralCategories = new ArrayList();
			ArrayList ProjectCategories = new ArrayList();

			Task.MakeProjectSysFieldsFromTemplate(TemplateId, dt, GeneralCategories, ProjectCategories);
			if (dt.Rows.Count > 0)
			{
				txtDeliverables.Text = dt.Rows[0]["Deliverables"].ToString();
				txtDescription.Text = dt.Rows[0]["Description"].ToString();
				txtGoals.Text = dt.Rows[0]["Goals"].ToString();
				txtScope.Text = dt.Rows[0]["Scope"].ToString();

				Util.CommonHelper.SafeSelect(ddCurrency, dt.Rows[0]["CurrencyId"].ToString());
				Util.CommonHelper.SafeSelect(ddlCalendar, dt.Rows[0]["CalendarId"].ToString());
				Util.CommonHelper.SafeSelect(ddlType, dt.Rows[0]["TypeId"].ToString());
				Util.CommonHelper.SafeSelect(ddlManager, dt.Rows[0]["ManagerId"].ToString());
				Util.CommonHelper.SafeSelect(ddlExecManager, dt.Rows[0]["ExecutiveManagerId"].ToString());
				ddlPriority.SelectedValue = dt.Rows[0]["Priority"].ToString();

				foreach (ListItem item in lbProjectCategory.Items)
				{
					if (ProjectCategories.Contains(int.Parse(item.Value)))
						item.Selected = true;
					else
						item.Selected = false;
				}

				foreach (ListItem item in lbCategory.Items)
				{
					if (GeneralCategories.Contains(int.Parse(item.Value)))
						item.Selected = true;
					else
						item.Selected = false;
				}

				EditControl.MetaClassName = String.Concat("ProjectsEx_", ddlType.SelectedValue);
				EditControl.BindData();
			}
		}

		private void BindTemplateValues(int TemplateId, int ProjectId)
		{
			Task.MakeProjectMetaRolesFromTemplate(TemplateId, ProjectId, int.Parse(ddlType.SelectedValue));
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
			this.CustomValidator1.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator);
			this.CustomValidator2.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.CustomValidator2_ServerValidate);
			this.ddlTemplate.SelectedIndexChanged += new EventHandler(ddlTemplate_SelectedIndexChanged);

		}
		#endregion

		#region --- Event Handlres ---

		#region btnSave_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtGoals.Text = HttpUtility.HtmlEncode(txtGoals.Text);
			txtScope.Text = HttpUtility.HtmlEncode(txtScope.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);
			txtDeliverables.Text = HttpUtility.HtmlEncode(txtDeliverables.Text);

			int iManager = 0;
			if (ddlManager.Visible)
				iManager = int.Parse(ddlManager.SelectedItem.Value);
			else
				iManager = int.Parse(iManagerId.Value);

			ArrayList alCategories = new ArrayList();

			for (int i = 0; i < lbCategory.Items.Count; i++)
				if (lbCategory.Items[i].Selected)
					alCategories.Add(int.Parse(lbCategory.Items[i].Value));

			ArrayList alPCategories = new ArrayList();

			for (int i = 0; i < lbProjectCategory.Items.Count; i++)
				if (lbProjectCategory.Items[i].Selected)
					alPCategories.Add(int.Parse(lbProjectCategory.Items[i].Value));

			bool fromtemplate = false;
			ArrayList alPort = new ArrayList();
			for (int i = 0; i < lbPortfolios.Items.Count; i++)
				if (lbPortfolios.Items[i].Selected)
					alPort.Add(int.Parse(lbPortfolios.Items[i].Value));

			PrimaryKeyId orgUid = PrimaryKeyId.Empty;
			PrimaryKeyId contactUid = PrimaryKeyId.Empty;
			if (ClientControl.ObjectType == OrganizationEntity.GetAssignedMetaClassName())
				orgUid = ClientControl.ObjectId;
			else if (ClientControl.ObjectType == ContactEntity.GetAssignedMetaClassName())
				contactUid = ClientControl.ObjectId;

			if (PID != 0)
			{
				Project2.Update(PID, txtTitle.Text, txtDescription.Text, txtGoals.Text, txtScope.Text,
					txtDeliverables.Text, iManager, int.Parse(ddlExecManager.SelectedItem.Value),
					dtcTargetStartDate.SelectedDate.Date, dtcTargetEndDate.SelectedDate.Date,
					dtcActualStartDate.SelectedDate.Date, dtcActualFinishDate.SelectedDate.Date,
					int.Parse(ddlStatus.SelectedItem.Value), int.Parse(ddlType.SelectedItem.Value),
					int.Parse(ddlCalendar.SelectedItem.Value), contactUid, orgUid,
					int.Parse(ddCurrency.SelectedItem.Value), int.Parse(ddlPriority.SelectedItem.Value),
					int.Parse(ddInitialPhase.SelectedValue), int.Parse(ddPrjPhases.SelectedValue),
					int.Parse(ddlOverallStatus.SelectedValue), int.Parse(ddlRiskLevel.SelectedItem.Value),
					int.Parse(ddlBlockType.SelectedValue),
					alCategories, alPCategories, alPort);
			}
			else
			{
				PID = Project.Create(txtTitle.Text, txtDescription.Text, txtGoals.Text, txtScope.Text,
					txtDeliverables.Text, iManager, int.Parse(ddlExecManager.SelectedItem.Value),
					dtcTargetStartDate.SelectedDate, dtcTargetEndDate.SelectedDate,
					dtcActualStartDate.SelectedDate, dtcActualFinishDate.SelectedDate,
					int.Parse(ddlStatus.SelectedItem.Value), int.Parse(ddlType.SelectedItem.Value),
					int.Parse(ddlCalendar.SelectedItem.Value), contactUid, orgUid,
					int.Parse(ddCurrency.SelectedItem.Value), int.Parse(ddlPriority.SelectedItem.Value),
					int.Parse(ddInitialPhase.SelectedValue), int.Parse(ddPrjPhases.SelectedValue),
					int.Parse(ddlOverallStatus.SelectedValue), int.Parse(ddlRiskLevel.SelectedItem.Value),
					int.Parse(ddlBlockType.SelectedValue),
					alCategories, alPCategories, alPort);
				if (int.Parse(ddlTemplate.SelectedValue) > 0)
					fromtemplate = true;
			}

			// Save meta field info
			EditControl.Save(PID);

			if (fromtemplate)
			{
				BindTemplateValues(int.Parse(ddlTemplate.SelectedValue), PID);
				if (!cbOneMore.Checked)
					Response.Redirect("../Projects/ProjectView.aspx?ProjectID=" + PID + "&Tab=1&TemplateId=" + ddlTemplate.SelectedValue);
				else
					Response.Redirect("../Projects/ProjectEdit.aspx?OldProjectId=" + PID + "&TemplateId=" + ddlTemplate.SelectedValue);
			}
			else
			{
				if (cbOneMore.Checked)
					Response.Redirect("../Projects/ProjectEdit.aspx");
				else
					Response.Redirect("../Projects/ProjectView.aspx?ProjectID=" + PID);
			}
		}
		#endregion

		#region CustomValidator
		private void CustomValidator(Object sender, ServerValidateEventArgs args)
		{
			if ((dtcTargetEndDate.SelectedDate != DateTime.MinValue) && (dtcTargetEndDate.SelectedDate < dtcTargetStartDate.SelectedDate))
			{
				//CustomValidator1.ErrorMessage = LocRM.GetString("tWrongDate");
				CustomValidator1.ErrorMessage = LocRM.GetString("EndDateError") + " (" + dtcTargetStartDate.SelectedDate.ToShortDateString() + ")";
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}

		#endregion

		#region CustomValidator2_ServerValidate
		private void CustomValidator2_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (dtcActualFinishDate.SelectedDate != DateTime.MinValue
				&& dtcActualStartDate.SelectedDate != DateTime.MinValue
				&& dtcActualFinishDate.SelectedDate < dtcActualStartDate.SelectedDate)
			{
				//CustomValidator2.ErrorMessage = LocRM.GetString("tWrongDate");
				CustomValidator2.ErrorMessage = LocRM.GetString("FinishDateError") + " (" + dtcActualStartDate.SelectedDate.ToShortDateString() + ")";
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}

		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			string BackUrl = "default.aspx";
			if (Request["Back"] != null)
			{
				BackUrl = "ProjectView.aspx?ProjectID=" + PID;
				Response.Redirect("../Projects/" + BackUrl);
			}
			else
			{
				string backlink = "~/Apps/ProjectManagement/Pages/ProjectList.aspx";
				Response.Redirect(backlink, true);
			}
		}

		#endregion

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			if (Page.IsPostBack)
			{
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

				ListBox lbTmp1 = lbProjectCategory;
				ArrayList alCategories1 = new ArrayList();
				for (int i = 0; i < lbTmp1.Items.Count; i++)
					if (lbTmp1.Items[i].Selected)
						alCategories1.Add(int.Parse(lbTmp1.Items[i].Value));
				BindProjectCategories();
				for (int i = 0; i < alCategories1.Count; i++)
				{
					ListItem liCategory = lbTmp1.Items.FindByValue(alCategories1[i].ToString());
					if (liCategory != null)
						liCategory.Selected = true;
				}

				string val = ddPrjPhases.SelectedValue;
				ddPrjPhases.DataSource = Project.GetListProjectPhases();
				ddPrjPhases.DataTextField = "PhaseName";
				ddPrjPhases.DataValueField = "PhaseId";
				ddPrjPhases.DataBind();
				ListItem li = ddPrjPhases.Items.FindByValue(val);
				if (li != null)
				{
					ddPrjPhases.ClearSelection();
					li.Selected = true;
				}

				val = ddInitialPhase.SelectedValue;
				ddInitialPhase.DataSource = Project.GetListProjectPhases();
				ddInitialPhase.DataTextField = "PhaseName";
				ddInitialPhase.DataValueField = "PhaseId";
				ddInitialPhase.DataBind();
				li = ddInitialPhase.Items.FindByValue(val);
				if (li != null)
				{
					ddInitialPhase.ClearSelection();
					li.Selected = true;
				}
			}
		}

		#endregion

		#region ddlTemplate_SelectedIndexChanged
		private void ddlTemplate_SelectedIndexChanged(object sender, EventArgs e)
		{
			BindFromTemplate(int.Parse(ddlTemplate.SelectedValue));
		}
		#endregion

		#region ddlType_SelectedIndexChanged
		protected void ddlType_SelectedIndexChanged(object sender, EventArgs e)
		{
			EditControl.MetaClassName = String.Concat("ProjectsEx_", ddlType.SelectedValue);
			EditControl.BindData();
		}
		#endregion

		#endregion
	}
}
