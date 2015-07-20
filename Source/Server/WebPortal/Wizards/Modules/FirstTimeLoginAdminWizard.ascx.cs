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
	using Mediachase.IBN.Business.Pop3;
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary> 
	///		Summary description for FirstTimeLoginAdminWizard.
	/// </summary>
	public partial class FirstTimeLoginAdminWizard : System.Web.UI.UserControl, IWizardControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strFirstLogAdmWd", typeof(FirstTimeLoginAdminWizard).Assembly);

		ArrayList steps = new ArrayList();
		ArrayList subtitles = new ArrayList();
		UserLightPropertyCollection pc = Security.CurrentUser.Properties;

		protected int _stepCount = 2;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				BindControls();
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>SwitchAlerts();</script>");
			}

			RequiredFieldValidator1.Enabled = cbEnableAlerts.Checked;
			RequiredFieldValidator2.Enabled = cbEnableAlerts.Checked;
			RequiredFieldValidator3.Enabled = cbEnableAlerts.Checked;

			using (IDataReader rdr = Company.GetAlertUserInfo())
			{
				if (rdr.Read())
				{
					tbAlertSenderFirstName.Text = HttpUtility.HtmlDecode(rdr["FirstName"].ToString());
					tbAlertSenderLastName.Text = HttpUtility.HtmlDecode(rdr["LastName"].ToString());
					tbAlertSenderEmail.Text = HttpUtility.HtmlDecode(rdr["Email"].ToString());
				}
			}
		}

		#region BindControls
		private void BindControls()
		{
			cbEnableAlerts.Checked = true;
			cbEnableAlerts.DataBind();
			cbShowNextTime.DataBind();
			s2ApplyLocalization();

			s3Title1.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageTitle1);
			s3Title2.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageTitle2);
			s3Text1.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText1);
			s3Text2.Text = HttpUtility.HtmlDecode(PortalConfig.PortalHomepageText2);
			cbEnableAlerts.Checked = PortalConfig.EnableAlerts;
			int index = (int)PortalConfig.PortalFirstDayOfWeek;

			DateTimeFormatInfo dtf = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat;
			for (int i = 0; i < 7; i++)
				ddFDOW.Items.Add(new ListItem(dtf.GetDayName((DayOfWeek)i), i.ToString()));
			CommonHelper.SafeSelect(ddFDOW, index.ToString());
			if (Mediachase.IbnNext.TimeTracking.TimeTrackingBlock.GetTotalCount() > 0)
			{
				fsFDOW.Visible = false;
			}
		}
		#endregion

		#region s2 ApplyLocalization
		private void s2ApplyLocalization()
		{
			lgdCompanyInfo.InnerText = LocRM.GetString("tCompanyInfo");
			lgdFDOW.InnerText = LocRM.GetString("tFirstDayOfWeek");
		}
		#endregion

		private void ProcessStep(int Step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[Step - 1]).Visible = true;

			if (Step == 1)
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "<script>SwitchAlerts();</script>");
			if (Step == 2 && ViewState["prevStep"] != null && (int)ViewState["prevStep"] == 1)
			{
				SaveAlerts();
			}

			if (Step == 3 && ViewState["prevStep"] != null && (int)ViewState["prevStep"] == 2)
			{
				SaveCompanySetup();
			}
		}

		#region SaveAlerts
		private void SaveAlerts()
		{
			try
			{
				Company.UpdateAlertInfo(cbEnableAlerts.Checked, tbAlertSenderFirstName.Text, tbAlertSenderLastName.Text, tbAlertSenderEmail.Text);
			}
			catch (EmailDuplicationException)
			{
				cvEmail.IsValid = false;
				cvEmail.ErrorMessage = LocRM.GetString("AlertDuplicatedEmail");
			}
		}
		#endregion

		#region SaveCompanySetup
		private void SaveCompanySetup()
		{
			byte[] PortalLogo = null;
			if (clogo.PostedFile != null && clogo.PostedFile.ContentLength > 0)
			{
				PortalLogo = new byte[clogo.PostedFile.ContentLength];
				clogo.PostedFile.InputStream.Read(PortalLogo, 0, clogo.PostedFile.ContentLength);
			}
			byte[] HomeImageLogo = null;
			if (cidentity.PostedFile != null && cidentity.PostedFile.ContentLength > 0)
			{
				HomeImageLogo = new byte[cidentity.PostedFile.ContentLength];
				cidentity.PostedFile.InputStream.Read(HomeImageLogo, 0, cidentity.PostedFile.ContentLength);
			}

			Company.UpdateCompanyInfo(s3Title1.Text, s3Title2.Text, s3Text1.Text, s3Text2.Text, PortalLogo, HomeImageLogo);
			if (Mediachase.IbnNext.TimeTracking.TimeTrackingBlock.GetTotalCount() == 0)
			{
				PortalConfig.PortalFirstDayOfWeek = byte.Parse(ddFDOW.SelectedValue);
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

			steps.Add(step1); steps.Add(step3); steps.Add(step4);
			subtitles.Add(LocRM.GetString("s1SubTitle")); subtitles.Add(LocRM.GetString("s3SubTitle")); subtitles.Add(LocRM.GetString("s4SubTitle"));
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cvEmail.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvEmail_Validate);
		}
		#endregion

		#region Implementation of IWizardControl

		public int StepCount { get { return _stepCount; } }
		public string TopTitle { get { return LocRM.GetString("GlobalTitle"); } }
		public bool ShowSteps { get { return true; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			Subtitle = (string)subtitles[stepNumber - 1];
			ProcessStep(stepNumber);
			ViewState["prevStep"] = stepNumber;
			if (stepNumber == _stepCount)
				MiddleButtonText = LocRM.GetString("Finish");
			else
				MiddleButtonText = null;

			CancelText = LocRM.GetString("Close");
		}

		public string GenerateFinalStepScript()
		{
			PortalConfig.PortalShowAdminWizard = cbShowNextTime.Checked;
			return "try{window.opener.top.location.href='" + ResolveClientUrl("~/Apps/Shell/Pages/default.aspx") + "';} catch (e) {} window.close();";
		}

		public void CancelAction()
		{
			PortalConfig.PortalShowAdminWizard = cbShowNextTime.Checked;
		}
		#endregion

		private void cvEmail_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			try
			{
				Company.UpdateAlertInfo(cbEnableAlerts.Checked, tbAlertSenderFirstName.Text, tbAlertSenderLastName.Text, tbAlertSenderEmail.Text);
			}
			catch (EmailDuplicationException)
			{
				args.IsValid = false;
				cvEmail.ErrorMessage = "<br>" + LocRM.GetString("AlertDuplicatedEmail");
			}
		}

	}
}
