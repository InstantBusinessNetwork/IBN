using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Drawing;
using System.Resources;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Configuration;


namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	/// Summary description for CompanyInfo.
	/// </summary>
	public partial class SiteCreate : System.Web.UI.UserControl, IPageTemplateTitle
	{
		[DllImport("advapi32.DLL", EntryPoint = "LogonUserW", SetLastError = true,
			CharSet = CharSet.Unicode, ExactSpelling = true,
			CallingConvention = CallingConvention.StdCall)]
		private static extern bool extLogonUserW(string lpszUsername, string lpszDomain,
			string lpszPassword, int dwLogonType, int dwLogonProvider, out int phToken);
		[DllImport("advapi32.DLL", EntryPoint = "ImpersonateLoggedOnUser", SetLastError = true,
			CharSet = CharSet.Unicode, ExactSpelling = true,
			CallingConvention = CallingConvention.StdCall)]
		public static extern bool extImpersonateLoggedOnUser(int hToken);
		[DllImport("advapi32.DLL", EntryPoint = "RevertToSelf", SetLastError = true,
			CharSet = CharSet.Unicode, ExactSpelling = true,
			CallingConvention = CallingConvention.StdCall)]
		private static extern bool extRevertToSelf();

		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.SiteCreate", typeof(SiteCreate).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				ApplyLocalization();
				BindData();
			}
			//DataBind();
			this.Page.ClientScript.RegisterStartupScript(this.GetType(), Guid.NewGuid().ToString(), "DemoClick();", true);
		}

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			secH.Title = LocRM.GetString("tbTitle");
			Demo.Text = LocRM.GetString("IsDemo");
			IsActive.Text = LocRM.GetString("IsActive");
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
			this.cvCompareDate.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cv_Validate);

		}
		#endregion

		#region BindData
		private void BindData()
		{
			ListItem li;
			IConfigurator config = Configurator.Create();

			ddLanguage.DataSource = config.ListLanguages();
			ddLanguage.DataTextField = "FriendlyName";
			ddLanguage.DataValueField = "Locale";
			ddLanguage.DataBind();

			ApplicationPoolList.Items.Add(new ListItem(LocRM.GetString("NewPool"), ""));
			foreach (string poolName in config.ListApplicationPools())
			{
				ApplicationPoolList.Items.Add(poolName);
			}

			AspSettings settings = AspSettings.Load();

			txtMaxUsers.Text = settings.MaxUsers.ToString();
			tbExternal.Text = settings.MaxExternalUsers.ToString();
			txtDiskMax.Text = settings.MaxHDD.ToString();
			txtDomain.Text = "." + settings.DnsParentDomain;

			Demo.Attributes.Add("onclick", "DemoClick()");
			txtStartDate.Text = DateTime.Now.ToShortDateString();
			txtDateTo.Text = DateTime.Now.AddDays(settings.TrialPeriod).ToShortDateString();

			if (Request["Trial"] != null && Request["Trial"].ToString() == "1")
			{
				Demo.Checked = true;

				li = ddLanguage.Items.FindByValue("ru-RU");
				if (li != null)
				{
					ddLanguage.ClearSelection();
					li.Selected = true;
				}
			}

			if (Demo.Checked)
			{
				if (!string.IsNullOrEmpty(settings.DefaultTrialPool))
				{
					li = ApplicationPoolList.Items.FindByValue(settings.DefaultTrialPool);
					if (li != null)
						li.Selected = true;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(settings.DefaultBillablePool))
				{
					li = ApplicationPoolList.Items.FindByValue(settings.DefaultBillablePool);
					if (li != null)
						li.Selected = true;
				}
			}
		}
		#endregion

		#region OnPreRender
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
		}
		#endregion

		protected void Submit_Click(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			try
			{
				CManage.CompanyCreate(
					txtCompanyName.Text
					, txtDomain.Text
					, ddLanguage.SelectedItem.Value
					, IsActive.Checked
					, int.Parse(txtMaxUsers.Text)
					, int.Parse(tbExternal.Text)
					, int.Parse(txtDiskMax.Text)
					, txtContactName.Text
					, txtContactPhone.Text
					, txtContactEmail.Text
					, txtAdminFirstName.Text
					, txtAdminLastName.Text
					, txtAdminLogin.Text
					, txtAdminPassword.Text
					, txtAdminEMail.Text
					, Demo.Checked
					, DateTime.Parse(txtStartDate.Text)
					, System.DateTime.Parse(txtDateTo.Text)
					, ApplicationPoolList.SelectedValue
					);

				Response.Redirect("../Pages/sites.aspx");
			}
			catch (ConfigurationException ex)
			{
				cvErrorCreation.IsValid = false;
				cvErrorCreation.ErrorMessage = ex.Message;
			}
		}

		protected void Cancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("../Pages/sites.aspx");
		}

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("tbTitle");
		}
		#endregion

		private void cv_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (Demo.Checked)
			{
				try
				{
					//System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
					DateTime dts = DateTime.Parse(txtStartDate.Text);
					DateTime dtf = DateTime.Parse(txtDateTo.Text);
					if (dtf < dts)
						args.IsValid = false;
				}
				catch
				{
					args.IsValid = false;
				}
			}
		}
	}
}
