namespace Mediachase.UI.Web.Wizards.Modules
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.SessionState;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Globalization;
	using System.Resources;
	using Mediachase.IBN.Business;
	using Mediachase.UI.Web.Util;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for NewIssueWizard.
	/// </summary>
	public partial class NewIssueWizard : System.Web.UI.UserControl, IWizardControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strNewIsWd", typeof(NewIssueWizard).Assembly);

		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();
		private int _stepCount = 3;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindStep1();
		}

		#region Step 1
		private void BindStep1()
		{
			ddPriority.DataSource = Incident.GetListPriorities();
			ddPriority.DataTextField = "PriorityName";
			ddPriority.DataValueField = "PriorityId";
			ddPriority.DataBind();
			ddPriority.ClearSelection();
			ListItem liPriority = ddPriority.Items.FindByValue(Priority.Normal.GetHashCode().ToString());
			if (liPriority != null)
				liPriority.Selected = true;

			ddType.DataSource = Incident.GetListIncidentTypes();
			ddType.DataTextField = "TypeName";
			ddType.DataValueField = "TypeId";
			ddType.DataBind();

			ddSeverity.DataSource = Incident.GetListIncidentSeverity();
			ddSeverity.DataTextField = "SeverityName";
			ddSeverity.DataValueField = "SeverityId";
			ddSeverity.DataBind();

			ddProject.DataSource = Incident.GetListProjects();
			ddProject.DataTextField = "Title";
			ddProject.DataValueField = "ProjectId";
			ddProject.DataBind();
			ListItem liNew = new ListItem(LocRM.GetString("ProjectNotSet"), "-1");
			ddProject.Items.Insert(0, liNew);
			ddProject.DataSource = null;
			ddProject.DataBind();

			lbIssueCategories.DataSource = Incident.GetListIncidentCategories();
			lbIssueCategories.DataTextField = "CategoryName";
			lbIssueCategories.DataValueField = "CategoryId";
			lbIssueCategories.DataBind();
		}
		#endregion

		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			if (step == _stepCount)
			{
				lblIssueTitle.Text = txtTitle.Text;
				lblProjectTitle.Text = ddProject.SelectedItem.Text;
				lblPriority.Text = ddPriority.SelectedItem.Text;
				lblSeverity.Text = ddSeverity.SelectedItem.Text;
				lblIssueType.Text = ddType.SelectedItem.Text;
			}

			#region Save
			if (step == _stepCount + 1)
			{
				ArrayList alIncidentCategories = new ArrayList();
				for (int i = 0; i < lbIssueCategories.Items.Count; i++)
					if (lbIssueCategories.Items[i].Selected)
						alIncidentCategories.Add(int.Parse(lbIssueCategories.Items[i].Value));
				int ProjectID = int.Parse(ddProject.SelectedItem.Value);
				ViewState["IssueID"] = Incident.Create(txtTitle.Text, txtDescr.Text, ProjectID,
					int.Parse(ddType.SelectedItem.Value), int.Parse(ddPriority.SelectedItem.Value),
					int.Parse(ddSeverity.SelectedItem.Value), 0, 0, 0, 0, -1, new ArrayList(),
					alIncidentCategories, null, null, false, DateTime.UtcNow, -1, PrimaryKeyId.Empty, PrimaryKeyId.Empty);
				Response.Redirect("../Wizards/CommonWizard.aspx?ObjectType=7&ObjectID=" + ViewState["IssueID"].ToString());
			}
			#endregion
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			subtitles.Add(LocRM.GetString("s1SubTitle"));
			subtitles.Add(LocRM.GetString("s2SubTitle"));
			subtitles.Add(LocRM.GetString("s3SubTitle"));
			subtitles.Add(LocRM.GetString("s4SubTitle"));

			steps.Add(step1);
			steps.Add(step2);
			steps.Add(step3);
			steps.Add(step4);
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("tTopTitle"); } }
		public bool ShowSteps { get { return true; } }
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
			if (ViewState["IssueID"] != null)
				return "try{window.opener.top.right.location.href='../Incidents/IncidentView.aspx?IncidentID=" + ViewState["IssueID"].ToString() + "';} catch (e) {} window.close();";
			else
				return String.Empty;
		}

		public void CancelAction()
		{

		}
		#endregion
	}
}
