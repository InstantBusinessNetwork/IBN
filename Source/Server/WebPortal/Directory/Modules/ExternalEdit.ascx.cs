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
	using Mediachase.Ibn.Web.UI.WebControls;


	/// <summary>
	///		Summary description for UserEdit.
	/// </summary>
	public partial class ExternalEdit : System.Web.UI.UserControl
	{
		#region HTML Vars

		protected System.Web.UI.WebControls.RequiredFieldValidator txtPasswordRFValidator;
		protected System.Web.UI.WebControls.CompareValidator txtPasswordCompareValidator;
		protected System.Web.UI.HtmlControls.HtmlTable MainTable;
		protected System.Web.UI.WebControls.Label lblIMGroupsTitle;

		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdBasicInfo;
		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdPersonalInfo;
		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdAdditionalInfo;
		//protected System.Web.UI.HtmlControls.HtmlGenericControl lgdGroupInfo;
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(ExternalEdit).Assembly);
		private int UID = 0;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			UtilHelper.RegisterScript(Page, "~/Scripts/List2List.js");

			if (Request["UserID"] != null) UID = int.Parse(Request["UserID"]);
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");

			if (!Page.IsPostBack)
			{
				ApplyLocalization();
				BindValues();
			}
			BindToolbar();
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
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

			lblGroupsTitle.Text = LocRM.GetString("groups");
			lblAvailable.Text = LocRM.GetString("Available");
			lblSelected.Text = LocRM.GetString("Selected");
			GroupValidator.ErrorMessage = LocRM.GetString("GroupSelectError") + LocRM.GetString("groups");
			RegularExpressionValidator2.ErrorMessage = LocRM.GetString("tWrongFormat");
			//lgdBasicInfo.InnerText = LocRM.GetString("tBasicInfo");
			//lgdPersonalInfo.InnerText = LocRM.GetString("tPersonalInfo");
			//lgdAdditionalInfo.InnerText = LocRM.GetString("tAdditionalInfo");
			//lgdGroupInfo.InnerText = LocRM.GetString("tGroupInfo");
		}
		#endregion

		private void BindToolbar()
		{
			string BackUrl = "directory.aspx";

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(ExternalEdit).Assembly);
			if (UID != 0)
				tbSave.Title = LocRM.GetString("EditExternalUser");
			else
				tbSave.Title = LocRM.GetString("CreateExternalUser");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("BackToTheList"), "../Directory/" + BackUrl);
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
			btnSave.Attributes.Add("onclick", "SaveGroups();DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			hdrBasicInfo.AddText(LocRM.GetString("tBasicInfo"));
			hdrPersonalInfo.AddText(LocRM.GetString("tPersonalInfo"));
			hdrAdditionalInfo.AddText(LocRM.GetString("tAdditionalInfo"));
			hdrGroupInfo.AddText(LocRM.GetString("tGroupInfo"));
		}


		private void BindValues()
		{
			btnAddOneGr.Attributes.Add("onclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnAddAllGr.Attributes.Add("onclick", "MoveAll(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			btnRemoveOneGr.Attributes.Add("onclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");
			btnRemoveAllGr.Attributes.Add("onclick", "MoveAll(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + ");return false;");

			lbAvailableGroups.Attributes.Add("ondblclick", "MoveOne(" + lbAvailableGroups.ClientID + "," + lbSelectedGroups.ClientID + "); return false;");
			lbSelectedGroups.Attributes.Add("ondblclick", "MoveOne(" + lbSelectedGroups.ClientID + "," + lbAvailableGroups.ClientID + "); return false;");

			using (IDataReader reader = SecureGroup.GetListGroupsWithParameters(false, false, false, false, false, false, false, false, false, false, false))
			{
				while (reader.Read())
				{
					lbAvailableGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}

			using (IDataReader reader = User.GetListSecureGroup(UID))
			{
				while (reader.Read())
				{
					lbSelectedGroups.Items.Add(new ListItem(CommonHelper.GetResFileString(reader["GroupName"].ToString()), reader["GroupId"].ToString()));
				}
			}
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.Everyone).ToString()));
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.Administrator).ToString()));
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.HelpDeskManager).ToString()));
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.PowerProjectManager).ToString()));
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.ExecutiveManager).ToString()));
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.ProjectManager).ToString()));
			lbSelectedGroups.Items.Remove(lbSelectedGroups.Items.FindByValue(((int)InternalSecureGroups.TimeManager).ToString()));

			for (int i = 0; i < lbSelectedGroups.Items.Count; i++)
			{
				if (lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value) != null)
					lbAvailableGroups.Items.Remove(lbAvailableGroups.Items.FindByValue(lbSelectedGroups.Items[i].Value));
				iGroups.Value += lbSelectedGroups.Items[i].Value + ",";

			}

			if (UID != 0)
			{
				using (IDataReader reader = User.GetUserInfo(UID))
				{
					if (reader.Read())
					{
						txtFirstName.Text = HttpUtility.HtmlDecode(reader["FirstName"].ToString());
						txtLastName.Text = HttpUtility.HtmlDecode(reader["LastName"].ToString());
						txtEmail.Text = HttpUtility.HtmlDecode(reader["Email"].ToString());
						if (!(bool)reader["IsActive"]) chbIsActive.Checked = false;
					}
				}
				if (!User.CanUpdateSecureFields(UID))
				{
					chbIsActive.Enabled = false;
					trGroups.Visible = false;
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
						//Picture.Visible=true;
						//}
					}
				}
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

			while (sGroups.Length > 0)
			{
				alGroups.Add(Int32.Parse(sGroups.Substring(0, sGroups.IndexOf(","))));
				sGroups = sGroups.Remove(0, sGroups.IndexOf(",") + 1);
			}

			//foreach (ListItem liRole in lbSecurityRoles.Items)
			//if(liRole.Selected)
			//alGroups.Add(Int32.Parse(liRole.Value));

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

				if (UID > 0)
				{

					User.UpdateExternalInfo(UID, txtFirstName.Text, txtLastName.Text, txtEmail.Text, alGroups,
						chbIsActive.Checked, txtPhone.Text, txtFax.Text, txtMobile.Text,
						txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, filename, strres);
					Response.Redirect("../Directory/UserView.aspx?UserID=" + UID);
				}
				else
				{
					Response.Redirect("../Directory/Directory.aspx");
				}

			}
			catch (MaxUsersCountException)
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(ExternalEdit).Assembly);

				lblError.Text = LocRM.GetString("MaxUsersCount");
				lblError.Visible = true;
			}
			catch (LoginDuplicationException)
			{
				// TODO: Сделать обработку
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(ExternalEdit).Assembly);
				int iUserId = User.GetUserByEmail(txtEmail.Text);
				string sUserName = String.Empty;
				if (iUserId > 0)
					sUserName = CommonHelper.GetUserStatusPureName(iUserId);
				lblError.Text = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";

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
	}
}
