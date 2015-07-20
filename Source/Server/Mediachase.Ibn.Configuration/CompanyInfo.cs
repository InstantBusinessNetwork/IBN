using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Xml;

using Mediachase.Database;

namespace Mediachase.Ibn.Configuration
{
	internal class CompanyInfo : ICompanyInfo
	{
		private LanguageInfo _defaultLanguage = new LanguageInfo();

		public string Id { get; internal set; }
		public bool IsActive { get; internal set; }
		public string Scheme { get; internal set; }
		public string Host { get; internal set; }
		public string Port { get; internal set; }
		public string Database { get; internal set; }
		public string CodePath { get; internal set; }
		public int CodeVersion { get; internal set; }
		public long SiteId { get; internal set; }
		public string IMPool { get; internal set; }
		public string PortalPool { get; internal set; }
		public bool IsPortalPoolCreated { get; internal set; }
		public DateTime Created { get; internal set; }
		public bool IsScheduleServiceEnabled { get; internal set; }

		public int DatabaseSize { get; internal set; }
		public int DatabaseState { get; internal set; }
		public int DatabaseVersion { get; internal set; }
		public int InternalUsersCount { get; internal set; }
		public int ExternalUsersCount { get; internal set; }

		public ILanguageInfo DefaultLanguage
		{
			get
			{
				return _defaultLanguage;
			}
		}

		internal LanguageInfo Language
		{
			get
			{
				return _defaultLanguage;
			}
		}

		internal CompanyInfo()
		{
			Id = Guid.NewGuid().ToString("D");
			Created = DateTime.UtcNow;
		}

		#region internal static CompanyInfo[] ListCompanies(XmlDocument serverConfigDocument)
		internal static CompanyInfo[] ListCompanies(XmlDocument serverConfigDocument)
		{
			return LoadServerConfig(serverConfigDocument, null);
		}
		#endregion

		#region internal static CompanyInfo LoadGeneralInfo(XmlDocument serverConfigDocument, string companyId)
		internal static CompanyInfo LoadGeneralInfo(XmlDocument serverConfigDocument, string companyId)
		{
			CompanyInfo result = null;

			CompanyInfo[] companies = LoadServerConfig(serverConfigDocument, companyId);
			if (companies.Length > 0)
				result = companies[0];

			return result;
		}
		#endregion

		#region internal static CompanyInfo LoadExtendedInfo(XmlDocument serverConfigDocument, string companyId, DBHelper _dbHelper)
		internal static CompanyInfo LoadExtendedInfo(XmlDocument serverConfigDocument, string companyId, DBHelper _dbHelper)
		{
			CompanyInfo company = LoadGeneralInfo(serverConfigDocument, companyId);

			if (company != null)
				company.LoadExtendedInfo(_dbHelper);

			return company;
		}
		#endregion


		#region internal void SaveGeneralInfo(XmlDocument serverConfigDocument)
		internal void SaveGeneralInfo(XmlDocument serverConfigDocument)
		{
			XmlNode companies = serverConfigDocument.SelectSingleNode("/configuration/companies");
			XmlNode company = companies.SelectSingleNode(string.Concat("company[@id='", Id, "']"));
			if(company == null)
				company = companies.AppendChild(serverConfigDocument.CreateElement("company"));

			XmlHelper.SetAttributeValue(company, "id", Id);
			XmlHelper.SetAttributeValue(company, "isActive", IsActive.ToString());
			XmlHelper.SetAttributeValue(company, "scheme", Scheme);
			XmlHelper.SetAttributeValue(company, "host", Host);
			XmlHelper.SetAttributeValue(company, "port", Port);
			XmlHelper.SetAttributeValue(company, "database", Database);
			XmlHelper.SetAttributeValue(company, "defaultLocale", _defaultLanguage.Locale);
			XmlHelper.SetAttributeValue(company, "codePath", CodePath);
			XmlHelper.SetAttributeValue(company, "codeVersion", CodeVersion.ToString(CultureInfo.InvariantCulture));
			XmlHelper.SetAttributeValue(company, "siteId", SiteId.ToString(CultureInfo.InvariantCulture));
			XmlHelper.SetAttributeValue(company, "imPool", IMPool);
			XmlHelper.SetAttributeValue(company, "portalPool", PortalPool);
			XmlHelper.SetAttributeValue(company, "portalPoolCreated", IsPortalPoolCreated.ToString());
			XmlHelper.SetAttributeValue(company, "created", Created.ToString(CultureInfo.InvariantCulture));
			XmlHelper.SetAttributeValue(company, "scheduleServiceEnabled", IsScheduleServiceEnabled.ToString());
		}
		#endregion

		#region internal void DeleteGeneralInfo(XmlDocument serverConfigDocument)
		internal void DeleteGeneralInfo(XmlDocument serverConfigDocument)
		{
			XmlNode companies = serverConfigDocument.SelectSingleNode("/configuration/companies");
			XmlNode company = companies.SelectSingleNode(string.Concat("company[@id='", Id, "']"));
			if (company != null)
			{
				companies.RemoveChild(company);
			}
		}
		#endregion

		#region internal void LoadExtendedInfo(DBHelper dbHelper)
		internal void LoadExtendedInfo(DBHelper dbHelper)
		{
			try
			{
				dbHelper.Database = Database;

				DatabaseSize = 8 * Convert.ToInt32(dbHelper.RunTextScalar("SELECT SUM([size]) FROM [sysfiles] WHERE ([status] & 0x42) = 2"), CultureInfo.InvariantCulture) / 1024; // size in Megabytes
				using (IDataReader reader = dbHelper.RunTextDataReader("SELECT [State],[Build] FROM [DatabaseVersion] WITH (NOLOCK)"))
				{
					reader.Read();
					DatabaseState = Convert.ToInt32(reader["State"], CultureInfo.InvariantCulture);
					DatabaseVersion = Convert.ToInt32(reader["Build"], CultureInfo.InvariantCulture);
				}

				if (DatabaseState == 6) // Ready
				{
					_defaultLanguage.Load(dbHelper);
					InternalUsersCount = Convert.ToInt32(dbHelper.RunTextScalar("SELECT COUNT(*) FROM [USERS] WHERE [Activity]=3 AND [IsExternal]=0"), CultureInfo.InvariantCulture);
					ExternalUsersCount = Convert.ToInt32(dbHelper.RunTextScalar("SELECT COUNT(*) FROM [USERS] WHERE [Activity]=3 AND [IsExternal]=1"), CultureInfo.InvariantCulture);
				}
			}
			finally
			{
				dbHelper.Database = null;
			}
		}
		#endregion


		#region private static CompanyInfo[] LoadServerConfig(XmlDocument serverConfigDocument, string companyId)
		private static CompanyInfo[] LoadServerConfig(XmlDocument serverConfigDocument, string companyId)
		{
			List<CompanyInfo> list = new List<CompanyInfo>();

			string query = "/configuration/companies/company";
			if (!string.IsNullOrEmpty(companyId))
				query = string.Concat(query, "[@id='", companyId, "']");

			foreach (XmlNode node in serverConfigDocument.SelectNodes(query))
			{
				CompanyInfo company = new CompanyInfo();
				company.LoadGeneralInfo(node);
				list.Add(company);
			}

			return list.ToArray();
		}
		#endregion


		#region private void LoadGeneralInfo(XmlNode company)
		private void LoadGeneralInfo(XmlNode company)
		{
			Id = XmlHelper.GetAttributeValue(company, "id");
			IsActive = GetBoolean(company, "isActive", false);
			Scheme = XmlHelper.GetAttributeValue(company, "scheme");
			Host = XmlHelper.GetAttributeValue(company, "host");
			Port = XmlHelper.GetAttributeValue(company, "port");
			Database = XmlHelper.GetAttributeValue(company, "database");
			_defaultLanguage.Locale = _defaultLanguage.FriendlyName = XmlHelper.GetAttributeValue(company, "defaultLocale");
			CodePath = XmlHelper.GetAttributeValue(company, "codePath");
			CodeVersion = int.Parse(XmlHelper.GetAttributeValue(company, "codeVersion"), CultureInfo.InvariantCulture);
			SiteId = long.Parse(XmlHelper.GetAttributeValue(company, "siteId"), CultureInfo.InvariantCulture);
			IMPool = XmlHelper.GetAttributeValue(company, "imPool");
			PortalPool = XmlHelper.GetAttributeValue(company, "portalPool");
			IsPortalPoolCreated = GetBoolean(company, "portalPoolCreated", false);
			Created = DateTime.Parse(XmlHelper.GetAttributeValue(company, "created"), CultureInfo.InvariantCulture);
			IsScheduleServiceEnabled = GetBoolean(company, "scheduleServiceEnabled", false);
		}
		#endregion


		#region private static bool GetBoolean(XmlNode node, string attributeName, bool defaultValue)
		private static bool GetBoolean(XmlNode node, string attributeName, bool defaultValue)
		{
			bool result = defaultValue;

			string value = XmlHelper.GetAttributeValue(node, attributeName);
			if (!string.IsNullOrEmpty(value))
				result = bool.Parse(value);

			return result;
		}
		#endregion
	}
}
