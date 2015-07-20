using System;
using System.Data;

using Mediachase.Net.Mail;

namespace Mediachase.Ibn.WebAsp
{
	/// <summary>
	/// Summary description for ASPSettings.
	/// </summary>
	public class AspSettings
	{
		public int MaxHDD { get; set; }
		public int MaxUsers { get; set; }
		public int MaxExternalUsers { get; set; }
		public int TrialPeriod { get; set; }
		public string EmailFrom { get; set; }
		public string OperatorEmail { get; set; }
		public string DnsParentDomain { get; set; }
		public string IisIpAddress { get; set; }
		public int IisPort { get; set; }
		public bool AutoDeactivateExpired { get; set; }
		public bool AutoDeleteOutdated { get; set; }
		public int OutdatePeriod { get; set; }
		public bool SendSpam { get; set; }
		public bool SendSpamOneDayAfter { get; set; }
		public bool SendSpamOneWeekAfter { get; set; }
		public bool SendSpamOneWeekBefore { get; set; }
		public bool SendSpamOneDayBefore { get; set; }
		public int OneDayAfterPeriod { get; set; }
		public int OneWeekAfterPeriod { get; set; }
		public int OneWeekBeforePeriod { get; set; }
		public int OneDayBeforePeriod { get; set; }
		public bool UseTariffs { get; set; }
		public int DefaultTariff { get; set; }
		public bool SendBillableSpam { get; set; }
		public bool SendBillableSpam7day { get; set; }
		public bool SendBillableSpam3day { get; set; }
		public bool SendBillableSpam1day { get; set; }
		public bool SendBillableSpamNegativeBalance { get; set; }
		public bool AutoDeactivateUnpaid { get; set; }
		
		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public SecureConnectionType SmtpSecureConnection { get; set; }
		public bool SmtpAuthenticate { get; set; }
		public string SmtpUser { get; set; }
		public string SmtpPassword { get; set; }

		public string DefaultTrialPool { get; set; }
		public string DefaultBillablePool { get; set; }

		private AspSettings()
		{
		}

		#region Load
		public static AspSettings Load()
		{
			AspSettings ret = new AspSettings();
			using(IDataReader r = CManage.SettingsGet())
			{
				if (r.Read())
				{
					ret.MaxHDD = (int)r["MaxHDD"];
					ret.MaxUsers = (int)r["MaxUsers"];
					ret.MaxExternalUsers = (int)r["MaxExternalUsers"];
					ret.TrialPeriod = (int)r["TrialPeriod"];
					ret.EmailFrom = r["EmailFrom"].ToString();
					ret.OperatorEmail = r["OperatorEmail"].ToString();
					ret.DnsParentDomain = r["DnsParentDomain"].ToString();
					ret.IisIpAddress = r["IisIpAddress"].ToString();
					ret.IisPort = (int)r["IisPort"];
					ret.AutoDeactivateExpired = (bool)r["AutoDeactivateExpired"];
					ret.AutoDeleteOutdated = (bool)r["AutoDeleteOutdated"];
					ret.OutdatePeriod = (int)r["OutdatePeriod"];
					ret.SendSpam = (bool)r["SendSpam"];
					ret.SendSpamOneDayAfter = (bool)r["SendSpamOneDayAfter"];
					ret.SendSpamOneWeekAfter = (bool)r["SendSpamOneWeekAfter"];
					ret.SendSpamOneWeekBefore = (bool)r["SendSpamOneWeekBefore"];
					ret.SendSpamOneDayBefore = (bool)r["SendSpamOneDayBefore"];
					ret.OneDayAfterPeriod = (int)r["OneDayAfterPeriod"];
					ret.OneWeekAfterPeriod = (int)r["OneWeekAfterPeriod"];
					ret.OneWeekBeforePeriod = (int)r["OneWeekBeforePeriod"];
					ret.OneDayBeforePeriod = (int)r["OneDayBeforePeriod"];
					ret.UseTariffs = (bool)r["UseTariffs"];
					ret.SendBillableSpam = (bool)r["SendBillableSpam"];
					ret.SendBillableSpam7day = (bool)r["SendBillableSpam7day"];
					ret.SendBillableSpam3day = (bool)r["SendBillableSpam3day"];
					ret.SendBillableSpam1day = (bool)r["SendBillableSpam1day"];
					ret.SendBillableSpamNegativeBalance = (bool)r["SendBillableSpamNegativeBalance"];
					ret.AutoDeactivateUnpaid = (bool)r["AutoDeactivateUnpaid"];
					ret.SmtpServer = (string)r["SmtpServer"];
					ret.SmtpPort = (int)r["SmtpPort"];
					ret.SmtpSecureConnection = (SecureConnectionType)r["SmtpSecureConnection"];
					ret.SmtpAuthenticate = (bool)r["SmtpAuthenticate"];
					ret.SmtpUser = (string)r["SmtpUser"];
					ret.SmtpPassword = (string)r["SmtpPassword"];
					ret.DefaultTrialPool = (string)r["DefaultTrialPool"];
					ret.DefaultBillablePool = (string)r["DefaultBillablePool"];
					ret.DefaultTariff = (int)r["DefaultTariff"];
				}
				else
				{
					ret.MaxHDD = 100;
					ret.MaxUsers = 10;
					ret.MaxExternalUsers = 10;
					ret.TrialPeriod = 30;
					ret.EmailFrom = string.Empty;
					ret.OperatorEmail = string.Empty;
					ret.DnsParentDomain = string.Empty;
					ret.IisIpAddress = string.Empty;
					ret.IisPort = 80;
					ret.AutoDeactivateExpired = false;
					ret.AutoDeleteOutdated = false;
					ret.OutdatePeriod = 30;
					ret.SendSpam = false;
					ret.SendSpamOneDayAfter = false;
					ret.SendSpamOneWeekAfter = false;
					ret.SendSpamOneWeekBefore = false;
					ret.SendSpamOneDayBefore = false;
					ret.OneDayAfterPeriod = 1;
					ret.OneWeekAfterPeriod = 7;
					ret.OneWeekBeforePeriod = 7;
					ret.OneDayBeforePeriod = 7;
					ret.UseTariffs = false;
					ret.SendBillableSpam = false;
					ret.SendBillableSpam7day = false;
					ret.SendBillableSpam3day = false;
					ret.SendBillableSpam1day = false;
					ret.SendBillableSpamNegativeBalance = false;
					ret.AutoDeactivateUnpaid = false;
					ret.SmtpServer = "localhost";
					ret.SmtpPort = 25;
					ret.SmtpSecureConnection = SecureConnectionType.None;
					ret.SmtpAuthenticate = false;
					ret.SmtpUser = string.Empty;
					ret.SmtpPassword = string.Empty;
					ret.DefaultTrialPool = string.Empty;
					ret.DefaultBillablePool = string.Empty;
					ret.DefaultTariff = -1;
				}
			}
			return ret;
		}
		#endregion

		#region Save
		public void Save()
		{
			CManage.SettingsUpdate(
				MaxHDD, MaxUsers, MaxExternalUsers, TrialPeriod,
				EmailFrom, OperatorEmail,
				DnsParentDomain, IisIpAddress, IisPort,
				AutoDeactivateExpired, AutoDeleteOutdated, OutdatePeriod, SendSpam,
				SendSpamOneDayAfter, SendSpamOneWeekAfter, SendSpamOneWeekBefore, SendSpamOneDayBefore,
				OneDayAfterPeriod, OneWeekAfterPeriod, OneWeekBeforePeriod, OneDayBeforePeriod,
				UseTariffs, SendBillableSpam, SendBillableSpam7day, SendBillableSpam3day,
				SendBillableSpam1day, SendBillableSpamNegativeBalance, AutoDeactivateUnpaid,
				SmtpServer, SmtpPort, (int)SmtpSecureConnection, SmtpAuthenticate, SmtpUser, SmtpPassword,
				DefaultTrialPool, DefaultBillablePool, DefaultTariff);
		}
		#endregion

		
	}
}
