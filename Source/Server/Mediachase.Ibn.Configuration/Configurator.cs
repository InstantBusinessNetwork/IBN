using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Xml;

using Mediachase.Database;

namespace Mediachase.Ibn.Configuration
{
	public sealed class Configurator : IConfigurator
	{
#if RADIUS
		private const string ConstSqlCommon = "SQL_COMMON_";
		private const string ConstSqlPortal = "SQL_PORTAL_";

		public const string ConstCommonVersion = "COMMON_VERSION";
#else
		private const string ConstSqlCommon = "MC_SQL_";
		private const string ConstSqlPortal = "IBN_SQL_";

		public const string ConstCommonVersion = "IBN_VERSION";
#endif

		private const string ConstSqlCommonAuthentication = ConstSqlCommon + "AUTHENTICATION";
		private const string ConstSqlCommonServer = ConstSqlCommon + "SERVER";
		private const string ConstSqlCommonUser = ConstSqlCommon + "USER";
		private const string ConstSqlCommonPassword = ConstSqlCommon + "PASSWORD";
		private const string ConstSqlPortalUser = ConstSqlPortal + "USER";
		private const string ConstSqlPortalPassword = ConstSqlPortal + "PASSWORD";

		private const string ConstDefaultAspName = IbnConst.ProductFamilyShort + "_" + IbnConst.VersionMajorDotMinor + "_ASP";
		private const string ConstDisableCustomSqlReport = "DisableCustomSqlReport";
		private const string ConstTarget = "Target";
		private const string ConstShowLog = "ShowLog";
		private const string ConstId = "Id";
		private const string ConstProcessId = "ProcessId";

		#region * Fields *

		private string _serverConfigPath;
		private string _schedulerConfigPath;

		private string _codePath;
		private string _sourcePath;
		private string _sourceToolsPath;
		private string _sourceWebPath;

		private string _aspPath;
		private string _aspScriptsPath;

		private string _updatesPath;

		private AuthenticationType _sqlAuthentication;
		private string _sqlServer;
		private string _sqlAdminUser;
		private string _sqlAdminPassword;
		private string _sqlPortalUser;
		private string _sqlPortalPassword;

		private bool _registerAspNet;

		private DBHelper _dbHelper;

		#endregion

		#region .ctor
		private Configurator()
		{
			HostName = Dns.GetHostName();
			CommonVersion = RegistrySettings.ReadInt32(ConstCommonVersion);
			InstallPath = RegistrySettings.ReadString("INSTALLDIR");
			_serverConfigPath = Path.Combine(InstallPath, "ibn.config");
			_schedulerConfigPath = Path.Combine(InstallPath, "ScheduleService.exe.config");

			_codePath = Path.Combine(InstallPath, "Code");
			_sourcePath = Path.Combine(_codePath, "_Source");
			_sourceToolsPath = Path.Combine(_sourcePath, "Tools");
			_sourceWebPath = Path.Combine(_sourcePath, "Web");

			_aspPath = Path.Combine(InstallPath, "Asp");
			_aspScriptsPath = Path.Combine(_aspPath, "Install");

			_updatesPath = Path.Combine(InstallPath, "Updates");

			string authentication = RegistrySettings.ReadString(ConstSqlCommonAuthentication);
			if (string.IsNullOrEmpty(authentication))
				_sqlAuthentication = AuthenticationType.SqlServer;
			else
				_sqlAuthentication = (AuthenticationType)Enum.Parse(typeof(AuthenticationType), authentication);

			_sqlServer = RegistrySettings.ReadString(ConstSqlCommonServer);
			_sqlAdminUser = RegistrySettings.ReadString(ConstSqlCommonUser);
			_sqlAdminPassword = RegistrySettings.ReadString(ConstSqlCommonPassword);
			_sqlPortalUser = RegistrySettings.ReadString(ConstSqlPortalUser);
			_sqlPortalPassword = RegistrySettings.ReadString(ConstSqlPortalPassword);

			_registerAspNet = RegistrySettings.ReadString("RegisterAspNet") == "True";

			CreateDBHelper();
		}
		#endregion

		public static IConfigurator Create()
		{
			return new Configurator();
		}

		#region IConfigurator Members

		public string HostName { get; private set; }
		public int CommonVersion { get; private set; }
		public string InstallPath { get; private set; }
		public ISqlServerSettings SqlSettings { get; private set; }
		public DateTime UpdatesExpirationDate
		{
			get
			{
				return License.UpdatesExpirationDate;
			}
		}

		#region public IConfigurationParameter[] ListServerProperties()
		public IConfigurationParameter[] ListServerProperties()
		{
			List<ConfigurationParameter> list = new List<ConfigurationParameter>();

			list.Add(new ConfigurationParameter("OSVersion", Environment.OSVersion.ToString()));
			list.Add(new ConfigurationParameter("EnvironmentVersion", Environment.Version.ToString()));
			list.Add(new ConfigurationParameter("SystemDirectory", Environment.SystemDirectory));
			list.Add(new ConfigurationParameter("InstallationDirectory", InstallPath));

			string edition = "Unknown";
			string path = Path.Combine(_sourceWebPath, @"Portal\App_GlobalResources\GlobalResources.xml");
			if (File.Exists(path))
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);
				XmlNode editionNode = doc.SelectSingleNode("/GlobalResources/String[@name='Edition']");
				if(editionNode != null)
					edition = editionNode.InnerText;
			}
			list.Add(new ConfigurationParameter("ServerEdition", edition));

			return list.ToArray();
		}
		#endregion

		#region public IConfigurationParameter[] ListLicenseProperties()
		public IConfigurationParameter[] ListLicenseProperties()
		{
			List<ConfigurationParameter> list = new List<ConfigurationParameter>();

			NameValueCollection properties = License.Properties;
			foreach (string name in properties.AllKeys)
			{
				string value = properties[name];
				string type = null;
				if (name == "ExpirationDate" || name == "UpdatesExpirationDate")
					type = "DateTime";
				list.Add(new ConfigurationParameter(name, value, type));
			}

			return list.ToArray();
		}
		#endregion


		#region public void ChangeSqlServerSettings(string server, AuthenticationType authentication, string adminUser, string adminPassword, string portalUser, string portalPassword)
		public void ChangeSqlServerSettings(string server, AuthenticationType authentication, string adminUser, string adminPassword, string portalUser, string portalPassword)
		{
			_sqlServer = server;
			_sqlAuthentication = authentication;
			_sqlAdminUser = adminUser;
			_sqlAdminPassword = adminPassword;
			_sqlPortalUser = portalUser;
			_sqlPortalPassword = portalPassword;

			RegistrySettings.WriteString(ConstSqlCommonServer, server);
			RegistrySettings.WriteString(ConstSqlCommonAuthentication, authentication.ToString());
			RegistrySettings.WriteString(ConstSqlCommonUser, adminUser);
			RegistrySettings.WriteString(ConstSqlCommonPassword, adminPassword);
			RegistrySettings.WriteString(ConstSqlPortalUser, portalUser);
			RegistrySettings.WriteString(ConstSqlPortalPassword, portalPassword);

			CreateDBHelper();
		}
		#endregion

		#region public ILanguageInfo[] ListLanguages()
		public ILanguageInfo[] ListLanguages()
		{
			return LanguageInfo.List(Path.Combine(_sourcePath, @"Tools\dictionaries.xml"));
		}
		#endregion


		#region public string[] ListIPAddresses()
		public string[] ListIPAddresses()
		{
			List<string> list = new List<string>();

			IPHostEntry host = Dns.GetHostEntry(HostName);
			foreach (IPAddress address in host.AddressList)
			{
				if (address.AddressFamily == AddressFamily.InterNetwork)
				{
					Console.WriteLine(address);
					list.Add(address.ToString());
				}
			}

			return list.ToArray();
		}
		#endregion

		#region public string[] ListApplicationPools()
		public string[] ListApplicationPools()
		{
			IIisManager iisManager = IisManager.Create(_sourceWebPath);
			return iisManager.ListApplicationPools();
		}
		#endregion
		#region public string ChangeAspApplicationPool(string poolName)
		public string ChangeAspApplicationPool(string poolName)
		{
			string newPool = null;

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				AspInfo aspInfo = AspInfo.Load(serverConfigDocument);
				if (aspInfo != null)
				{
					string oldPool = aspInfo.ApplicationPool;
					bool oldPoolCreated = aspInfo.IsApplicationPoolCreated;

					// Update IIS
					IIisManager iisManager = IisManager.Create(_aspPath);
					newPool = ChangeApplicationPool(iisManager, aspInfo.SiteId, oldPool, oldPoolCreated, poolName, ConstDefaultAspName);

					// Update ibn.config
					aspInfo.ApplicationPool = newPool;
					aspInfo.IsApplicationPoolCreated = (newPool != poolName);

					aspInfo.Save(serverConfigDocument);
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}

			return newPool;
		}
		#endregion
		#region public string ChangeCompanyApplicationPool(string companyId, string poolName)
		public string ChangeCompanyApplicationPool(string companyId, string poolName)
		{
			string newPool = null;

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				CompanyInfo company = CompanyInfo.LoadGeneralInfo(serverConfigDocument, companyId);
				if (company != null)
				{
					string oldPool = company.PortalPool;
					bool oldPoolCreated = company.IsPortalPoolCreated;

					// Update IIS
					IIisManager iisManager = IisManager.Create(_aspPath);
					newPool = ChangeApplicationPool(iisManager, company.SiteId, oldPool, oldPoolCreated, poolName, GetDefaultWebName(company.Host));

					// Update ibn.config
					company.PortalPool = newPool;
					company.IsPortalPoolCreated = (newPool != poolName);

					company.SaveGeneralInfo(serverConfigDocument);
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}

			return newPool;
		}
		#endregion


		#region public ICompanyInfo[] ListCompanies(bool includeDatabaseInfo)
		public ICompanyInfo[] ListCompanies(bool includeDatabaseInfo)
		{
			CompanyInfo[] companies = null;

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				companies = CompanyInfo.ListCompanies(serverConfig.LoadDocument());
			}

			if (companies != null && includeDatabaseInfo)
			{
				foreach (CompanyInfo company in companies)
					company.LoadExtendedInfo(_dbHelper);
			}

			return companies;
		}
		#endregion

		#region public ICompanyInfo GetCompanyInfo(string companyId)
		public ICompanyInfo GetCompanyInfo(string companyId)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				return CompanyInfo.LoadExtendedInfo(serverConfig.LoadDocument(), companyId, _dbHelper);
			}
		}
		#endregion

		#region public bool CheckIfHostIsRegistered(string host)
		public bool CheckIfHostIsRegistered(string host)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				return ValidateHostName(serverConfig.LoadDocument(), host, null, false);
			}
		}
		#endregion

		#region public string CreateCompany(...)
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		public string CreateCompany(string companyName, string host, string defaultLocale, bool isActive
			, string iisIPAddress, int iisPort, string iisApplicationPool
			, string adminAccountName, string adminPassword, string adminFirstName, string adminLastName, string adminEmail)
		{
			string context = "Create Company: " + host;

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				ValidatePortalsCount(serverConfigDocument);
				ValidateHostName(serverConfigDocument, host, null, true);
				host = host.ToLowerInvariant();

				CompanyInfo company = null;

				string commonName = host;
				string database = null;
				bool databaseCreated = false;

				try
				{
					CreateSqlUser(_sourceToolsPath);

					// Create database
					string prospectiveDatabaseName = BuildDatabaseName(commonName);
					database = BuildUniqueName(prospectiveDatabaseName, GetDatabases(prospectiveDatabaseName));
					string port = (iisPort != 80 ? iisPort.ToString(CultureInfo.InvariantCulture) : string.Empty);
					CreateCompanyDatabase(_sourceToolsPath, database
						, companyName, "http", host, port
						, defaultLocale, isActive
						, adminAccountName, adminPassword, adminFirstName, adminLastName, adminEmail);
					databaseCreated = true;

					// Create web site
					company = CreateCompanyForDatabase(serverConfigDocument, database, defaultLocale, DateTime.UtcNow, isActive, host, iisIPAddress, iisPort, iisApplicationPool, true);
				}
				catch
				{
					#region Undo changes

					// Delete web
					try
					{
						if (company != null)
							DeleteCompanyWeb(company, context);
					}
					catch
					{
					}

					// Delete database
					try
					{
						if (databaseCreated)
							DeleteDatabase(database);
					}
					catch
					{
					}

					#endregion

					throw;
				}

				serverConfig.SaveDocument(serverConfigDocument);

				return company.Id;
			}
		}
		#endregion

		#region public string CreateCompanyForDatabase(string database, DateTime created, bool isActive, string host, string iisIPAddress, int iisPort, string iisApplicationPool, bool createClientScripts)
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		public string CreateCompanyForDatabase(string database, DateTime created, bool isActive, string host, string iisIPAddress, int iisPort, string iisApplicationPool, bool createClientScripts)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				ValidatePortalsCount(serverConfigDocument);
				ValidateHostName(serverConfigDocument, host, null, true);
				host = host.ToLowerInvariant();

				string defaultLocale;
				try
				{
					_dbHelper.Database = database;
					LanguageInfo language = new LanguageInfo();
					language.Load(_dbHelper);
					defaultLocale = language.Locale;
				}
				finally
				{
					_dbHelper.Database = null;
				}

				string port = (iisPort != 80 ? iisPort.ToString(CultureInfo.InvariantCulture) : string.Empty);

				CompanyInfo company;

				using (DBTransaction transaction = _dbHelper.BeginTransaction())
				{
					// Update database
					_dbHelper.Database = database;
					Portal.SetPortalParameterValue(_dbHelper, "system.isactive", isActive.ToString());
					Portal.SetPortalParameterValue(_dbHelper, "system.scheme", "http");
					Portal.SetPortalParameterValue(_dbHelper, "system.host", host);
					Portal.SetPortalParameterValue(_dbHelper, "system.port", port);

					company = CreateCompanyForDatabase(serverConfigDocument, database, defaultLocale, created, isActive, host, iisIPAddress, iisPort, iisApplicationPool, createClientScripts);

					transaction.Commit();
				}

				serverConfig.SaveDocument(serverConfigDocument);

				return company.Id;
			}
		}
		#endregion

		#region public void ActivateCompany(string companyId, bool isActive, bool updateAspDatabase)
		public void ActivateCompany(string companyId, bool isActive, bool updateAspDatabase)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				AspInfo aspInfo = AspInfo.Load(serverConfigDocument);
				CompanyInfo company = CompanyInfo.LoadExtendedInfo(serverConfigDocument, companyId, _dbHelper);

				if (company != null)
				{
					try
					{
						using (DBTransaction transaction = _dbHelper.BeginTransaction())
						{
							// Update company database
							_dbHelper.Database = company.Database;
							Portal.SetPortalParameterValue(_dbHelper, "system.isactive", isActive.ToString());

							// Update ASP database
							if (updateAspDatabase && aspInfo != null)
							{
								_dbHelper.Database = aspInfo.Database;
								_dbHelper.RunSP("ASP_COMPANY_UPDATE_IS_ACTIVE"
									, DBHelper.MP("@company_uid", SqlDbType.UniqueIdentifier, new Guid(company.Id))
									, DBHelper.MP("@is_active", SqlDbType.Bit, isActive)
									);
							}

							// Update ibn.config
							company.IsActive = isActive;
							company.SaveGeneralInfo(serverConfigDocument);

							transaction.Commit();
						}
					}
					finally
					{
						_dbHelper.Database = null;
					}
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion

		#region public void EnableScheduleService(string companyId, bool isEnabled)
		public void EnableScheduleService(string companyId, bool isEnabled)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				CompanyInfo company = CompanyInfo.LoadGeneralInfo(serverConfigDocument, companyId);
				if (company != null)
				{
					// Update ibn.config
					company.IsScheduleServiceEnabled = isEnabled;
					company.SaveGeneralInfo(serverConfigDocument);
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion

		#region public void ChangeCompanyAddress(string companyId, string scheme, string host, string port, bool updateAspDatabase)
		public void ChangeCompanyAddress(string companyId, string scheme, string host, string port, bool updateAspDatabase)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				AspInfo aspInfo = AspInfo.Load(serverConfigDocument);
				CompanyInfo company = CompanyInfo.LoadExtendedInfo(serverConfigDocument, companyId, _dbHelper);

				if (company != null)
				{
					string oldHost = company.Host;
					ValidateHostName(serverConfigDocument, host, oldHost, true);
					host = host.ToLowerInvariant();
					int oldPort = CalculatePortNumber(company.Port);
					int newPort = CalculatePortNumber(port);

					string codeRoot = Path.Combine(InstallPath, company.CodePath);
					string webPath = Path.Combine(codeRoot, "Web");
					IIisManager iisManager = IisManager.Create(webPath);

					bool bindingChanged = false;

					try
					{
						using (DBTransaction transaction = _dbHelper.BeginTransaction())
						{
							// Update company database
							_dbHelper.Database = company.Database;
							Portal.SetPortalParameterValue(_dbHelper, "system.scheme", scheme);
							Portal.SetPortalParameterValue(_dbHelper, "system.host", host);
							Portal.SetPortalParameterValue(_dbHelper, "system.port", port);

							// Update ASP database
							if (updateAspDatabase && aspInfo != null)
							{
								_dbHelper.Database = aspInfo.Database;
								_dbHelper.RunSP("ASP_COMPANY_UPDATE_DOMAIN"
									, DBHelper.MP("@company_uid", SqlDbType.UniqueIdentifier, new Guid(company.Id))
									, DBHelper.MP("@domain", SqlDbType.NVarChar, 255, host)
									);
							}

							// Update bindings
							bindingChanged = iisManager.ChangeBinding(company.SiteId, oldHost, oldPort, host, newPort);

							// Update ibn.config
							company.Scheme = scheme;
							company.Host = host;
							company.Port = port;
							company.SaveGeneralInfo(serverConfigDocument);

							transaction.Commit();
						}
					}
					catch
					{
						try
						{
							// Roll back bindings
							if (bindingChanged)
								iisManager.ChangeBinding(company.SiteId, host, newPort, oldHost, oldPort);
						}
						catch
						{
						}

						throw;
					}
					finally
					{
						_dbHelper.Database = null;
					}
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion

		#region public void DeleteCompany(string companyId, bool deleteDatabase)
		public void DeleteCompany(string companyId, bool deleteDatabase)
		{
			string context = "Delete Company";
			string companyCodePath = null;

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				CompanyInfo company = CompanyInfo.LoadGeneralInfo(serverConfigDocument, companyId);
				if (company != null)
				{
					context += (": " + company.Host);

					companyCodePath = Path.Combine(InstallPath, company.CodePath);

					// Delete web site
					try
					{
						DeleteCompanyWeb(company, context);
					}
					catch (IisManagerException ex)
					{
						Log.WriteException(context, ex);
					}

					// Delete database
					if (deleteDatabase)
					{
						try
						{
							DeleteDatabase(company.Database);
						}
						catch (Exception ex)
						{
							Log.WriteException(context, ex);
						}
					}

					// Update ibn.config
					try
					{
						company.DeleteGeneralInfo(serverConfigDocument);
					}
					catch (Exception ex)
					{
						Log.WriteException(context, ex);
					}
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}

			// Rename the code directory if it wasn't deleted
			try
			{
				if (!string.IsNullOrEmpty(companyCodePath) && Directory.Exists(companyCodePath))
				{
					string codeDirectory = Path.GetDirectoryName(companyCodePath);
					string codeName = Path.GetFileName(companyCodePath);
					string deletedCodePath = Path.Combine(codeDirectory, "_deleted_" + codeName);
					Directory.Move(companyCodePath, deletedCodePath);
				}
			}
			catch (Exception ex)
			{
				Log.WriteException(context, ex);
			}
		}
		#endregion

		#region public void UpdateCompanyVersion(string companyId)
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		public void UpdateCompanyVersion(string companyId)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				CompanyInfo company = CompanyInfo.LoadGeneralInfo(serverConfigDocument, companyId);
				if (company != null)
				{
					// Get code version
					string companyRoot = Path.Combine(InstallPath, company.CodePath);
					FileVersionInfo version = FileVersionInfo.GetVersionInfo(Path.Combine(companyRoot, @"Web\Portal\bin\Mediachase.IBN.Business.dll"));

					// Set code version
					company.CodeVersion = version.FileBuildPart;

					// Update ibn.config
					company.SaveGeneralInfo(serverConfigDocument);
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion


		#region public ICompanyParameter[] ListCompanyProperties(string companyId)
		public IConfigurationParameter[] ListCompanyProperties(string companyId)
		{
			IConfigurationParameter[] result = new IConfigurationParameter[] { };

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				CompanyInfo company = CompanyInfo.LoadExtendedInfo(serverConfig.LoadDocument(), companyId, _dbHelper);
				if (company != null)
				{
					try
					{
						_dbHelper.Database = company.Database;
						result = Portal.ListPortalParameters(_dbHelper);
					}
					finally
					{
						_dbHelper.Database = null;
					}
				}
			}

			return result;
		}
		#endregion

		#region public string GetCompanyPropertyValue(string companyId, string propertyName)
		public string GetCompanyPropertyValue(string companyId, string propertyName)
		{
			string result = null;

			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				CompanyInfo company = CompanyInfo.LoadExtendedInfo(serverConfig.LoadDocument(), companyId, _dbHelper);
				if (company != null)
				{
					try
					{
						_dbHelper.Database = company.Database;
						result = Portal.GetPortalParameterValue(_dbHelper, propertyName);
					}
					finally
					{
						_dbHelper.Database = null;
					}
				}
			}

			return result;
		}
		#endregion

		#region public void SetCompanyPropertyValue(string companyId, string propertyName, string propertyValue)
		public void SetCompanyPropertyValue(string companyId, string propertyName, string propertyValue)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				CompanyInfo company = CompanyInfo.LoadExtendedInfo(serverConfig.LoadDocument(), companyId, _dbHelper);
				if (company != null)
				{
					try
					{
						_dbHelper.Database = company.Database;
						Portal.SetPortalParameterValue(_dbHelper, propertyName, propertyValue);
					}
					finally
					{
						_dbHelper.Database = null;
					}
				}
			}
		}
		#endregion


		#region public IAspInfo GetAspInfo()
		public IAspInfo GetAspInfo()
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				return AspInfo.Load(serverConfig.LoadDocument());
			}
		}
		#endregion
		#region public bool CanCreateAspSite()
		public bool CanCreateAspSite()
		{
			bool canCreate;

			try
			{
				int allowedPortalsCount = License.PortalsCount;
				canCreate = (allowedPortalsCount < 0 || allowedPortalsCount > 1);
			}
			catch (LicenseExpiredException)
			{
				canCreate = false;
			}

			if (canCreate)
			{
				using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
				{
					AspInfo aspSettings = AspInfo.Load(serverConfig.LoadDocument());
					canCreate = (aspSettings == null);
				}
			}

			return canCreate;
		}
		#endregion
		#region public bool CanDeleteAspSite()
		public bool CanDeleteAspSite()
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.Read))
			{
				AspInfo aspSettings = AspInfo.Load(serverConfig.LoadDocument());
				return (aspSettings != null);
			}
		}
		#endregion
		#region public void CreateAspSite(string host, string iisIPAddress, int iisPort, string iisApplicationPool)
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		public void CreateAspSite(string host, string iisIPAddress, int iisPort, string iisApplicationPool)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			using (SafeXmlDocument schedulerConfig = new SafeXmlDocument(_schedulerConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();
				XmlDocument schedulerConfigDocument = schedulerConfig.LoadDocument();

				ValidateHostName(serverConfigDocument, host, null, true);
				host = host.ToLowerInvariant();

				string context = "Create ASP Site";

				string database = null;
				bool databaseCreated = false;
				AspInfo aspSettings = null;

				try
				{
					CreateSqlUser(_sourceToolsPath);

					// Create database
					string prospectiveDatabaseName = string.Concat(IbnConst.ProductFamilyShort, IbnConst.VersionMajorMinor, "ASP");
					database = BuildUniqueName(prospectiveDatabaseName, GetDatabases(prospectiveDatabaseName));
					CreateAspDatabase(database);
					databaseCreated = true;

					try
					{
						_dbHelper.Database = database;

						// Create database structure
						SqlExecuteScript("2CreateStructure.sql", _aspScriptsPath, _dbHelper, null);

						// Add data to database
						SqlExecuteScript("3AddData.sql", _aspScriptsPath, _dbHelper, null);

						// Update database version
						FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(Path.Combine(_aspPath, @"bin\Mediachase.Ibn.WebAsp.dll"));
						Version version = new Version(fileVersion.FileVersion);
						NameValueCollection replace = new NameValueCollection();
						replace["{MajorVersion}"] = version.Major.ToString(CultureInfo.InvariantCulture);
						replace["{MinorVersion}"] = version.Minor.ToString(CultureInfo.InvariantCulture);
						replace["{BuildNumber}"] = version.Build.ToString(CultureInfo.InvariantCulture);
						SqlExecuteScript("version.sql", _sourceToolsPath, _dbHelper, replace);

						// Update database state
						_dbHelper.RunText("UPDATE [DatabaseVersion] SET [State]=6");
					}
					finally
					{
						_dbHelper.Database = null;
					}

					// Create web site
					aspSettings = CreateAspSiteForDatabase(serverConfigDocument, schedulerConfigDocument, database, host, iisIPAddress, iisPort, iisApplicationPool);

					// Disable custom SQL reports
					RegistrySettings.WriteString(ConstDisableCustomSqlReport, bool.TrueString);
				}
				catch
				{
					#region Undo changes

					// Delete web
					try
					{
						if (aspSettings != null)
							DeleteAspWeb(aspSettings, context);
					}
					catch
					{
					}

					// Delete database
					try
					{
						if (databaseCreated)
							DeleteDatabase(database);
					}
					catch
					{
					}

					#endregion

					throw;
				}

				schedulerConfig.SaveDocument(schedulerConfigDocument);
				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion
		#region public void ChangeAspAddress(string scheme, string host, string port)
		public void ChangeAspAddress(string scheme, string host, string port)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();

				AspInfo aspInfo = AspInfo.Load(serverConfigDocument);
				if (aspInfo != null)
				{
					string oldHost = aspInfo.Host;
					ValidateHostName(serverConfigDocument, host, oldHost, true);
					host = host.ToLowerInvariant();
					int oldPort = CalculatePortNumber(aspInfo.Port);
					int newPort = CalculatePortNumber(port);

					// Update ibn.config
					aspInfo.Scheme = scheme;
					aspInfo.Host = host;
					aspInfo.Port = port;
					aspInfo.Save(serverConfigDocument);

					// Update bindings
					IIisManager iisManager = IisManager.Create(_aspPath);
					iisManager.ChangeBinding(aspInfo.SiteId, oldHost, oldPort, host, newPort);
				}

				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion
		#region public void DeleteAspSite(bool deleteDatabase)
		public void DeleteAspSite(bool deleteDatabase)
		{
			using (SafeXmlDocument serverConfig = new SafeXmlDocument(_serverConfigPath, FileAccess.ReadWrite))
			using (SafeXmlDocument schedulerConfig = new SafeXmlDocument(_schedulerConfigPath, FileAccess.ReadWrite))
			{
				XmlDocument serverConfigDocument = serverConfig.LoadDocument();
				XmlDocument schedulerConfigDocument = schedulerConfig.LoadDocument();

				string context = "Delete ASP Site";

				// Load settings
				AspInfo aspSettings = AspInfo.Load(serverConfigDocument);

				// Delete record from ScheduleService.exe.config
				try
				{
					aspSettings.DeleteWebServiceUri(schedulerConfigDocument);
				}
				catch (Exception ex)
				{
					Log.WriteException(context, ex);
				}

				// Delete web
				try
				{
					DeleteAspWeb(aspSettings, context);
				}
				catch (IisManagerException ex)
				{
					Log.WriteException(context, ex);
				}

				// Delete database
				if (deleteDatabase)
				{
					try
					{
						DeleteDatabase(aspSettings.Database);
					}
					catch (Exception ex)
					{
						Log.WriteException(context, ex);
					}
				}

				// Update ibn.config
				try
				{
					AspInfo.Delete(serverConfigDocument);
				}
				catch (Exception ex)
				{
					Log.WriteException(context, ex);
				}

				schedulerConfig.SaveDocument(schedulerConfigDocument);
				serverConfig.SaveDocument(serverConfigDocument);
			}
		}
		#endregion


		#region public int[] ListUpdates()
		public int[] ListUpdates()
		{
			return UpdateInfo.List(_updatesPath);
		}
		#endregion
		#region public IUpdateInfo[] GetUpdateInfo(int version)
		public IUpdateInfo[] GetUpdateInfo(int version)
		{
			return UpdateInfo.LoadExtendedInfo(Path.Combine(_updatesPath, version.ToString(CultureInfo.InvariantCulture)));
		}
		#endregion
		#region public IUpdateInfo[] GetUpdateInfo(int version)
		public IUpdateInfo[] GetUpdateInfo(string updateDirectory)
		{
			return UpdateInfo.LoadExtendedInfo(updateDirectory);
		}
		#endregion
		#region public bool CheckIfCommonComponentsUpdateIsRequired(int version)
		public bool CheckIfCommonComponentsUpdateIsRequired(int version)
		{
			bool result = false;

			IUpdateInfo[] updates = GetUpdateInfo(version);
			foreach (UpdateInfo update in updates)
			{
				if (update.Version > this.CommonVersion && update.RequiresCommonComponentsUpdate)
				{
					result = true;
					break;
				}
			}

			return result;
		}
		#endregion
		#region public ProcessStartInfo BuildUpdateCommandForCompany(int version, string companyId)
		public ProcessStartInfo BuildUpdateCommandForCompany(int version, string companyId)
		{
			return BuildUpdateCommand(version, "Company", false, companyId, 0);
		}
		#endregion
		#region public ProcessStartInfo BuildUpdateCommandForCommonComponents(int version, int callingProcessId)
		public ProcessStartInfo BuildUpdateCommandForCommonComponents(int version, int callingProcessId)
		{
			return BuildUpdateCommand(version, "Common", true, null, callingProcessId);
		}
		#endregion
		#region public ProcessStartInfo BuildUpdateCommandForServer(int version, int callingProcessId)
		public ProcessStartInfo BuildUpdateCommandForServer(int version, int callingProcessId)
		{
			return BuildUpdateCommand(version, "Server", true, null, callingProcessId);
		}
		#endregion

		#endregion


		#region private void CreateDBHelper()
		private void CreateDBHelper()
		{
			SqlSettings = new SqlServerSettings(_sqlServer, _sqlAuthentication, _sqlAdminUser, _sqlAdminPassword, _sqlPortalUser, _sqlPortalPassword);
			_dbHelper = new DBHelper(SqlSettings.AdminConnectionString);
			_dbHelper.CommandTimeout = 600;
		}
		#endregion

		#region private static void CopyDirectory(string sourcePath, string targetPath)
		private static void CopyDirectory(string sourcePath, string targetPath)
		{
			Directory.CreateDirectory(targetPath);

			foreach (string sourceFilePath in Directory.GetFiles(sourcePath))
			{
				string fileName = Path.GetFileName(sourceFilePath);
				string targetFilePath = Path.Combine(targetPath, fileName);
				File.Copy(sourceFilePath, targetFilePath);
			}

			foreach (string sourceDirectoryPath in Directory.GetDirectories(sourcePath))
			{
				string directoryName = Path.GetFileName(sourceDirectoryPath);
				string targetDirectoryPath = Path.Combine(targetPath, directoryName);
				CopyDirectory(sourceDirectoryPath, targetDirectoryPath);
			}
		}
		#endregion

		#region private static void SqlExecuteScript(string fileName, string scriptDirectory, DBHelper target, NameValueCollection replace)
		private static void SqlExecuteScript(string fileName, string scriptDirectory, DBHelper target, NameValueCollection replace)
		{
			using (StreamReader reader = File.OpenText(Path.Combine(scriptDirectory, fileName)))
			{
				target.RunScript(reader, replace, false);
			}
		}
		#endregion

		#region private static void UpdateWebConfig(string directoryPath, string connsectionString, string companyId, string imDownloadPath)
		private static void UpdateWebConfig(string directoryPath, string connsectionString, string companyId, string imDownloadPath)
		{
			string filePath = Path.Combine(directoryPath, "Web.config");

			XmlDocument doc = new XmlDocument();
			doc.Load(filePath);

			if (!string.IsNullOrEmpty(connsectionString))
				doc.SelectSingleNode("/configuration/appSettings/add[@key='ConnectionString']/@value").Value = connsectionString;

			if (!string.IsNullOrEmpty(companyId))
				doc.SelectSingleNode("/configuration/appSettings/add[@key='CompanyUid']/@value").Value = companyId;

			if (!string.IsNullOrEmpty(imDownloadPath))
				doc.SelectSingleNode("/configuration/appSettings/add[@key='TempDownloadDir']/@value").Value = imDownloadPath;

			//doc.SelectSingleNode("/configuration/mediachase.fileUploader/tempFileStorage/providers/add[@name='McLocalDiskTempFileStorageProvider']/@tempStoragePath").Value = ;

			doc.Save(filePath);
		}
		#endregion

		#region private static void SetWritePermissions(string parentPath, string relativePath)
		private static void SetWritePermissions(string parentPath, string relativePath)
		{
			// Everyone, FullControl
			string path = Path.Combine(parentPath, relativePath);

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

			DirectorySecurity security = new DirectorySecurity();
			security.SetAccessRuleProtection(true, false);
			security.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

			Directory.SetAccessControl(path, security);
		}
		#endregion

		#region private static string BuildUniqueName(string prospectiveName, string[] items)
		private static string BuildUniqueName(string prospectiveName, string[] items)
		{
			string result = null;

			int maxNumber = -1;
			foreach (string item in items)
			{
				int index = item.LastIndexOf(prospectiveName, StringComparison.OrdinalIgnoreCase);
				if (index >= 0)
				{
					index += prospectiveName.Length;
					string numberString;
					if (index + 1 < item.Length)
						numberString = item.Substring(index + 1);
					else
						numberString = "0";

					int number;
					if (Int32.TryParse(numberString, out number))
						maxNumber = number > maxNumber ? number : maxNumber;
				}
			}

			if (maxNumber >= 0)
				result = prospectiveName + "_" + (maxNumber + 1).ToString(CultureInfo.InvariantCulture);

			if (string.IsNullOrEmpty(result))
				result = prospectiveName;

			return result;
		}
		#endregion

		#region private static string BuildDatabaseName(string commonName)
		private static string BuildDatabaseName(string commonName)
		{
			string result = string.Concat(IbnConst.ProductFamilyShort, IbnConst.VersionMajorMinor, "_", commonName.Replace('.', '_'));

			if (result.Length > 120)
				result = result.Substring(0, 120);

			return result;
		}
		#endregion

		#region private static string EncodeSqlString(string value)
		private static string EncodeSqlString(string value)
		{
			string result = value;

			if (!string.IsNullOrEmpty(result))
				result = result.Replace("'", "''");

			return result;
		}
		#endregion

		#region private static int StartProcess(string fileName, string arguments)
		private static int StartProcess(string fileName, string arguments)
		{
			using (Process p = new Process())
			{
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.FileName = fileName;
				p.StartInfo.Arguments = arguments;

				p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
				p.Start();
				p.BeginOutputReadLine();
				p.WaitForExit();

				return p.ExitCode;
			}
		}
		#endregion
		#region private static void OutputHandler(object sendingProcess, DataReceivedEventArgs e)
		private static void OutputHandler(object sendingProcess, DataReceivedEventArgs e)
		{
			//LogFile.WriteMessage(e.Data);
		}
		#endregion

		#region private static string ChangeApplicationPool(IIisManager iisManager, long siteId, string oldPool, bool oldPoolCreated, string prospectivePoolName, string defaultPoolName)
		private static string ChangeApplicationPool(IIisManager iisManager, long siteId, string oldPool, bool oldPoolCreated, string prospectivePoolName, string defaultPoolName)
		{
			string newPool = null;

			if (iisManager.IsApplicationPoolSupported)
			{
				bool newPoolCreated = false;
				bool poolChanged = false;

				try
				{
					// Create new pool
					if (string.IsNullOrEmpty(prospectivePoolName))
					{
						string[] pools = iisManager.ListApplicationPools();
						newPool = BuildUniqueName(defaultPoolName, pools);
						iisManager.CreateApplicationPool(newPool, true);
						newPoolCreated = true;
					}
					else
						newPool = prospectivePoolName;

					// Change pool
					iisManager.ChangeApplicationPool(siteId, newPool);
					poolChanged = true;

					// Delete old pool
					if (oldPoolCreated)
					{
						try
						{
							iisManager.DeleteApplicationPool(oldPool);
						}
						catch (Exception ex)
						{
							Log.WriteException("ChangeApplicationPool", ex);
						}
					}
				}
				catch
				{
					#region Undo changes

					try
					{
						if (poolChanged)
							iisManager.ChangeApplicationPool(siteId, oldPool);
					}
					catch
					{
					}

					try
					{
						if (newPoolCreated)
							iisManager.DeleteApplicationPool(newPool);
					}
					catch
					{
					}

					#endregion

					throw;
				}
			}

			return newPool;
		}
		#endregion

		#region private static string GetDefaultWebName(string host)
		private static string GetDefaultWebName(string host)
		{
			return string.Concat(IbnConst.ProductFamilyShort, " ", IbnConst.VersionMajorDotMinor, " ", host);
		}
		#endregion

		#region private string[] GetDirectories()
		private string[] GetDirectories()
		{
			List<string> list = new List<string>();

			foreach(string path in Directory.GetDirectories(_codePath))
			{
				string item = Path.GetFileName(path);
				list.Add(item);
			}

			return list.ToArray();
		}
		#endregion
		#region private string[] GetDatabases(string name)
		private string[] GetDatabases(string name)
		{
			List<string> list = new List<string>();

			using (IDataReader reader = GetDatabasesByName(name))
			{
				while (reader.Read())
				{
					string item = (string)reader["name"];
					list.Add(item);
				}
			}

			return list.ToArray();
		}
		#endregion

		#region private IDataReader GetDatabasesByName(string databaseName)
		private IDataReader GetDatabasesByName(string databaseName)
		{
			return _dbHelper.RunTextDataReader("SELECT name FROM sysdatabases WHERE charindex(@name, name) > 0",
				DBHelper.MP("@name", SqlDbType.NVarChar, 128, databaseName));
		}
		#endregion
		#region private void CreateSqlUser(string toolsPath)
		private void CreateSqlUser(string toolsPath)
		{
			NameValueCollection replace = new NameValueCollection();

			replace["{Login}"] = EncodeSqlString(_sqlPortalUser);
			replace["{Password}"] = EncodeSqlString(_sqlPortalPassword);

			SqlExecuteScript("create_user.sql", toolsPath, _dbHelper, replace);
		}
		#endregion
		#region private void CreateCompanyDatabase(...)
		private void CreateCompanyDatabase(string toolsPath, string databaseName
			, string companyName, string scheme, string host, string port
			, string defaultLocale, bool isActive
			, string adminAccountName, string adminPassword, string adminFirstName, string adminLastName, string adminEmail)
		{
			NameValueCollection replace = new NameValueCollection();

			replace["{Database}"] = EncodeSqlString(databaseName);
			replace["{Login}"] = EncodeSqlString(_sqlPortalUser);

			replace["{CompanyName}"] = EncodeSqlString(companyName);
			replace["{Scheme}"] = EncodeSqlString(scheme);
			replace["{Host}"] = EncodeSqlString(host);
			replace["{Port}"] = EncodeSqlString(port);
			replace["{DefaultLocale}"] = EncodeSqlString(defaultLocale);
			replace["{IsActive}"] = EncodeSqlString(isActive.ToString());
			replace["{AdminAccountName}"] = EncodeSqlString(adminAccountName);
			replace["{AdminPassword}"] = EncodeSqlString(adminPassword);
			replace["{AdminFirstName}"] = EncodeSqlString(adminFirstName);
			replace["{AdminLastName}"] = EncodeSqlString(adminLastName);
			replace["{AdminEmail}"] = EncodeSqlString(adminEmail);

			SqlExecuteScript("create_database.sql", toolsPath, _dbHelper, replace);
		}
		#endregion
		#region private void CreateAspDatabase(string databaseName)
		private void CreateAspDatabase(string databaseName)
		{
			NameValueCollection replace = new NameValueCollection();

			replace["{Database}"] = EncodeSqlString(databaseName);
			replace["{Login}"] = EncodeSqlString(_sqlPortalUser);

			SqlExecuteScript("1CreateDatabase.sql", _aspScriptsPath, _dbHelper, replace);
		}
		#endregion

		#region private void DeleteDatabase(string databaseName)
		private void DeleteDatabase(string databaseName)
		{
			KillDatabaseConnections(databaseName);
			_dbHelper.RunText(string.Concat("DROP DATABASE [" + databaseName + "]"));
		}
		#endregion

		#region private void KillDatabaseConnections(string databaseName)
		private void KillDatabaseConnections(string databaseName)
		{
			List<short> list = new List<short>();

			using (IDataReader reader = _dbHelper.RunSPDataReader("sp_who"))
			{
				while (reader.Read())
				{
					if (0 == string.Compare(reader["dbname"].ToString(), databaseName, StringComparison.OrdinalIgnoreCase))
						list.Add((short)reader["spid"]);
				}
			}

			foreach (short spid in list)
				_dbHelper.RunText(string.Format(CultureInfo.InvariantCulture, "KILL {0}", spid));
		}
		#endregion

		#region private void ValidatePortalsCount(XmlDocument serverConfigDocument)
		private static void ValidatePortalsCount(XmlDocument serverConfigDocument)
		{
			ICompanyInfo[] companies = CompanyInfo.ListCompanies(serverConfigDocument);
			if (License.PortalsCount >= 0 && companies.Length >= License.PortalsCount)
				throw new ConfigurationException("Cannot create one more company due to license restrictions.");
		}
		#endregion

		#region private CompanyInfo CreateCompanyForDatabase(XmlDocument serverConfigDocument, string database, string defaultLocale, DateTime created, bool isActive, string host, string iisIPAddress, int iisPort, string iisApplicationPool, bool createClientScripts)
		[EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
		private CompanyInfo CreateCompanyForDatabase(XmlDocument serverConfigDocument, string database, string defaultLocale, DateTime created, bool isActive, string host, string iisIPAddress, int iisPort, string iisApplicationPool, bool createClientScripts)
		{
			string directoryName = BuildUniqueName(host, GetDirectories());
			string targetPath = Path.Combine(_codePath, directoryName);
			string webPath = Path.Combine(targetPath, "Web");
			string webCommonName = GetDefaultWebName(host);

			IIisManager iisManager = IisManager.Create(webPath);
			bool x64 = iisManager.Is64Bit();
			string[] sites = iisManager.ListSites();
			string[] pools = iisManager.ListApplicationPools();
			string siteName = BuildUniqueName(webCommonName, sites);

			CompanyInfo company = new CompanyInfo();
			company.Created = created;
			company.IsActive = isActive;
			company.IsScheduleServiceEnabled = true;
			company.Scheme = "http";
			company.Host = host;
			company.Port = (iisPort != 80 ? iisPort.ToString(CultureInfo.InvariantCulture) : string.Empty);
			company.Database = database;
			company.CodePath = Path.Combine("Code", directoryName);
			company.Language.Locale = defaultLocale;

			bool directoryCopied = false;
			bool isapiExtensionCreated = false;
			bool imPoolCreated = false;

			try
			{
				// Copy code to new directory
				CopyDirectory(_sourcePath, targetPath);
				directoryCopied = true;

				// Get code version
				FileVersionInfo version = FileVersionInfo.GetVersionInfo(Path.Combine(webPath, @"Portal\bin\Mediachase.IBN.Business.dll"));
				company.CodeVersion = version.FileBuildPart;

				// Allow writing to directories
				SetWritePermissions(webPath, WebName.IMDownload);
				SetWritePermissions(webPath, @"Portal\Admin\Log\Error");
				SetWritePermissions(webPath, @"Portal\Layouts\Images\Charts");

				// Update web.config
				string connectionString = SqlServerSettings.BuildConnectionString(AuthenticationType.SqlServer, _sqlServer, company.Database, _sqlPortalUser, _sqlPortalPassword);
				UpdateWebConfig(Path.Combine(webPath, "Portal"), connectionString, company.Id, Path.Combine(webPath, WebName.IMDownload));

				// Create ISAPI extension
				isapiExtensionCreated = iisManager.AddIsapiExtension(webCommonName);

				if (iisManager.IsApplicationPoolSupported)
				{
					// Create IM application pool
					company.IMPool = BuildUniqueName(webCommonName + " IM", pools);
					iisManager.CreateApplicationPool(company.IMPool, false);
					imPoolCreated = true;

					// Create portal application pool
					if (string.IsNullOrEmpty(iisApplicationPool))
					{
						company.PortalPool = BuildUniqueName(webCommonName + " Portal", pools);
						iisManager.CreateApplicationPool(company.PortalPool, true);
						company.IsPortalPoolCreated = true;
					}
					else
						company.PortalPool = iisApplicationPool;
				}

				// Create web site
				company.SiteId = iisManager.CreateCompanySite(siteName, iisIPAddress, iisPort, host, x64, company.IMPool, company.PortalPool);

				// Register ASP.NET 2.0
				if (iisManager.IisVersion < 7)
				{
					string windir = System.Environment.ExpandEnvironmentVariables(@"%SystemRoot%");
					string regiis = string.Format(CultureInfo.InvariantCulture, @"{0}\Microsoft.NET\Framework{1}\v2.0.50727\aspnet_regiis.exe", windir, x64 ? "64" : "");

					if (_registerAspNet)
						StartProcess(regiis, string.Format(CultureInfo.InvariantCulture, "-s w3svc/{0}/", company.SiteId));

					if (createClientScripts)
						StartProcess(regiis, "-c");
				}

				// Start web site
				try
				{
					iisManager.StartSite(company.SiteId);
				}
				catch (Exception ex)
				{
					Log.WriteException("CreateCompanyForDatabase", ex);
				}

				// Update ibn.config
				company.SaveGeneralInfo(serverConfigDocument);
			}
			catch
			{
				#region Undo changes

				// Delete web site
				try
				{
					if (company.SiteId > 0)
						iisManager.DeleteSite(company.SiteId);
				}
				catch
				{
				}

				if (iisManager.IsApplicationPoolSupported)
				{
					// Delete portal application pool
					try
					{
						if (company.IsPortalPoolCreated)
							iisManager.DeleteApplicationPool(company.PortalPool);
					}
					catch
					{
					}

					// Delete IM application pool
					try
					{
						if (imPoolCreated)
							iisManager.DeleteApplicationPool(company.IMPool);
					}
					catch
					{
					}
				}

				// Delete ISAPI extension
				try
				{
					if (isapiExtensionCreated)
						iisManager.DeleteIsapiExtension();
				}
				catch
				{
				}

				// Delete code directory
				try
				{
					if (directoryCopied)
						Directory.Delete(targetPath, true);
				}
				catch
				{
				}

				#endregion

				throw;
			}

			return company;
		}
		#endregion

		#region private void DeleteCompanyWeb(ICompanyInfo company, string context)
		private void DeleteCompanyWeb(ICompanyInfo company, string context)
		{
			if (company != null)
			{
				string codeRoot = Path.Combine(InstallPath, company.CodePath);
				string webPath = Path.Combine(codeRoot, "Web");

				IIisManager iisManager = IisManager.Create(webPath);

				// Delete web site
				try
				{
					iisManager.DeleteSite(company.SiteId);
				}
				catch (Exception ex)
				{
					Log.WriteException(context, ex);
				}

				if (iisManager.IsApplicationPoolSupported)
				{
					// Delete portal application pool
					try
					{
						if (company.IsPortalPoolCreated)
							iisManager.DeleteApplicationPool(company.PortalPool);
					}
					catch (Exception ex)
					{
						Log.WriteException(context, ex);
					}

					// Delete IM application pool
					try
					{
						iisManager.DeleteApplicationPool(company.IMPool);
					}
					catch (Exception ex)
					{
						Log.WriteException(context, ex);
					}
				}

				// Delete ISAPI extension
				try
				{
					iisManager.DeleteIsapiExtension();
				}
				catch (Exception ex)
				{
					Log.WriteException(context, ex);
				}

				// Delete code directory
				try
				{
					Directory.Delete(codeRoot, true);
				}
				catch
				{
				}
			}
		}
		#endregion

		#region private AspSettings CreateAspSiteForDatabase(XmlDocument serverConfigDocument, XmlDocument schedulerConfigDocument, string database, string host, string iisIPAddress, int iisPort, string iisApplicationPool)
		private AspInfo CreateAspSiteForDatabase(XmlDocument serverConfigDocument, XmlDocument schedulerConfigDocument, string database, string host, string iisIPAddress, int iisPort, string iisApplicationPool)
		{
			AspInfo aspSettings = new AspInfo();
			aspSettings.Database = database;
			aspSettings.Scheme = "http";
			aspSettings.Host = host;
			aspSettings.Port = (iisPort != 80 ? iisPort.ToString(CultureInfo.InvariantCulture) : string.Empty);

			// Update web.config
			string connectionString = SqlServerSettings.BuildConnectionString(AuthenticationType.SqlServer, _sqlServer, database, _sqlPortalUser, _sqlPortalPassword);
			UpdateWebConfig(_aspPath, connectionString, null, null);

			IIisManager iisManager = IisManager.Create(_aspPath);
			string prospectiveSiteName = ConstDefaultAspName;

			try
			{
				if (iisManager.IsApplicationPoolSupported)
				{
					string[] pools = iisManager.ListApplicationPools();

					if (string.IsNullOrEmpty(iisApplicationPool))
					{
						// Create application pool
						aspSettings.ApplicationPool = BuildUniqueName(prospectiveSiteName, pools);
						iisManager.CreateApplicationPool(aspSettings.ApplicationPool, true);
						aspSettings.IsApplicationPoolCreated = true;
					}
					else
						aspSettings.ApplicationPool = iisApplicationPool;
				}

				// Create web site
				string[] sites = iisManager.ListSites();
				string siteName = BuildUniqueName(prospectiveSiteName, sites);
				aspSettings.SiteId = iisManager.CreateSite(siteName, _aspPath, iisIPAddress, iisPort, host, aspSettings.ApplicationPool, false, true);

				// Start web site
				try
				{
					iisManager.StartSite(aspSettings.SiteId);
				}
				catch (Exception ex)
				{
					Log.WriteException("CreateAspSiteForDatabase", ex);
				}

				// Update ibn.config
				aspSettings.Save(serverConfigDocument);

				// Add record to ScheduleService.exe.config
				aspSettings.AddWebServiceUri(schedulerConfigDocument);
			}
			catch
			{
				if (iisManager.IsApplicationPoolSupported)
				{
					// Delete application pool
					try
					{
						if (aspSettings.IsApplicationPoolCreated)
							iisManager.DeleteApplicationPool(aspSettings.ApplicationPool);
					}
					catch
					{
					}
				}

				throw;
			}

			return aspSettings;
		}
		#endregion

		#region private void DeleteAspWeb(AspSettings aspSettings, string context)
		private void DeleteAspWeb(AspInfo aspSettings, string context)
		{
			if (aspSettings != null)
			{
				IIisManager iisManager = IisManager.Create(_aspPath);

				// Delete web site
				try
				{
					iisManager.DeleteSite(aspSettings.SiteId);
				}
				catch (Exception ex)
				{
					Log.WriteException(context, ex);
				}

				if (iisManager.IsApplicationPoolSupported)
				{
					// Delete application pool
					try
					{
						if (aspSettings.IsApplicationPoolCreated)
							iisManager.DeleteApplicationPool(aspSettings.ApplicationPool);
					}
					catch (Exception ex)
					{
						Log.WriteException(context, ex);
					}
				}

				// Enable custom SQL reports
				try
				{
					RegistrySettings.WriteString(ConstDisableCustomSqlReport, null);
				}
				catch (Exception ex)
				{
					Log.WriteException(context, ex);
				}
			}
		}
		#endregion

		#region private static bool ValidateHostName(XmlDocument serverConfigDocument, string host, string oldHost, bool throwExceptionIfRegistered)
		private static bool ValidateHostName(XmlDocument serverConfigDocument, string host, string oldHost, bool throwExceptionIfRegistered)
		{
			if (host == null)
				throw new ArgumentNullException("host");

			string trimmedHost = host.Trim().Trim('.', '/', '\\', '_');
			if (trimmedHost != host || Uri.CheckHostName(host) == UriHostNameType.Unknown)
				throw new ConfigurationException(string.Format(CultureInfo.InvariantCulture, "Invalid host name: '{0}'.", host));

			bool registered = (string.Compare(host, oldHost, StringComparison.OrdinalIgnoreCase) == 0);

			if (!registered)
			{
				// Check ibn.config
				CompanyInfo[] companies = CompanyInfo.ListCompanies(serverConfigDocument);
				foreach (CompanyInfo company in companies)
				{
					if (string.Compare(company.Host, host, StringComparison.OrdinalIgnoreCase) == 0)
					{
						registered = true;
						break;
					}
				}

				// Check bindings
				if (!registered)
					registered = IisManager.Create(string.Empty).CheckIfHostIsRegistered(host);

				if (registered && throwExceptionIfRegistered)
					throw new ConfigurationException(string.Format(CultureInfo.InvariantCulture, "Host '{0}' is already registered.", host));
			}

			return registered;
		}
		#endregion

		#region private ProcessStartInfo BuildUpdateCommand(int version, string target, bool showLog, string companyId, int callingProcessId)
		private ProcessStartInfo BuildUpdateCommand(int version, string target, bool showLog, string companyId, int callingProcessId)
		{
			string filePath = Path.Combine(Path.Combine(_updatesPath, version.ToString(CultureInfo.InvariantCulture)), "Update.exe");

			StringBuilder builder = new StringBuilder();
			AddArgument(builder, ConstTarget, target);
			AddArgument(builder, ConstShowLog, showLog.ToString());
			if (!string.IsNullOrEmpty(companyId))
				AddArgument(builder, ConstId, companyId);
			if(callingProcessId > 0)
				AddArgument(builder, ConstProcessId, callingProcessId.ToString(CultureInfo.InvariantCulture));

			return new ProcessStartInfo(filePath, builder.ToString());
		}
		#endregion
		#region private static void AddArgument(StringBuilder builder, string name, string value)
		private static void AddArgument(StringBuilder builder, string name, string value)
		{
			if (builder.Length > 0)
				builder.Append(' ');

			builder.Append('/');
			builder.Append(name);
			builder.Append(':');
			builder.Append(value);
		}
		#endregion

		#region private static int CalculatePortNumber(string port)
		private static int CalculatePortNumber(string port)
		{
			int portNumber;

			if (string.IsNullOrEmpty(port))
				portNumber = 80;
			else
				portNumber = int.Parse(port, CultureInfo.InvariantCulture);

			return portNumber;
		}
		#endregion
	}
}
