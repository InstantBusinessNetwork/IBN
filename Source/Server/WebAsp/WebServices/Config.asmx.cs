using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

using Mediachase.Ibn.Configuration;
using System.Data;

namespace Mediachase.Ibn.WebAsp.WebServices
{
	/// <summary>
	/// Summary description for Config
	/// </summary>
	[WebService(Namespace = "http://mediachase.com/webservices/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	public class Config : System.Web.Services.WebService
	{
		#region ListServerProperties()
		[WebMethod(Description = "Returns the following server properties: OSVersion, EnvironmentVersion, SystemDirectory, InstallationDirectory.")]
		public IConfigurationParameter[] ListServerProperties()
		{
			return Configurator.Create().ListServerProperties();
		}
		#endregion

		#region ListLicenseProperties()
		[WebMethod(Description = "Returns license properties.")]
		public IConfigurationParameter[] ListLicenseProperties()
		{
			return Configurator.Create().ListLicenseProperties();
		}
		#endregion

		#region SqlSettings()
		[WebMethod(Description = "Returns SQL Server settings.")]
		public ISqlServerSettings SqlSettings()
		{
			return Configurator.Create().SqlSettings;
		}
		#endregion

		#region ChangeSqlServerSettings()
		[WebMethod(Description = "Changes SQL Server settings.")]
		public void ChangeSqlServerSettings(string server, AuthenticationType authentication, string adminUser, string adminPassword, string ibnUser, string ibnPassword)
		{
			Configurator.Create().ChangeSqlServerSettings(server, authentication, adminUser, adminPassword, ibnUser, ibnPassword);
		}
		#endregion

		#region string InstallPath()
		[WebMethod(Description = "Returns Installation Path.")]
		public string InstallPath()
		{
			return Configurator.Create().InstallPath;
		}
		#endregion

		#region ILanguageInfo[] ListLanguages()
		[WebMethod(Description = "Returns list of the languages.")]
		public ILanguageInfo[] ListLanguages()
		{
			return Configurator.Create().ListLanguages();
		}
		#endregion

		#region ICompanyInfo[] ListCompanies()
		[WebMethod(Description = "Returns the list of companies.")]
		public ICompanyInfo[] ListCompanies(bool includeDatabaseInfo)
		{
			return Configurator.Create().ListCompanies(includeDatabaseInfo);
		}
		#endregion

		#region ICompanyInfo GetCompanyInfo(string companyId)
		[WebMethod(Description = "Returns information about the company.")]
		public ICompanyInfo GetCompanyInfo(string companyId)
		{
			return Configurator.Create().GetCompanyInfo(companyId);
		}
		#endregion

		#region bool CheckIfHostIsRegistered(string host)
		[WebMethod(Description = "Returns 'true' if the host specified already exists.")]
		public bool CheckIfHostIsRegistered(string host)
		{
			return Configurator.Create().CheckIfHostIsRegistered(host);
		}
		#endregion

		#region string CreateCompany(...)
		[WebMethod(Description = "Creates company and returns company ID.")]
		public string CreateCompany(string companyName, string host, string defaultLocale, bool isActive
			, string iisIPAddress, int iisPort, string applicationPool
			, string adminAccountName, string adminPassword, string adminFirstName, string adminLastName, string adminEmail)
		{
			return Configurator.Create().CreateCompany(
				companyName,
				host,
				defaultLocale,
				isActive,
				iisIPAddress,
				iisPort,
				applicationPool,
				adminAccountName,
				adminPassword,
				adminFirstName,
				adminLastName,
				adminEmail);
		}
		#endregion

		#region string CreateCompanyForDatabase(...)
		[WebMethod(Description = "Assigns company to existing database.")]
		public string CreateCompanyForDatabase(string database, DateTime created, bool isActive, string host, string iisIPAddress, int iisPort, bool createClientScripts)
		{
			return Configurator.Create().CreateCompanyForDatabase(database, created, isActive, host, iisIPAddress, iisPort, null, createClientScripts);
		}
		#endregion

		#region void ActivateCompany(string companyId, bool isActive)
		[WebMethod(Description = "Activates or deactivates company.")]
		public void ActivateCompany(string companyId, bool isActive)
		{
			Configurator.Create().ActivateCompany(companyId, isActive, true);
		}
		#endregion

		#region void ChangeDefaultAddress(string companyId, string scheme, string host, string port)
		[WebMethod(Description = "Modifies company address.")]
		public void ChangeDefaultAddress(string companyId, string scheme, string host, string port)
		{
			Configurator.Create().ChangeCompanyAddress(companyId, scheme, host, port, true);
		}
		#endregion

		#region void DeleteCompany(string companyId, bool deleteDatabase)
		[WebMethod(Description = "Deletes company.")]
		public void DeleteCompany(string companyId, bool deleteDatabase)
		{
			Configurator.Create().DeleteCompany(companyId, deleteDatabase);
		}
		#endregion

		#region ICompanyParameter[] ListCompanyProperties(string companyId)
		[WebMethod(Description = "Returns company properties.")]
		public IConfigurationParameter[] ListCompanyProperties(string companyId)
		{
			return Configurator.Create().ListCompanyProperties(companyId);
		}
		#endregion

		#region string GetCompanyPropertyValue(string companyId, string parameterName)
		[WebMethod(Description = "Returns company property.")]
		public string GetCompanyPropertyValue(string companyId, string parameterName)
		{
			return Configurator.Create().GetCompanyPropertyValue(companyId, parameterName);
		}
		#endregion

		#region void UpdateCompanyPropertyValue(string companyId, string parameterName, string parameterValue)
		[WebMethod(Description = "Updates company property.")]
		public void UpdateCompanyPropertyValue(string companyId, string parameterName, string parameterValue)
		{
			Configurator.Create().SetCompanyPropertyValue(companyId, parameterName, parameterValue);
		}
		#endregion

		#region bool CanCreateAspSite()
		[WebMethod(Description = "Returns 'true' if ASP site can be created.")]
		public bool CanCreateAspSite()
		{
			return Configurator.Create().CanCreateAspSite();
		}
		#endregion

		#region bool CanDeleteAspSite()
		[WebMethod(Description = "Returns 'true' if ASP site can be deleted.")]
		public bool CanDeleteAspSite()
		{
			return Configurator.Create().CanDeleteAspSite();
		}
		#endregion

		#region void CreateAspSite()
		[WebMethod(Description = "Creates ASP site.")]
		public void CreateAspSite(string host, string iisIPAddress, int iisPort)
		{
			Configurator.Create().CreateAspSite(host, iisIPAddress, iisPort, null);
		}
		#endregion

		#region void DeleteAspSite()
		[WebMethod(Description = "Deletes the ASP site.")]
		public void DeleteAspSite(bool deleteDatabase)
		{
			Configurator.Create().DeleteAspSite(deleteDatabase);
		}
		#endregion

		#region int[] ListUpdates()
		[WebMethod(Description = "Returns the list of available updates.")]
		public int[] ListUpdates()
		{
			return Configurator.Create().ListUpdates();
		}
		#endregion

		#region IUpdateInfo[] GetUpdateInfo(int version)
		[WebMethod(Description = "Returns the detailed info for update.")]
		public IUpdateInfo[] GetUpdateInfo(int version)
		{
			return Configurator.Create().GetUpdateInfo(version);
		}
		#endregion

		//#region void UpdateCompany(string companyId, int version)
		//[WebMethod(Description = "Changes the version of company.")]
		//public void UpdateCompany(string companyId, int version)
		//{
		//    Configurator.Create().UpdateCompany(companyId, version);
		//}
		//#endregion

		//#region void UpdateCommonComponents(int version)
		//[WebMethod(Description = "Changes the version of common components.")]
		//public void UpdateCommonComponents(int version)
		//{
		//    Configurator.Create().UpdateCommonComponents(version);
		//}
		//#endregion

		//#region void UpdateServer(int version)
		//[WebMethod(Description = "Changes the version of common components and all companies.")]
		//public void UpdateServer(int version)
		//{
		//    Configurator.Create().UpdateServer(version);
		//}
		//#endregion

		#region void ChangeCompanyApplicationPool(string companyId, string poolName)
		[WebMethod(Description = "Changes the application pool for the company.")]
		public void ChangeCompanyApplicationPool(string companyId, string poolName)
		{
			Configurator.Create().ChangeCompanyApplicationPool(companyId, poolName);
		}
		#endregion
	}
}
