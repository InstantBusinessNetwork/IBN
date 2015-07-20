using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.WebAsp.Modules
{
	/// <summary>
	///		Summary description for CompanyInfo.
	/// </summary>
	public partial class SiteEdit : System.Web.UI.UserControl, IPageTemplateTitle
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.SiteEdit", typeof(SiteEdit).Assembly);

		#region CompanyUid
		protected Guid CompanyUid
		{
			get
			{
				try
				{
					return new Guid(Request["id"].ToString());
				}
				catch (Exception)
				{
					return Guid.Empty;
				}
			}
		}
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			btnSave.Text = LocRM.GetString("Save");
			btnCancel.Text = LocRM.GetString("Cancel");

			if (!IsPostBack)
			{
				BindLists();
				BindData();
			}
			BindToolbar();
		}

		#region BindLists()
		private void BindLists()
		{
			TypeList.Items.Add(new ListItem("Billable", ((byte)CompanyType.Billable).ToString()));
			TypeList.Items.Add(new ListItem("Trial", ((byte)CompanyType.Trial).ToString()));

			SchemeList.Items.Add(new ListItem("http"));
			SchemeList.Items.Add(new ListItem("https"));

			TariffList.Items.Add(new ListItem(LocRM.GetString("NoTariff"), "-1"));
			using (IDataReader reader = Tariff.GetTariff(0, 0))
			{
				while (reader.Read())
				{
					string text = String.Concat((string)reader["tariffName"], ", ", (string)reader["symbol"]);
					TariffList.Items.Add(new ListItem(text, reader["tariffId"].ToString()));
				}
			}

		}
		#endregion

		#region BindToolbar
		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbTitle");
			IsActive.Text = LocRM.GetString("IsActive");
			SendSpamCheckBox.Text = LocRM.GetString("SendSpam");
		}
		#endregion

		#region BindData
		private void BindData()
		{
			ListItem li;

			AspSettings settings = AspSettings.Load();

			string companyId = CompanyUid.ToString();
			IConfigurator config = Configurator.Create();
			ICompanyInfo info = config.GetCompanyInfo(companyId);

			txtOccupiedDiskSpace.Text = info.DatabaseSize.ToString(CultureInfo.CurrentUICulture);
			MaxDiskSpaceValue.Text = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyDatabaseSize);

			txtActiveInternalUsers.Text = info.InternalUsersCount.ToString(CultureInfo.CurrentUICulture);
			MaxUsersValue.Text = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyMaxUsers);

			txtActiveExternalUsers.Text = info.ExternalUsersCount.ToString(CultureInfo.CurrentUICulture);
			MaxExternalUsersValue.Text = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyMaxExternalUsers);

			txtDomain.Text = info.Host;
			IsActive.Checked = info.IsActive;

			li = SchemeList.Items.FindByValue(info.Scheme);
			if (li != null)
				li.Selected = true;

			PortValue.Text = info.Port;

			using (IDataReader reader = CManage.GetCompany(CompanyUid))
			{
				if (reader.Read())
				{
					txtCompanyName.Text = reader["company_name"].ToString();
					if (reader["contact_name"] != DBNull.Value)
						txtContactName.Text = reader["contact_name"].ToString();
					if (reader["contact_phone"] != DBNull.Value)
						txtContactPhone.Text = reader["contact_phone"].ToString();
					if (reader["contact_email"] != DBNull.Value)
						txtContactEmail.Text = reader["contact_email"].ToString();

					bool showTariffInfo = true;
					if (config.GetCompanyPropertyValue(companyId, CManage.keyCompanyType) == ((byte)CompanyType.Trial).ToString())
					{
						TypeList.Items.FindByValue(((byte)CompanyType.Trial).ToString()).Selected = true;
						txtDateFrom.Text = ((DateTime)reader["start_date"]).ToShortDateString();

						string endDateString = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyEndDate);
						if (!String.IsNullOrEmpty(endDateString))
							txtDateTo.Text = DateTime.Parse(endDateString, CultureInfo.InvariantCulture).ToShortDateString();

						TariffRow.Visible = false;
						showTariffInfo = false;
					}
					else
					{
						TypeList.Items.FindByValue(((byte)CompanyType.Billable).ToString()).Selected = true;
						txtDateFrom.Text = DateTime.Now.Date.ToShortDateString();
						txtDateTo.Text = DateTime.Now.Date.AddDays(settings.TrialPeriod).ToShortDateString();

						DateFromRow.Visible = false;
						DateToRow.Visible = false;
					}

					SendSpamCheckBox.Checked = (bool)reader["send_spam"];

					if (reader["tariffId"] != DBNull.Value)
					{
						li = TariffList.Items.FindByValue(reader["tariffId"].ToString());
						if (li != null)
							li.Selected = true;
					}
					else
					{
						showTariffInfo = false;
					}

					BalanceValue.Text = ((decimal)reader["Balance"]).ToString("f");
					DiscountValue.Text = reader["Discount"].ToString();
					CreditLimitValue.Text = ((decimal)reader["CreditLimit"]).ToString("f");
					AlertThresholdValue.Text = ((decimal)reader["AlertThreshold"]).ToString("f");

					if (!showTariffInfo)
					{
						BalanceRow.Visible = false;
						DiscountRow.Visible = false;
						CreditLimitRow.Visible = false;
						AlertThresholdRow.Visible = false;
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
			this.cvCompareDate.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cv_Validate);

		}
		#endregion

		#region OnSaveCommand
		protected void OnSaveCommand(object sender, System.EventArgs e)
		{
			if (!Page.IsValid)
				return;

			byte companyType = (byte)CompanyType.Billable;
			if (TypeList.SelectedValue == ((byte)CompanyType.Trial).ToString())
				companyType = (byte)CompanyType.Trial;

			string scheme = SchemeList.SelectedValue;
			string port = PortValue.Text;

			try
			{
				if (companyType == (byte)CompanyType.Billable)
				{
					CManage.UpdateCompany(
						CompanyUid,
						txtCompanyName.Text,
						txtDomain.Text,
						scheme,
						port,
						companyType,
						IsActive.Checked,
						txtContactName.Text,
						txtContactPhone.Text,
						txtContactEmail.Text,
						int.Parse(MaxUsersValue.Text),
						int.Parse(MaxExternalUsersValue.Text),
						int.Parse(MaxDiskSpaceValue.Text),
						true,
						int.Parse(TariffList.SelectedValue),
						decimal.Parse(BalanceValue.Text),
						int.Parse(DiscountValue.Text),
						decimal.Parse(CreditLimitValue.Text),
						decimal.Parse(AlertThresholdValue.Text),
						SendSpamCheckBox.Checked);
				}
				else
				{
					DateTime datas = DateTime.Parse(txtDateFrom.Text);
					DateTime data = DateTime.Parse(txtDateTo.Text);

					CManage.UpdateTrialCompany(
						CompanyUid,
						txtCompanyName.Text,
						txtDomain.Text,
						scheme,
						port,
						companyType,
						datas,
						data,
						IsActive.Checked,
						txtContactName.Text,
						txtContactPhone.Text,
						txtContactEmail.Text,
						int.Parse(MaxUsersValue.Text),
						int.Parse(MaxExternalUsersValue.Text),
						int.Parse(MaxDiskSpaceValue.Text),
						SendSpamCheckBox.Checked,
						true);
				}
			}
			catch (ConfigurationException ex)
			{
				cvError.IsValid = false;
				cvError.ErrorMessage = ex.Message;
				return;
			}
			Response.Redirect("../Pages/SiteView.aspx?id=" + CompanyUid.ToString());
		}
		#endregion

		#region btnCancel_Click
		protected void btnCancel_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("../Pages/SiteView.aspx?id=" + CompanyUid.ToString());
		}
		#endregion

		#region cv_Validate
		private void cv_Validate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
		{
			if (TypeList.SelectedValue == ((byte)CompanyType.Trial).ToString())
			{
				try
				{
					DateTime dts = DateTime.Parse(txtDateFrom.Text);
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
		#endregion

		#region Implementation of IPageTemplateTitle
		public string Modify(string oldValue)
		{
			return LocRM.GetString("tbTitle");
		}
		#endregion

		#region TariffList_SelectedIndexChanged
		protected void TariffList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (TariffList.SelectedValue == "-1")
			{
				string companyId = CompanyUid.ToString();
				IConfigurator config = Configurator.Create();
				ICompanyInfo info = config.GetCompanyInfo(companyId);

				MaxDiskSpaceValue.Text = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyDatabaseSize);
				MaxUsersValue.Text = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyMaxUsers);
				MaxExternalUsersValue.Text = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyMaxExternalUsers);

				BalanceRow.Visible = false;
				DiscountRow.Visible = false;
				CreditLimitRow.Visible = false;
				AlertThresholdRow.Visible = false;
			}
			else
			{
				using (IDataReader reader = Tariff.GetTariff(int.Parse(TariffList.SelectedValue), 0))
				{
					if (reader.Read())
					{
						MaxDiskSpaceValue.Text = reader["maxHdd"].ToString();
						MaxUsersValue.Text = reader["maxUsers"].ToString();
						MaxExternalUsersValue.Text = reader["maxExternalUsers"].ToString();
					}
				}

				BalanceRow.Visible = true;
				DiscountRow.Visible = true;
				CreditLimitRow.Visible = true;
				AlertThresholdRow.Visible = true;
			}
		}
		#endregion

		#region TypeList_SelectedIndexChanged
		protected void TypeList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (TypeList.SelectedValue == ((byte)CompanyType.Billable).ToString())
			{
				TariffRow.Visible = true;

				if (TariffList.SelectedValue == "-1")
				{
					BalanceRow.Visible = false;
					DiscountRow.Visible = false;
					CreditLimitRow.Visible = false;
					AlertThresholdRow.Visible = false;
				}
				else
				{
					BalanceRow.Visible = true;
					DiscountRow.Visible = true;
					CreditLimitRow.Visible = true;
					AlertThresholdRow.Visible = true;
				}

				DateFromRow.Visible = false;
				DateToRow.Visible = false;
			}
			else
			{
				TariffRow.Visible = false;
				BalanceRow.Visible = false;
				DiscountRow.Visible = false;
				CreditLimitRow.Visible = false;
				AlertThresholdRow.Visible = false;

				DateFromRow.Visible = true;
				DateToRow.Visible = true;
			}

		}
		#endregion
	}
}
