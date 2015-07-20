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


	/// <summary>
	///		Summary description for UserEdit.
	/// </summary>
	public partial class PendingEdit : System.Web.UI.UserControl
	{

		protected System.Web.UI.WebControls.RequiredFieldValidator txtPasswordRFValidator;
		protected System.Web.UI.WebControls.CompareValidator txtPasswordCompareValidator;
		protected System.Web.UI.HtmlControls.HtmlTable MainTable;
		protected System.Web.UI.WebControls.Label lblIMGroupsTitle;
		private int UID = 0;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/saveitem.gif");

			ApplyLocalization();
			BindToolbar();
			if (!Page.IsPostBack)
			{
				BindValues();
			}
		}

		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(PendingEdit).Assembly);
			RegularExpressionValidator2.ErrorMessage = LocRM.GetString("tWrongFormat");
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
			txtRELoginValidator.ErrorMessage = LocRM.GetString("LoginReg");
		}

		private void BindToolbar()
		{
			string BackUrl = "directory.aspx";

			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(PendingEdit).Assembly);
			tbSave.Title = LocRM.GetString("CreatePendingUser");
			tbSave.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("BackToTheList"), "../Directory/" + BackUrl);
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
		}


		private void BindValues()
		{

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
				UID = User.CreatePending(txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, new ArrayList(),
						txtPhone.Text, txtFax.Text, txtMobile.Text,
						txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, int.Parse(lstTimeZone.SelectedItem.Value),
						int.Parse(lstLang.SelectedItem.Value), "", filename, strres);
				Response.Redirect("../Directory/UserView.aspx?UserID=" + UID);
			}
			catch (EmailDuplicationException)
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(PendingEdit).Assembly);
				int iUserId = User.GetUserByEmail(txtEmail.Text);
				string sUserName = String.Empty;
				if (iUserId > 0)
					sUserName = CommonHelper.GetUserStatusPureName(iUserId);
				lblError.Text = LocRM.GetString("EmailDuplicate") + " (" + sUserName + ")";
				lblError.Visible = true;
			}
			catch (LoginDuplicationException)
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(PendingEdit).Assembly);
				lblError.Text = LocRM.GetString("DuplicatedLogin");
				lblError.Visible = true;
				// TODO: Сделать обработку
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
