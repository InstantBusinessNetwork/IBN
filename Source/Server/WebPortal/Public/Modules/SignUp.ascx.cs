namespace Mediachase.UI.Web.Public.Modules
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
	using System.Web.UI;
	using Mediachase.Ibn;


	/// <summary>
	///		Summary description for UserEdit.
	/// </summary>
	public partial class SignUp : System.Web.UI.UserControl
	{
		#region HTMLVars
		protected System.Web.UI.WebControls.RequiredFieldValidator txtPasswordRFValidator;
		protected System.Web.UI.WebControls.CompareValidator txtPasswordCompareValidator;
		protected System.Web.UI.WebControls.Label lblIMGroupsTitle;
		#endregion

		protected ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strUserEdit", typeof(SignUp).Assembly);
		//private int UID = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnCancel.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.Attributes.Add("onclick", "DisableButtons(this);");
			btnSave.CustomImage = this.Page.ResolveUrl("~/layouts/images/accept.gif");

			tblInputForm.Visible = true;
			lblReqWasSent.Visible = false;
			if (!Page.IsPostBack)
			{
				ApplyLocalization();
				BindValues();
			}
			BindToolbar();
			SetRequiredFields();

			Page.ClientScript.RegisterStartupScript(this.GetType(), "GetDate", "SetTimeZone();", true);
		}

		private void SetRequiredFields()
		{
			SetStyles(txtLogin);
			SetStyles(txtPassword);
			SetStyles(txtConfirm);
			SetStyles(txtFirstName);
			SetStyles(txtLastName);
			SetStyles(txtEmail);
		}

		private void SetStyles(TextBox txt)
		{
			txt.BorderWidth = 1;
			txt.BorderStyle = BorderStyle.Solid;
			txt.Style.Add(HtmlTextWriterStyle.Padding, "2px");
			txt.BorderColor = Color.FromArgb(127, 157, 185);
			txt.BackColor = (String.IsNullOrEmpty(txt.Text)) ? Color.FromArgb(247, 250, 195) : Color.White;
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Public.Resources.strUserEdit", typeof(SignUp).Assembly);
			ResourceManager LocRM2 = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(SignUp).Assembly);
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
			RegularExpressionValidator2.ErrorMessage = LocRM.GetString("tWrongFormat");
			lblWarning.Text = LocRM2.GetString("LoginReg");
			lblHeader.Text = String.Format(LocRM.GetString("Header"), IbnConst.ProductFamilyShort);
		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			tbSave.Title = LocRM.GetString("AccessRequest");
			btnSave.Text = LocRM.GetString("tbsave_save");
			btnCancel.Text = LocRM.GetString("tbsave_cancel");
		}
		#endregion

		#region BindValues
		private void BindValues()
		{
			using (IDataReader reader = Common.GetListLanguages())
			{
				while (reader.Read())
				{
					ListItem liItem = new ListItem(reader["FriendlyName"].ToString(), reader["LanguageId"].ToString());
					if (string.Compare(Thread.CurrentThread.CurrentUICulture.Name, reader["Locale"].ToString(), StringComparison.OrdinalIgnoreCase) == 0)
						liItem.Selected = true;
					lstLang.Items.Add(liItem);
				}
			}
			
			using (IDataReader reader = User.GetListTimeZone())
			{
				while (reader.Read())
				{
					lstTimeZone.Items.Add(new ListItem(reader["DisplayName"].ToString(), reader["Bias"].ToString() + "_" + reader["TimeZoneId"].ToString()));
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

			string filename = "";
			System.IO.Stream strres = new System.IO.MemoryStream();
			int UID = 0;
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
			
			string timeZone = lstTimeZone.SelectedValue;
			timeZone = timeZone.Substring(timeZone.IndexOf("_") + 1);
			int tZone = int.Parse(timeZone);

			try
			{
				UID = User.CreatePending(txtLogin.Text, txtPassword.Text, txtFirstName.Text, txtLastName.Text, txtEmail.Text, new ArrayList(),
				txtPhone.Text, txtFax.Text, txtMobile.Text,
				txtJobTitle.Text, txtDepartment.Text, txtCompany.Text, txtLocation.Text, tZone,
				int.Parse(lstLang.SelectedItem.Value), "", filename, strres);
				tblInputForm.Visible = false;
				lblReqWasSent.Visible = true;
				lblReqWasSent.Text = LocRM.GetString("tPendingWasCreated") + "<br><br><a href='../Public/default.aspx'>" + LocRM.GetString("tBackToHome") + "</a>";
			}
			catch (EmailDuplicationException)
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(SignUp).Assembly);
				lblError.Text = LocRM.GetString("EmailDuplicate");
				lblError.Visible = true;
			}
			catch (LoginDuplicationException)
			{
				ResourceManager LocRM = new ResourceManager("Mediachase.UI.Web.App_GlobalResources.Directory.Resources.strUserEdit", typeof(SignUp).Assembly);
				lblError.Text = LocRM.GetString("DuplicatedLogin");
				lblError.Visible = true;
			}
		}
		#endregion

		protected void btnCancel_ServerClick(object sender, System.EventArgs e)
		{
			Response.Redirect("../Public/default.aspx");
		}
	}
}
