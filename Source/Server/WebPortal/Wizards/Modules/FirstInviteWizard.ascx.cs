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
	using Mediachase.Ibn.Web.Interfaces;

	/// <summary>
	///		Summary description for FirstInviteWizard.
	/// </summary>
	public partial class FirstInviteWizard : System.Web.UI.UserControl, IWizardControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strFirstLogAdmWd", typeof(FirstInviteWizard).Assembly);
		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();
		private int _stepCount = 1;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
				BindStep1();
		}

		#region BindStep1
		private void BindStep1()
		{
			lgdContactInf.InnerText = LocRM.GetString("s1Invite");
			cbShowNextTime.DataBind();
		}
		#endregion

		#region ShowStep
		private void ShowStep(int step)
		{
			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			if (step == _stepCount + 1)
			{
				ArrayList alGroups = new ArrayList();
				using (IDataReader reader = SecureGroup.GetListGroups())
				{
					while (reader.Read())
					{
						int iGroupId = (int)reader["GroupId"];
						if (iGroupId > 8)
						{
							alGroups.Add(iGroupId);
							break;
						}
					}
				}
				int iIMGroup = 0;
				using (DataTable table = IMGroup.GetListIMGroup())
				{
					if (table.Rows.Count > 0)
						iIMGroup = (int)table.Rows[0]["IMGroupId"];
				}

				#region txtEMail1
				if (txtEMail1.Text != "")
				{
					int id = User.GetUserByEmail(txtEMail1.Text);
					if (id <= 0)
					{
						string strLogin = txtEMail1.Text.Substring(0, txtEMail1.Text.IndexOf("@"));
						int i = 0;
						bool fl = false;
						do
						{
							string tmpLogin = strLogin;
							if (i > 0)
								tmpLogin = tmpLogin + i.ToString();
							using (IDataReader reader = User.GetUserInfoByLogin(tmpLogin))
							{
								if (reader.Read())
									fl = true;
								else
									fl = false;
							}
							i++;
						}
						while (fl);

						string sFirst = (txtFirstName1.Text == "") ? strLogin : txtFirstName1.Text;
						string sLast = (txtLastName1.Text == "") ? strLogin : txtLastName1.Text;
						if (i > 1)
							strLogin = strLogin + (i - 1).ToString();

						User.Create(strLogin, "ibn", sFirst, sLast, txtEMail1.Text,
							alGroups, iIMGroup, "", "", "", "", "", "", Security.CurrentUser.LanguageId, "");
					}
				}
				#endregion

				#region txtEMail2
				if (txtEMail2.Text != "")
				{
					int id = User.GetUserByEmail(txtEMail2.Text);
					if (id <= 0)
					{
						string strLogin = txtEMail2.Text.Substring(0, txtEMail2.Text.IndexOf("@"));
						int i = 0;
						bool fl = false;
						do
						{
							string tmpLogin = strLogin;
							if (i > 0)
								tmpLogin = tmpLogin + i.ToString();
							using (IDataReader reader = User.GetUserInfoByLogin(tmpLogin))
							{
								if (reader.Read())
									fl = true;
								else
									fl = false;
							}
							i++;
						}
						while (fl);
						string sFirst = (txtFirstName2.Text == "") ? strLogin : txtFirstName2.Text;
						string sLast = (txtLastName2.Text == "") ? strLogin : txtLastName2.Text;
						if (i > 1)
							strLogin = strLogin + (i - 1).ToString();

						User.Create(strLogin, "ibn", sFirst, sLast, txtEMail2.Text,
							alGroups, iIMGroup, "", "", "", "", "", "", Security.CurrentUser.LanguageId, "");
					}
				}
				#endregion

				#region txtEMail3
				if (txtEMail3.Text != "")
				{
					int id = User.GetUserByEmail(txtEMail3.Text);
					if (id <= 0)
					{
						string strLogin = txtEMail3.Text.Substring(0, txtEMail3.Text.IndexOf("@"));
						int i = 0;
						bool fl = false;
						do
						{
							string tmpLogin = strLogin;
							if (i > 0)
								tmpLogin = tmpLogin + i.ToString();
							using (IDataReader reader = User.GetUserInfoByLogin(tmpLogin))
							{
								if (reader.Read())
									fl = true;
								else
									fl = false;
							}
							i++;
						}
						while (fl);
						string sFirst = (txtFirstName3.Text == "") ? strLogin : txtFirstName3.Text;
						string sLast = (txtLastName3.Text == "") ? strLogin : txtLastName3.Text;
						if (i > 1)
							strLogin = strLogin + (i - 1).ToString();
						User.Create(strLogin, "ibn", sFirst, sLast, txtEMail3.Text,
							alGroups, iIMGroup, "", "", "", "", "", "", Security.CurrentUser.LanguageId, "");
					}
				}
				#endregion

				#region txtEMail4
				if (txtEMail4.Text != "")
				{
					int id = User.GetUserByEmail(txtEMail4.Text);
					if (id <= 0)
					{
						string strLogin = txtEMail4.Text.Substring(0, txtEMail4.Text.IndexOf("@"));
						int i = 0;
						bool fl = false;
						do
						{
							string tmpLogin = strLogin;
							if (i > 0)
								tmpLogin = tmpLogin + i.ToString();
							using (IDataReader reader = User.GetUserInfoByLogin(tmpLogin))
							{
								if (reader.Read())
									fl = true;
								else
									fl = false;
							}
							i++;
						}
						while (fl);
						string sFirst = (txtFirstName4.Text == "") ? strLogin : txtFirstName4.Text;
						string sLast = (txtLastName4.Text == "") ? strLogin : txtLastName4.Text;
						if (i > 1)
							strLogin = strLogin + (i - 1).ToString();
						User.Create(strLogin, "ibn", sFirst, sLast, txtEMail4.Text,
							alGroups, iIMGroup, "", "", "", "", "", "", Security.CurrentUser.LanguageId, "");
					}
				}
				#endregion

				UserLightPropertyCollection pc = Security.CurrentUser.Properties;
				PortalConfig.PortalShowAdminWizard = cbShowNextTime.Checked;
				if (pc["USetup_ShowStartupWizard"] == null || pc["USetup_ShowStartupWizard"] == "True")
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try{window.opener.top.right.location.href='" + ResolveClientUrl("~/Workspace/default.aspx") + "?BTab=Workspace&wizard=1';} catch (e) {} window.close();",
					  true);
				else
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					  "try{window.opener.top.location.href='" + ResolveClientUrl("~/Apps/Shell/Pages/default.aspx") + "';} catch (e) {} window.close();",
					  true);
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

			subtitles.Add(LocRM.GetString("s1Invite"));
			subtitles.Add(LocRM.GetString("s2Invite"));

			steps.Add(step1);
			steps.Add(step2);
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
		public string TopTitle { get { return LocRM.GetString("InviteGlobalTitle"); } }
		public bool ShowSteps { get { return true; } }
		public string Subtitle { get; private set; }
		public string MiddleButtonText { get; private set; }
		public string CancelText { get; private set; }

		public void SetStep(int stepNumber)
		{
			ShowStep(stepNumber);
			Subtitle = (string)subtitles[stepNumber - 1];
			CancelText = LocRM.GetString("tClose");
		}

		public string GenerateFinalStepScript()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			PortalConfig.PortalShowAdminWizard = cbShowNextTime.Checked;
			if (pc["USetup_ShowStartupWizard"] == null || pc["USetup_ShowStartupWizard"] == "True")
				return "try{window.opener.top.right.location.href='../Workspace/default.aspx?BTab=Workspace&wizard=1';} catch (e) {} window.close();";
			else
				return "try{window.opener.top.location.href='" + ResolveClientUrl("~/Apps/Shell/Pages/default.aspx") + "';} catch (e) {} window.close();";
		}

		public void CancelAction()
		{
			UserLightPropertyCollection pc = Security.CurrentUser.Properties;
			PortalConfig.PortalShowAdminWizard = cbShowNextTime.Checked;
			if (pc["USetup_ShowStartupWizard"] == null || pc["USetup_ShowStartupWizard"] == "True")
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
				  "try{window.opener.top.right.location.href='../Workspace/default.aspx?BTab=Workspace&wizard=1';} catch (e) {} window.close();",
							true);
		}
		#endregion
	}
}
