using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using Mediachase.Database;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Meta.Schema;
using Mediachase.Ibn.Data.Sql;
using Mediachase.IBN.Business;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;

namespace Mediachase.Ibn.Business.Configuration
{
	public class DatabaseConfigurator
	{
		private const string DictionariesFileNameFormat = "{0}dictionaries{1}{2}.xml";

		private const string ConstAdminLastName = IbnConst.ProductFamilyShort;
		private const string ConstAdminFistName = "Administrator";
		private const string ConstAlertLastName = IbnConst.ProductFamilyShort;
		private const string ConstAlertFistName = "Alert Service";

		private string _applicationPhysicalPath;
		private string _toolsDir;
		private DBHelper _dbHelper;

		public DatabaseConfigurator(string applicationPhysicalPath)
		{
			_applicationPhysicalPath = applicationPhysicalPath;
			_toolsDir = Path.Combine(applicationPhysicalPath, @"..\..\tools\");

			_dbHelper = new DBHelper(ConfigurationManager.AppSettings["ConnectionString"]);
			_dbHelper.CommandTimeout = 600;
		}

		public void InitializeDatabase()
		{
			try
			{
				DbHelper2.Init2();

				string companyName = null;
				string scheme = null;
				string host = null;
				string port = null;
				string defaultLocale = null;
				bool isActive = false;
				string adminAccountName = null;
				string adminPassword = null;
				string adminFirstName = null;
				string adminLastName = null;
				string adminEmail = null;

				using (DBTransaction tran = _dbHelper.BeginTransaction())
				{
					_dbHelper.RunText("UPDATE [DatabaseVersion] SET [State]=1"); // Set state to "Locked"

					#region Load initialization parameters
					using (IDataReader reader = _dbHelper.RunTextDataReader("SELECT [Name],[Value] FROM [_InitializationParameters]"))
					{
						while (reader.Read())
						{
							switch (reader[0].ToString())
							{
								case "CompanyName":
									companyName = reader[1].ToString();
									break;
								case "Scheme":
									scheme = reader[1].ToString();
									break;
								case "Host":
									host = reader[1].ToString();
									break;
								case "Port":
									port = reader[1].ToString();
									break;
								case "DefaultLocale":
									defaultLocale = reader[1].ToString();
									break;
								case "IsActive":
									isActive = reader[1].ToString() == "True";
									break;
								case "AdminAccountName":
									adminAccountName = reader[1].ToString();
									break;
								case "AdminPassword":
									adminPassword = reader[1].ToString();
									break;
								case "AdminFirstName":
									adminFirstName = reader[1].ToString();
									break;
								case "AdminLastName":
									adminLastName = reader[1].ToString();
									break;
								case "AdminEmail":
									adminEmail = reader[1].ToString();
									break;
							}
						}
					}
					#endregion

					#region Load parameters from PortalConfig
					using (IDataReader reader = _dbHelper.RunTextDataReader("SELECT [Key],[Value] FROM [PortalConfig]"))
					{
						while (reader.Read())
						{
							switch (reader[0].ToString())
							{
								case "system.isactive":
									isActive = reader[1].ToString() == "True";
									break;
								case "system.scheme":
									scheme = reader[1].ToString();
									break;
								case "system.host":
									host = reader[1].ToString();
									break;
								case "system.port":
									port = reader[1].ToString();
									break;
							}
						}
					}
					#endregion

					SqlTransaction sqlTransaction = tran.SqlTran;

					SqlTransaction previousDbContextTransaction = DbContext.Current.Transaction;
					DbContext.Current.Transaction = sqlTransaction;

					SqlTransaction previousSqlContextTransaction = SqlContext.Current.Transaction;
					SqlContext.Current.Transaction = sqlTransaction;

					try
					{
						FillPortalDatabase(companyName, host, defaultLocale, true, adminFirstName, adminLastName, adminAccountName, adminPassword, adminEmail, null, 0);
						FillIMDatabase(companyName);

						_dbHelper.RunText("DROP TABLE [_InitializationParameters]");
						_dbHelper.RunText("UPDATE [DatabaseVersion] SET [State]=6"); // Set state to "Ready"

						tran.Commit();
					}
					finally
					{
						SqlContext.Current.Transaction = previousSqlContextTransaction;
						DbContext.Current.Transaction = previousDbContextTransaction;
					}
				}
			}
			catch (Exception ex)
			{
				Log.WriteException(ex);
			}
		}


		#region private void FillPortalDatabase(...)
		private void FillPortalDatabase(
			string companyName
			, string host
			, string defaultLocale
			, bool addAllLanguages
			, string adminFirstName
			, string adminLastName
			, string adminLogin
			, string adminPassword
			, string adminEmail
			, string principalsXml
			, int timeZoneId
			)
		{
			CreateDatabaseSchema();

			int defaultLangId = FillDictionaries(defaultLocale, addAllLanguages);

			int adminId = CreateUsersAndGroupsInCompany(
				adminLogin
				, adminPassword
				, adminEmail
				, adminFirstName
				, adminLastName
				, companyName
				, host
				, defaultLangId
				, timeZoneId
				, principalsXml
				);

			FillDictionaries2(defaultLocale, addAllLanguages, adminId);
			CreateMetadata();
			CreateDatabaseSchema2();

			// Execute final script
			SqlExecuteScript("PortalFinal.sql", _dbHelper, null);
		}
		#endregion

		#region private void FillIMDatabase(string companyName)
		private void FillIMDatabase(string companyName)
		{
			int newImGroupId;
			int newImAlertGroupId;

			newImGroupId = IMGroupCreate(companyName);
			_dbHelper.RunSP("ASP_SET_DEFAULT_IMGROUP", DBHelper.MP("@IMGROUP_ID", SqlDbType.Int, newImGroupId));

			newImAlertGroupId = IMGroupCreate("{406979C1-DBAD-4dea-8622-1D515B98A470}");
			_dbHelper.RunText(string.Format(CultureInfo.InvariantCulture, "UPDATE [IM_GROUPS] SET [IS_INTERNAL]=1 WHERE [IMGROUP_ID]={0}", newImAlertGroupId));

			ImCopyUsers(newImGroupId, newImAlertGroupId);
		}
		#endregion


		#region private int CreateUsersAndGroupsInCompany(...)
		private int CreateUsersAndGroupsInCompany(
			string adminLogin
			, string adminPassword
			, string adminEMail
			, string adminFirstName
			, string adminLastName
			, string companyName
			, string host
			, int langId
			, int timeZoneId
			, string principalsXml
			)
		{
			int adminId = AddObligatoryUsersAndGroups(
				adminLogin
				, adminPassword
				, adminEMail
				, adminFirstName
				, adminLastName
				, companyName
				, host
				, langId
				, timeZoneId
				);

			if (principalsXml != null && principalsXml.Length > 0)
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(principalsXml);
				AddPredefinedUsersAndGroups(langId, timeZoneId, doc);
			}
			return adminId;
		}
		#endregion
		#region private int AddObligatoryUsersAndGroups(...)
		private int AddObligatoryUsersAndGroups(
			string adminLogin
			, string adminPassword
			, string adminEMail
			, string adminFirstName
			, string adminLastName
			, string companyName
			, string host
			, int languageId
			, int timeZoneId
			)
		{
			string login = adminLogin;
			string password = adminPassword;
			string firstName = adminFirstName;
			string lastName = adminLastName;

			if (adminLogin == null)
				login = "admin";
			if (adminPassword == null)
				password = "admin";
			if (adminFirstName == null || adminLastName == null)
			{
				firstName = ConstAdminFistName;
				lastName = ConstAdminLastName;
			}

			// Add group for company
			int groupId = CreateGroup(9, companyName);

			// Add admin user
			int adminId = CreateUser(login, password, firstName, lastName, adminEMail, languageId, timeZoneId);
			AddUserToGroup(adminId, groupId);

			// Add admin to groups: Admins, PPM, HDM
			AddUserToGroup(adminId, 2);
			AddUserToGroup(adminId, 4);
			AddUserToGroup(adminId, 5);

			// Add roles to admin
			_dbHelper.RunText(string.Format(CultureInfo.InvariantCulture, "INSERT INTO [fsc_UserRoles] ([PrincipalId], [ContainerKey], [Role]) VALUES ({0}, NULL, 'Admin')", adminId));
			_dbHelper.RunText(string.Format(CultureInfo.InvariantCulture, "INSERT INTO [fsc_UserRoles] ([PrincipalId], [ContainerKey], [Role]) VALUES ({0}, NULL, 'PPM')", adminId));
			_dbHelper.RunText(string.Format(CultureInfo.InvariantCulture, "INSERT INTO [fsc_UserRoles] ([PrincipalId], [ContainerKey], [Role]) VALUES ({0}, NULL, 'HDM')", adminId));

			// Add alert user
			login = "alert";
			password = Guid.NewGuid().ToString("B");
			firstName = ConstAlertFistName;
			lastName = ConstAlertLastName;
			string email = string.Format(CultureInfo.InvariantCulture, "alert@{0}", host);

			CreateUser(login, password, firstName, lastName, email, languageId, timeZoneId, 1, false);

			return adminId;
		}
		#endregion
		#region private void AddPredefinedUsersAndGroups(int langId, int timeZoneId, XmlDocument doc)
		private void AddPredefinedUsersAndGroups(int langId, int timeZoneId, XmlDocument doc)
		{
			int newId;

			// add groups
			// a). add "roots"
			foreach (XmlNode group in doc.SelectNodes("/root/groups/group"))
			{
				int oldId = int.Parse(group.Attributes["id"].Value, CultureInfo.InvariantCulture);
				if (doc.SelectNodes(string.Format(CultureInfo.InvariantCulture,
					"/root/containership/row[@id='{0}']", oldId)).Count != 0)
					continue;

				string strGroupName = group.Attributes["name"].Value;
				newId = CreateGroup(1, strGroupName);
				group.Attributes.Append(doc.CreateAttribute("new_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
				foreach (XmlNode row in doc.SelectNodes(string.Format(CultureInfo.InvariantCulture, "/root/containership/row[@parent_id='{0}' and not(@new_parent_id)]", oldId)))
					row.Attributes.Append(doc.CreateAttribute("new_parent_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
			}

			// b). add other groups
			int count = 0; //need to avoid infinite loop
			while (true)
			{
				count = 0;
				foreach (XmlNode group in doc.SelectNodes("/root/groups/group[not(@new_id)]"))
				{
					int oldId = int.Parse(group.Attributes["id"].Value, CultureInfo.InvariantCulture);
					if ((doc.SelectNodes(string.Format(CultureInfo.InvariantCulture,
						"/root/containership/row[@id='{0}' and @new_parent_id]", oldId)).Count == 0))
						continue;

					string strGroupName = group.Attributes["name"].Value;
					int parentID = int.Parse(doc.SelectNodes(string.Format(CultureInfo.InvariantCulture,
						"/root/containership/row[@id='{0}' and @new_parent_id]", oldId))[0].Attributes["new_parent_id"].Value, CultureInfo.InvariantCulture);
					newId = CreateGroup(parentID, strGroupName);
					group.Attributes.Append(doc.CreateAttribute("new_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
					foreach (XmlNode row in doc.SelectNodes(string.Format(CultureInfo.InvariantCulture, "/root/containership/row[@id='{0}' and not(@new_id)]", oldId)))
						row.Attributes.Append(doc.CreateAttribute("new_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
					foreach (XmlNode row in doc.SelectNodes(string.Format(CultureInfo.InvariantCulture, "/root/containership/row[@parent_id='{0}' and not(@new_parent_id)]", oldId)))
						row.Attributes.Append(doc.CreateAttribute("new_parent_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
					count++;
				}
				if ((count == 0) || (doc.SelectNodes("/root/groups/group[not(@new_id)]").Count == 0))
					break;
			}

			// add users
			foreach (XmlNode user in doc.SelectNodes("/root/users/user"))
			{
				int oldID = int.Parse(user.Attributes["id"].Value, CultureInfo.InvariantCulture);

				newId = CreateUser(
					user.Attributes["login"].Value
					, user.Attributes["password"].Value
					, user.Attributes["first_name"].Value
					, user.Attributes["last_name"].Value
					, user.Attributes["email"].Value
					, langId
					, timeZoneId
					);
				user.Attributes.Append(doc.CreateAttribute("new_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
				foreach (XmlNode row in doc.SelectNodes(String.Format(CultureInfo.InvariantCulture, "/root/containership/row[@id='{0}' and not(@new_id)]", oldID)))
					row.Attributes.Append(doc.CreateAttribute("new_id")).Value = newId.ToString(CultureInfo.InvariantCulture);
			}

			// add principals to groups
			foreach (XmlNode row in doc.SelectNodes("/root/containership/row[@new_id=/root/users/user/@new_id]"))
			{
				int id = int.Parse(row.Attributes["new_id"].Value, CultureInfo.InvariantCulture);
				int parent_id = int.Parse(row.Attributes["new_parent_id"].Value, CultureInfo.InvariantCulture);
				AddUserToGroup(id, parent_id);
			}
		}
		#endregion

		#region int CreateGroup()
		private int CreateGroup(int parentId, string groupName)
		{
			return _dbHelper.RunSPInteger("GroupCreate",
				DBHelper.MP("@ParentPrincipalId", SqlDbType.Int, parentId),
				DBHelper.MP("@GroupName", SqlDbType.NVarChar, 50, groupName));
		}
		#endregion
		#region private int CreateUser(...)
		private int CreateUser(
			string login
			, string password
			, string firstName
			, string lastName
			, string email
			, int languageId
			, int timeZoneId
			)
		{
			return CreateUser(login, password, firstName, lastName, email, languageId, timeZoneId, 3, false);
		}

		private int CreateUser(
			string login
			, string password
			, string firstName
			, string lastName
			, string email
			, int languageId
			, int timeZoneId
			, int activity
			, bool isExternal
			)
		{
			string salt = PasswordUtil.CreateSalt(PasswordUtil.SaltSize);
			string hash = PasswordUtil.CreateHash(password, salt);

			int userid;
			userid = _dbHelper.RunSPInteger("UserCreate",
				DBHelper.MP("@Login", SqlDbType.NVarChar, 250, login),
				DBHelper.MP("@Password", SqlDbType.NVarChar, 50, password),
				DBHelper.MP("@FirstName", SqlDbType.NVarChar, 50, firstName),
				DBHelper.MP("@LastName", SqlDbType.NVarChar, 50, lastName),
				DBHelper.MP("@Email", SqlDbType.NVarChar, 250, email),
				DBHelper.MP("@salt", SqlDbType.VarChar, 50, salt),
				DBHelper.MP("@hash", SqlDbType.VarChar, 50, hash),
				DBHelper.MP("@Activity", SqlDbType.TinyInt, activity),
				DBHelper.MP("@IsExternal", SqlDbType.Bit, isExternal),
				DBHelper.MP("@CreatedBy", SqlDbType.Int, DBNull.Value),
				DBHelper.MP("@InviteText", SqlDbType.NText, ""));

			_dbHelper.RunSP("UserDetailsCreateUpdate",
				DBHelper.MP("@UserId", SqlDbType.Int, userid),
				DBHelper.MP("@phone", SqlDbType.NVarChar, 100, ""),
				DBHelper.MP("@fax", SqlDbType.NVarChar, 100, ""),
				DBHelper.MP("@mobile", SqlDbType.NVarChar, 100, ""),
				DBHelper.MP("@position", SqlDbType.NVarChar, 100, ""),
				DBHelper.MP("@department", SqlDbType.NVarChar, 100, ""),
				DBHelper.MP("@company", SqlDbType.NVarChar, 100, ""),
				DBHelper.MP("@location", SqlDbType.NVarChar, 100, ""));

			_dbHelper.RunSP("UserPreferencesCreateUpdate",
				DBHelper.MP("@UserId", SqlDbType.Int, userid),
				DBHelper.MP("@IsNotified", SqlDbType.Bit, true),
				DBHelper.MP("@IsNotifiedByEmail", SqlDbType.Bit, true),
				DBHelper.MP("@IsNotifiedByIBN", SqlDbType.Bit, true),
				DBHelper.MP("@IsBatchNotifications", SqlDbType.Bit, false),
				DBHelper.MP("@Period", SqlDbType.Int, 60),
				DBHelper.MP("@From", SqlDbType.Int, 8),
				DBHelper.MP("@Till", SqlDbType.Int, 20),
				DBHelper.MP("@TimeZoneId", SqlDbType.Int, timeZoneId),
				DBHelper.MP("@LanguageId", SqlDbType.Int, languageId),
				DBHelper.MP("@ReminderType", SqlDbType.Int, 3));

			_dbHelper.RunSP("UserCalendarAdd",
				DbHelper2.mp("@CalendarId", SqlDbType.Int, 1),
				DbHelper2.mp("@UserId", SqlDbType.Int, userid));

			return userid;
		}
		#endregion
		#region private void AddUserToGroup(int userId, int groupId)
		private void AddUserToGroup(int userId, int groupId)
		{
			_dbHelper.RunSP("UserAddToGroup",
				DBHelper.MP("@UserID", SqlDbType.Int, userId),
				DBHelper.MP("@GroupID", SqlDbType.Int, groupId));
		}
		#endregion

		#region private void CreateDatabaseSchema()
		private void CreateDatabaseSchema()
		{
			SqlExecuteScript("dbportal.sql", _dbHelper, null);

			Version version = new Version(IbnConst.FullVersion);
			NameValueCollection replace = new NameValueCollection();

			replace["{MajorVersion}"] = version.Major.ToString(CultureInfo.InvariantCulture);
			replace["{MinorVersion}"] = version.Minor.ToString(CultureInfo.InvariantCulture);
			replace["{BuildNumber}"] = version.Build.ToString(CultureInfo.InvariantCulture);

			SqlExecuteScript("version.sql", _dbHelper, replace);
		}
		#endregion
		#region private void CreateDatabaseSchema2()
		private void CreateDatabaseSchema2()
		{
			SqlExecuteScript("dbportal2.sql", _dbHelper, null);
		}
		#endregion
		#region private int FillDictionaries(string defaultLocale, bool addAllLanguages)
		private int FillDictionaries(string defaultLocale, bool addAllLanguages)
		{
			int defaultLangId;
			XmlDocument doc = new XmlDocument();
			Hashtable languages = new Hashtable();

			// Write language independent dictionaries - first pass
			doc.Load(string.Format(CultureInfo.InvariantCulture, DictionariesFileNameFormat, _toolsDir, "", ""));
			WriteTables(doc.SelectNodes("/dictionaries/dictionary[not(@order) or @order='1']"), _dbHelper, 0, 0, defaultLocale, addAllLanguages, languages);

			defaultLangId = (int)languages[defaultLocale];
			// Write language dependent dictionaries - first pass
			foreach (string locale in languages.Keys)
			{
				if (addAllLanguages || locale == defaultLocale)
				{
					int langId = (int)languages[locale];
					doc.Load(string.Format(CultureInfo.InvariantCulture, DictionariesFileNameFormat, _toolsDir, "_", locale));
					WriteTables(doc.SelectNodes("/dictionaries/dictionary[not(@order) or @order='1']"), _dbHelper, langId, defaultLangId, defaultLocale, addAllLanguages, languages);
				}
			}

			// Write language independent dictionaries - second pass
			doc.Load(string.Format(CultureInfo.InvariantCulture, DictionariesFileNameFormat, _toolsDir, "", ""));
			WriteTables(doc.SelectNodes("/dictionaries/dictionary[@order='2']"), _dbHelper, 0, defaultLangId, defaultLocale, addAllLanguages, languages);

			FillContentTypes();

			FillTimeZonesLocalUtcTable(2000, 2020, defaultLangId);
			FillTimeZonesUtcLocalTable(2000, 2020, defaultLangId);

			using (DBTransaction tran = _dbHelper.BeginTransaction())
			{
				Mediachase.MetaDataPlus.Configurator.MetaInstaller.RestoreFromFile(tran.SqlTran, string.Concat(_toolsDir, "metadata_", defaultLocale, ".xml"));
				tran.Commit();
			}

			doc.Load(string.Concat(_toolsDir, "metadata2_", defaultLocale, ".xml"));
			WriteTables(doc.SelectNodes("/dictionaries/dictionary"), _dbHelper, 0, defaultLangId, defaultLocale, addAllLanguages, languages);

			SetSecurity();

			return defaultLangId;
		}
		#endregion
		#region private int FillDictionaries2(string defaultLocale, bool addAllLanguages, int adminId)
		private int FillDictionaries2(string defaultLocale, bool addAllLanguages, int adminId)
		{
			int defaultLangId;
			XmlDocument doc = new XmlDocument();
			Hashtable languages = new Hashtable();

			// Write language independent dictionaries - first pass
			doc.Load(string.Format(CultureInfo.InvariantCulture, DictionariesFileNameFormat, _toolsDir, "", ""));
			foreach (XmlNode item in doc.SelectNodes("/dictionaries/dictionary[@name='LANGUAGES']/item"))
				languages[item.Attributes["Locale"].Value] = int.Parse(item.Attributes["LanguageId"].Value, CultureInfo.InvariantCulture);

			NameValueCollection replace = new NameValueCollection();
			replace["{ManagerId}"] = adminId.ToString(CultureInfo.InvariantCulture);

			defaultLangId = (int)languages[defaultLocale];
			// Write language dependent dictionaries - second pass
			foreach (string locale in languages.Keys)
			{
				if (addAllLanguages || locale == defaultLocale)
				{
					int langId = (int)languages[locale];
					doc.Load(string.Format(CultureInfo.InvariantCulture, DictionariesFileNameFormat, _toolsDir, "_", locale));
					WriteTables(doc.SelectNodes("/dictionaries/dictionary[@order='2']"), _dbHelper, langId, defaultLangId, defaultLocale, addAllLanguages, languages, replace);
				}
			}

			return defaultLangId;
		}
		#endregion
		#region private void CreateMetadata()
		private void CreateMetadata()
		{
			SchemaDocument schema = new SchemaDocument();
			schema.Load(string.Concat(_toolsDir, "metamodelSchema.xml"));

			SyncCommand[] commands = McXmlSerializer.GetObjectFromFile<SyncCommand[]>(string.Concat(_toolsDir, "metamodelCommands.xml"));
			MetaModelSync.Execute(schema, commands);

			// Fill tables
			XmlDocument doc = new XmlDocument();
			doc.Load(Path.Combine(_toolsDir, "metamodelData.xml"));
			WriteTables(doc.SelectNodes("/dictionaries/dictionary"), _dbHelper, 0, 0, null, true, null);
		}
		#endregion

		#region private void SqlExecuteScript(string fileName, DBHelper target, NameValueCollection replace)
		private void SqlExecuteScript(string fileName, DBHelper target, NameValueCollection replace)
		{
			using (StreamReader reader = File.OpenText(string.Concat(_toolsDir, fileName)))
			{
				target.RunScript(reader, replace, false);
			}
		}
		#endregion

		#region private static void WriteTables(XmlNodeList dictionaries, DBHelper target, int langId, int defaultLangId, string defaultLocale, bool addAllLanguages, Hashtable languages)
		private static void WriteTables(XmlNodeList dictionaries, DBHelper target, int langId, int defaultLangId, string defaultLocale, bool addAllLanguages, Hashtable languages)
		{
			WriteTables(dictionaries, target, langId, defaultLangId, defaultLocale, addAllLanguages, languages, null);
		}
		#endregion
		#region private static void WriteTables(XmlNodeList dictionaries, DBHelper target, int langId, int defaultLangId, string defaultLocale, bool addAllLanguages, Hashtable languages, NameValueCollection replace)
		private static void WriteTables(XmlNodeList dictionaries, DBHelper target, int langId, int defaultLangId, string defaultLocale, bool addAllLanguages, Hashtable languages, NameValueCollection replace)
		{
			string name, lang, colName;
			XmlAttribute a;
			StringBuilder builder = new StringBuilder();
			bool isIdentity, hasIdentity;
			ArrayList columns = new ArrayList();
			Type type;
			Hashtable colTypes = new Hashtable();

			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.Text;

			foreach (XmlNode dict in dictionaries)
			{
				builder.Length = 0;
				columns.Clear();
				colTypes.Clear();
				hasIdentity = false;

				name = dict.Attributes["name"].Value;

				//Console.WriteLine("Writing table: {0}", name);

				a = dict.Attributes["lang"];
				lang = (a != null) ? a.Value : null;

				if (langId == defaultLangId || lang != "single")
				{
					foreach (XmlNode item in dict.SelectNodes("item"))
					{
						if (name == "LANGUAGES")
						{
							languages[item.Attributes["Locale"].Value] = int.Parse(item.Attributes["LanguageId"].Value, CultureInfo.InvariantCulture);
							if (!addAllLanguages && item.Attributes["Locale"].Value != defaultLocale)
								continue;
						}

						builder.Append(string.Format(CultureInfo.InvariantCulture, "\r\nINSERT [{0}] (", name));
						int n = 0;
						if (lang == "multi")
						{
							builder.Append("[LanguageId]");
							n++;
						}
						foreach (XmlAttribute attr in item.Attributes)
						{
							colName = attr.Name;
							if (!columns.Contains(colName))
							{
								columns.Add(colName);
								if (CheckColumn(target, cmd, name, colName, out isIdentity))
								{
									if (isIdentity)
										hasIdentity = true;
								}
							}
							if (n > 0)
								builder.Append(", ");
							builder.Append("[");
							builder.Append(colName);
							builder.Append("]");

							n++;
						}
						if (name == "LANGUAGES")
						{
							builder.Append(", [IsDefault]");
							n++;
						}

						builder.Append(") VALUES(");
						n = 0;
						if (lang == "multi")
						{
							builder.Append(langId.ToString(CultureInfo.InvariantCulture));
							n++;
						}
						foreach (XmlAttribute attr in item.Attributes)
						{
							colName = attr.Name;
							if (n > 0)
								builder.Append(", ");

							string val = attr.Value;
							if (replace != null)
							{
								foreach (string key in replace.Keys)
									val = val.Replace(key, replace[key]);
							}

							if (val == "[=NULL=]")
								builder.Append("NULL");
							else
							{
								type = (Type)colTypes[colName];
								if (type == null)
								{
									type = GetColumnType(target, cmd, name, colName);
									colTypes[colName] = type;
								}

								if (type == typeof(string))
								{
									builder.Append("N'");
									builder.Append(val.Replace("'", "''"));
									builder.Append("'");
								}
								else if (type == typeof(Guid))
								{
									builder.Append("'");
									builder.Append(val);
									builder.Append("'");
								}
								else
									builder.Append(val);
							}
							n++;
						}
						if (name == "LANGUAGES")
						{
							builder.Append(", ");
							builder.Append(item.Attributes["Locale"].Value == defaultLocale ? "1" : "0");
							n++;
						}
						builder.Append(")");
					}

					using (DBTransaction tran = target.BeginTransaction())
					{
						if (hasIdentity)
						{
							cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}] ON", name);
							target.RunCmd(cmd);
						}
						try
						{
							cmd.CommandText = builder.ToString();
							target.RunCmd(cmd);
						}
						finally
						{
							if (hasIdentity)
							{
								cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}] OFF", name);
								target.RunCmd(cmd);
							}
						}

						tran.Commit();
					}
				}
			}
		}
		#endregion
		#region CheckColumn()
		private static bool CheckColumn(DBHelper dbh, SqlCommand cmd, string tableName, string columnName, out bool isIdentity)
		{
			isIdentity = false;
			cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT COLUMNPROPERTY(OBJECT_ID('{0}'), '{1}', 'IsIdentity')", tableName, columnName);
			object o = dbh.RunCmdScalar(cmd);
			if (o != DBNull.Value)
				isIdentity = (int)o != 0;
			return o != DBNull.Value;
		}
		#endregion
		#region GetColumnType()
		private static Type GetColumnType(DBHelper dbh, SqlCommand cmd, string tableName, string columnName)
		{
			cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "SELECT [{0}] FROM [{1}]", columnName, tableName);
			DataTable dt = dbh.RunCmdDataTable(cmd);
			return dt.Columns[columnName].DataType;
		}
		#endregion

		#region private int IMGroupCreate(string groupName)
		private int IMGroupCreate(string groupName)
		{
			int groupId = _dbHelper.RunSPInteger("ASP_ADD_IMGROUP",
				DBHelper.MP("@IMGROUP_ID", SqlDbType.Int, 0),
				DBHelper.MP("@IMGROUP_NAME", SqlDbType.NVarChar, 50, groupName),
				DBHelper.MP("@COLOR", SqlDbType.Char, 6, "2B6087"),
				DBHelper.MP("@IS_PARTNER", SqlDbType.Bit, false));

			byte[] logo = null;
			try
			{
				using (FileStream fileStream = File.OpenRead(Path.Combine(_applicationPhysicalPath, @"Layouts\Images\default_group_image.jpg")))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						logo = binaryReader.ReadBytes((int)fileStream.Length);
					}
				}

			}
			catch (IOException)
			{
			}

			if (logo != null)
			{
				_dbHelper.RunSP("ASP_UPDATE_IMGROUP_LOGO",
					DBHelper.MP("@IMGROUP_ID", SqlDbType.Int, groupId),
					DBHelper.MP("@IMGROUP_LOGO", SqlDbType.Image, logo.Length, logo));
			}

			return groupId;
		}
		#endregion
		#region private void ImCopyUsers(int groupId, int alertGroupID)
		private void ImCopyUsers(int groupId, int alertGroupId)
		{
			User[] users = User.GetUsers(_dbHelper, groupId, alertGroupId);

			foreach (User user in users)
			{
				// Add user to IM database
				user.originalId = _dbHelper.RunSPInteger("ASP_ADD_USER",
					DBHelper.MP("@user_id", SqlDbType.Int, 0),
					DBHelper.MP("@login", SqlDbType.NVarChar, 50, user.login),
					DBHelper.MP("@password", SqlDbType.NVarChar, 50, user.password),
					DBHelper.MP("@first_name", SqlDbType.NVarChar, 50, user.firstName),
					DBHelper.MP("@last_name", SqlDbType.NVarChar, 50, user.lastName),
					DBHelper.MP("@email", SqlDbType.NVarChar, 50, user.email),
					DBHelper.MP("@salt", SqlDbType.VarChar, 50, user.salt),
					DBHelper.MP("@hash", SqlDbType.VarChar, 50, user.hash),
					DBHelper.MP("@imgroup_id", SqlDbType.Int, user.imGroupId),
					DBHelper.MP("@is_active", SqlDbType.Bit, user.isActive));

				// Update IMGroupID & OriginalID for each added user in IBN_PORTAL
				_dbHelper.RunText(string.Format(CultureInfo.InvariantCulture, "UPDATE [USERS] SET ImGroupId={0}, OriginalId={1} WHERE PrincipalId = {2}", user.imGroupId, user.originalId, user.principalId));
			}
		}
		#endregion

		#region private void FillContentTypes()
		private void FillContentTypes()
		{
			int contentTypeId;

			XmlDocument doc = new XmlDocument();
			doc.Load(_toolsDir + "content_types.xml");

			foreach (XmlNode nd in doc.SelectNodes("content_types/item"))
			{
				contentTypeId = int.Parse(nd.Attributes["contentTypeId"].Value, CultureInfo.InvariantCulture);

				ContentTypeInsertIcon(false, contentTypeId, nd.Attributes["fileName"].Value);
				ContentTypeInsertIcon(true, contentTypeId, nd.Attributes["bigFileName"].Value);
			}
		}
		#endregion
		#region private void ContentTypeInsertIcon(bool bigIcon, int contentTypeId, string fileName)
		private void ContentTypeInsertIcon(bool bigIcon, int contentTypeId, string fileName)
		{
			byte[] data;
			using (FileStream fs = File.OpenRead(string.Concat(_toolsDir, "images\\", fileName)))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					data = br.ReadBytes((int)fs.Length);
				}
			}

			object binaryId = _dbHelper.RunTextScalar("INSERT [fsc_FileBinaries] ([Data]) VALUES (@Data) SELECT @@IDENTITY",
				DBHelper.MP("@Data", SqlDbType.Image, data));

			if (binaryId != DBNull.Value)
			{
				object fileId = _dbHelper.RunTextScalar("INSERT [fsc_Files] ([Name], [DirectoryId], [FileBinaryId]) VALUES (@Name, 1, @FileBinaryId) SELECT @@IDENTITY",
					DBHelper.MP("@Name", SqlDbType.NVarChar, 255, fileName),
					DBHelper.MP("@FileBinaryId", SqlDbType.Int, binaryId));

				if (fileId != DBNull.Value)
				{
					if (bigIcon)
						_dbHelper.RunSP("ContentTypeUpdateBigIconFileId",
							DBHelper.MP("@ContentTypeId", SqlDbType.Int, contentTypeId),
							DBHelper.MP("@BigIconFileId", SqlDbType.Int, Convert.ToInt32(fileId, CultureInfo.InvariantCulture)));
					else
						_dbHelper.RunSP("ContentTypeUpdateIconFileId",
							DBHelper.MP("@ContentTypeId", SqlDbType.Int, contentTypeId),
							DBHelper.MP("@IconFileId", SqlDbType.Int, Convert.ToInt32(fileId, CultureInfo.InvariantCulture)));
				}
			}
		}
		#endregion

		// TODO: Optimize code
		#region internal void FillTimeZonesLocalUtcTable(int firstYear, int lastYear, int languageId)
		internal void FillTimeZonesLocalUtcTable(int firstYear, int lastYear, int languageId)
		{
			TimeZoneInfo[] zones = TimeZoneInfo.GetZones(_dbHelper, languageId);

			foreach (TimeZoneInfo zone in zones)
			{
				if (zone.DaylightMonth == 0 || zone.StandardMonth == 0)
				{
					// Add Standard interval [6/18/2004]
					AddTimeToLocalUtc(zone.TimeZoneId, new DateTime(firstYear, 1, 1, 0, 0, 0, 0), new DateTime(lastYear, 1, 1, 0, 0, 0, 0), zone.Bias, false);
				}
				else
				{
					DateTime lastEnd = new DateTime(firstYear, 1, 1, 0, 0, 0, 0);

					for (int year = firstYear; year < lastYear; year++)
					{
						if (zone.DaylightMonth < zone.StandardMonth)
						{
							DateTime startDaylight = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour - (zone.StandardBias + zone.DaylightBias) / 60, 0, 0, 0);
							DateTime endDaylight = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);

							// Add Standard interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, lastEnd, startDaylight, (zone.Bias + zone.StandardBias), false);

							// Add Daylight interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, startDaylight, endDaylight, (zone.Bias + zone.DaylightBias), true);

							lastEnd = endDaylight;
						}
						else
						{
							DateTime startStandard = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);
							DateTime endStandard = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour - (zone.StandardBias + zone.DaylightBias) / 60, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);
							endStandard = TransformDate(endStandard, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);

							// Add Standard interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, lastEnd, startStandard, (zone.Bias + zone.DaylightBias), true);

							// Add Daylight interval [6/18/2004]
							AddTimeToLocalUtc(zone.TimeZoneId, startStandard, endStandard, (zone.Bias + zone.StandardBias), false);

							lastEnd = endStandard;
						}
					}
					// Add Final Interval [6/18/2004]
					// Add Standard interval [6/18/2004]
					AddTimeToLocalUtc(zone.TimeZoneId, lastEnd, new DateTime(lastYear, 1, 1, 0, 0, 0, 0), zone.Bias + (zone.DaylightMonth < zone.StandardMonth ? zone.StandardBias : zone.DaylightBias), false);
				}
			}
		}
		#endregion
		#region FillTimeZonesUtcLocalTable()
		internal void FillTimeZonesUtcLocalTable(int firstYear, int lastYear, int languageId)
		{
			TimeZoneInfo[] zones = TimeZoneInfo.GetZones(_dbHelper, languageId);

			foreach (TimeZoneInfo zone in zones)
			{
				if (zone.DaylightMonth == 0 || zone.StandardMonth == 0)
				{
					// Add Standard interval [6/18/2004]
					AddTimeToUtcLocal(zone.TimeZoneId, new DateTime(firstYear, 1, 1, 0, 0, 0, 0), new DateTime(lastYear, 1, 1, 0, 0, 0, 0), -zone.Bias, false);
				}
				else
				{
					DateTime lastEnd = new DateTime(firstYear, 1, 1, 0, 0, 0, 0);

					for (int year = firstYear; year < lastYear; year++)
					{
						if (zone.DaylightMonth < zone.StandardMonth)
						{
							DateTime startDaylight = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour/*-(tmz.StandardBias+tmz.DaylightBias)/60*/, 0, 0, 0);
							DateTime endDaylight = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);

							startDaylight = startDaylight.AddMinutes((zone.Bias + zone.StandardBias));
							endDaylight = endDaylight.AddMinutes((zone.Bias + zone.StandardBias));

							// Add Standard interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, startDaylight, -(zone.Bias + zone.StandardBias), false);

							// Add Daylight interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, startDaylight, endDaylight, -(zone.Bias + zone.DaylightBias), true);

							lastEnd = endDaylight;
						}
						else
						{
							DateTime startStandard = new DateTime(year, zone.StandardMonth, 1, zone.StandardHour, 0, 0, 0);
							DateTime endStandard = new DateTime(year, zone.DaylightMonth, 1, zone.DaylightHour/*-(tmz.StandardBias+tmz.DaylightBias)/60*/, 0, 0, 0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard, zone.StandardWeek, (DayOfWeek)zone.StandardDayOfWeek);
							endStandard = TransformDate(endStandard, zone.DaylightWeek, (DayOfWeek)zone.DaylightDayOfWeek);

							startStandard = startStandard.AddMinutes((zone.Bias + zone.StandardBias));
							endStandard = endStandard.AddMinutes((zone.Bias + zone.StandardBias));

							// Add Standard interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, startStandard, -(zone.Bias + zone.DaylightBias), true);

							// Add Daylight interval [6/18/2004]
							AddTimeToUtcLocal(zone.TimeZoneId, startStandard, endStandard, -(zone.Bias + zone.StandardBias), false);

							lastEnd = endStandard;
						}
					}
					// Add Final Interval [6/18/2004]
					// Add Standard interval [6/18/2004]
					if (zone.DaylightMonth < zone.StandardMonth)
						AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, new DateTime(lastYear, 1, 1, 0, 0, 0, 0), -(zone.Bias + zone.StandardBias), false);
					else
						AddTimeToUtcLocal(zone.TimeZoneId, lastEnd, new DateTime(lastYear, 1, 1, 0, 0, 0, 0), -(zone.Bias + zone.DaylightBias), false);
				}
			}
		}
		#endregion

		#region internal int AddTimeToLocalUtc(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		internal int AddTimeToLocalUtc(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		{
			return _dbHelper.RunSPInteger("TimeZoneAddLocalUtc",
				DBHelper.MP("@TimeZoneId", SqlDbType.Int, zoneId),
				DBHelper.MP("@Start", SqlDbType.DateTime, start),
				DBHelper.MP("@End", SqlDbType.DateTime, end),
				DBHelper.MP("@TimeOffset", SqlDbType.Int, offset),
				DBHelper.MP("@IsDayLight", SqlDbType.Bit, isDayLight)
				);
		}
		#endregion
		#region internal int AddTimeToUtcLocal(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		internal int AddTimeToUtcLocal(int zoneId, DateTime start, DateTime end, int offset, bool isDayLight)
		{
			return _dbHelper.RunSPInteger("TimeZoneAddUtcLocal",
				DBHelper.MP("@TimeZoneId", SqlDbType.Int, zoneId),
				DBHelper.MP("@Start", SqlDbType.DateTime, start),
				DBHelper.MP("@End", SqlDbType.DateTime, end),
				DBHelper.MP("@TimeOffset", SqlDbType.Int, offset),
				DBHelper.MP("@IsDayLight", SqlDbType.Bit, isDayLight)
				);
		}
		#endregion

		#region internal static DateTime TransformDate(DateTime date, int week, DayOfWeek dayOfWeek)
		internal static DateTime TransformDate(DateTime date, int week, DayOfWeek dayOfWeek)
		{
			DateTime retVal = date;
			int tmpMonth = date.Month;
			while (week > 0 && date.Month == tmpMonth)
			{
				if (date.DayOfWeek == dayOfWeek)
				{
					retVal = date;
					week--;
				}
				date = date.AddDays(1);
			}
			return retVal;
		}
		#endregion

		#region private void SetSecurity()
		private void SetSecurity()
		{
			Dictionary<int, string> dirs = new Dictionary<int, string>();
			using (IDataReader reader = _dbHelper.RunTextDataReader("SELECT [DirectoryId], [ContainerKey] FROM [fsc_Directories]"))
			{
				while (reader.Read())
				{
					int id = (int)reader["DirectoryId"];
					dirs[id] = reader["ContainerKey"].ToString();
				}
			}

			FileStorage fs = new FileStorage();

			foreach (int dirId in dirs.Keys)
			{
				string containerKey = dirs[dirId];
				BaseIbnContainer baseCont = BaseIbnContainer.Create("FileLibrary", containerKey);
				DefaultAccessControlList defAcl = baseCont.Info.Controls["FileStorage"].DefaultAccessControlList;
				AccessControlList acl = AccessControlList.GetACL(dirId);

				foreach (AccessControlEntry ace in defAcl.GetACL(containerKey))
					acl.Add(ace);

				AccessControlList.SetACL(fs, acl, false);
			}
		}
		#endregion
	}
}
