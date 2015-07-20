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
	using Mediachase.Ibn.Core.Business;
	using Mediachase.Ibn.Business.Customization;
	using Mediachase.Ibn.Data;
	using Mediachase.Ibn.Web.UI.WebControls;


	/// <summary>
	///		Summary description for UserEdit.
	/// </summary>
	public partial class UserEdit : System.Web.UI.UserControl
	{

		#region HTMLVars

		protected System.Web.UI.WebControls.RequiredFieldValidator txtPasswordRFValidator;
		protected System.Web.UI.WebControls.CompareValidator txtPasswordCompareValidator;
		protected System.Web.UI.HtmlControls.HtmlTable MainTable;
		protected System.Web.UI.WebControls.Label lblIMGroupsTitle;

		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdBasicInfo;
		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdPersonalInfo;
		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdAdditionalInfo;
		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdGroupInfo;
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(UserEdit).Assembly);
		private int UID = 0;
		private bool userHasEmptyPassword = true;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			if (Request["UserID"] != null)
				UID = int.Parse(Request["UserID"]);

			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			ApplyLocalization();

			if (!Page.IsPostBack)
				BindValues();
		}

		#region Page_PreRender
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lblLoginTitle.Text = LocRM.GetString("login");
			lblPasswordTitle.Text = LocRM.GetString("password");
			lblConfirmTitle.Text = LocRM.GetString("confirm");
			lblFirstNameTitle.Text = LocRM.GetString("first_name");
			lblLastNameTitle.Text = LocRM.GetString("last_name");
			lblEmailTitle.Text = LocRM.GetString("email");
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
			lblIMGroupTitle.Text = LocRM.GetString("im_groups");
			chbIsActive.Text = LocRM.GetString("active");
			lblAvailable.Text = LocRM.GetString("Available");
			lblSelected.Text = LocRM.GetString("Selected");
			txtRELoginValidator.ErrorMessage = "<br/>" + LocRM.GetString("LoginReg");
			lblSecurityRolesTitle.Text = LocRM.GetString("SecurityRoles") + ":";
			GroupValidator.ErrorMessage = LocRM.GetString("GroupSelectError") + LocRM.GetString("groups") + " " + LocRM.GetString("or") + " " + LocRM.GetString("SecurityRoles");
			if (ViewState[UID + "IsPending"] != null && (bool)ViewState[UID + "IsPending"])
				btnSave.Text = LocRM.GetString("btnapprove");
			else
				btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			RegularExpressionValidator2.ErrorMessage = LocRM.GetString("tWrongFormat");

			//lgdBasicInfo.InnerText = LocRM.GetString("tBasicInfo");
			//lgdPersonalInfo.InnerText = LocRM.GetString("tPersonalInfo");
			//lgdAdditionalInfo.InnerText = LocRM.GetString("tAdditionalInfo");
			//lgdGroupInfo.InnerText = LocRM.GetString("tGroupInfo");
			lbWindowsLoginTitle.Text = LocRM.GetString("tWindowsLogin");
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			string BackUrl = "directory.aspx";

			if (UID != 0 && groupInfoRow.Visible)	// don't show for AlertService user
			{
				tbSave.Title = LocRM.GetString("EditUser");
				if (UID != Security.CurrentUser.UserID
					&& !userHasEmptyPassword
					&& Security.IsUserInGroup(InternalSecureGroups.Administrator))
				{
					tbSave.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
							ResolveClientUrl("~/Layouts/Images/mail.gif"), LocRM.GetString("tSendProfile")),
						"javascript:SendNotification()");
					tbSave.AddSeparator();
				}
			}
			else
				tbSave.Title = LocRM.GetString("CreateUser");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("BackToTheList"), "../Directory/" + BackUrl);
			btnSave.Attributes.Add("onclick", "SaveGroups();DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrPersonalInfo.AddText(LocRM.GetString("tPersonalInfo"));
			hdrAdditionalInfo.AddText(LocRM.GetString("tAdditionalInfo"));
			hdrGroupInfo.AddText(LocRM.GetString("tGroupInfo"));
		}
		#endregion

		#region btnSend_Click
		protected void btnSend_Click(object sender, System.EventArgs e)
		{
			User.Notify(UID);
		}
		#endregion

		#region GetGroupTitleById
		private string GetGroupTitleById(int GrId)
		{
			///		GroupId, GroupName, IMGroupId
			using (IDataReader reader = SecureGroup.GetGroup(GrId))
			{
				if (reader.Read())
				{
					return CommonHelper.GetResFileString(reader["GroupName"].ToString());
				}
			}
			return String.Empty;
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
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

			lstTimeZone.DataSource = User.GetListTimeZone();
			lstTimeZone.DataTextField = "DisplayName";
			lstTimeZone.DataValueField = "TimeZoneId";
			lstTimeZone.DataBind();

			int TimeZoneId = Security.CurrentUser.TimeZoneId;
			lstTimeZone.ClearSelection();
			ListItem li = lstTimeZone.Items.FindByValue(TimeZoneId.ToString());
			if (li != null) li.Selected = true;

			EntityObject[] profiles = BusinessManager.List(CustomizationProfileEntity.ClassName, (new FilterElementCollection()).ToArray(), (new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc))).ToArray());
			foreach (CustomizationProfileEntity profile in profiles)
			{
				ProfileList.Items.Add(new ListItem(CommonHelper.GetResFileString(profile.Name), profile.PrimaryKeyId.ToString()));
			}
			EntityObject[] list = BusinessManager.List(CustomizationProfileUserEntity.ClassName, new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, UID) });
			if (list.Length > 0)
			{
				li = ProfileList.Items.FindByValue(((CustomizationProfileUserEntity)list[0]).ProfileId.ToString());
				if (li != null)
					li.Selected = true;
			}
			else
			{
				li = ProfileList.Items.FindByValue("-1");
				if (li != null)
					li.Selected = true;
			}

			if (UID != 0)
			{
				if (Security.IsUserInGroup(InternalSecureGroups.Administrator))
				{
					tdWindowsLoginLabel.Visible = false;
					tdWindowsLoginTextBox.Visible = true;
				}
				else
				{
					tdWindowsLoginLabel.Visible = true;
					tdWindowsLoginTextBox.Visible = false;
				}
				using (IDataReader reader = User.GetUserInfo(UID))
				{
					if (reader.Read())
					{
						if ((bool)reader["IsExternal"])
							ViewState[UID + "IsExternal"] = true;
						else
							ViewState[UID + "IsExternal"] = false;

						if ((bool)reader["IsPending"])
						{
							ViewState[UID + "IsPending"] = true;
							btnSave.Text = LocRM.GetString("btnapprove");
							chbIsActive.Text = LocRM.GetString("active") + ",&nbsp;<font color='red'>" + LocRM.GetString("PendingUser") + "</font>";
						}
						else
							ViewState[UID + "IsPending"] = false;

						string sLogin = reader["Login"].ToString();
						if (sLogin.ToLower() == "alert")	// AlertService user
						{
							groupInfoRow.Visible = false;
							chbIsActive.Enabled = false;
							tbWindowsLogin.Enabled = false;
							txtPassword.Enabled = false;
							txtConfirm.Enabled = false;
						}

						if (!(bool)ViewState[UID + "IsExternal"])
						{
							trTimeZone.Visible = false;
							trLang.Visible = false;
							PasswordValidator1.Enabled = false;
							if (sLogin.IndexOf("___PENDING_USER___") < 0)
							{
								txtLogin.Visible = false;
								txtLoginRFValidator.Enabled = false;
								txtRELoginValidator.Enabled = false;
								txtLogin.Text = "";
								lblLogin.Text = sLogin;
								lblLogin.Visible = true;
							}
						}
						txtFirstName.Text = HttpUtility.HtmlDecode(reader["FirstName"].ToString());
						txtLastName.Text = HttpUtility.HtmlDecode(reader["LastName"].ToString());
						txtEmail.Text = HttpUtility.HtmlDecode(reader["Email"].ToString());
						if (reader["IMGroupId"] != null)
							CommonHelper.SafeSelect(ddlIMGroup, reader["IMGroupId"].ToString());
						if (!(bool)reader["IsActive"])
							chbIsActive.Checked = false;

						lbWindowsLogin.Text = tbWindowsLogin.Text = reader["WindowsLogin"].ToString();

						if (reader["Password"].ToString() != string.Empty)
							userHasEmptyPassword = false;
					}
				}

				//if(!(bool)ViewState[UID + "IsExternal"] && !(bool)ViewState[UID + "IsPending"])
				//{
				using (IDataReader reader = User.GetListSecureGroup(UID))
				{
					while (reader.Read())
					{
						lbSelectedGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
					}
				}
				foreach (ListItem liSelectedGroup in lbSelectedGroups.Items)
					CommonHelper.SafeMultipleSelect(lbSecurityRoles, liSelectedGroup.Value);
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.Everyone).ToString()));
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.Administrator).ToString()));
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.HelpDeskManager).ToString()));
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.PowerProjectManager).ToString()));
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.ProjectManager).ToString()));
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.ExecutiveManager).ToString()));
				lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.TimeManager).ToString()));
				lbSelectedGroups.DataBind();
				//}		

				for (int i = 0; i < lbSelectedGroups.Items.Count; i++)
				{
					if (lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value) != null)
						lbAvailableGroups.Items.Remove(lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value));
					iGroups.Value += lbSelectedGroups.Items[i].Value + ",";

				}

				if (!User.CanUpdateSecureFields(UID))
				{
					trGroups.Visible = false;
					ddlIMGroup.Enabled = false;
					chbIsActive.Enabled = false;
					tblSecurityRoles.Visible = false;
					ProfileRow.Visible = false;
				}

				using (IDataReader reader = User.GetUserProfile(UID))
				{
					if (reader.Read())
					{
						txtPhone.Text = HttpUtility.HtmlDecode(reader["phone"].ToString());
						txtFax.Text = HttpUtility.HtmlDecode(reader["fax"].ToString());
						txtMobile.Text = HttpUtility.HtmlDecode(reader["mobile"].ToString());
						txtJobTitle.Text = HttpUtility.HtmlDecode(reader["position"].ToString());
						txtDepartment.Text = HttpUtility.HtmlDecode(reader["department"].ToString());
						txtLocation.Text = HttpUtility.HtmlDecode(reader["location"].ToString());
						txtCompany.Text = HttpUtility.HtmlDecode(reader["company"].ToString());
						if (reader["PictureUrl"] == DBNull.Value || (string)reader["PictureUrl"] == "")
						{
							Picture.Visible = false;
							cbDelete.Visible = false;
						}
						else
						{
							imgPhoto.Src = "~/Common/GetUserPhoto.aspx?UserID=" + UID.ToString() + "&t=" + DateTime.Now.Millisecond.ToString();
							Picture.Visible = true;
							cbDelete.Text = "&nbsp;" + LocRM.GetString("tDeletePhoto");
							cbDelete.Visible = true;
						}
					}
				}
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

		#region btnSave_ServerClick
		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if (!Page.IsValid)
				return;
			string sGroups = iGroups.Value;
			ArrayList alGroups = new ArrayList();

			while (sGroups.Length > 0)
			{
				alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
				sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
			}

			foreach (ListItem liRole in lbSecurityRoles.Items)
				if (liRole.Selected)
					alGroups.Add(Int32.Parse(liRole.Value));

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

				bool bApprovePending = false;
				if (UID > 0)
				{
					if (User.CheckUserGroup(Security.CurrentUser.UserID, (int)InternalSecureGroups.Administrator))
					{
						if (tdWindowsLoginTextBox.Visible)
						{
							User.UpdateWindowsLogin(UID, tbWindowsLogin.Text.Trim());
						}
					}
					if ((bool)ViewState[UID + "IsPending"])
					{
						User.ApprovePending(UID, (lblLogin.Text != "") ? lblLogin.Text : txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text,
							chbIsActive.Checked, alGroups, imgroup, txtPhone.Text, txtFax.Text, txtMobile.Text,
							txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, filename, strres,
							int.Parse(ProfileList.SelectedValue));
						ViewState[UID + "IsPending"] = false;
						bApprovePending = true;
					}
					else if ((bool)ViewState[UID + "IsExternal"])
					{
						User.CreateFromExternal(UID, txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text,
							chbIsActive.Checked, alGroups, imgroup, txtPhone.Text, txtFax.Text, txtMobile.Text,
							txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value),
							int.Parse(lstLang.SelectedItem.Value), filename, strres);
						ViewState[UID + "IsExternal"] = false;
					}
					else
						User.UpdateUserInfo(UID, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, chbIsActive.Checked,
							alGroups, imgroup, txtPhone.Text, txtFax.Text, txtMobile.Text,
							txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, filename, strres, cbDelete.Checked,
							int.Parse(ProfileList.SelectedValue));
				}
				else
					UID = User.Create(txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, chbIsActive.Checked,
						alGroups, imgroup, txtPhone.Text, txtFax.Text, txtMobile.Text,
						txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value),
						int.Parse(lstLang.SelectedItem.Value), filename, strres, int.Parse(ProfileList.SelectedValue));
				if (UID != Security.CurrentUser.UserID)
				{
					if (!bApprovePending)
						Response.Redirect("../Directory/UserView.aspx?UserID=" + UID);
					else
						Util.CommonHelper.ReloadTopFrame("ActiveWork.ascx", "../Directory/UserView.aspx?UserID=" + UID, Response);
				}
				else
				{
					Picture.Visible = false;
					Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(),
								  "window.top.location.href='../Directory/UserView.aspx?UserID=" + UID.ToString() + "';", true);
				}
			}
			catch (MaxUsersCountException)
			{
				lblError.Text = LocRM.GetString("MaxUsersCount");
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
			catch (LoginDuplicationException)
			{
				lblError.Text = LocRM.GetString("DuplicatedLogin");
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
		#endregion

		#region btnCancel_ServerClick
		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			string BackUrl = "directory.aspx";
			if ((Request["Back"] != null) && (Request["Back"] == "View"))
				BackUrl = "UserView.aspx?UserID=" + UID;
			Response.Redirect("../Directory/" + BackUrl);

		}
		#endregion
	}
}
