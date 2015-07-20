namespace Mediachase.UI.Web.Tasks.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Collections;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Web.UI.WebControls;
	using System.Collections.Generic;
	using System.Web.UI;


	/// <summary>
	///		Summary description for TaskEdit.
	/// </summary>
	public partial class TaskEdit : System.Web.UI.UserControl
	{
		#region HTML Vars

		//, lgdBasicInfo, lgdStatusInfo, lgdCategoryInfo;
		#endregion

		private int TID = 0;
		//private int ProjID = 0;
		private int BeforeTaskId = 0;
		private int AfterTaskId = 0;
		private int FirstForTaskId = 0;
		private int LastForTaskId = 0;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Tasks.Resources.strTaskEdit", typeof(TaskEdit).Assembly);
		private bool isMSProject = false;
		protected List<int> TaskResources = new List<int>();

		#region ProjID
		private int ProjID
		{
			get
			{
				if (ViewState["ProjID"] != null)
					return (int)ViewState["ProjID"];
				else return 0;
			}
			set
			{
				ViewState["ProjID"] = value;
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Configuration.ProjectManagementEnabled)
				throw new Mediachase.Ibn.LicenseRestrictionException();

			if (Request["TaskID"] != null)
			{
				TID = int.Parse(Request["TaskID"]);
				btnSaveAssign.Visible = false;
				trLoad.Visible = false;

				isMSProject = Project.GetIsMSProject(Task.GetProject(TID));
			}
			if (Request["ProjectID"] != null)
			{
				ProjID = int.Parse(Request["ProjectID"]);
			}
			if (Request["BeforeTaskId"] != null)
				BeforeTaskId = int.Parse(Request["BeforeTaskId"]);
			if (Request["AfterTaskId"] != null)
				AfterTaskId = int.Parse(Request["AfterTaskId"]);
			if (Request["FirstForTaskId"] != null)
				FirstForTaskId = int.Parse(Request["FirstForTaskId"]);
			if (Request["LastForTaskId"] != null)
				LastForTaskId = int.Parse(Request["LastForTaskId"]);

			if (BeforeTaskId != 0)
				ProjID = Task.GetProject(BeforeTaskId);
			if (AfterTaskId != 0)
				ProjID = Task.GetProject(AfterTaskId);
			if (FirstForTaskId != 0)
				ProjID = Task.GetProject(FirstForTaskId);
			if (LastForTaskId != 0)
				ProjID = Task.GetProject(LastForTaskId);
			btnAddGeneralCategory.Attributes.Add("onclick", "AddCategory(" + (int)DictionaryTypes.Categories + ")");
			btnAddGeneralCategory.Attributes.Add("title", LocRM.GetString("EditDictionary"));
			btnAddGeneralCategory.Visible = Security.IsManager();
			vFile.Visible = false;
			ApplyLocalization();
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");
			btnSaveAssign.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			if (!Page.IsPostBack)
				BindValues();
			if (TID > 0)
				cbOneMore.Visible = false;
			else
				cbOneMore.Visible = true;
			if (Request["OldTaskID"] != null && Request["Assign"] != null)
			{
				CommandManager cm = CommandManager.GetCurrent(this.Page);
				CommandParameters cp = new CommandParameters("MC_PM_OldTaskResEdit");
				string cmd = cm.AddCommand("Task", "", "TaskView", cp);
				cmd = cmd.Replace("\"", "&quot;");
				ClientScript.RegisterStartupScript(this.Page, this.GetType(), Guid.NewGuid().ToString(),
					 "function OpenAssignWizard(){" + cmd + "} setTimeout('OpenAssignWizard()', 400);", true);
			}

			if (Request["Checked"] != null)
				cbOneMore.Checked = true;
			BindVisibility();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
			if (Page.IsPostBack)
			{
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

			if (isMSProject)
			{
				dtcStartDate.Enabled = false;
				dtcEndDate.Enabled = false;

				chbMilestone.Enabled = false;
			}
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			trLoad.Visible = (TID <= 0) && PortalConfig.TaskAllowEditAttachmentField;
			trPriority.Visible = PortalConfig.CommonTaskAllowEditPriorityField;
			trTaskTime.Visible = PortalConfig.CommonTaskAllowEditTaskTimeField;
			trActivation.Visible = PortalConfig.TaskAllowEditActivationTypeField;
			trCompletion.Visible = PortalConfig.TaskAllowEditCompletionTypeField;
			trMustConfirm.Visible = PortalConfig.TaskAllowEditMustConfirmField;
			tblCategory.Visible = PortalConfig.CommonTaskAllowEditGeneralCategoriesField;
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblTitleTitle.Text = LocRM.GetString("title");
			lblProjectTitle.Text = LocRM.GetString("project");
			lblStartDateTitle.Text = LocRM.GetString("start_date");
			lblEndDateTitle.Text = LocRM.GetString("end_date");
			lblManagerTitle.Text = LocRM.GetString("manager");
			lblPriorityTitle.Text = LocRM.GetString("priority");
			lblActivationTitle.Text = LocRM.GetString("activation");
			lblCompletionTitle.Text = LocRM.GetString("completion");
			lblPhaseTitle.Text = LocRM.GetString("phase");
			chbMilestone.Text = LocRM.GetString("milestone");
			lblDescriptionTitle.Text = LocRM.GetString("description");
			lblCategoryTitle.Text = LocRM.GetString("category");
			chbMustBeConfirmed.Text = LocRM.GetString("MustConfirm");
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			btnSaveAssign.Text = LocRM.GetString("SaveAssign");
			lblFileLoad.Text = LocRM.GetString("FileLoad");
			lblTeam.Text = LocRM.GetString("Resources");
			cbConfirmed.Text = LocRM.GetString("MustBeConfirmed");
			cbOneMore.Text = LocRM.GetString("tAnotherOne");
			//lgdBasicInfo.InnerText = LocRM.GetString("tBasicInfo");
			//lgdStatusInfo.InnerText = LocRM.GetString("tStatusInfo");
			//lgdCategoryInfo.InnerText = LocRM.GetString("tCategoryInfo");
			lblTaskTimeTitle.Text = LocRM.GetString("taskTime");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrStatusInfo.AddText(LocRM.GetString("tStatusInfo"));
			hdrCategoryInfo.AddText(LocRM.GetString("tCategoryInfo"));
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");

			if (TID != 0)
			{
				tbSave.Title = LocRM.GetString("tbSaveEdit");
				btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			}
			else
			{
				tbSave.Title = LocRM.GetString("tbSaveCreate");
				btnSaveAssign.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");
				btnSave.Attributes.Add("onclick", "DisableButtons(this);ShowProgress();");
			}
			//			tbSave.AddSeparator();
			//			tbSave.AddLink("<img alt='' src='../Layouts/Images/SaveItem.gif'/> " + LocRM.GetString("tbsave_save"),"javascript: document.forms[0]." + btnSave.ClientID + ".click();");
			//			tbSave.AddSeparator();
			//			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbsave_gotolist"),"../Calendar/");
		}
		#endregion

		#region BindCategories
		private void BindCategories()
		{
			lbCategory.DataSource = Task.GetListCategoriesAll();
			lbCategory.DataTextField = "CategoryName";
			lbCategory.DataValueField = "CategoryId";
			lbCategory.DataBind();
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			dtcStartDate.DefaultTimeString = PortalConfig.WorkTimeStart;
			dtcEndDate.DefaultTimeString = PortalConfig.WorkTimeFinish;

			DateTime dt = DateTime.Today.AddHours(DateTime.UtcNow.Hour + 1);
			dtcStartDate.SelectedDate = User.GetLocalDate(Security.CurrentUser.TimeZoneId, dt);
			dtcEndDate.SelectedDate = dtcStartDate.SelectedDate.AddDays(1);

			BindCategories();

			ddlActivationType.DataSource = Task.GetListActivationTypes();
			ddlActivationType.DataTextField = "ActivationTypeName";
			ddlActivationType.DataValueField = "ActivationTypeId";
			ddlActivationType.DataBind();

			ddlCompletionType.DataSource = Task.GetListCompletionTypes();
			ddlCompletionType.DataTextField = "CompletionTypeName";
			ddlCompletionType.DataValueField = "CompletionTypeId";
			ddlCompletionType.DataBind();

			ddlPhase.Items.Add(new ListItem(LocRM.GetString("dontChange"), "-1"));
			using (IDataReader reader = Project.GetListProjectPhases())
			{
				while (reader.Read())
					ddlPhase.Items.Add(new ListItem(reader["PhaseName"].ToString(), reader["PhaseId"].ToString()));
			}

			ddlPriority.DataSource = Task.GetListPriorities();
			ddlPriority.DataTextField = "PriorityName";
			ddlPriority.DataValueField = "PriorityId";
			ddlPriority.DataBind();

			if (TID != 0)
			{
				using (IDataReader reader = Task.GetTask(TID))
				{
					if (reader.Read())
					{
						ProjID = (int)reader["ProjectId"];
						txtTitle.Text = HttpUtility.HtmlDecode(reader["Title"].ToString());
						dtcStartDate.SelectedDate = (DateTime)reader["StartDate"];
						dtcEndDate.SelectedDate = (DateTime)reader["FinishDate"];
						if ((bool)reader["IsMilestone"])
						{
							chbMilestone.Checked = true;
						}
						else
						{
							chbMilestone.Checked = false;
							trPhase.Style.Add("visibility", "hidden");
							ddlPhase.Style.Add("visibility", "hidden");
						}
						chbMustBeConfirmed.Checked = (bool)reader["MustBeConfirmed"];

						if ((bool)reader["IsSummary"])
						{
							//dtcStartDate.Enabled = false;
							dtcStartDate.ReadOnly = false;
							//dtcEndDate.Enabled = false;
							dtcEndDate.ReadOnly = false;
							CustomValidator1.Visible = false;
							chbMilestone.Visible = false;
						}

						ddlPriority.ClearSelection();
						ddlPriority.Items.FindByValue(reader["PriorityId"].ToString()).Selected = true;

						CommonHelper.SafeSelect(ddlActivationType, reader["ActivationTypeId"].ToString());
						CommonHelper.SafeSelect(ddlCompletionType, reader["CompletionTypeId"].ToString());

						if (reader["PhaseId"] != DBNull.Value)
							CommonHelper.SafeSelect(ddlPhase, reader["PhaseId"].ToString());

						if (reader["Description"] != DBNull.Value)
							txtDescription.Text = HttpUtility.HtmlDecode(reader["Description"].ToString());

						ucTaskTime.Value = DateTime.MinValue.AddMinutes((int)reader["TaskTime"]);
					}
				}

				using (IDataReader reader = Task.GetListCategories(TID))
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

				using (IDataReader reader = Task.GetListResources(TID))
				{
					while (reader.Read())
						TaskResources.Add((int)reader["UserId"]);
				}

				EditControl.ObjectId = TID;
				EditControl.BindData();
			}
			else //Create
			{
				trPhase.Style.Add("visibility", "hidden");
				ddlPhase.Style.Add("visibility", "hidden");

				ucTaskTime.Value = DateTime.MinValue.AddMinutes(int.Parse(PortalConfig.TaskDefaultValueTaskTimeField));
				CommonHelper.SafeSelect(ddlPriority, PortalConfig.TaskDefaultValuePriorityField);
				chbMustBeConfirmed.Checked = bool.Parse(PortalConfig.TaskDefaultValueMustConfirmField);
				ArrayList list = Common.StringToArrayList(PortalConfig.TaskDefaultValueGeneralCategoriesField);
				foreach (int i in list)
					CommonHelper.SafeMultipleSelect(lbCategory, i.ToString());
				CommonHelper.SafeSelect(ddlActivationType, PortalConfig.TaskDefaultValueActivationTypeField);
				CommonHelper.SafeSelect(ddlCompletionType, PortalConfig.TaskDefaultValueCompetionTypeField);
			}

			ResourcesGrid.DataSource = Project.GetListTeamMemberNamesWithManager(ProjID);
			ResourcesGrid.DataBind();

			chbMilestone.Attributes.Add("onclick",
				String.Format("ShowHidePhase(this, '{0}', '{1}')", trPhase.ClientID, ddlPhase.ClientID));

			lblProject.Text = "<a href='../Projects/ProjectView.aspx?ProjectID=" + ProjID + "'>" + Task.GetProjectTitle(ProjID) + "</a>";
			lblManager.Text = CommonHelper.GetUserStatus(Task.GetProjectManager(ProjID));
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

		#region btnSave_Click
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			//Page.Validate();
			if (!Page.IsValid)
				return;
			bool valid = true;

			bool SaveAndAssign = false;

			if (sender == (object)btnSaveAssign)
				SaveAndAssign = true;

			txtTitle.Text = HttpUtility.HtmlEncode(txtTitle.Text);
			txtDescription.Text = HttpUtility.HtmlEncode(txtDescription.Text);

			ArrayList alCategories = new ArrayList();
			for (int i = 0; i < lbCategory.Items.Count; i++)
				if (lbCategory.Items[i].Selected)
					alCategories.Add(int.Parse(lbCategory.Items[i].Value));

			ArrayList alResources = new ArrayList();
			foreach (DataGridItem dgi in ResourcesGrid.Items)
			{
				foreach (Control control in dgi.Cells[1].Controls)
				{
					if (control is CheckBox)
					{
						CheckBox checkBox = (CheckBox)control;
						if (checkBox.Checked)
							alResources.Add(int.Parse(dgi.Cells[0].Text));
					}
				}
			}

			DateTime dtBegin = dtcStartDate.SelectedDate;
			DateTime dtEnd = dtcEndDate.SelectedDate;
			if (chbMilestone.Checked)
				dtEnd = dtBegin;

			TimeSpan ts = new TimeSpan(ucTaskTime.Value.Ticks);
			int Minutes = (int)ts.TotalMinutes;

			if (TID != 0)
			{
				if (!isMSProject)
				{
					Task.Update(TID, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
						int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
						int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
						chbMustBeConfirmed.Checked,
						alCategories, int.Parse(ddlPhase.SelectedValue), Minutes);
				}
				else
				{
					Task2.UpdateSimple(TID, txtTitle.Text, txtDescription.Text, int.Parse(ddlPriority.SelectedItem.Value),
						int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
						chbMustBeConfirmed.Checked, Minutes, int.Parse(ddlPhase.SelectedValue), alCategories);
				}

				// Resources
				DataTable resources = new DataTable();
				resources.Columns.Add(new DataColumn("UserId", typeof(int)));
				resources.Columns.Add(new DataColumn("MustBeConfirmed", typeof(bool)));
				resources.Columns.Add(new DataColumn("CanManage", typeof(bool)));

				foreach (int userId in alResources)
				{
					DataRow row = resources.NewRow();
					row["UserId"] = userId;
					row["MustBeConfirmed"] = cbConfirmed.Checked;
					row["CanManage"] = false;

					resources.Rows.Add(row);
				}
				Task2.UpdateResources(TID, resources);
			}
			else
			{
				if (BeforeTaskId != 0)
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						TID = Task.InsertTo(ProjID, BeforeTaskId, PlaceTypes.Before_this, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedValue),
							chbMustBeConfirmed.Checked, alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream,
							alResources, cbConfirmed.Checked, int.Parse(ddlPhase.SelectedValue), Minutes);
					else if (fAssetFile.PostedFile == null)
						TID = Task.InsertTo(ProjID, BeforeTaskId, PlaceTypes.Before_this, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, null, null, alResources, cbConfirmed.Checked,
							int.Parse(ddlPhase.SelectedValue), Minutes);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
				else if (AfterTaskId != 0)
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						TID = Task.InsertTo(ProjID, AfterTaskId, PlaceTypes.After_this, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream,
							alResources, cbConfirmed.Checked, int.Parse(ddlPhase.SelectedValue), Minutes);
					else if (fAssetFile.PostedFile == null)
						TID = Task.InsertTo(ProjID, AfterTaskId, PlaceTypes.After_this, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, null, null, alResources, cbConfirmed.Checked,
							int.Parse(ddlPhase.SelectedValue), Minutes);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
				else if (FirstForTaskId != 0)
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						TID = Task.InsertTo(ProjID, FirstForTaskId, PlaceTypes.First_child, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream,
							alResources, cbConfirmed.Checked, int.Parse(ddlPhase.SelectedValue), Minutes);
					else if (fAssetFile.PostedFile == null)
						TID = Task.InsertTo(ProjID, FirstForTaskId, PlaceTypes.First_child, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, null, null, alResources, cbConfirmed.Checked,
							int.Parse(ddlPhase.SelectedValue), Minutes);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
				else if (LastForTaskId != 0)
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						TID = Task.InsertTo(ProjID, LastForTaskId, PlaceTypes.Last_child, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream,
							alResources, cbConfirmed.Checked, int.Parse(ddlPhase.SelectedValue), Minutes);
					else if (fAssetFile.PostedFile == null)
						TID = Task.InsertTo(ProjID, LastForTaskId, PlaceTypes.Last_child, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, null, null, alResources, cbConfirmed.Checked,
							int.Parse(ddlPhase.SelectedValue), Minutes);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
				else
				{
					if ((fAssetFile.PostedFile != null && fAssetFile.PostedFile.ContentLength > 0))
						TID = Task.Create(ProjID, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked,
							alCategories, fAssetFile.PostedFile.FileName, fAssetFile.PostedFile.InputStream, alResources,
							cbConfirmed.Checked, int.Parse(ddlPhase.SelectedValue), Minutes);
					else if (fAssetFile.PostedFile == null)
						TID = Task.Create(ProjID, txtTitle.Text, txtDescription.Text, dtBegin, dtEnd,
							int.Parse(ddlPriority.SelectedItem.Value), chbMilestone.Checked,
							int.Parse(ddlActivationType.SelectedValue), int.Parse(ddlCompletionType.SelectedItem.Value),
							chbMustBeConfirmed.Checked, alCategories, alResources, cbConfirmed.Checked, int.Parse(ddlPhase.SelectedValue), Minutes);
					else
					{
						vFile.Visible = true;
						valid = false;
					}
				}
			}

			if (TID > 0 && valid)
			{
				EditControl.Save(TID);

				if (!SaveAndAssign)
				{
					if (!cbOneMore.Checked)
					{
						if (Request["Back"] == "Gantt")
						{
							if (PortalConfig.ProjectViewControl == String.Empty || PortalConfig.ProjectViewControl == PortalConfig.ProjectViewControlDefaultValue)
								Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjID + "&Tab=6&SubTab=GanttChart2");
							else
								Response.Redirect("../Projects/ProjectView.aspx?ProjectId=" + ProjID + "&Tab=tabGant");
						}
						else
						{
							Response.Redirect("../Tasks/TaskView.aspx?TaskId=" + TID);
						}
					}
					else
						Response.Redirect("../Tasks/TaskEdit.aspx?Checked=1&ProjectId=" + ProjID);
				}
				else
				{
					if (!cbOneMore.Checked)
						Response.Redirect("../Tasks/TaskView.aspx?TaskId=" + TID + "&Assign=1");
					else
						Response.Redirect("../Tasks/TaskEdit.aspx?Checked=1&ProjectId=" + ProjID + "&OldTaskID=" + TID + "&Assign=1");
				}
			}
		}
		#endregion

		#region CustomValidator1_ServerValidate
		private void CustomValidator1_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if ((dtcEndDate.SelectedDate != DateTime.MinValue) && (dtcEndDate.SelectedDate < dtcStartDate.SelectedDate) && (!chbMilestone.Checked))
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

			if (Request["Back"] == "Gantt")
			{
				if (PortalConfig.ProjectViewControl == String.Empty || PortalConfig.ProjectViewControl == PortalConfig.ProjectViewControlDefaultValue)
					BackUrl = "../Projects/ProjectView.aspx?ProjectId=" + ProjID + "&Tab=6&SubTab=GanttChart2";
				else
					BackUrl = "../Projects/ProjectView.aspx?ProjectId=" + ProjID + "&Tab=tabGant";
			}
			else
			{
				if (TID != 0)
					BackUrl = "../Tasks/TaskView.aspx?TaskId=" + TID;
				else if (ProjID != 0)
					BackUrl = "../Projects/ProjectView.aspx?ProjectId=" + ProjID;
			}

			Response.Redirect(BackUrl);
		}
		#endregion
	}
}
