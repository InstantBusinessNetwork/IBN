namespace Mediachase.UI.Web.Directory.Modules
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Resources;
	using System.Globalization;
	using System.Threading;
	using Mediachase.IBN.Business;
	using System.Collections;
	using Mediachase.UI.Web.Util;
	using System.IO;
	using Mediachase.Ibn.Business.Customization;
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Web.UI.WebControls;


	/// <summary>
	///		Summary description for MultipleUserEdit.
	/// </summary>
	public partial class MultipleUserEdit : System.Web.UI.UserControl
	{
		#region HTML Vars

		protected System.Web.UI.WebControls.RequiredFieldValidator txtPasswordRFValidator;
		protected System.Web.UI.WebControls.CompareValidator txtPasswordCompareValidator;
		protected System.Web.UI.HtmlControls.HtmlTable MainTable;
		protected System.Web.UI.WebControls.Label lblIMGroupsTitle;

		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(MultipleUserEdit).Assembly);

		private int UID = 0;

		#region GroupId
		private int GroupId
		{
			get
			{
				try
				{
					return Request["GroupId"] != null ? int.Parse(Request["GroupId"]) : 0;

				}
				catch
				{
					return 0;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			if (Request["UserID"] != null) UID = int.Parse(Request["UserID"]);
			RegularExpressionValidator2.ErrorMessage = LocRM.GetString("tWrongFormat");
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");

			if (!Page.IsPostBack)
			{
				BindDDType();
				ApplyLocalization();
				BindValues();
			}
			if (UID == 0)
				cbOneMore.Visible = true;
			else
				cbOneMore.Visible = false;
		}

		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblUserType.Text = LocRM.GetString("UserType");
			lblLoginTitle.Text = LocRM.GetString("login");
			lblWinLoginTitle.Text = LocRM.GetString("tWindowsLogin");
			lblPasswordTitle.Text = LocRM.GetString("password");
			lblConfirmTitle.Text = LocRM.GetString("confirm");
			lblFirstNameTitle.Text = LocRM.GetString("first_name");
			lblLastNameTitle.Text = LocRM.GetString("last_name");
			lblEmailTitle.Text = LocRM.GetString("email") + ":";
			lblPhotoTitle.Text = LocRM.GetString("photo");
			lblPhoneTitle.Text = LocRM.GetString("phone");
			lblFaxTitle.Text = LocRM.GetString("fax");
			lblMobileTitle.Text = LocRM.GetString("mobile");
			lblJobTitleTitle.Text = LocRM.GetString("job_title");
			lblDepartmentTitle.Text = LocRM.GetString("department");
			lblLocationTitle.Text = LocRM.GetString("location");
			lblCompanyTitle.Text = LocRM.GetString("company");
			lblTimeZoneTitle.Text = LocRM.GetString("time_zone");
			lblLangTitle.Text = LocRM.GetString("lang");
			lblGroupsTitle.Text = LocRM.GetString("groups");
			lblPGroupsTitle.Text = LocRM.GetString("groups");
			lblIMGroupTitle.Text = LocRM.GetString("im_groups");
			chbIsActive.Text = LocRM.GetString("active");
			lblAvailable.Text = LocRM.GetString("Available");
			lblSelected.Text = LocRM.GetString("Selected");
			txtRELoginValidator.ErrorMessage = "<br/>" + LocRM.GetString("LoginReg");
			lblSecurityRolesTitle.Text = LocRM.GetString("SecurityRoles");
			GroupValidator.ErrorMessage = LocRM.GetString("GroupSelectError") + LocRM.GetString("groups") + " " + LocRM.GetString("or") + " " + LocRM.GetString("SecurityRoles");
			cbOneMore.Text = LocRM.GetString("tAnotherOne");
			//lgdBasicInfo.InnerText = LocRM.GetString("tBasicInfo");
			//lgdPersonalInfo.InnerText = LocRM.GetString("tPersonalInfo");
			//lgdPreferencesInfo.InnerText = LocRM.GetString("tPreferences");
			//lgdGroupInfo.InnerText = LocRM.GetString("tGroupInfo");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			string BackUrl = "directory.aspx";

			if (UID != 0)
				tbSave.Title = LocRM.GetString("EditUser");
			else
				tbSave.Title = LocRM.GetString("CreateUser");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("BackToTheList"), "../Directory/" + BackUrl);
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			btnSave.Attributes.Add("onclick", "SaveGroups();DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrPersonalInfo.AddText(LocRM.GetString("tPersonalInfo"));
			hdrPreferencesInfo.AddText(LocRM.GetString("tPreferences"));
			hdrGroupInfo.AddText(LocRM.GetString("tGroupInfo"));

		}
		#endregion

		#region SelectGroup
		private void SelectGroup()
		{
			ListItem liPartnerGroup = ddPartnerGroups.Items.FindByValue(GroupId.ToString());
			if (liPartnerGroup != null)
			{
				ddPartnerGroups.ClearSelection();
				liPartnerGroup.Selected = true;
			}

			ListItem liSecurityRole = lbSecurityRoles.Items.FindByValue(GroupId.ToString());
			if (liSecurityRole != null)
				liSecurityRole.Selected = true;

			ListItem liGroup = lbAvailableGroups.Items.FindByValue(GroupId.ToString());
			if (liGroup != null)
			{
				ListItem liNewGroup = new ListItem(liGroup.Text, liGroup.Value);
				lbSelectedGroups.Items.Add(liNewGroup);
				lbAvailableGroups.Items.Remove(liGroup);
			}
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			BindVisibility();

			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + ");return false;");

			lbAvailableGroups.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			lbSelectedGroups.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");

			ddlIMGroup.DataTextField = "IMGroupName";
			ddlIMGroup.DataValueField = "IMGroupId";
			ddlIMGroup.DataSource = IMGroup.GetListIMGroupsWithoutPartners();
			ddlIMGroup.DataBind();

			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(false, false, false, false, false, false, false, false, false, false, false))
			{
				while (reader.Read())
				{
					lbAvailableGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			lbSecurityRoles.Items.Clear();
			int iRoleId = (int)InternalSecureGroups.Administrator;
			ListItem liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.HelpDeskManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.PowerProjectManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.ProjectManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());

			if (Configuration.ProjectManagementEnabled)
				liSecRole.Selected = true;

			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.ExecutiveManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			iRoleId = (int)InternalSecureGroups.TimeManager;
			liSecRole = new ListItem(GetGroupTitleById(iRoleId), iRoleId.ToString());
			lbSecurityRoles.Items.Add(liSecRole);

			lstLang.DataSource = Common.GetListLanguages();
			lstLang.DataTextField = "FriendlyName";
			lstLang.DataValueField = "LanguageId";
			lstLang.DataBind();
			int LanguageId = Security.CurrentUser.LanguageId;
			Util.CommonHelper.SafeSelect(lstLang, LanguageId.ToString());

			lstTimeZone.DataSource = User.GetListTimeZone();
			lstTimeZone.DataTextField = "DisplayName";
			lstTimeZone.DataValueField = "TimeZoneId";
			lstTimeZone.DataBind();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			lstTimeZone.ClearSelection();
			ListItem li = lstTimeZone.Items.FindByValue(TimeZoneId.ToString());
			if (li != null) li.Selected = true;

			using (IDataReader reader = SecureGroup.GetListChildGroups((int)InternalSecureGroups.Partner))
			{
				while (reader.Read())
				{
					ddPartnerGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			if (ddPartnerGroups.Items.Count == 0)
			{
				ListItem liUserType = ddUserType.Items.FindByValue("1");
				if (liUserType != null)
				{
					ddUserType.Items.Remove(liUserType);
					if (ddUserType.SelectedIndex < 0)
						ddUserType.SelectedIndex = 0;
					BindVisibility();
				}
			}

			EntityObject[] profiles = BusinessManager.List(CustomizationProfileEntity.ClassName, (new FilterElementCollection()).ToArray(), (new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc))).ToArray());
			foreach (CustomizationProfileEntity profile in profiles)
			{
				ProfileList.Items.Add(new ListItem(CommonHelper.GetResFileString(profile.Name), profile.PrimaryKeyId.ToString()));
			}
			ListItem liProfile = ProfileList.Items.FindByValue("-1");
			if (liProfile != null)
				liProfile.Selected = true;

			SelectGroup();
		}
		#endregion

		#region BindVisibility
		private void BindVisibility()
		{
			switch (ddUserType.SelectedItem.Value)
			{
				case "0": //Regular
					trLogin.Visible = true;
					trWinLogin.Visible = true;
					trPassword1.Visible = true;
					trPassword2.Visible = true;
					chbIsActive.Visible = true;
					ddlIMGroup.Visible = true;
					lblIMGroupTitle.Visible = true;
					trGroups.Visible = true;
					trSecurityRoles.Visible = true;
					trPGroups.Visible = false;
					ProfileRow.Visible = true;
					break;
				case "1": // partner
					trLogin.Visible = true;
					trWinLogin.Visible = false;
					trPassword1.Visible = true;
					trPassword2.Visible = true;
					chbIsActive.Visible = true;
					ddlIMGroup.Visible = false;
					lblIMGroupTitle.Visible = false;
					trGroups.Visible = false;
					trSecurityRoles.Visible = false;
					trPGroups.Visible = true;
					ProfileRow.Visible = true;
					break;
				case "2": //External
					trLogin.Visible = false;
					trWinLogin.Visible = false;
					trPassword1.Visible = false;
					trPassword2.Visible = false;
					chbIsActive.Visible = true;
					ddlIMGroup.Visible = false;
					lblIMGroupTitle.Visible = false;
					trGroups.Visible = true;
					trSecurityRoles.Visible = false;
					trPGroups.Visible = false;
					ProfileRow.Visible = false;
					break;
				case "3": //Pending
					trLogin.Visible = true;
					trWinLogin.Visible = false;
					trPassword1.Visible = true;
					trPassword2.Visible = true;
					chbIsActive.Visible = false;

					ddlIMGroup.Visible = false;
					lblIMGroupTitle.Visible = false;
					trGroups.Visible = true;
					trSecurityRoles.Visible = false;
					trPGroups.Visible = false;
					ProfileRow.Visible = false;
					break;
			}
		}
		#endregion

		#region BindDDType
		private void BindDDType()
		{
			if (User.CanCreate())
				ddUserType.Items.Add(new ListItem(LocRM.GetString("RegularUser"), "0"));
			if (User.CanCreatePartner())
				ddUserType.Items.Add(new ListItem(LocRM.GetString("PartnerUser"), "1"));
			if (User.CanCreateExternal())
				ddUserType.Items.Add(new ListItem(LocRM.GetString("ExternalUser"), "2"));
			if (User.CanCreatePending())
				ddUserType.Items.Add(new ListItem(LocRM.GetString("PendingUser"), "3"));
			if (SecureGroup.IsPartner(GroupId) || GroupId == (int)InternalSecureGroups.Partner)
			{
				CommonHelper.SafeSelect(ddUserType, "1");
				BindVisibility();
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
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			string sGroups = iGroups.Value;
			ArrayList alGroups = new ArrayList();

			if (ddUserType.SelectedItem.Value != "1") //non partner
			{
				while (sGroups.Length > 0)
				{
					alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
					sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
				}

				if (ddUserType.SelectedItem.Value == "0") //regular
					foreach (ListItem liRole in lbSecurityRoles.Items)
						if (liRole.Selected)
							alGroups.Add(Int32.Parse(liRole.Value));

				if (alGroups.Count <= 0)
				{
					GroupValidator.IsValid = false;
					return;
				}
			}

			string filename = "";
			System.IO.Stream strres = new System.IO.MemoryStream();

			if (fPhoto.PostedFile != null && fPhoto.PostedFile.ContentLength > 0)
			{
				System.Drawing.Image img;
				string extension = "";
				img = Mediachase.Ibn.Web.UI.Images.ProcessImage(fPhoto.PostedFile, out extension);
				string photoid = Guid.NewGuid().ToString().Substring(0, 6);
				filename = photoid + extension;
				img.Save(strres, img.RawFormat);
				strres.Position = 0;
			}

			try
			{
				int imgroup = int.Parse(ddlIMGroup.SelectedItem.Value);

				switch (ddUserType.SelectedItem.Value)
				{
					case "0":
						UID = User.Create(txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text,
							txtEmail.Text, chbIsActive.Checked, alGroups, imgroup, txtPhone.Text,
							txtFax.Text, txtMobile.Text, txtJobTitle.Text, txtDepartment.Text, txtCompany.Text,
							txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value),
							int.Parse(lstLang.SelectedItem.Value), "", filename, strres, txtWinLogin.Text,
							int.Parse(ProfileList.SelectedValue));
						break;
					case "1":
						UID = User.CreatePartnerUser(txtLogin.Text, txtPassword.Text, txtFirstName.Text,
							txtLastName.Text, txtEmail.Text, chbIsActive.Checked,
							int.Parse(ddPartnerGroups.SelectedItem.Value), txtPhone.Text, txtFax.Text,
							txtMobile.Text, txtJobTitle.Text, txtDepartment.Text, txtCompany.Text,
							txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value),
							int.Parse(lstLang.SelectedItem.Value), filename, strres,
							int.Parse(ProfileList.SelectedValue));
						break;
					case "2":
						UID = User.CreateExternal(txtFirstName.Text, txtLastName.Text, txtEmail.Text, alGroups,
							chbIsActive.Checked, txtPhone.Text, txtFax.Text, txtMobile.Text, txtJobTitle.Text,
							txtDepartment.Text, txtCompany.Text, txtLocation.Text,
							int.Parse(lstTimeZone.SelectedItem.Value),
							int.Parse(lstLang.SelectedItem.Value), "", filename, strres);
						break;
					case "3":
						UID = User.CreatePending(txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, alGroups, txtPhone.Text, txtFax.Text, txtMobile.Text, txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value), int.Parse(lstLang.SelectedItem.Value), "", filename, strres);
						break;
				}
				if (!cbOneMore.Checked)
					Response.Redirect("~/Directory/UserView.aspx?UserID=" + UID);
				else
					Response.Redirect("~/Directory/MultipleUserEdit.aspx");
			}
			catch (MaxUsersCountException)
			{
				lblError.Text = LocRM.GetString("MaxUsersCount");
				lblError.Visible = true;
			}

			catch (LoginDuplicationException)
			{
				if (ddUserType.SelectedItem.Value == "2")
				{
					int iUserId = User.GetUserByEmail(txtEmail.Text);
					string sUserName = String.Empty;
					if (iUserId > 0)
						sUserName = CommonHelper.GetUserStatusPureName(iUserId);
					lblError.Text = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";
				}
				else
					lblError.Text = LocRM.GetString("DuplicatedLogin");
				lblError.Visible = true;
			}
			catch (EmailDuplicationException)
			{
				int iUserId = User.GetUserByEmail(txtEmail.Text);
				string sUserName = String.Empty;
				if (iUserId > 0)
					sUserName = CommonHelper.GetUserStatusPureName(iUserId);
				lblError.Text = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";
				lblError.Visible = true;
			}
			catch (PasswordRequiredException)
			{
				for (int i = 0; i < alGroups.Count; i++)
				{
					using (IDataReader reader = SecureGroup.GetGroup((int)alGroups[i]))
					{
						if (reader.Read())
						{
							lbSelectedGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), ((int)alGroups[i]).ToString()));
							ListItem li = lbAvailableGroups.Items.FindByValue(((int)alGroups[i]).ToString());
							if (li != null)
								lbAvailableGroups.Items.Remove(li);
						}
					}
				}
				PasswordValidator1.Validate();
				PasswordValidator1.Enabled = true;
				lblError.Text = LocRM.GetString("PasswordRequest");
				lblError.Visible = true;
			}
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			string BackUrl = "directory.aspx";
			if ((Request["Back"] != null) && (Request["Back"] == "View"))
				BackUrl = "UserView.aspx?UserID=" + UID;
			Response.Redirect("../Directory/" + BackUrl);

		}

		protected void ddUserType_PageChange(object sender, System.EventArgs e)
		{
			BindVisibility();
		}

		private string GetGroupTitleById(int GrId)
		{
			///		GroupId, GroupName, IMGroupId
			using (IDataReader reader = SecureGroup.GetGroup(GrId))
			{
				if (reader.Read())
					return CommonHelper.GetResFileString(reader["GroupName"].ToString());
			}
			return String.Empty;
		}
	}
}