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
	///		Summary description for EditProjectTemplate.
	/// </summary>
	public partial class EditProjectTemplate : System.Web.UI.UserControl, IWizardControl
	{


		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Projects.Resources.strProjectView", typeof(EditProjectTemplate).Assembly);
		ArrayList steps = new ArrayList();
		ArrayList subtitles = new ArrayList();
		int _stepCount = 1;

		private int ProjectId
		{
			get
			{
				return int.Parse(Request["ProjectID"].ToString());
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindStep1();
		}

		private void BindStep1()
		{
			lgdRolesAssigning.InnerText = LocRM.GetString("tResRoles");
			lgdTitle.InnerText = LocRM.GetString("tTitleTemplate");
			tbTitle.Text = "Project Template " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
			cbOnlyForMe.Text = LocRM.GetString("tOnlyForMe");
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
			if (dtRoles.Rows.Count == 0)
				((WizardTemplate)Page.Controls[0]).btnNext.Disabled = true;
		}

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

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
							dr[0]["RoleName"] = tb.Text;
					}
				}
				Task.MakeTemplateFromProject(ProjectId, dtRoles, tbTitle.Text, !cbOnlyForMe.Checked);
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "window.close();", true);
			}
		}

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

			subtitles.Add(LocRM.GetString("tSubTitle1"));
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

		public bool ShowSteps { get { return false; } }
		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("tGlobalTitle"); } }
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
	}
}
