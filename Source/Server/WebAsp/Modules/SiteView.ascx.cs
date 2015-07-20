using System;
using System.Data;
using System.Globalization;
using System.Resources;
using System.Web.UI.WebControls;

using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class SiteView : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.SiteView", typeof(SiteView).Assembly);

		#region CompanyUid
		public Guid CompanyUid
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
			if (CompanyUid == Guid.Empty)
			{
				Response.Redirect("sites.aspx");
				return;
			}
			if (!Page.IsPostBack)
			{
				BindToolbar();
				BindData();
			}

		}

		#region BindToolbar
		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbTitle");
			secH.AddLink("<img alt='' src='../Layouts/Images/edit.gif'/> " + LocRM.GetString("tbEdit"), "../Pages/SiteEdit.aspx?id=" + CompanyUid);
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/delete.gif'/> " + LocRM.GetString("Delete"), "javascript:DeleteSite()");
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> " + LocRM.GetString("tbBack"), "../Pages/sites.aspx");
			//secH.AddLink("<img alt='' src='../Layouts/Images/delete.gif'/> " + LocRM.GetString("Delete"),"javascript:DeleteSite()");
		}
		#endregion

		#region private void BindUsersCount(Label activeLabel, Label allowedLabel, int activeUsers, string allowedPropertyName, string companyId, IConfigurator config)
		private void BindUsersCount(Label activeLabel, Label allowedLabel, int activeUsers, string allowedPropertyName, string companyId, IConfigurator config)
		{
			int allowedUsers = int.Parse(config.GetCompanyPropertyValue(companyId, allowedPropertyName), CultureInfo.InvariantCulture);
			allowedLabel.Text = allowedUsers.ToString();
			if (allowedUsers < 0)
				allowedLabel.Text = LocRM.GetString("unlimited");
			else if (activeUsers > allowedUsers)
				activeLabel.CssClass = "ibn-error";
			activeLabel.Text = activeUsers.ToString();
		}
		#endregion

		#region BindData
		private void BindData()
		{
			string companyId = CompanyUid.ToString();
			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(companyId);

			int dbSize = company.DatabaseSize;
			int maxDbSize = int.Parse(config.GetCompanyPropertyValue(companyId, CManage.keyCompanyDatabaseSize));
			lblOccupiedDiskSpace.Text = dbSize.ToString();
			lblMaxDiskSpace.Text = maxDbSize.ToString();
			if (maxDbSize == -1)
				lblMaxDiskSpace.Text = LocRM.GetString("unlimited");
			else if (dbSize >= maxDbSize)
				lblOccupiedDiskSpace.CssClass = "ibn-error";

			BindUsersCount(lblActiveUsers, lblAllowedUsers, company.InternalUsersCount, CManage.keyCompanyMaxUsers, companyId, config);
			BindUsersCount(lblActiveExternalUsers, lblAllowedExternalUsers, company.ExternalUsersCount, CManage.keyCompanyMaxExternalUsers, companyId, config);

			int port = (string.IsNullOrEmpty(company.Port) ? -1 : int.Parse(company.Port, CultureInfo.InvariantCulture));
			UriBuilder uriBuilder = new UriBuilder(company.Scheme, company.Host, port);

			PortalLink.Text = uriBuilder.ToString();
			PortalLink.NavigateUrl = PortalLink.Text;

			if (company.IsActive)
				lblIsActive.Text = string.Concat("<img alt='' src='../layouts/images/accept.gif'/> ", LocRM.GetString("Yes"));
			else
				lblIsActive.Text = LocRM.GetString("No");

			DatabaseLabel.Text = company.Database;

			using (IDataReader reader = CManage.GetCompany(CompanyUid))
			{
				if (reader.Read())
				{
					lblCompanyName.Text = reader["company_name"].ToString();

					if (reader["contact_name"] != DBNull.Value)
						ContactNameLabel.Text = reader["contact_name"].ToString();
					if (reader["contact_phone"] != DBNull.Value)
						ContactPhoneLabel.Text = reader["contact_phone"].ToString();
					if (reader["contact_email"] != DBNull.Value && reader["contact_email"].ToString() != string.Empty)
						ContactEmailLabel.Text = String.Format(CultureInfo.InvariantCulture,
							"<a href=\"mailto:{0}\">{0}</a>", reader["contact_email"].ToString());

					if ((bool)reader["send_spam"])
						SendSpamLabel.Text = string.Concat("<img alt='' src='../layouts/images/accept.gif'/> ", LocRM.GetString("Yes"));
					else
						SendSpamLabel.Text = LocRM.GetString("No");

					if (config.GetCompanyPropertyValue(companyId, CManage.keyCompanyType) == ((byte)CompanyType.Trial).ToString())
					{
						lblType.Text = "Trial";
						string endDateString = config.GetCompanyPropertyValue(companyId, CManage.keyCompanyEndDate);
						if (!String.IsNullOrEmpty(endDateString))
							lblDateTo.Text = DateTime.Parse(endDateString, CultureInfo.InvariantCulture).ToShortDateString();

						TariffDelimiterRow.Visible = false;
						TariffRow.Visible = false;
						BalanceRow.Visible = false;
						DiscountRow.Visible = false;
						PaidTillRow.Visible = false;
						CreditLimitRow.Visible = false;
						AlertThresholdRow.Visible = false;
					}
					else
					{
						lblType.Text = "Billable";
						trStart.Visible = false;
						trEnd.Visible = false;
						lblDateTo.Text = "";

						if (reader["tariffName"] != DBNull.Value)
						{
							TariffValue.Text = reader["tariffName"].ToString();
							TariffValue.NavigateUrl = "~/Pages/TariffEdit.aspx?TariffId=" + reader["tariffId"].ToString();
							BalanceValue.Text = ((decimal)reader["Balance"]).ToString("f");
							DiscountValue.Text = reader["Discount"].ToString();
							CreditLimitValue.Text = ((decimal)reader["CreditLimit"]).ToString("f");
							AlertThresholdValue.Text = ((decimal)reader["AlertThreshold"]).ToString("f");

							decimal credit = (decimal)reader["CreditLimit"];
							decimal balance = (decimal)reader["balance"];
							decimal dailyCost = (decimal)reader["dailyCost30"];
							int discount = (int)reader["discount"];
							try
							{
								int daysBeforeEnd = (int)((credit + balance) / (dailyCost - dailyCost * discount / 100m));
								PaidTillValue.Text = DateTime.Now.AddDays(daysBeforeEnd).ToShortDateString();
								if (daysBeforeEnd < 0)
									PaidTillValue.Text = String.Concat("<span class='ibn-alerttext'>", PaidTillValue.Text, "</span>");
							}
							catch (Exception)
							{
							}

							if (reader["symbol"] != DBNull.Value)
							{
								BalanceValue.Text = String.Concat(BalanceValue.Text, " ", reader["symbol"].ToString());
								CreditLimitValue.Text = String.Concat(CreditLimitValue.Text, " ", reader["symbol"].ToString());
								AlertThresholdValue.Text = String.Concat(AlertThresholdValue.Text, " ", reader["symbol"].ToString());
							}
						}
						else
						{
							TariffValue.Text = LocRM.GetString("NoTariff");
							BalanceRow.Visible = false;
							DiscountRow.Visible = false;
							PaidTillRow.Visible = false;
							CreditLimitRow.Visible = false;
							AlertThresholdRow.Visible = false;
						}
					}

					if (reader["start_date"] != DBNull.Value)
						lblDateFrom.Text = ((DateTime)reader["start_date"]).ToShortDateString();
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

		#region lbDelete_Click
		protected void lbDelete_Click(object sender, System.EventArgs e)
		{
			CManage.DeleteCompany(CompanyUid);
			Response.Redirect("../Pages/Sites.aspx");
		}
		#endregion
	}
}
