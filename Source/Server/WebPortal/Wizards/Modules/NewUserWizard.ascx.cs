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
	using Mediachase.Ibn.Web.UI.WebControls;

	/// <summary>
	///		Summary description for NewUserWizard.
	/// </summary>
	public partial class NewUserWizard : System.Web.UI.UserControl, IWizardControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Wizards.Resources.strNewUsWd", typeof(NewUserWizard).Assembly);

		ArrayList subtitles = new ArrayList();
		ArrayList steps = new ArrayList();
		private int _stepCount = 4;
		public string s1Comments = "";

		#region HTML Vars
		#endregion

		private int user_type = 0; //1-admin,2-PM,3-Regular

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			if (user_type == 0)
			{
				Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
					"<script language=javascript>" +
					"try {window.opener.top.right.location.href='../Directory/UserView.aspx?UserID=" + Security.CurrentUser.UserID.ToString() + "';}" +
					"catch (e){} window.close();</script>");
				return;
			}
			if (!Page.IsPostBack)
				BindStep1();
		}

		#region BindStep1
		private void BindStep1()
		{
			lgdContactInf.InnerText = LocRM.GetString("tContactInformation");
			lgdCompanyinf.InnerText = LocRM.GetString("tCompanyInformation");
			lgdSelGroup.InnerText = LocRM.GetString("tSelectIBNGroups");
			lgdTextPartner.InnerText = LocRM.GetString("tSelectPartnerGroup");
			lblGroupsTitle.Text = LocRM.GetString("tPartnerGroups");
			lgdLang.InnerText = LocRM.GetString("tDefLanguage");
			lgdRole.InnerText = LocRM.GetString("tRole");
			lgdTextExt.InnerText = LocRM.GetString("tExternalAdding");
			lgdWelcome.InnerText = LocRM.GetString("tWelcomeComments");
			txtRELoginValidator.ErrorMessage = LocRM.GetString("LoginReg");
			lblSelected.Text = LocRM.GetString("tSelected");
			lblAvailable.Text = LocRM.GetString("tAvailable");

			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); SaveGroups(); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); SaveGroups(); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); SaveGroups(); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); SaveGroups();return false;");

			lbAvailableGroups.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); SaveGroups(); return false;");
			lbSelectedGroups.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); SaveGroups(); return false;");

			if (user_type == 1 || user_type == 2)
			{
				rbAccount.Items.Add(new ListItem(" " + LocRM.GetString("tIBNAccount"), "0"));
				if (User.CanCreateExternal())
					rbAccount.Items.Add(new ListItem(" " + LocRM.GetString("tExternalAccount"), "1"));
				if (user_type != 2 && User.CanCreatePartner())
					rbAccount.Items.Add(new ListItem(" " + LocRM.GetString("tPartnerAccount"), "2"));
				rbAccount.SelectedIndex = 0;
			}
			if (user_type == 3)
				lblUserType.Text = LocRM.GetString("tOnlyPending");

			cbRole.Items.Add(new ListItem(" " + LocRM.GetString("tPPM"), "0"));
			cbRole.Items.Add(new ListItem(" " + LocRM.GetString("tPM"), "1"));
			cbRole.Items.Add(new ListItem(" " + LocRM.GetString("tHDM"), "2"));
			cbRole.RepeatColumns = 3;

			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(false, false, false, false, false, false, false, false, false, false, false))
			{
				while (reader.Read())
				{
					lbAvailableGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			ddLang.DataSource = Common.GetListLanguages();
			ddLang.DataTextField = "FriendlyName";
			ddLang.DataValueField = "LanguageId";
			ddLang.DataBind();

			ddIMGroup.DataTextField = "IMGroupName";
			ddIMGroup.DataValueField = "IMGroupId";
			ddIMGroup.DataSource = IMGroup.GetListIMGroupsWithoutPartners();
			ddIMGroup.DataBind();
		}
		#endregion

		#region ShowStep
		private void ShowStep(int step)
		{

			for (int i = 0; i <= _stepCount; i++)
				((Panel)steps[i]).Visible = false;

			((Panel)steps[step - 1]).Visible = true;

			if (step == _stepCount)
			{
				if (user_type == 1 && rbAccount.SelectedItem.Value == "0")
					lblstep4.Text = LocRM.GetString("tTextForRegularAccount");
				if ((user_type == 2 && rbAccount.SelectedItem.Value == "0") || (user_type == 3))
					lblstep4.Text = LocRM.GetString("tTextForPendingAccount");
				if (user_type < 3 && rbAccount.SelectedItem.Value == "1")
					lblstep4.Text = LocRM.GetString("tTextForExternalAccount");
				if (user_type < 3 && rbAccount.SelectedItem.Value == "2")
					lblstep4.Text = LocRM.GetString("tTextForPartnerAccount");
			}
			if (step == 2)
			{
				lstTimeZone.DataSource = User.GetListTimeZone();
				lstTimeZone.DataTextField = "DisplayName";
				lstTimeZone.DataValueField = "TimeZoneId";
				lstTimeZone.DataBind();
				EmaiDuplication.Enabled = true;

				int TimeZoneId = Security.CurrentUser.TimeZoneId;
				lstTimeZone.ClearSelection();
				ListItem li = lstTimeZone.Items.FindByValue(TimeZoneId.ToString());
				if (li != null) li.Selected = true;
			}
			if (step == 2 && user_type < 3)
			{
				if (rbAccount.SelectedItem.Value == "1")
				{
					trLogin.Visible = false;
					trPass.Visible = false;
					trConfirm.Visible = false;
				}
				else
				{
					trLogin.Visible = true;
					trPass.Visible = true;
					trConfirm.Visible = true;
				}
			}

			if (step == 3)
				ViewState["password"] = txtPassword.Text;

			if (user_type == 1 && step == 3)
			{
				if (rbAccount.SelectedItem.Value == "1")
				{
					fsGroups.Visible = false;
					trIMGroup.Visible = false;
					fsRole.Visible = false;
					fsTextExt.Visible = true;
					EmaiDuplication.Enabled = true;
					fsPartner.Visible = false;
				}
				else if (rbAccount.SelectedItem.Value == "0")
				{
					fsPartner.Visible = false;
					fsTextExt.Visible = false;
					fsGroups.Visible = true;
					trIMGroup.Visible = true;
					fsRole.Visible = true;
					EmaiDuplication.Enabled = true;
					string sGroups = iGroups.Value;
					ArrayList alGroups = new ArrayList();
					while (sGroups.Length > 0)
					{
						alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
						sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
					}
					foreach (int i in alGroups)
					{
						ListItem lItm = lbAvailableGroups.Items.FindByValue(i.ToString());
						if (lItm != null)
						{
							lbAvailableGroups.Items.Remove(lItm);
							lbSelectedGroups.Items.Add(lItm);
						}
					}
				}
				else
				{
					EmaiDuplication.Enabled = true;
					fsGroups.Visible = false;
					trIMGroup.Visible = false;
					fsRole.Visible = false;
					fsTextExt.Visible = false;
					fsPartner.Visible = true;
					ddPartnerGroups.DataTextField = "GroupName";
					ddPartnerGroups.DataValueField = "GroupId";
					ddPartnerGroups.DataSource = SecureGroup.GetListChildGroups((int)InternalSecureGroups.Partner);
					ddPartnerGroups.DataBind();
				}
			}

			#region Save
			if (step == _stepCount + 1)
			{
				try
				{
					if (user_type == 1)
					{
						if (rbAccount.SelectedItem.Value == "0")
						{
							ArrayList alGroups = new ArrayList();
							string sGroups = iGroups.Value;
							while (sGroups.Length > 0)
							{
								alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
								sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
							}
							if (cbRole.Items[0].Selected)
								alGroups.Add(4);
							if (cbRole.Items[1].Selected)
								alGroups.Add(3);
							if (cbRole.Items[2].Selected)
								alGroups.Add(5);
							ViewState["UserID"] = User.Create(txtLogin.Text, ViewState["password"].ToString(), txtFirstName.Text, txtLastName.Text,
								txtEMail.Text, true, alGroups, int.Parse(ddIMGroup.SelectedItem.Value), txtWorkPhone.Text,
								"", txtMobilePhone.Text, txtPosition.Text, txtDepartment.Text, txtCompany.Text,
								txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(ddLang.SelectedItem.Value), txtWelcome.Text, "", null, "", -1);
						}
						else if (rbAccount.SelectedItem.Value == "1")
						{
							ViewState["UserID"] = User.CreateExternal(txtFirstName.Text, txtLastName.Text, txtEMail.Text,
								new ArrayList(),
								true, txtWorkPhone.Text, "", txtMobilePhone.Text, txtPosition.Text, txtDepartment.Text,
								txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(ddLang.SelectedItem.Value),
								txtWelcome.Text, "", null);
						}
						else
						{
							ViewState["UserID"] = User.CreatePartnerUser(txtLogin.Text, ViewState["password"].ToString(), txtFirstName.Text, txtLastName.Text,
								txtEMail.Text, true, int.Parse(ddPartnerGroups.SelectedItem.Value), txtWorkPhone.Text,
								"", txtMobilePhone.Text, txtPosition.Text, txtDepartment.Text, txtCompany.Text,
								txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(ddLang.SelectedItem.Value), txtWelcome.Text, "", null, -1);
						}
					}
					else if (user_type == 2)
					{
						if (rbAccount.SelectedItem.Value == "0")
						{
							ViewState["UserID"] = User.CreatePending(txtLogin.Text, ViewState["password"].ToString(), txtFirstName.Text, txtLastName.Text,
								txtEMail.Text, new ArrayList(), txtWorkPhone.Text, "", txtMobilePhone.Text, txtPosition.Text,
								txtDepartment.Text, txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(ddLang.SelectedItem.Value), txtWelcome.Text, "", null);
						}
						else
						{
							ViewState["UserID"] = User.CreateExternal(txtFirstName.Text, txtLastName.Text, txtEMail.Text, new ArrayList(),
								true, txtWorkPhone.Text, "", txtMobilePhone.Text, txtPosition.Text, txtDepartment.Text,
								txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(ddLang.SelectedItem.Value),
								txtWelcome.Text, "", null);
						}
					}
					else
						ViewState["UserID"] = User.CreatePending(txtLogin.Text, ViewState["password"].ToString(), txtFirstName.Text, txtLastName.Text,
							txtEMail.Text, new ArrayList(), txtWorkPhone.Text, "", txtMobilePhone.Text, txtPosition.Text,
							txtDepartment.Text, txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(ddLang.SelectedItem.Value), txtWelcome.Text, "", null);
				}
				catch (MaxUsersCountException)
				{
					lblError.Text = LocRM.GetString("MaxUsersCount");
					lblError.Visible = true;
				}
				catch (EmailDuplicationException)
				{
					int iUserId = User.GetUserByEmail(txtEMail.Text);
					string sUserName = String.Empty;
					if (iUserId > 0)
						sUserName = CommonHelper.GetUserStatusPureName(iUserId);
					lblError.Text = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";
					lblError.Visible = true;
				}
				catch (LoginDuplicationException)
				{
					lblError.Text = LocRM.GetString("DuplicatedLogin");
					lblError.Visible = true;
				}
			}
			#endregion
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

			if (User.CanCreate())
			{
				user_type = 1;			//1=admin
				subtitles.Add(LocRM.GetString("s1SubTitle"));
				subtitles.Add(LocRM.GetString("s2SubTitle"));
				subtitles.Add(LocRM.GetString("s3SubTitle"));
				subtitles.Add(LocRM.GetString("s4SubTitle"));
				subtitles.Add(LocRM.GetString("s5SubTitle"));
				steps.Add(step1);
				steps.Add(step2);
				steps.Add(step3);
				steps.Add(step4);
				steps.Add(step5);
				s1Comments = LocRM.GetString("s1Comments");
			}

			else if (!User.CanCreate() && Security.IsManager())
			{
				user_type = 2;			//2=PM, PPM
				_stepCount = 3;
				subtitles.Add(LocRM.GetString("s1SubTitle"));
				subtitles.Add(LocRM.GetString("s2SubTitle"));
				subtitles.Add(LocRM.GetString("s4SubTitle"));
				subtitles.Add(LocRM.GetString("s5SubTitle"));
				steps.Add(step1);
				steps.Add(step2);
				steps.Add(step4);
				steps.Add(step5);
				step3.Visible = false;
				s1Comments = LocRM.GetString("s1CommentsPM");
			}
			else if (!User.CanCreate() && !Security.IsManager() && User.CanCreatePending())
			{
				user_type = 3;			//3=Regular
				_stepCount = 3;
				subtitles.Add(LocRM.GetString("s1SubTitle"));
				subtitles.Add(LocRM.GetString("s2SubTitle"));
				subtitles.Add(LocRM.GetString("s4SubTitle"));
				subtitles.Add(LocRM.GetString("s5SubTitle"));
				steps.Add(step1);
				steps.Add(step2);
				steps.Add(step4);
				steps.Add(step5);
				step3.Visible = false;
				s1Comments = LocRM.GetString("s1CommentsGU");
			}

		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cvLogin.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvLogin_ServerValidate);
			this.EmaiDuplication.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.EmaiDuplication_ServerValidate);

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
			Subtitle = "";
			if (user_type > 0)
			{
				ShowStep(stepNumber);
				Subtitle = (string)subtitles[stepNumber - 1];
			}
		}

		public string GenerateFinalStepScript()
		{
			if (ViewState["UserID"] != null)
				return "try{window.opener.top.right.location.href='../Directory/UserView.aspx?UserID=" + ViewState["UserID"].ToString() + "';} catch (e) {} window.close();";
			else
				return String.Empty;
		}

		public void CancelAction()
		{

		}
		#endregion

		private void cvLogin_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			using (IDataReader reader = User.GetUserInfoByLogin(txtLogin.Text))
			{
				if (reader.Read())
					args.IsValid = false;
				else
					args.IsValid = true;
			}
		}

		private void EmaiDuplication_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			int id = User.GetUserByEmail(txtEMail.Text);
			if (id > 0)
			{
				string sUserName = String.Empty;
				sUserName = CommonHelper.GetUserStatusPureName(id);
				EmaiDuplication.ErrorMessage = LocRM.GetString("DuplicateEmail") + " (" + sUserName + ")";
				args.IsValid = false;
			}
			else
				args.IsValid = true;
		}
	}
}
