using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Configuration;


namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for TrialReqEdit.
	/// </summary>
	public partial class TrialReqEdit : System.Web.UI.UserControl
	{

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.TrialReqEdit", typeof(TrialReqEdit).Assembly);

		private int ReqId
		{
			get
			{
				try
				{
					return int.Parse(Request["id"]);
				}
				catch
				{
					return -1;
				}
			}
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!Page.IsPostBack)
			{
				BindToolbar();
				ApplyLocalization();
				BindInfo();
			}
		}

		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbEditTitle");
		}

		private void ApplyLocalization()
		{
			lblCompName.Text = LocRM.GetString("tCompName") + ":";
			lblDescr.Text = LocRM.GetString("tDescr") + ":";
			lblDomain.Text = LocRM.GetString("tDomain") + ":";
			lblDefLang.Text = LocRM.GetString("tDefLang") + ":";
			lblEMail.Text = LocRM.GetString("tEMail") + ":";
			lblFirstName.Text = LocRM.GetString("tFName") + ":";
			lblLastName.Text = LocRM.GetString("tLName") + ":";
			lblLogin.Text = LocRM.GetString("tLogin") + ":";
			lblPassword.Text = LocRM.GetString("tPassword") + ":";
			lblPhone.Text = LocRM.GetString("tPhone") + ":";
			btnSave.Text = LocRM.GetString("tSave");
			btnCreate.Text = LocRM.GetString("tCreate");
			btnCancel.Text = LocRM.GetString("tCancel");
			lblCountry.Text = LocRM.GetString("tCountry");
		}

		private void BindInfo()
		{
			string locale = "en-US";
			if (ReqId > 0)
			{
				using (IDataReader reader = DBTrialRequest.Get(ReqId))
				{
					if (reader.Read())
					{
						txtCompName.Text = reader["CompanyName"].ToString();
						txtDescr.Text = reader["Description"].ToString();
						txtDomain.Text = reader["Domain"].ToString();
						txtEMail.Text = reader["Email"].ToString();
						txtFirstName.Text = reader["FirstName"].ToString();
						txtLastName.Text = reader["LastName"].ToString();
						txtLogin.Text = reader["Login"].ToString();
						txtPassword.Text = reader["Password"].ToString();
						txtPhone.Text = reader["Phone"].ToString();
						locale = reader["Locale"].ToString();
						txtCountry.Text = reader["Country"].ToString();
					}
				}

				ddLanguage.DataSource = Configurator.Create().ListLanguages(); 
				ddLanguage.DataTextField = "FriendlyName";
				ddLanguage.DataValueField = "Locale";
				ddLanguage.DataBind();
				ListItem li = ddLanguage.Items.FindByValue(locale);
				if (li != null)
					li.Selected = true;
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

		#region SaveRequest()
		private void SaveRequest()
		{
			Page.Validate();
			if (!Page.IsValid)
				return;

			DBTrialRequest.Update(
				ReqId
				, txtCompName.Text
				, txtDescr.Text
				, txtDomain.Text
				, txtFirstName.Text
				, txtLastName.Text
				, txtEMail.Text
				, txtPhone.Text
				, txtCountry.Text
				, txtLogin.Text
				, txtPassword.Text
				, ddLanguage.SelectedItem.Value
				);
		}
		#endregion

		protected void btnSave_Click(object sender, System.EventArgs e)
		{
			SaveRequest();
			Response.Redirect("TrialReqView.aspx?id=" + ReqId.ToString());
		}

		protected void btnCreate_Click(object sender, System.EventArgs e)
		{
			SaveRequest();
			CManage.ASPCreateTrialCompany(ReqId);
			Response.Redirect("TrialRequests.aspx");
		}

		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("TrialReqView.aspx?id=" + ReqId.ToString());
		}
	}
}
