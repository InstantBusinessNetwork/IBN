namespace Mediachase.UI.Web.Projects.Modules
{
	using System;
	using System.Resources;
	using System.Collections;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Mediachase.UI.Web.Wizards.Modules;
	using Mediachase.IBN.Business;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for EditProjectTemplate2.
	/// </summary>
	public partial class EditProjectTemplate2 : System.Web.UI.UserControl, IWizardControl
	{
		#region Controls
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdRolesAssigning;
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(EditProjectTemplate2).Assembly);
		protected ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectEdit", typeof(EditProjectTemplate2).Assembly);
		protected ResourceManager LocRM3 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strPrGeneral", typeof(EditProjectTemplate2).Assembly);
		ArrayList steps = new ArrayList();
		ArrayList subtitles = new ArrayList();
		int _stepCount = 3;
		#endregion

		#region prop: ProjectId
		private int ProjectId
		{
			get
			{
				return int.Parse(Request["ProjectID"].ToString());
			}
		}
		#endregion

		#region Page_Load
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindStep1();
				BindStep2();
				BindStep3();
			}

			BindToolBar();
			cbImportRole.CheckedChanged += new EventHandler(cbImportRole_CheckedChanged);
			rblTask.SelectedIndexChanged += new EventHandler(rblTask_SelectedIndexChanged);
		}
		#endregion

		#region BindToolBar
		void BindToolBar()
		{
			hdrTitle.AddText(LocRM.GetString("step1Title"));
			hdrTitle2.AddText(LocRM.GetString("step2Title"));
			hdrTitle3.AddText(LocRM.GetString("tAction"));
			hdrTitle3Top.AddText(LocRM.GetString("tTitleTemplate"));
		}
		#endregion

		#region Bind steps

		#region Bind: Step 1
		private void BindStep1()
		{

			//lgdTitle.InnerText = LocRM.GetString("step1Title");
			cbImportSystem.Text = LocRM.GetString("step1ImportBox");
			cbImportSystem.Checked = true;
			cbImportMeta.Text = LocRM.GetString("step1ImportMeta");
			cbImportMeta.Checked = true;

			using (IDataReader reader = Project.GetProject(ProjectId))
			{
				if (reader.Read())
				{
					/*
					using (IDataReader reader2 = User.GetUserInfo((int)reader["ManagerId"]))
					{
						if (reader2.Read())
							txtManager.Text = String.Format("{0} {1}",(string)reader2["LastName"], (string)reader2["FirstName"]);
					}
					if (reader["ExecutiveManagerId"] != DBNull.Value)
					{
						using (IDataReader reader2 = User.GetUserInfo((int)reader["ExecutiveManagerId"]))
						{
							if (reader2.Read())
								txtExecManager.Text = String.Format("{0} {1}",(string)reader2["LastName"], (string)reader2["FirstName"]);
						}
					}*/

					if (reader["Goals"] != DBNull.Value)
						txtGoals.Text = (string)reader["Goals"];
					if (reader["Description"] != DBNull.Value)
						txtDescription.Text = (string)reader["Description"];
					if (reader["Deliverables"] != DBNull.Value)
						txtDeliverables.Text = (string)reader["Deliverables"];
					if (reader["Scope"] != DBNull.Value)
						txtScope.Text = (string)reader["Scope"];

					txtCalendar.Text = (string)reader["CalendarName"];
					txtCurrency.Text = (string)reader["CurrencySymbol"];
					txtType.Text = (string)reader["TypeName"];
				}
			}

			using (IDataReader reader = Project.GetListProjectCategoriesByProject(ProjectId))
			{
				while (reader.Read())
				{
					txtProjectCategory.Text += (string)reader["CategoryName"] + " <br>";
				}
			}

			using (IDataReader reader = Project.GetListCategories(ProjectId))
			{
				while (reader.Read())
				{
					txtCategory.Text += (string)reader["CategoryName"] + " <br>";
				}
			}

		}
		#endregion

		#region Bind: Step 2
		private void BindStep2()
		{
			cbImportRole.Text = LocRM.GetString("step2ImportRoles");
			//lgdTitle2.InnerText = LocRM.GetString("step2Title");
			cbImportRole.Checked = true;

			#region Bind: CheckBoxList
			DataTable dt = new DataTable();
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Value", typeof(string));
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 1, LocRM3.GetString("ProjectTeam") });
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 2, LocRM3.GetString("ProjectStakeholders") });
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 3, LocRM3.GetString("ProjectSponsors") });
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 4, LocRM2.GetString("manager") });
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 5, LocRM2.GetString("exec_manager") });

			cblRoles.DataSource = dt;
			cblRoles.DataTextField = "Value";
			cblRoles.DataValueField = "Id";
			cblRoles.DataBind();

			foreach (ListItem list in cblRoles.Items)
			{
				list.Selected = true;
			}
			#endregion
		}
		#endregion

		#region Bind: Step 3
		private void BindStep3()
		{
			//lgdTitle3.InnerText = LocRM.GetString("tTitleTemplate");
			//lgdTitle3Top.InnerText = LocRM.GetString("tTitleTemplate");
			tbTitle.Text = "Project Template " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
			cbOnlyForMe.Text = LocRM.GetString("tOnlyForMe");

			#region Bind RadioButtons list
			DataTable dt = new DataTable();
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Value", typeof(string));

			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 1, LocRM.GetString("step3Radio1") });
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 2, LocRM.GetString("step3Radio2") });
			dt.Rows.Add(dt.NewRow().ItemArray = new object[] { 3, LocRM.GetString("step3Radio3") });

			rblTask.DataSource = dt;
			rblTask.DataTextField = "Value";
			rblTask.DataValueField = "Id";
			rblTask.DataBind();

			rblTask.Items.FindByText(LocRM.GetString("step3Radio2")).Selected = true;
			divRoles.Style.Add("display", "none");
			#endregion

			#region Bind Roles Grid
			DataTable dtRoles = Task.MakeTemplateAssignments(ProjectId);
			dgRoles.DataSource = dtRoles.DefaultView;
			dgRoles.DataBind();
			foreach (DataGridItem dgi in dgRoles.Items)
			{
				TextBox tb = (TextBox)dgi.FindControl("tbRole");
				if (tb != null)
				{
					DataRow[] dr = dtRoles.Select("PrincipalId = '" + dgi.Cells[2].Text + "'");
					if (dr.Length > 0)
					{
						string s = "";
						if (dr[0]["RoleName"].ToString() == "")
							s = dr[0]["PrincipalName"].ToString();
						else
							s = dr[0]["RoleName"].ToString();
						tb.Text = s;
					}
				}
			}
			#endregion
		}
		#endregion

		#endregion

		#region ShowStep
		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			//BindStep(step);

			((Panel)steps[step - 1]).Visible = true;

			if (step == _stepCount + 1)
			{
				DataTable dtRoles = Task.MakeTemplateAssignments(ProjectId);
				foreach (DataGridItem dgi in dgRoles.Items)
				{
					TextBox tb = (TextBox)dgi.FindControl("tbRole");
					if (tb != null)
					{
						DataRow[] dr = dtRoles.Select("PrincipalId = '" + dgi.Cells[2].Text + "'");
						if (dr.Length > 0)
						{
							string s = string.Empty;
							if (dr[0]["RoleName"].ToString() == string.Empty)
								s = dr[0]["PrincipalName"].ToString();
							else
								s = dr[0]["RoleName"].ToString();


							if (rblTask.SelectedValue == "2")
								dr[0]["RoleName"] = s; // store default role
							else if (rblTask.SelectedValue == "3")
								dr[0]["RoleName"] = tb.Text; // store user-specified role
						}
					}
				}


				TemplateMakeInfo info = new TemplateMakeInfo(cbImportSystem.Checked, cbImportMeta.Checked, cbImportRole.Checked, cblRoles.Items[0].Selected, cblRoles.Items[1].Selected, cblRoles.Items[2].Selected,
					cblRoles.Items[3].Selected, cblRoles.Items[4].Selected, TemplateTaskInfo.TaskWithRoleDefine);

				switch (rblTask.SelectedValue)
				{
					case "1":
						{
							info.TaskInfo = TemplateTaskInfo.NoTask;
							break;
						}
					case "2":
						{
							info.TaskInfo = TemplateTaskInfo.Task;
							break;
						}
					case "3":
						{
							info.TaskInfo = TemplateTaskInfo.TaskWithRoleDefine;
							break;
						}
				}


				Task.MakeTemplateFromProject2(ProjectId, dtRoles, tbTitle.Text, !cbOnlyForMe.Checked, info);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  "window.close();", true);
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

			steps.Add(step1);
			steps.Add(step2);
			steps.Add(step3);
			steps.Add(step4);

			subtitles.Add(LocRM.GetString("tStep1SubTitle"));
			subtitles.Add(LocRM.GetString("tStep2SubTitle"));
			subtitles.Add(LocRM.GetString("tStep3SubTitle"));
			subtitles.Add("");
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("tGlobalTitle"); } }
		public bool ShowSteps { get { return false; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
		}

		public string GenerateFinalStepScript()
		{
			return "window.close();";
		}

		public void CancelAction()
		{

		}
		#endregion

		#region cbImportRole_CheckedChanged
		private void cbImportRole_CheckedChanged(object sender, EventArgs e)
		{
			foreach (ListItem item in cblRoles.Items)
			{
				item.Selected = cbImportRole.Checked;
			}
			if (cbImportRole.Checked)
				tdRoles.Style.Add("display", "block");
			else
				tdRoles.Style.Add("display", "none");
		}
		#endregion

		#region rblTask_SelectedIndexChanged
		private void rblTask_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (rblTask.SelectedValue == "3")
			{
				divRoles.Style.Add("display", "block");
			}
			else
			{
				divRoles.Style.Add("display", "none");
			}
		}
		#endregion
	}
}
