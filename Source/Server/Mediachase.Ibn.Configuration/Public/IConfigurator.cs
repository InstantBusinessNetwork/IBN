using System;
using System.Diagnostics;
using System.Security.Permissions;

namespace Mediachase.Ibn.Configuration
{
	public interface IConfigurator
	{
		string HostName { get; }
		int CommonVersion { get; }
		string InstallPath { get; }
		ISqlServerSettings SqlSettings { get; }
		DateTime UpdatesExpirationDate { get; }

		IConfigurationParameter[] ListServerProperties();
		IConfigurationParameter[] ListLicenseProperties();

		void ChangeSqlServerSettings(string server, AuthenticationType authentication, string adminUser, string adminPassword, string portalUser, string portalPassword);

		ILanguageInfo[] ListLanguages();

		string[] ListIPAddresses();

		string[] ListApplicationPools();
		string ChangeAspApplicationPool(string poolName);
		string ChangeCompanyApplicationPool(string companyId, string poolName);

		ICompanyInfo[] ListCompanies(bool includeDatabaseInfo);
		ICompanyInfo GetCompanyInfo(string companyId);
		bool CheckIfHostIsRegistered(string host);
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		string CreateCompany(string companyName, string host, string defaultLocale, bool isActive
			, string iisIPAddress, int iisPort, string iisApplicationPool
			, string adminAccountName, string adminPassword, string adminFirstName, string adminLastName, string adminEmail);
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		string CreateCompanyForDatabase(string database, DateTime created, bool isActive, string host, string iisIPAddress, int iisPort, string iisApplicationPool, bool createClientScripts);
		void ActivateCompany(string companyId, bool isActive, bool updateAspDatabase);
		void EnableScheduleService(string companyId, bool isEnabled);
		void ChangeCompanyAddress(string companyId, string scheme, string host, string port, bool updateAspDatabase);
		void DeleteCompany(string companyId, bool deleteDatabase);
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		void UpdateCompanyVersion(string companyId);

		IConfigurationParameter[] ListCompanyProperties(string companyId);
		string GetCompanyPropertyValue(string companyId, string propertyName);
		void SetCompanyPropertyValue(string companyId, string propertyName, string propertyValue);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		IAspInfo GetAspInfo();
		bool CanCreateAspSite();
		bool CanDeleteAspSite();
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		void CreateAspSite(string host, string iisIPAddress, int iisPort, string iisApplicationPool);
		void ChangeAspAddress(string scheme, string host, string port);
		void DeleteAspSite(bool deleteDatabase);

		int[] ListUpdates();
		IUpdateInfo[] GetUpdateInfo(int version);
		IUpdateInfo[] GetUpdateInfo(string updateDirectory);
		bool CheckIfCommonComponentsUpdateIsRequired(int version);
		ProcessStartInfo BuildUpdateCommandForCompany(int version, string companyId);
		ProcessStartInfo BuildUpdateCommandForCommonComponents(int version, int callingProcessId);
		ProcessStartInfo BuildUpdateCommandForServer(int version, int callingProcessId);
	}
}
