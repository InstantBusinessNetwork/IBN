using System;
using System.Collections.Generic;
using System.Text;

namespace Mediachase.Ibn.WebAsp
{
	public struct CompanyInfo
	{
		public string CompanyId;
		public string CompanyName;
		public bool IsActive;
		public string Scheme;
		public string Host;
		public string Port;
		public string PortalUrl;
		public int DatabaseSize;
		public int UsersCount;
		public int MaxDatabaseSize;
		public int MaxUsersCount;
		public int MaxExternalUsersCount;
		public DateTime TrialStartDate;
		public DateTime TrialEndDate;
		public bool IsTrial;
		public string ContactName;
		public string ContactPhone;
		public string ContactEmail;
		public bool SendNotifications;
		public int TariffId;
		public string TariffName;
		public string TariffDescription;
		public decimal TariffMonthlyCost;
		public DateTime TariffStartDate;
		public string TariffCurrencySymbol;
		public decimal Balance;
		public int Discount;
		public decimal CreditLimit;
		public decimal AlertThreshold;

		public CompanyInfo(string companyId, string companyName, bool isActive, 
			string scheme, string host, string port, string portalUrl,
			int databaseSize, int usersCount, int maxDatabaseSize, int maxUsersCount, int maxExternalUsersCount,
			DateTime trialStartDate, DateTime trialEndDate, bool isTrial, 
			string contactName, string contactPhone, string contactEmail, bool sendNotifications,
			int tariffId, string tariffName, string tariffDescription, 
			decimal tariffMonthlyCost, DateTime tariffStartDate, string tariffCurrencySymbol,
			decimal balance, int discount, decimal creditLimit, decimal alertThreshold)
		{
			this.CompanyId = companyId;
			this.CompanyName = companyName;
			this.IsActive = isActive;
			this.Scheme = scheme;
			this.Host = host;
			this.Port = port;
			this.PortalUrl = portalUrl;
			this.DatabaseSize = databaseSize;
			this.UsersCount = usersCount;
			this.MaxDatabaseSize = maxDatabaseSize;
			this.MaxUsersCount = maxUsersCount;
			this.MaxExternalUsersCount = maxExternalUsersCount;
			this.TrialStartDate = trialStartDate;
			this.TrialEndDate = trialEndDate;
			this.IsTrial = isTrial;
			this.ContactName = contactName;
			this.ContactPhone = contactPhone;
			this.ContactEmail = contactEmail;
			this.SendNotifications = sendNotifications;
			this.TariffId = tariffId;
			this.TariffName = tariffName;
			this.TariffDescription = tariffDescription;
			this.TariffMonthlyCost = tariffMonthlyCost;
			this.TariffStartDate = tariffStartDate;
			this.TariffCurrencySymbol = tariffCurrencySymbol;
			this.Balance = balance;
			this.Discount = discount;
			this.CreditLimit = creditLimit;
			this.AlertThreshold = alertThreshold;
		}
	}
}
