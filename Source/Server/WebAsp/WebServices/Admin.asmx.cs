using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using Mediachase.Ibn.Configuration;

namespace Mediachase.Ibn.WebAsp.WebServices
{
	/// <summary>
	/// Summary description for Admin.
	/// </summary>
	[WebService(Namespace="http://mediachase.com/webservices/",Description="IBN Administration Service")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class Admin : System.Web.Services.WebService
	{
		#region [Web] CheckIfCompanyExists()
		[WebMethod(Description="Checks if the company with given domain name exists.")]
		public bool CheckIfCompanyExists(string domainName)
		{
			return Configurator.Create().CheckIfHostIsRegistered(domainName);
		}
		#endregion

		#region [Web] Create
		[WebMethod(Description="Creates chargeable company.")]
		public string Create(
			string companyName,
			string domainName,
			string defaultLocale,
			bool isActive,
			int maxUsers,
			int maxExternalUsers,
			int maxDiskSpace,

			string contactName,
			string contactPhone,
			string contactEmail,
			
			string adminFirstName,
			string adminLastName,
			string adminLogin,
			string adminPassword,
			string adminEmail
			)
		{
			return CManage.CompanyCreate(companyName, domainName, defaultLocale, isActive,
				maxUsers, maxExternalUsers, maxDiskSpace, contactName, contactPhone, contactEmail,
				adminFirstName, adminLastName, adminLogin, adminPassword, adminEmail, 
				false, DateTime.Now, DateTime.Now, null).ToString();
		}
		#endregion

		#region [Web] CreateTrial()
		[WebMethod(Description="Creates trial company.")]
		public string CreateTrial(
			string companyName,
			string domainName,
			string defaultLocale,
			bool isActive,
			int maxUsers,
			int maxExternalUsers,
			int maxDiskSpace,

			string contactName,
			string contactPhone,
			string contactEmail,
			
			string adminFirstName,
			string adminLastName,
			string adminLogin,
			string adminPassword,
			string adminEmail,
			
			DateTime trialStartDate,
			DateTime trialEndDate
			)
		{
			return CManage.CompanyCreate(companyName, domainName, defaultLocale, isActive,
				maxUsers, maxExternalUsers, maxDiskSpace, contactName, contactPhone, contactEmail,
				adminFirstName, adminLastName, adminLogin, adminPassword, adminEmail,
				true, trialStartDate, trialEndDate, null).ToString();
		}
		#endregion

		#region [Web] Delete()
		[WebMethod(Description="Deletes company.")]
		public void Delete(string companyId)
		{
			CManage.DeleteCompany(new Guid(companyId));
		}
		#endregion

		#region [Web] ChangeDefaultAddress()
		[WebMethod(Description="Changes the adress of a company.")]
		public void ChangeDefaultAddress(string companyId, string scheme, string host, string port)
		{
			CManage.ChangeDefaultAddress(new Guid(companyId), scheme, host, port);
		}
		#endregion

		#region [Web] SetActive()
		[WebMethod(Description="Changes the active state of a company.")]
		public void SetActive(string companyId, bool isActive)
		{
			CManage.ActivateCompany(new Guid(companyId), isActive);
		}
		#endregion

		#region [Web] SetTrial()
		[WebMethod(Description="Changes the type of a company.")]
		public void SetTrial(string companyId, bool isTrial)
		{
			if (isTrial)
				CManage.UpdateCompanyType(new Guid(companyId), 2);
			else
				CManage.UpdateCompanyType(new Guid(companyId), 1);
		}
		#endregion

		#region [Web] Update()
		[WebMethod(Description="Updates non-trial company.")]
		public void Update(
			string companyId,
			string companyName,
			bool isActive,
			int maxUsers,
			int maxExternalUsers,
			int maxDiskSpace,
			string contactName,
			string contactPhone,
			string contactEmail
			)
		{
			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(companyId);

			int tariffId = -1;
			decimal balance = 0m;
			int discount = 0;
			decimal creditLimit = 0m;
			decimal alertThreshold = 0m;
			bool sendSpam = true;
			//DateTime? tariffStartDate = null;
			using (IDataReader reader = CManage.GetCompany(new Guid(companyId)))
			{
				if (reader.Read())
				{
					if (reader["tariffId"] != DBNull.Value)
						tariffId = (int)reader["tariffId"];
					balance = (decimal)reader["balance"];
					discount = (int)reader["discount"];
					creditLimit = (decimal)reader["creditLimit"];
					alertThreshold = (decimal)reader["alertThreshold"];
					sendSpam = (bool)reader["send_spam"];
				}
			}

			CManage.UpdateCompany(new Guid(companyId), companyName, company.Host, company.Scheme, company.Port, 2,
				isActive, contactName, contactPhone, contactEmail, maxUsers, maxExternalUsers, maxDiskSpace,
				false, tariffId, balance, discount, creditLimit, alertThreshold, sendSpam);
		}
		#endregion

		#region [Web] UpdateTrial()
		[WebMethod(Description="Updates trial company.")]
		public void UpdateTrial(
			string companyId,
			string companyName,
			bool isActive,
			int maxUsers,
			int maxExternalUsers,
			int maxDiskSpace,

			string contactName,
			string contactPhone,
			string contactEmail,
			
			DateTime trialStartDate,
			DateTime trialEndDate
			)
		{
			IConfigurator config = Configurator.Create();
			ICompanyInfo company = config.GetCompanyInfo(companyId);

			int tariffId = -1;
			decimal balance = 0m;
			int discount = 0;
			decimal creditLimit = 0m;
			bool sendSpam = true;
			using (IDataReader reader = CManage.GetCompany(new Guid(companyId)))
			{
				if (reader.Read())
				{
					if (reader["tariffId"] != DBNull.Value)
						tariffId = (int)reader["tariffId"];
					balance = (decimal)reader["balance"];
					discount = (int)reader["discount"];
					creditLimit = (decimal)reader["creditLimit"];
					sendSpam = (bool)reader["send_spam"];
				}
			}

			CManage.UpdateTrialCompany(new Guid(companyId), companyName, company.Host, company.Scheme, company.Port, 1,
				trialStartDate, trialEndDate, isActive, contactName, contactPhone, contactEmail, 
				maxUsers, maxExternalUsers, maxDiskSpace, sendSpam, false);
		}
		#endregion

		#region [Web] UpdateTrialPeriod()
		[WebMethod(Description="Updates trial period.")]
		public void UpdateTrialPeriod(string companyId, DateTime trialStartDate, DateTime trialEndDate)
		{
			CManage.UpdateCompanyTrialPeriod(new Guid(companyId), trialStartDate, trialEndDate);
		}
		#endregion

	}
}
