using System;
using System.Data;
using System.Resources;

namespace Mediachase.Ibn.WebAsp.Modules
{
	public partial class Settings : System.Web.UI.UserControl
	{
		protected ResourceManager LocRM = new ResourceManager("Mediachase.Ibn.WebAsp.App_GlobalResources.Resources.Sites", typeof(Settings).Assembly);

		protected void Page_Load(object sender, System.EventArgs e)
		{
			BindToolbar();
			ApplyLocalization();
			BindInfo();
		}

		#region BindToolbar
		private void BindToolbar()
		{
			secH.Title = LocRM.GetString("tbTitle3");
			secH.AddLink("<img alt='' src='../Layouts/Images/edit.gif'/> " + LocRM.GetString("tbEdit"), "../Pages/SettingsEdit.aspx");
			secH.AddSeparator();
			secH.AddLink("<img alt='' src='../Layouts/Images/cancel.gif'/> Default Page", "../Pages/ASPHome.aspx");
		}
		#endregion

		#region ApplyLocalization
		private void ApplyLocalization()
		{
			lgdSets.InnerText = LocRM.GetString("tTrialSets");
			lblMaxHDD.Text = LocRM.GetString("MaxHDD");
			lblPeriod.Text = LocRM.GetString("Period");
			lblAllowedUsers.Text = LocRM.GetString("AllowedInternalUsers");
			lblAllowedExternalUsers.Text = LocRM.GetString("AllowedExternalUsers");

			legendDns.InnerText = LocRM.GetString("DnsSettings");
			lblDnsParentDomain.Text = LocRM.GetString("DnsParentDomain");

			legendIis.InnerText = LocRM.GetString("IisSettings");
			lblIisIpAddress.Text = LocRM.GetString("IisIpAddress");
			lblIisPort.Text = LocRM.GetString("IisPort");

			lgdNotif.InnerText = LocRM.GetString("NotifSets");
			lblEMailFrom.Text = LocRM.GetString("EMailFrom");
			lblOperatorEmail.Text = LocRM.GetString("OperatorEmail");

			lgdGeneral.InnerText = LocRM.GetString("GeneralSettings");
			lblAutoDeactivateExpired.Text = LocRM.GetString("AutoDeactivateExpired");
			lblAutoDeleteOutdated.Text = LocRM.GetString("AutoDeleteOutdated");
			lblOutdatePeriod.Text = LocRM.GetString("OutdatePeriod");

			lgdTariffSettings.InnerText = LocRM.GetString("TariffSettings");
			lblUseTariffs.Text = LocRM.GetString("UseTariffs");
			lblAutoDeactivateUnpaid.Text = LocRM.GetString("AutoDeactivateUnpaid");

			lblSendSpam.Text = LocRM.GetString("SendSpam");
			lblSendSpamOneDayAfter.Text = LocRM.GetString("SendSpamOneDayAfter");
			lblOneDayAfterPeriod.Text = LocRM.GetString("OneDayAfterPeriod");
			lblSendSpamOneWeekAfter.Text = LocRM.GetString("SendSpamOneWeekAfter");
			lblOneWeekAfterPeriod.Text = LocRM.GetString("OneWeekAfterPeriod");
			lblSendSpamOneWeekBefore.Text = LocRM.GetString("SendSpamOneWeekBefore");
			lblOneWeekBeforePeriod.Text = LocRM.GetString("OneWeekBeforePeriod");
			lblSendSpamOneDayBefore.Text = LocRM.GetString("SendSpamOneDayBefore");
			lblOneDayBeforePeriod.Text = LocRM.GetString("OneDayBeforePeriod");

			SmtpLegend.InnerText = LocRM.GetString("SmtpSettings");
		}
		#endregion

		#region BindInfo
		private void BindInfo()
		{
			AspSettings settings = AspSettings.Load();

			valMaxHDD.Text = settings.MaxHDD.ToString();
			valPeriod.Text = settings.TrialPeriod.ToString();
			valMaxUsers.Text = settings.MaxUsers.ToString();
			lblExternalUsers.Text = settings.MaxExternalUsers.ToString();

			valDnsParentDomain.Text = settings.DnsParentDomain;

			valIisIpAddress.Text = settings.IisIpAddress;
			valIisPort.Text = settings.IisPort.ToString();

			if (string.IsNullOrEmpty(settings.DefaultTrialPool))
				DefaultTrialPoolValue.Text = LocRM.GetString("NewPool");
			else
				DefaultTrialPoolValue.Text = settings.DefaultTrialPool;

			if (string.IsNullOrEmpty(settings.DefaultTrialPool))
				DefaultBillablePoolValue.Text = LocRM.GetString("NewPool");
			else
				DefaultBillablePoolValue.Text = settings.DefaultBillablePool;

			valEMailFrom.Text = settings.EmailFrom;
			if (string.IsNullOrEmpty(settings.EmailFrom))
			{
				valEMailFrom.Text = LocRM.GetString("NotSpecified");
				valEMailFrom.CssClass = "ibn-error";
			}

			valOperatorEmail.Text = settings.OperatorEmail;
			if (string.IsNullOrEmpty(settings.OperatorEmail))
			{
				valOperatorEmail.Text = LocRM.GetString("NotSpecified");
				valOperatorEmail.CssClass = "ibn-error";
			}

			if (settings.AutoDeactivateExpired)
				valAutoDeactivateExpired.Text = LocRM.GetString("Yes");
			else
				valAutoDeactivateExpired.Text = LocRM.GetString("No");

			if (settings.AutoDeleteOutdated)
				valAutoDeleteOutdated.Text = LocRM.GetString("Yes");
			else
				valAutoDeleteOutdated.Text = LocRM.GetString("No");
			valOutdatePeriod.Text = settings.OutdatePeriod.ToString();

			if (settings.UseTariffs)
			{
				valUseTariffs.Text = LocRM.GetString("Yes");
				AutoDeactivateUnpaidRow.Visible = true;
				DefaultTariffRow.Visible = true;

				if (settings.AutoDeactivateUnpaid)
					valAutoDeactivateUnpaid.Text = LocRM.GetString("Yes");
				else
					valAutoDeactivateUnpaid.Text = LocRM.GetString("No");

				if (settings.DefaultTariff > 0)
				{
					using (IDataReader reader = Tariff.GetTariff(settings.DefaultTariff, 0))
					{
						if (reader.Read())
							DefaultTariff.Text = (string)reader["TariffName"];
					}
				}
			}
			else
			{
				valUseTariffs.Text = LocRM.GetString("No");
				AutoDeactivateUnpaidRow.Visible = false;
				DefaultTariffRow.Visible = false;
			}

			if (settings.SendSpam)
			{
				valSendSpam.Text = LocRM.GetString("Yes");
				trSendSpamOneDayAfter.Visible = true;
				trSendSpamOneWeekAfter.Visible = true;
				trSendSpamOneWeekBefore.Visible = true;
				trSendSpamOneDayBefore.Visible = true;

				if (settings.SendSpamOneDayAfter)
				{
					valSendSpamOneDayAfter.Text = LocRM.GetString("Yes");
					tdlblOneDayAfterPeriod.Visible = true;
					tdvalOneDayAfterPeriod.Visible = true;
					valOneDayAfterPeriod.Text = settings.OneDayAfterPeriod.ToString();
				}
				else
				{
					valSendSpamOneDayAfter.Text = LocRM.GetString("No");
					tdlblOneDayAfterPeriod.Visible = false;
					tdvalOneDayAfterPeriod.Visible = false;
				}

				if (settings.SendSpamOneWeekAfter)
				{
					valSendSpamOneWeekAfter.Text = LocRM.GetString("Yes");
					tdlblOneWeekAfterPeriod.Visible = true;
					tdvalOneWeekAfterPeriod.Visible = true;
					valOneWeekAfterPeriod.Text = settings.OneWeekAfterPeriod.ToString();
				}
				else
				{
					valSendSpamOneWeekAfter.Text = LocRM.GetString("No");
					tdlblOneWeekAfterPeriod.Visible = false;
					tdvalOneWeekAfterPeriod.Visible = false;
				}

				if (settings.SendSpamOneWeekBefore)
				{
					valSendSpamOneWeekBefore.Text = LocRM.GetString("Yes");
					tdlblOneWeekBeforePeriod.Visible = true;
					tdvalOneWeekBeforePeriod.Visible = true;
					valOneWeekBeforePeriod.Text = settings.OneWeekBeforePeriod.ToString();
				}
				else
				{
					valSendSpamOneWeekBefore.Text = LocRM.GetString("No");
					tdlblOneWeekBeforePeriod.Visible = false;
					tdvalOneWeekBeforePeriod.Visible = false;
				}

				if (settings.SendSpamOneDayBefore)
				{
					valSendSpamOneDayBefore.Text = LocRM.GetString("Yes");
					tdlblOneDayBeforePeriod.Visible = true;
					tdvalOneDayBeforePeriod.Visible = true;
					valOneDayBeforePeriod.Text = settings.OneDayBeforePeriod.ToString();
				}
				else
				{
					valSendSpamOneDayBefore.Text = LocRM.GetString("No");
					tdlblOneDayBeforePeriod.Visible = false;
					tdvalOneDayBeforePeriod.Visible = false;
				}
			}
			else
			{
				valSendSpam.Text = LocRM.GetString("No");
				trSendSpamOneDayAfter.Visible = false;
				trSendSpamOneWeekAfter.Visible = false;
				trSendSpamOneWeekBefore.Visible = false;
				trSendSpamOneDayBefore.Visible = false;
			}

			if (settings.SendBillableSpam)
			{
				SendBillableSpamValue.Text = LocRM.GetString("Yes");
				SendBillableSpam7dayRow.Visible = true;
				SendBillableSpam3dayRow.Visible = true;
				SendBillableSpam1dayRow.Visible = true;
				SendBillableSpamNegativeBalanceRow.Visible = true;

				if (settings.SendBillableSpam7day)
					SendBillableSpam7dayValue.Text = LocRM.GetString("Yes");
				else
					SendBillableSpam7dayValue.Text = LocRM.GetString("No");

				if (settings.SendBillableSpam3day)
					SendBillableSpam3dayValue.Text = LocRM.GetString("Yes");
				else
					SendBillableSpam3dayValue.Text = LocRM.GetString("No");

				if (settings.SendBillableSpam1day)
					SendBillableSpam1dayValue.Text = LocRM.GetString("Yes");
				else
					SendBillableSpam1dayValue.Text = LocRM.GetString("No");

				if (settings.SendBillableSpamNegativeBalance)
					SendBillableSpamNegativeBalanceValue.Text = LocRM.GetString("Yes");
				else
					SendBillableSpamNegativeBalanceValue.Text = LocRM.GetString("No");
			}
			else
			{
				SendBillableSpamValue.Text = LocRM.GetString("No");
				SendBillableSpam7dayRow.Visible = false;
				SendBillableSpam3dayRow.Visible = false;
				SendBillableSpam1dayRow.Visible = false;
				SendBillableSpamNegativeBalanceRow.Visible = false;
			}

			SmtpServerValue.Text = settings.SmtpServer;
			SmtpPortValue.Text = settings.SmtpPort.ToString();
			SmtpSecureConnectionValue.Text = settings.SmtpSecureConnection.ToString();
			if (settings.SmtpAuthenticate)
			{
				SmtpAuthenticateValue.Text = LocRM.GetString("Yes");
				SmtpUserValue.Text = settings.SmtpUser;
			}
			else
			{
				SmtpAuthenticateValue.Text = LocRM.GetString("No");
				SmtpUserRow.Visible = false;
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
	}
}
