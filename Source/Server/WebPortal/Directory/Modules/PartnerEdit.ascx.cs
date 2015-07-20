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


	/// <summary>
	///		Summary description for UserEdit.
	/// </summary>
	public partial  class PartnerEdit : System.Web.UI.UserControl
	{

		#region HTML Vars

		protected System.Web.UI.WebControls.RequiredFieldValidator txtPasswordRFValidator;
		protected System.Web.UI.WebControls.CompareValidator txtPasswordCompareValidator;
		protected System.Web.UI.HtmlControls.HtmlTable MainTable;
		protected System.Web.UI.WebControls.Label lblIMGroupsTitle;

		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdBasicInfo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdPersonalInfo;
		protected System.Web.UI.HtmlControls.HtmlGenericControl lgdAdditionalInfo;
		#endregion
		protected ResourceManager LocRM;
		private int UID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(Request["UserID"]!=null) UID = int.Parse(Request["UserID"]);
			LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(PartnerEdit).Assembly);
			RegularExpressionValidator2.ErrorMessage = LocRM.GetString("tWrongFormat");
			btnCancel.Attributes.Add("onclick","DisableButtons(this);");

			if(!Page.IsPostBack)
			{
				ApplyLocalization();
				BindValues();
			}
		}
		protected void Page_PreRender(object sender, System.EventArgs e)
		{
			BindToolbar();
		}
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
			chbIsActive.Text = LocRM.GetString("active");
			txtRELoginValidator.ErrorMessage = "<br>" + LocRM.GetString("LoginReg");
		}

		private void BindToolbar()
		{
			string BackUrl = "directory.aspx";

			if(UID!=0)
			{
				tbSave.Title = LocRM.GetString("EditPartnerUser");
				if(Security.IsUserInGroup(InternalSecureGroups.Administrator))
				{
					tbSave.AddLink(String.Format("<img alt='' src='{0}'/> {1}",
							ResolveClientUrl("~/Layouts/Images/mail.gif"), LocRM.GetString("tSendProfile")),
						"javascript:SendNotification()");
					tbSave.AddSeparator();			
				}
			}
			else
				tbSave.Title = LocRM.GetString("CreatePartnerUser");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("BackToTheList"),"../Directory/" + BackUrl);
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			btnSave.Attributes.Add("onclick","DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrPersonalInfo.AddText(LocRM.GetString("tPersonalInfo"));
			hdrAdditionalInfo.AddText(LocRM.GetString("tAdditionalInfo"));
		}

		protected void btnSend_Click(object sender, System.EventArgs e)
		{
			int[] recipients = new int[1];
			recipients[0] = UID;
			//Alert.SendAlertNow(AlertEventType.User_New, ObjectTypes.User, UID, recipients, false);
		}

		#region BindValues
		private void BindValues()
		{		
			if(UID!=0)
			{
				using (IDataReader reader = User.GetUserInfo(UID))
				{
					if (reader.Read())
					{	
						ViewState[UID + "IsExternal"] = false;
						ViewState[UID + "IsPending"] = false;

						string sLogin = reader["Login"].ToString();

						txtLogin.Visible=false;
						txtLoginRFValidator.Enabled = false;
						txtRELoginValidator.Enabled = false;
						txtLogin.Text = "";
						lblLogin.Text = sLogin;
						lblLogin.Visible = true;

						txtFirstName.Text = HttpUtility.HtmlDecode(reader["FirstName"].ToString());
						txtLastName.Text = HttpUtility.HtmlDecode(reader["LastName"].ToString());
						txtEmail.Text = HttpUtility.HtmlDecode(reader["Email"].ToString());

						if(!(bool)reader["IsActive"])chbIsActive.Checked = false;
					}
				}

				if(!User.CanUpdateSecureFields(UID))
				{
					chbIsActive.Enabled = false;
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
						//if ( reader["PictureUrl"] == DBNull.Value || (string)reader["PictureUrl"]=="" )
						//{
						//    Picture.Visible=false;
						//}
						//else
						//{
							imgPhoto.Src = "~/Common/GetUserPhoto.aspx?UserID=" + UID.ToString() + "&t=" + DateTime.Now.Millisecond.ToString();
						//    Picture.Visible=true;
						//}
					}
				}

				EntityObject[] profiles = BusinessManager.List(CustomizationProfileEntity.ClassName, (new FilterElementCollection()).ToArray(), (new SortingElementCollection(new SortingElement("Name", SortingElementType.Asc))).ToArray());
				foreach (CustomizationProfileEntity profile in profiles)
				{
					ProfileList.Items.Add(new ListItem(CommonHelper.GetResFileString(profile.Name), profile.PrimaryKeyId.ToString()));
				}
				EntityObject[] list = BusinessManager.List(CustomizationProfileUserEntity.ClassName, new FilterElement[] { FilterElement.EqualElement(CustomizationProfileUserEntity.FieldPrincipalId, UID) });
				if (list.Length > 0)
				{
					ListItem li = ProfileList.Items.FindByValue(((CustomizationProfileUserEntity)list[0]).ProfileId.ToString());
					if (li != null)
						li.Selected = true;
				}
				else
				{
					ListItem li = ProfileList.Items.FindByValue("-1");
					if (li != null)
						li.Selected = true;
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


		protected void btnSave_ServerClick(object sender, System.EventArgs e)
		{
			Page.Validate();
			if(!Page.IsValid)
				return;

			string filename = "";
			System.IO.Stream strres = new System.IO.MemoryStream();
			if (fPhoto.PostedFile!=null && fPhoto.PostedFile.ContentLength>0)
			{
				System.Drawing.Image img;
				string extension = "";
				img = Mediachase.Ibn.Web.UI.Images.ProcessImage(fPhoto.PostedFile, out extension);
				string photoid = Guid.NewGuid().ToString().Substring(0,6);
				filename = photoid+extension;
				img.Save(strres,img.RawFormat);
				strres.Position = 0;
			}

			try		
			{
				if(UID>0)
				{
					User.UpdatePartnerUser(UID,txtPassword.Text,txtFirstName.Text,txtLastName.Text,txtEmail.Text,chbIsActive.Checked, txtPhone.Text,txtFax.Text,txtMobile.Text,txtJobTitle.Text,txtDepartment.Text,txtCompany.Text,txtLocation.Text,filename,strres, 
						int.Parse(ProfileList.SelectedValue));

					Response.Redirect("../Directory/UserView.aspx?UserID=" + UID);
				}
				else
				{
					Response.Redirect("../Directory/Directory.aspx");
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
				if(iUserId > 0)
					sUserName = CommonHelper.GetUserStatusPureName(iUserId);
				lblError.Text = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";
				lblError.Visible = true;
			}
			catch (LoginDuplicationException) 
			{
				lblError.Text = LocRM.GetString("DuplicatedLogin");
				lblError.Visible = true;
			}		
			catch(PasswordRequiredException)
			{
				PasswordValidator1.Validate();
				PasswordValidator1.Enabled = true;
				lblError.Text = LocRM.GetString("PasswordRequest");
				lblError.Visible = true;
			}
		}

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			string BackUrl = "directory.aspx";
			if((Request["Back"]!=null) && (Request["Back"]=="View"))
				BackUrl = "UserView.aspx?UserID=" + UID;
			Response.Redirect("../Directory/" + BackUrl);

		}
	}
}