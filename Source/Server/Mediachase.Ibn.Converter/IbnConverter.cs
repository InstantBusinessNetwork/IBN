using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Mediachase.Database;
//using Mediachase.Ibn.ControlSystem;
using MD45 = Mediachase.MetaDataPlus;
using MD47 = Mediachase.Ibn.Data;

using StringCollection = System.Collections.Generic.List<string>;


namespace Mediachase.Ibn.Converter
{
	/// <summary>
	/// Summary description for IbnConverter.
	/// </summary>
	public class IbnConverter
	{
		private Version _requiredSourceVersion = new Version(4, 5, 68);
		private Version _requiredTargetVersion = new Version(IbnConst.FullVersion);

		// Public
		public event EventHandler<ConverterEventArgs> Progress;
		public event EventHandler<ConverterEventArgs> Warning;
		public event EventHandler<ConverterEventArgs> Completed;

		#region * Properties *
		public string SourceConnectionString
		{
			get
			{
				return _sourceConnectionString;
			}
			set
			{
				_sourceConnectionString = value;
			}
		}
		public string TargetConnectionString
		{
			get
			{
				return _targetConnectionString;
			}
			set
			{
				_targetConnectionString = value;
			}
		}

		public Version RequiredSourceVersion
		{
			get
			{
				return _requiredSourceVersion;
			}
		}
		public Version RequiredTargetVersion
		{
			get
			{
				return _requiredTargetVersion;
			}
		}
		#endregion

		#region * Constructors *
		public IbnConverter(int sqlCommandTimeout, int binaryBufferSize, string sourceConnectionStr, string targetConnectionStr)
			: this(sqlCommandTimeout, binaryBufferSize)
		{
			_sourceConnectionString = sourceConnectionStr;
			_targetConnectionString = targetConnectionStr;
		}

		public IbnConverter(int sqlCommandTimeout, int binaryBufferSize)
		{
			_sqlCommandTimeout = sqlCommandTimeout;
			_binaryBufferSize = binaryBufferSize;
			_buffer = new byte[_binaryBufferSize];

			_asyncConvert = new AsyncConvert(Convert);
			_convertAsyncCallback = new AsyncCallback(OnAsyncConvertResult);
		}
		#endregion

		#region BeginConvert()
		public void BeginConvert(int sourceCompanyId, int targetCompanyId)
		{
			_asyncResult = _asyncConvert.BeginInvoke(sourceCompanyId, targetCompanyId, _convertAsyncCallback, null);
		}
		#endregion
		#region EndConvert()
		public void EndConvert()
		{
			_asyncConvert.EndInvoke(_asyncResult);
		}
		#endregion

		#region Convert()
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Convert(int sourceCompanyId, int targetCompanyId)
		{
			bool convertAll = true;

			DBHelper source = new DBHelper(_sourceConnectionString);
			DBHelper target = new DBHelper(_targetConnectionString);
			source.CommandTimeout = target.CommandTimeout = _sqlCommandTimeout;

			#region Validate databases' versions

			ValidateDatabaseVersion(source, RequiredSourceVersion, true);
			ValidateDatabaseVersion(target, RequiredTargetVersion, false);

			string sourcePortalDatabase = GetPortalDB(source, sourceCompanyId);
			string targetPortalDatabase = GetPortalDB(target, targetCompanyId);

			source.Database = sourcePortalDatabase;
			target.Database = targetPortalDatabase;

			ValidateDatabaseVersion(source, RequiredSourceVersion, true);
			ValidateDatabaseVersion(target, RequiredTargetVersion, false);

			source.Database = null;
			target.Database = null;

			#endregion

			#region Declare variables

			XmlDocument doc = new XmlDocument();
			string tableName, selectCommand;
			List<StringCollection> nameLists = new List<StringCollection>();
			List<Hashtable> valueHashTables = new List<Hashtable>();

			StringCollection namesCompanyId = new StringCollection();
			StringCollection namesImGroupId = new StringCollection();
			StringCollection namesUserId = new StringCollection();
			StringCollection namesChatId = new StringCollection();

			Hashtable valuesCompanyId = new Hashtable();
			Hashtable valuesImGroupId = new Hashtable();
			Hashtable valuesUserId = new Hashtable();
			Hashtable valuesChatId = new Hashtable();

			nameLists.Add(namesCompanyId);
			nameLists.Add(namesImGroupId);
			nameLists.Add(namesUserId);
			nameLists.Add(namesChatId);

			valueHashTables.Add(valuesCompanyId);
			valueHashTables.Add(valuesImGroupId);
			valueHashTables.Add(valuesUserId);
			valueHashTables.Add(valuesChatId);

			namesCompanyId.Add("company_id");
			namesImGroupId.Add("imgroup_id");
			namesImGroupId.Add("dep_imgroup_id");
			namesImGroupId.Add("imgroupid");
			namesUserId.Add("user_id");
			namesUserId.Add("from_user_id");
			namesUserId.Add("to_user_id");
			namesUserId.Add("owner_id");
			namesUserId.Add("cont_user_id");
			namesUserId.Add("originalid");
			namesChatId.Add("chat_id");
			#endregion

			if (convertAll)
			{
				#region Convert Main DB
				SendProgress("*** Converting main database ***");
				SendProgress("Deleting target company data");

				target.RunSP("ASP_EmptyTables",
					DBHelper.MP("@company_id", SqlDbType.Int, targetCompanyId),
					DBHelper.MP("@delete_trial_requests", SqlDbType.Bit, true));

				SendProgress("Done");

				doc.Load("ConvertMain.xml");
				foreach (XmlNode table in doc.SelectNodes("/tables/table"))
				{
					bool copyBinary = true;
					{
						XmlAttribute attr = table.Attributes["copyBinary"];
						if (attr != null)
							copyBinary = bool.Parse(attr.Value);
					}

					tableName = table.Attributes["name"].Value;
					switch (tableName)
					{
						case "COMPANIES":
							valuesCompanyId[sourceCompanyId] = targetCompanyId;
							ArrayList skip = new ArrayList();
							skip.Add("company_id");
							skip.Add("domain");
							skip.Add("db_name");
							skip.Add("app_id");
							UpdateRecord(tableName, string.Format(CultureInfo.InvariantCulture, "WHERE [company_id] = {0}", sourceCompanyId), string.Format(CultureInfo.InvariantCulture, "WHERE [company_id] = {0}", targetCompanyId), skip, source, target);
							break;
						case "IMGROUPS":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "WHERE company_id={1}", tableName, sourceCompanyId);
							CopyTable(tableName, "", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, valuesImGroupId);
							break;
						case "USER":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "U JOIN IMGROUPS G ON (G.imgroup_id = U.imgroup_id) WHERE G.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "U.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, valuesUserId);
							break;
						case "CHATS":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "C JOIN [USER] U ON (U.user_id = C.owner_id) JOIN IMGROUPS G ON (G.imgroup_id = U.imgroup_id) WHERE G.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "C.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, valuesChatId);
							break;

						case "BINARY_DATA":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "WHERE [fid] IN (SELECT D2.file_id FROM [FILE] D2 JOIN [USER] U1 ON (U1.user_id = D2.from_user_id) JOIN IMGROUPS G1 ON (G1.imgroup_id = U1.imgroup_id) JOIN [USER] U2 ON (U2.user_id = D2.to_user_id) JOIN IMGROUPS G2 ON (G2.imgroup_id = U2.imgroup_id) WHERE G1.company_id = {0} AND G2.company_id = {0})", sourceCompanyId);
							CopyTable(tableName, "", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, null);
							break;
						case "CONTACT_LIST":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "D2 JOIN [USER] U1 ON (U1.user_id = D2.user_id) JOIN IMGROUPS G1 ON (G1.imgroup_id = U1.imgroup_id) JOIN [USER] U2 ON (U2.user_id = D2.cont_user_id) JOIN IMGROUPS G2 ON (G2.imgroup_id = U2.imgroup_id) WHERE G1.company_id = {1} AND G2.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "D2.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, null);
							break;
						case "DEPENDENCES":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "D JOIN IMGROUPS G1 ON (G1.imgroup_id = D.imgroup_id) JOIN IMGROUPS G2 ON (G2.imgroup_id = D.DEP_IMGROUP_ID) WHERE G1.company_id = {1} AND G2.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "D.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, null);
							break;
						case "CHAT_USERS":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "D2 JOIN [USER] U1 ON (U1.user_id = D2.user_id) JOIN IMGROUPS G1 ON (G1.imgroup_id = U1.imgroup_id) JOIN [USER] U2 ON (U2.user_id = D2.from_user_id) JOIN IMGROUPS G2 ON (G2.imgroup_id = U2.imgroup_id) WHERE G1.company_id = {1} AND G2.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "D2.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, null);
							break;

						case "AUTH_LIST":
						case "FILE":
						case "USER_MESS":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "D2 JOIN [USER] U1 ON (U1.user_id = D2.from_user_id) JOIN IMGROUPS G1 ON (G1.imgroup_id = U1.imgroup_id) JOIN [USER] U2 ON (U2.user_id = D2.to_user_id) JOIN IMGROUPS G2 ON (G2.imgroup_id = U2.imgroup_id) WHERE G1.company_id = {1} AND G2.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "D2.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, null);
							break;

						case "PORTAL_SESSIONS":
						case "SESSIONS":
						case "STATUS_LOG":
						case "CHAT_MESS":
							selectCommand = string.Format(CultureInfo.InvariantCulture, "D1 JOIN [USER] U ON (U.user_id = D1.user_id) JOIN IMGROUPS G ON (G.imgroup_id = U.imgroup_id) WHERE G.company_id = {1}", tableName, sourceCompanyId);
							CopyTable(tableName, "D1.", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, false, null);
							break;
					}
				}

				SendProgress("Hashing passwords...");
				HashPasswords(target, "USER", "USER_ID", "PASSWORD");
				SendProgress("Done");

				SendProgress("*** Done ***");
				#endregion
			}

			#region Convert portal DB
			SendProgress("*** Converting portal database ***");

			source.Database = sourcePortalDatabase;
			target.Database = targetPortalDatabase;

			if (convertAll)
			{
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.Text;

				doc.Load("ConvertPortal.xml");
				XmlNodeList tablesCopy = doc.SelectNodes("/tables/table");

				#region Clear tables in reversed order

				ClearTables(tablesCopy, target);

				#endregion

				#region Load calendars for imported projects
				Hashtable calendars = new Hashtable();
				using (IDataReader reader = source.RunTextDataReader("SELECT CalendarId, ProjectId FROM [CALENDARS] WHERE ProjectId IS NOT NULL"))
				{
					while (reader.Read())
					{
						calendars[reader[0]] = reader[1];
					}
				}
				#endregion

				#region Copy tables

				CopyPortalTables(1, 1, tablesCopy, source, target, nameLists, valueHashTables);

				#endregion

				#region Update ProjectId column for CALENDARS
				SendProgress("Updating project calendars");

				foreach (object key in calendars.Keys)
				{
					object projectId = calendars[key];
					target.RunTextDataReader("UPDATE [CALENDARS] SET ProjectId=@pid WHERE CalendarId=@cid"
						, DBHelper.MP("@pid", SqlDbType.Int, projectId)
						, DBHelper.MP("@cid", SqlDbType.Int, key)
						);
				}

				SendProgress("Done");
				#endregion

				#region Copy meta data
				SendProgress("* Copying metadata *");

				// Load meta classes
				ArrayList metaClasses = MyMetaClass.LoadList(source);

				StringBuilder sbMetaClassesToDelete = new StringBuilder();

				// Load meta fields, create and copy tables
				foreach (MyMetaClass mc in metaClasses)
				{
					if (mc.Name.StartsWith("ListsEx_", StringComparison.Ordinal) || mc.Name.StartsWith("ListTemplate_", StringComparison.Ordinal))
					{
						if(sbMetaClassesToDelete.Length > 0)
							sbMetaClassesToDelete.Append(",");
						sbMetaClassesToDelete.Append(mc.MetaClassId.ToString(CultureInfo.InvariantCulture));
					}
					else
					{
						mc.LoadFields(source);

						SendProgress("Creating tables for MetaData class: {0}", mc.Name);

						mc.CreateTables(target);

						SendProgress("Done");

						selectCommand = "";//string.Format("SELECT * FROM [{0}]", mc.TableName);
						CopyTable(mc.TableName, "", "*", selectCommand, true, source, target, null, null, true, null);
						CopyTable(mc.TableNameHistory, "", "*", selectCommand, true, source, target, null, null, true, null);
					}
				}

				string metaClassesToDelete = sbMetaClassesToDelete.ToString();
				if (!string.IsNullOrEmpty(metaClassesToDelete))
				{
					target.RunText(string.Concat("DELETE FROM [MetaClassMetaFieldRelation] WHERE [MetaClassId] IN (", metaClassesToDelete, ")"));
					target.RunText(string.Concat("DELETE FROM [MetaClass] WHERE [MetaClassId] IN (", metaClassesToDelete, ")"));
				}

				string defaultLocale = (string)source.RunTextScalar("SELECT [Locale] FROM [LANGUAGES] WHERE [IsDefault] = 1");
				using (DBTransaction tran = target.BeginTransaction())
				{
					//target.RunText("DROP TABLE [OrganizationsEx]");
					//target.RunText("DROP TABLE [OrganizationsEx_History]");
					Mediachase.MetaDataPlus.Configurator.MetaInstaller.RestoreFromFile(tran.SqlTran, string.Format(CultureInfo.InvariantCulture, "metadata_{0}.xml", defaultLocale));

					tran.Commit();
				}

				SendProgress("* Done *");
				#endregion

				#region Copy GROUPS to cls_Principal

				using (IDataReader reader = source.RunTextDataReader("SELECT [PrincipalId], [GroupName] FROM [GROUPS] WHERE [PrincipalId] > 9"))
				{
					while (reader.Read())
					{
						int principalId = (int)reader["PrincipalId"];
						string groupName = reader["GroupName"].ToString();

						target.RunText(
							"INSERT INTO [cls_Principal] ([PrincipalId],[Card],[Name],[Activity]) VALUES (@1,N'Department',@2,3)"
						, DBHelper.MP("@1", SqlDbType.Int, principalId)
						, DBHelper.MP("@2", SqlDbType.NVarChar, 100, groupName)
						);
					}
				}

				#endregion

				#region Copy USERS to cls_Principal

				using (IDataReader reader = source.RunTextDataReader("SELECT [PrincipalId], [FirstName], [LastName], [Activity] FROM [USERS]"))
				{
					while (reader.Read())
					{
						int principalId = (int)reader["PrincipalId"];
						string firstName = reader["FirstName"].ToString();
						string lastName = reader["LastName"].ToString();
						byte activity = (byte)reader["Activity"];

						target.RunText(
							"INSERT INTO [cls_Principal] ([PrincipalId],[Card],[Name],[Activity]) VALUES (@1,N'User',@2,@3)"
						, DBHelper.MP("@1", SqlDbType.Int, principalId)
						, DBHelper.MP("@2", SqlDbType.NVarChar, 100, lastName + ", " + firstName)
						, DBHelper.MP("@3", SqlDbType.Int, activity)
						);
					}
				}

				#endregion


				SendProgress("Hashing passwords...");
				HashPasswords(target, "USERS", "PrincipalId", "Password");
				SendProgress("Done");

				// Update ExpectedAssignDate for issues
				target.RunText("SET DATEFIRST 7 UPDATE [INCIDENTS] SET [ExpectedAssignDate] = [dbo].GetFinishDateByDuration(B.CalendarId, I.CreationDate, B.ExpectedAssignTime) FROM [INCIDENTS] I JOIN [IncidentBox] B ON (B.IncidentBoxId = I.IncidentBoxId)");

				#region Convert timesheets

				SendProgress("* Converting timesheets *");

				using (DBTransaction tran = target.BeginTransaction())
				{
					ConvertTimesheets(source, target);

					tran.Commit();
				}

				SendProgress("* Done *");

				#endregion
			}

			string installationDirectoryPath = @"..\"; // TODO: get from registry
			Mediachase.Ibn.GlobalContext.Current = new GlobalContext(System.IO.Path.Combine(installationDirectoryPath, @"Web\portal\Apps"));

			#region Convert lists

			SendProgress("* Converting lists *");

			using (MD47.DataContext dataContext47 = new MD47.DataContext(string.Empty))
			{
				// Initialize metadata
				MD47.DataContext.Current = dataContext47;
				dataContext47.SqlContext.CommandTimeout = _sqlCommandTimeout;
				//MD45.MetaDataContext.Current.ConnectionString = sourcePortalConnectionString;

				SqlTransaction previousSourceTransaction = MD45.MetaDataContext.Current.Transaction;
				SqlTransaction previousTargetTransaction = MD47.DataContext.Current.SqlContext.Transaction;
				SqlTransaction previousDatabaseTransaction = Mediachase.IBN.Database.DbContext.Current.Transaction;

				using (DBTransaction tranSource = source.BeginTransaction())
				using (DBTransaction tran = target.BeginTransaction())
				{
					MD45.MetaDataContext.Current.Transaction = tranSource.SqlTran;
					MD47.DataContext.Current.SqlContext.Transaction = tran.SqlTran;
					Mediachase.IBN.Database.DbContext.Current.Transaction = tran.SqlTran;
					try
					{
						ConvertLists(source, target);

						tran.Commit();
					}
					finally
					{
						MD45.MetaDataContext.Current.Transaction = previousSourceTransaction;
						MD47.DataContext.Current.SqlContext.Transaction = previousTargetTransaction;
						Mediachase.IBN.Database.DbContext.Current.Transaction = previousDatabaseTransaction;
					}
				}
			}

			SendProgress("* Done *");

			#endregion

			#region Convert contacts and organizations

			SendProgress("* Converting contacts and organizations *");

			ClientsConverter clientsConverter = new ClientsConverter(_sqlCommandTimeout);
			try
			{
				clientsConverter.Warning += new EventHandler<ClientsConverterEventArgs>(OnClientsConverterWarning);
				clientsConverter.Convert(source, target);
			}
			finally
			{
				clientsConverter.Warning -= OnClientsConverterWarning;
			}

			SendProgress("* Done *");

			#endregion

			target.RunText("DELETE [HISTORY] WHERE [ObjectTypeId] = 21 OR [ObjectTypeId] = 22");
			target.RunText("DELETE FROM [USER_SETTINGS] WHERE [Value] LIKE '%org%' OR [Value] LIKE '%vcard%' OR [Key] LIKE '%org%' OR [Key] LIKE '%vcard%'");

			SendProgress("*** Done ***");
			#endregion
		}

		void OnClientsConverterWarning(object sender, ClientsConverterEventArgs e)
		{
			if (e != null)
			{
				SendWarning(e.Message);
			}
		}
		#endregion

		#region GetDatabases()
		public static ArrayList GetDatabases(string connectionString)
		{
			ArrayList ret = new ArrayList();
			DBHelper dbh = new DBHelper(connectionString);
			using (IDataReader reader = dbh.RunSPDataReader("sp_databases"))
			{
				while (reader.Read())
					ret.Add(reader["DATABASE_NAME"].ToString());
			}
			return ret;
		}
		#endregion
		#region GetCompanies()
		public static IList<CompanyInfo> GetCompanies(string connectionString)
		{
			DBHelper dbh = new DBHelper(connectionString);
			return CompanyInfo.LoadList(dbh);
		}
		#endregion
		#region GetCalendars()
		public static NameValueCollection GetCalendars(string connectionString)
		{
			DBHelper dbh = new DBHelper(connectionString);
			return LoadList(dbh, "SELECT [CalendarId] AS [ItemId], [CalendarName] AS [ItemName] FROM [CALENDARS]");
		}
		#endregion
		#region GetManagers()
		public static NameValueCollection GetManagers(string connectionString)
		{
			DBHelper dbh = new DBHelper(connectionString);
			return LoadList(dbh, "SELECT [PrincipalId] AS [ItemId], [FirstName]+' '+[LastName] AS [ItemName] FROM [USERS] WHERE [Activity] = 3");
		}
		#endregion

		// Private
		private delegate void AsyncConvert(int sourceCompanyId, int targetCompanyId);

		#region * Fields *
		private int _sqlCommandTimeout;
		private string _sourceConnectionString;
		private string _targetConnectionString;
		private int _binaryBufferSize;
		private byte[] _buffer;

		private AsyncCallback _convertAsyncCallback;
		private AsyncConvert _asyncConvert;
		private IAsyncResult _asyncResult;
		#endregion

		#region LoadList()
		private static NameValueCollection LoadList(DBHelper source, string commandText)
		{
			NameValueCollection ret = new NameValueCollection();
			using (IDataReader reader = source.RunTextDataReader(commandText))
			{
				while (reader.Read())
				{
					ret.Add(reader["ItemId"].ToString(), reader["ItemName"].ToString());
				}
			}
			return ret;
		}
		#endregion

		#region private ClearTables(XmlNodeList tablesCopy, DBHelper target)
		private void ClearTables(XmlNodeList tables, DBHelper target)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.Text;

				for (int i = tables.Count - 1; i >= 0; i--)
				{
					string tableName = tables[i].Attributes["name"].Value;
					XmlAttribute del = tables[i].Attributes["delete"];

					if (del == null || del.Value == "1")
					{
						SendProgress("Clearing table: {0}", tableName);

						StringBuilder sb = new StringBuilder();

						sb.Append("DELETE [");
						sb.Append(tableName);
						sb.Append("]");

						XmlAttribute where = tables[i].Attributes["where"];
						if (where != null)
						{
							sb.Append(" WHERE ");
							sb.Append(where.Value);
						}

						cmd.CommandText = sb.ToString();
						target.RunCmd(cmd);

						SendProgress("Done");
					}
				}
			}
		}
		#endregion
		#region CopyPortalTables()
		private void CopyPortalTables(int currentPass, int maxPass, XmlNodeList tablesCopy, DBHelper source, DBHelper target, IList<StringCollection> nameLists, IList<Hashtable> valueHashTables)
		{
			SendProgress("* Copying tables. Pass {0} of {1} *", currentPass, maxPass);

			string tableName, selectCommand;
			//NameValueCollection columnsMapping = new NameValueCollection();

			foreach (XmlNode table in tablesCopy)
			{
				XmlAttribute aCopy = table.Attributes["copy"];
				XmlAttribute aPass = table.Attributes["pass"];

				bool copy = (aCopy == null || aCopy.Value == "1");
				int tPass = (aPass == null ? 1 : int.Parse(aPass.Value, CultureInfo.InvariantCulture));

				if (copy && tPass == currentPass)
				{
					tableName = table.Attributes["name"].Value;

					bool copyBinary = true;
					{
						XmlAttribute attr = table.Attributes["copyBinary"];
						if (attr != null)
							copyBinary = bool.Parse(attr.Value);
					}

					selectCommand = string.Empty;
					XmlAttribute where = table.Attributes["where"];
					if (where != null)
						selectCommand = string.Concat("WHERE ", where.Value);

					switch (tableName)
					{
						case "CALENDARS":
							// Fill ProjectId column with NULLs
							StringCollection namesProjectId = new StringCollection();
							namesProjectId.Add("projectid");
							List<StringCollection> names = new List<StringCollection>();
							names.Add(namesProjectId);
							Hashtable valuesProjectId = new Hashtable();
							List<Hashtable> values = new List<Hashtable>();
							values.Add(valuesProjectId);

							CopyTable(tableName, "", "*", selectCommand, copyBinary, source, target, names, values, true, null);
							break;
						case "CONTAINERSHIP":
							CopyTable(tableName, "", "*", selectCommand, copyBinary, source, target, null, null, false, null);
							break;
						case "USERS":
							CopyTable(tableName, "", "*", selectCommand, copyBinary, source, target, nameLists, valueHashTables, true, null);
							break;
						// TODO: Update IMGroupId for partner groups
						default:
							CopyTable(tableName, "", "*", selectCommand, copyBinary, source, target, null, null, true, null);
							break;
					}
				}
			}

			SendProgress("* Done *");
		}
		#endregion

		#region SendProgress()
		private void SendProgress(string format, params object[] args)
		{
			if (Progress != null)
			{
				ConverterEventArgs cea = new ConverterEventArgs();
				cea.Message = string.Format(CultureInfo.CurrentUICulture, format, args);

				Progress(this, cea);
				if (cea.Cancel)
					throw new IbnConverterException("Conversion has been canceled by user.");
			}
		}
		#endregion
		#region SendWarning()
		protected void SendWarning(string format, params object[] args)
		{
			if (Warning != null)
			{
				ConverterEventArgs cea = new ConverterEventArgs();
				cea.Message = string.Format(CultureInfo.CurrentUICulture, format, args);
				Warning(this, cea);
				if (cea.Cancel)
					throw new IbnConverterException("Conversion has been canceled by user.");
			}
		}
		#endregion
		#region OnAsyncConvertResult()
		[MethodImpl(MethodImplOptions.Synchronized)]
		private void OnAsyncConvertResult(IAsyncResult result)
		{
			if (Completed != null)
				Completed(this, null);
		}
		#endregion

		#region CopyTable()
		public void CopyTable(string tableName, string selPrefix, string selColumns, string selCondition, bool copyBinary, DBHelper source, DBHelper target, IList<StringCollection> nameLists, IList<Hashtable> valueHashTables, bool insertIdentity, Hashtable identities)
		{
			CopyTable(tableName, tableName, null, selPrefix, selColumns, selCondition, copyBinary, source, target, nameLists, valueHashTables, insertIdentity, identities);
		}

		private void CopyTable(string sourceTableName, string targetTableName, NameValueCollection columnsMapping, string selPrefix, string selColumns, string selCondition, bool copyBinary, DBHelper source, DBHelper target, IList<StringCollection> nameLists, IList<Hashtable> valueHashTables, bool insertIdentity, Hashtable identities)
		{
			SendProgress("Copying data from table {0} to table {1}", sourceTableName, targetTableName);

			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();
			SqlCommand cmd = new SqlCommand();

			cmd.CommandType = CommandType.Text;

			DataTable dt;
			Type dataType;
			bool isIdentity, hasIdentity = false;
			string selectCommand;
			string insertCommand;

			ArrayList columnTypes = new ArrayList();
			ArrayList sourceColumns = new ArrayList();
			ArrayList targetColumns = new ArrayList();
			ArrayList sourceBinColumns = new ArrayList();
			ArrayList targetBinColumns = new ArrayList();
			Hashtable binIdentities = new Hashtable();
			string sourceIdentityColumn = null, targetIdentityColumn = null;
			int identityIndex = -1;
			string sourceColumnName, targetColumnName;

			// Get column names to be inserted
			selectCommand = string.Format(CultureInfo.InvariantCulture, "SELECT {0}{1} FROM [{2}] {3}", selPrefix, selColumns, sourceTableName, selCondition);
			using (IDataReader reader = source.RunTextDataReaderSchemaOnly(selectCommand))
			{
				dt = reader.GetSchemaTable();
				foreach (DataRow row in dt.Rows)
				{
					sourceColumnName = (string)row["ColumnName"];
					targetColumnName = GetTargetColumn(sourceColumnName, columnsMapping);
					dataType = (Type)row["DataType"];

					// Check if column exists and is identity
					if (CheckColumn(target, cmd, targetTableName, targetColumnName, out isIdentity))
					{
						if (isIdentity)
						{
							hasIdentity = true;
							sourceIdentityColumn = sourceColumnName;
							targetIdentityColumn = targetColumnName;
						}

						if (dataType == typeof(byte[]))
						{
							sourceBinColumns.Add(sourceColumnName);
							targetBinColumns.Add(targetColumnName);
						}
						else
						{
							columnTypes.Add(dataType);
							sourceColumns.Add(sourceColumnName);
							targetColumns.Add(targetColumnName);
						}
					}
				}
			}

			// Build select command
			sb.Append("SELECT ");
			int n = 0;
			foreach (string colName in sourceColumns)
			{
				if (n > 0)
					sb.Append(", ");
				sb.Append(selPrefix);
				sb.Append("[");
				sb.Append(colName);
				sb.Append("]");
				n++;
			}
			sb.Append(" FROM [");
			sb.Append(sourceTableName);
			sb.Append("] ");
			sb.Append(selCondition);

			// Build insert command
			sb2.Append("INSERT INTO [");
			sb2.Append(targetTableName);
			sb2.Append("] (");
			int j = n = 0;
			foreach (string colName in targetColumns)
			{
				if (colName == targetIdentityColumn)
					identityIndex = j;
				if (insertIdentity || identityIndex != j)
				{
					if (n > 0)
						sb2.Append(", ");
					sb2.Append("[");
					sb2.Append(colName);
					sb2.Append("]");
					n++;
				}
				j++;
			}
			sb2.Append(") VALUES (");
			n = 0;
			for (int i = 0; i < targetColumns.Count; i++)
			{
				if (insertIdentity || identityIndex != i)
				{
					if (n > 0)
						sb2.Append(", ");
					sb2.Append("@p");
					sb2.Append(i);
					n++;
				}
			}
			sb2.Append(")");

			if (hasIdentity && !insertIdentity)
				sb2.Append(" SELECT @@IDENTITY");

			selectCommand = sb.ToString();
			insertCommand = sb2.ToString();

			// Read data from source and insert into target
			using (DBTransaction tran = target.BeginTransaction())
			{
				object identity;
				using (IDataReader reader = source.RunTextDataReader(selectCommand))
				{
					dt = reader.GetSchemaTable();
					if (insertIdentity && hasIdentity)
						SetInsertIdentity(target, targetTableName, true);

					try
					{
						cmd.CommandText = insertCommand;

						while (reader.Read())
						{
							if (cmd.Parameters.Count == 0)
							{
								// Create parameters
								for (int i = 0; i < targetColumns.Count; i++)
								{
									SqlParameter p = cmd.Parameters.AddWithValue(string.Format(CultureInfo.InvariantCulture, "@p{0}", i), GetColumnValue((string)sourceColumns[i], reader[i], nameLists, valueHashTables));
									if (columnTypes[i] == typeof(Decimal))
										p.DbType = DbType.Decimal;
								}
							}
							else
							{
								// Set values
								for (int i = 0; i < targetColumns.Count; i++)
								{
									cmd.Parameters[i].Value = GetColumnValue((string)sourceColumns[i], reader[i], nameLists, valueHashTables);
								}
							}
							identity = target.RunCmdScalar(cmd);
							if (hasIdentity)
							{
								object oldIdentity = reader[identityIndex];
								if (insertIdentity)
									binIdentities[oldIdentity] = oldIdentity;
								else
									binIdentities[oldIdentity] = identity;
								if (!insertIdentity && identities != null)
									identities[oldIdentity] = identity;
							}
						}
					}
					finally
					{
						if (insertIdentity && hasIdentity)
							SetInsertIdentity(target, targetTableName, false);
					}
				}
				tran.Commit();
			}

			// Copy binary and image fields
			if (copyBinary)
			{
				if (sourceIdentityColumn != null && sourceBinColumns.Count > 0)
				{
					SqlParameter paramTextPtr = DBHelper.MP("@ptr", SqlDbType.VarBinary, 16, null);

					SqlParameter paramData = new SqlParameter("@data", SqlDbType.VarBinary);
					paramData.Value = _buffer;

					SqlParameter paramId = new SqlParameter();
					paramId.ParameterName = "@id";

					foreach (object id in binIdentities.Keys)
					{
						paramId.Value = binIdentities[id];

						for (int i = 0; i < sourceBinColumns.Count; i++)
						{
							CopyBinaryData(source, sourceTableName, (string)sourceBinColumns[i], sourceIdentityColumn, id,
								target, targetTableName, (string)targetBinColumns[i], targetIdentityColumn,
								cmd, paramTextPtr, paramData, paramId);
						}
					}
				}
			}
			SendProgress("Done");
		}
		#endregion

		#region GetTargetColumn()
		private static string GetTargetColumn(string sourceColumn, NameValueCollection columnsMapping)
		{
			string ret = null;
			if (columnsMapping != null)
				ret = columnsMapping[sourceColumn];
			if (ret == null)
				ret = sourceColumn;
			return ret;
		}
		#endregion

		#region UpdateRecord()
		private static void UpdateRecord(string tableName, string sourceCondition, string targetCondition, ArrayList skip, DBHelper source, DBHelper target)
		{
			string colName;
			DataTable dt;
			bool isIdentity;

			ArrayList columns = new ArrayList();
			SqlCommand cmd = new SqlCommand();

			cmd.CommandType = CommandType.Text;

			using (IDataReader reader = source.RunTextDataReader(string.Format(CultureInfo.InvariantCulture, "SELECT * FROM [{0}] {1}", tableName, sourceCondition)))
			{
				dt = reader.GetSchemaTable();
				foreach (DataRow row in dt.Rows)
				{
					colName = (string)row["ColumnName"];
					if (!skip.Contains(colName) && CheckColumn(target, cmd, tableName, colName, out isIdentity))
						columns.Add(colName);
				}
			}
			StringBuilder sb = new StringBuilder();
			StringBuilder sb2 = new StringBuilder();

			sb.Append("SELECT ");
			sb2.Append("UPDATE [");
			sb2.Append(tableName);
			sb2.Append("] SET ");

			for (int i = 0; i < columns.Count; i++)
			{
				colName = (string)columns[i];
				if (i > 0)
				{
					sb.Append(", ");
					sb2.Append(", ");
				}
				sb.Append("[");
				sb.Append(colName);
				sb.Append("]");

				sb2.Append("[");
				sb2.Append(colName);
				sb2.Append("] = @p");
				sb2.Append(i);
			}
			sb.Append(" FROM [");
			sb.Append(tableName);
			sb.Append("] ");
			sb.Append(sourceCondition);

			sb2.Append(" ");
			sb2.Append(targetCondition);

			string selectCommand = sb.ToString();
			string updateCommand = sb2.ToString();

			// Read data from source and update target
			using (IDataReader reader = source.RunTextDataReader(selectCommand))
			{
				dt = reader.GetSchemaTable();
				cmd.CommandText = updateCommand;
				cmd.Parameters.Clear();

				if (reader.Read())
				{
					// Create parameters
					for (int i = 0; i < columns.Count; i++)
					{
						cmd.Parameters.AddWithValue(string.Format(CultureInfo.InvariantCulture, "@p{0}", i), GetColumnValue((string)columns[i], reader[i], null, null));
						if ((Type)dt.Rows[i]["DataType"] == typeof(byte[]))
							cmd.Parameters[i].SqlDbType = SqlDbType.Binary;
					}
					target.RunCmd(cmd);
				}
			}
		}
		#endregion

		#region GetColumnValue()
		private static object GetColumnValue(string columnName, object defaultValue, IList<StringCollection> nameLists, IList<Hashtable> valueHashTables)
		{
			object ret = defaultValue;

			string columnNameLow = columnName.ToLower(CultureInfo.InvariantCulture);

			if (defaultValue != null && defaultValue != DBNull.Value && nameLists != null && valueHashTables != null)
			{
				for (int i = 0; i < nameLists.Count; i++)
				{
					if (nameLists[i].Contains(columnNameLow))
					{
						ret = valueHashTables[i][defaultValue];
						break;
					}
				}
			}

			if (ret == null)
				ret = DBNull.Value;

			return ret;
		}
		#endregion

		#region GetPortalDb()
		private static string GetPortalDB(DBHelper dbh, int companyId)
		{
			using (IDataReader reader = dbh.RunTextDataReader(string.Format(CultureInfo.InvariantCulture, "SELECT db_name FROM COMPANIES WHERE company_id={0}", companyId)))
			{
				reader.Read();
				return reader["db_name"].ToString();
			}
		}
		#endregion

		#region internal static void SetInsertIdentity(DBHelper dbh, string tableName, bool on)
		internal static void SetInsertIdentity(DBHelper dbh, string tableName, bool on)
		{
			dbh.RunText(string.Format(CultureInfo.InvariantCulture, "SET IDENTITY_INSERT [{0}] {1}", tableName, on ? "ON" : "OFF"));
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

		#region CopyBinaryData()
		protected void CopyBinaryData(DBHelper source, string sourceTableName, string sourceBinColumn, string sourceIdentityColumn, object sourceId,
			DBHelper target, string targetTableName, string targetBinColumn, string targetIdentityColumn, object targetId)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.CommandType = CommandType.Text;

				SqlParameter paramTextPtr = DBHelper.MP("@ptr", SqlDbType.VarBinary, 16, null);

				SqlParameter paramData = new SqlParameter("@data", SqlDbType.VarBinary);
				paramData.Value = _buffer;

				SqlParameter paramId = new SqlParameter();
				paramId.ParameterName = "@id";
				paramId.Value = targetId;

				CopyBinaryData(source, sourceTableName, sourceBinColumn, sourceIdentityColumn, sourceId,
					target, targetTableName, targetBinColumn, targetIdentityColumn,
					cmd, paramTextPtr, paramData, paramId);
			}
		}

		protected void CopyBinaryData(DBHelper source, string sourceTableName, string sourceBinColumn, string sourceIdentityColumn, object sourceId,
			DBHelper target, string targetTableName, string targetBinColumn, string targetIdentityColumn,
			SqlCommand cmd, SqlParameter paramTextPtr, SqlParameter paramData, SqlParameter paramId)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			if (cmd == null)
				throw new ArgumentNullException("cmd");

			if (paramTextPtr == null)
				throw new ArgumentNullException("paramTextPtr");

			if (target == null)
				throw new ArgumentNullException("target");

			if (paramData == null)
				throw new ArgumentNullException("paramData");

			object size = source.RunTextScalar(string.Format(CultureInfo.InvariantCulture, "SELECT DATALENGTH([{0}]) FROM [{1}] WHERE [{2}] = @id", sourceBinColumn, sourceTableName, sourceIdentityColumn), new SqlParameter("@id", sourceId));
			if (size != null && size != DBNull.Value && (int)size > 0)
			{
				cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "SET NOCOUNT ON UPDATE [{1}] SET [{0}] = 0x0 WHERE [{2}] = @id SELECT @ptr = TEXTPTR([{0}]) FROM [{1}] WHERE [{2}] = @id", targetBinColumn, targetTableName, targetIdentityColumn);
				cmd.Parameters.Clear();
				cmd.Parameters.Add(paramId);
				cmd.Parameters.Add(paramTextPtr);
				paramTextPtr.Direction = ParameterDirection.Output;
				target.RunCmd(cmd);

				cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "UPDATETEXT [{0}].[{1}] @ptr 0 NULL @data", targetTableName, targetBinColumn);
				cmd.Parameters.Clear();
				cmd.Parameters.Add(paramData);
				cmd.Parameters.Add(paramTextPtr);
				paramTextPtr.Direction = ParameterDirection.Input;

				using (IDataReader reader = source.RunTextDataReaderBlob(string.Format(CultureInfo.InvariantCulture, "SELECT [{0}] FROM [{1}] WHERE [{2}] = @id", sourceBinColumn, sourceTableName, sourceIdentityColumn), new SqlParameter("@id", sourceId)))
				{
					if (reader.Read())
					{
						long read, startIndex = 0;
						do
						{
							read = reader.GetBytes(0, startIndex, _buffer, 0, _binaryBufferSize);

							paramData.Size = (int)read;
							target.RunCmd(cmd);

							startIndex += read;
							cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "UPDATETEXT [{0}].[{1}] @ptr NULL NULL @data", targetTableName, targetBinColumn);
						} while (read == _binaryBufferSize);
					}
				}
			}
		}
		#endregion

		#region BoolToStr()
		protected static string BoolToStr(object value)
		{
			return (bool)value ? "1" : "0";
		}
		#endregion

		#region private static void ConvertTimesheets(DBHelper source, DBHelper target)
		private static void ConvertTimesheets(DBHelper source, DBHelper target)
		{
			IDictionary<int, Project> projects = Project.LoadList(source);

			#region Copy Projects to cls_Project

			foreach (Project project in projects.Values)
			{
				project.Save(target);
			}

			#endregion

			#region Copy WeekTimeSheet to cls_TimeTrackingBlock

			IDictionary<int, TTBlock> blocks = TTBlock.LoadList(source);
			string ttGlobalBlockTitle = (string)target.RunTextScalar("SELECT [Title] FROM [cls_TimeTrackingBlockTypeInstance] WHERE [ProjectId] IS NULL");
			foreach (TTBlock block in blocks.Values)
			{
				Project project = null;

				if (block.ProjectId < 0)
					block.Title = ttGlobalBlockTitle;
				else
					project = projects[block.ProjectId];

				block.Save(target, project);
			}

			#endregion

			#region Copy Timesheets to cls_TimeTrackingEntry

			IDictionary<int, TTEntry> entries = TTEntry.LoadList(source);
			foreach (TTEntry entry in entries.Values)
			{
				TTBlock block = blocks[entry.WeekTimeSheetId];

				entry.BlockTypeInstanceId = block.BlockTypeInstanceId;
				entry.OwnerId = block.OwnerId;
				entry.ParentBlockId = block.BlockId;

				entry.Save(target);
			}

			#endregion

			target.RunSP("TimeTrackingBlockRecalculateDayAll");

			target.RunText("UPDATE ActualFinances SET ObjectTypeId = 3, ObjectId = I.ProjectId FROM ActualFinances F JOIN cls_TimeTrackingBlock B ON (F.BlockId = B.TimeTrackingBlockId) JOIN cls_TimeTrackingBlockTypeInstance I ON (I.TimeTrackingBlockTypeInstanceId = B.BlockTypeInstanceId) WHERE F.BlockId IS NOT NULL AND F.ObjectTypeId = 14");
			target.RunText("UPDATE cls_TimeTrackingEntry SET Rate = ISNULL(M.Rate, 0) FROM cls_TimeTrackingEntry E JOIN cls_TimeTrackingBlock B ON E.ParentBlockId = B.TimeTrackingBlockId JOIN cls_TimeTrackingBlockTypeInstance I ON E.BlockTypeInstanceId = I.TimeTrackingBlockTypeInstanceId LEFT JOIN PROJECT_MEMBERS M ON (I.ProjectId = M.ProjectId AND E.OwnerId = M.PrincipalId AND M.IsTeamMember = 1) WHERE B.AreFinancesRegistered = 1 AND E.Rate IS NULL");

			target.RunSP("TimeTrackingRecalculateAllObjects");
		}
		#endregion

		#region private static void ConvertLists(DBHelper source, DBHelper target)
		private static void ConvertLists(DBHelper source, DBHelper target)
		{
			#region Copy LIST_FOLDERS to cls_ListFolder

			IDictionary<int, ListFolder> folders = ListFolder.LoadList(source);
			ListFolder.SaveList(target, folders);

			#endregion

			//load list45
			IDictionary<int, ListInfo45> lists45 = ListInfo45.LoadList(source);

			#region LIST_TYPES [4.5] -> ListType enum[4.7]

			//Get metaType for ListType meta field
			MD47.Meta.Management.MetaFieldType mfTypeListType47 = Mediachase.Ibn.Lists.ListInfo.GetAssignedMetaClass().Fields["ListType"].GetMetaType();

			// Delete old values
			foreach (MD47.Meta.MetaEnumItem enumItem47 in mfTypeListType47.EnumItems)
			{
				MD47.Meta.Management.MetaEnum.RemoveItem(mfTypeListType47, enumItem47.Handle);
			}

			//create listTypes in 47
			int orderId = 1;
			foreach (string listTypeName45 in ListInfo45.ListTypes.Values)
			{
				MD47.Meta.Management.MetaEnum.AddItem(mfTypeListType47, listTypeName45, orderId++);
			}

			#endregion

			Dictionary<int, int> listId45to47 = new Dictionary<int, int>();

			#region List[4.5] -> ListInfo[4.7]

			foreach (ListInfo45 listInfo45 in lists45.Values)
			{
				Mediachase.Ibn.Lists.ListInfo listInfo47 = Mediachase.Ibn.Lists.ListManager.CreateList(listInfo45.FolderId, listInfo45.Title);

				listInfo47.Properties["ListType"].Value = ListInfo45.ListTypes[listInfo45.TypeId];
				listInfo47.Description = listInfo45.Description;
				listInfo47.Created = listInfo45.CreationDate;
				listInfo47.CreatorId = listInfo45.CreatorId;
				listInfo47.Status = listInfo45.StatusId;

				listInfo47.Save();

				//List_{0}[4.5] -> List_{0}[4.6]
				MD47.Meta.Management.MetaClass mcList47 = Mediachase.Ibn.Lists.ListManager.GetListMetaClass(listInfo47);
				MD45.Configurator.MetaClass mcList45 = MD45.Configurator.MetaClass.Load(string.Format(CultureInfo.InvariantCulture, "ListsEx_{0}", listInfo45.ListId));

				foreach (MD45.Configurator.MetaField mfFix in mcList45.MetaFields)
				{
					MD45.Configurator.MetaDictionary dic = mfFix.Dictionary;
				}
				MetadataPlusToMetadataConverter metaDataConverter = new MetadataPlusToMetadataConverter(mcList45, mcList47);
				metaDataConverter.CopyObjects(null);
				metaDataConverter.CopyViewPreferences();

				Mediachase.Ibn.Lists.ListManager.CreateDefaultForm(mcList47);

				listId45to47.Add(listInfo45.ListId, listInfo47.PrimaryKeyId.Value);
			}

			#endregion

			#region Convert list templates

			foreach (MD45.Configurator.MetaClass mcList45 in MD45.Configurator.MetaClass.GetList("Mediachase.IBN40.ListTemplate", true))
			{
				string templateName = string.IsNullOrEmpty(mcList45.FriendlyName) ? mcList45.Name : mcList45.FriendlyName;

				Mediachase.Ibn.Lists.ListInfo mcListInfo47 = Mediachase.Ibn.Lists.ListManager.CreateTemplate(templateName);
				mcListInfo47.Description = mcList45.Description;
				mcListInfo47.Save();

				//ListTemplate{0}[4.5] -> List_{0}[4.7]
				MD47.Meta.Management.MetaClass mcList47 = Mediachase.Ibn.Lists.ListManager.GetListMetaClass(mcListInfo47);

				foreach (MD45.Configurator.MetaField mfFix in mcList45.MetaFields)
				{
					MD45.Configurator.MetaDictionary dic = mfFix.Dictionary;
				}
				MetadataPlusToMetadataConverter metaDataConverter = new MetadataPlusToMetadataConverter(mcList45, mcList47);
				metaDataConverter.CopyObjects(null);
				metaDataConverter.CopyViewPreferences();
			}

			#endregion

			#region Copy access restrictions.

			target.RunText("DELETE [LISTINFO_ACCESS]");
			using (IDataReader reader = source.RunTextDataReader("SELECT [ListId],[PrincipalId],[AllowLevel] FROM [LIST_ACCESS]"))
			{
				while (reader.Read())
				{
					int listId45 = (int)reader["ListId"];
					int principalId = (int)reader["PrincipalId"];
					byte allowLevel = (byte)reader["AllowLevel"];

					if (listId45to47.ContainsKey(listId45))
					{
						int listId47 = listId45to47[listId45];
						target.RunText(
							"INSERT INTO [LISTINFO_ACCESS] ([ListId],[PrincipalId],[AllowLevel]) VALUES (@1,@2,@3)"
						, DBHelper.MP("@1", SqlDbType.Int, listId47)
						, DBHelper.MP("@2", SqlDbType.Int, principalId)
						, DBHelper.MP("@3", SqlDbType.TinyInt, allowLevel)
						);
					}
				}
			}

			#endregion
		}
		#endregion

		#region private void ValidateDatabaseVersion(DBHelper dbHelper, Version requiredVersion, bool isSource)
		private void ValidateDatabaseVersion(DBHelper dbHelper, Version requiredVersion, bool isSource)
		{
			using (DBTransaction tran = dbHelper.BeginTransaction())
			{
				Version currentVersion = new Version();

				using (IDataReader reader = dbHelper.RunTextDataReader("SELECT [Major],[Minor],[Build] FROM [DatabaseVersion]"))
				{
					if (reader.Read())
					{
						int major = (int)reader["Major"];
						int minor = (int)reader["Minor"];
						int build = (int)reader["Build"];

						currentVersion = new Version(major, minor, build);
					}
				}

				if (currentVersion != requiredVersion)
				{
					string databaseType;
					string advise;

					if (isSource)
					{
						databaseType = "Source";
						if (currentVersion < requiredVersion)
							advise = "Update IBN 4.5 to the required version.";
						else
							advise = "Update IBN 4.7 to the latest version or wait for the next update if you already have the latest version.";
					}
					else
					{
						databaseType = "Target";
						advise = "Select different database.";
					}

					throw new IbnConverterException(string.Format("{0} database {1}.{2} has unsupported version {3}. Required version is {4}. {5}"
						, databaseType, tran.SqlTran.Connection.DataSource, tran.SqlTran.Connection.Database, currentVersion, requiredVersion, advise)
						);
				}
			}
		}
		#endregion

		#region private static void HashPasswords(DBHelper dbHelper, string tableName, string keyColumnName, string passwordColumnName)
		private static void HashPasswords(DBHelper dbHelper, string tableName, string keyColumnName, string passwordColumnName)
		{
			Dictionary<string, string> passwords = new Dictionary<string, string>();

			// Read passwords from database
			string selectCommand = string.Format(CultureInfo.InvariantCulture, "SELECT [{0}],[{1}] FROM [{2}] WHERE [salt] IS NULL OR [hash] IS NULL", keyColumnName, passwordColumnName, tableName);
			using (IDataReader reader = dbHelper.RunTextDataReader(selectCommand))
			{
				while (reader.Read())
				{
					passwords.Add(reader[0].ToString(), reader[1].ToString());
				}
			}

			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			HashAlgorithm hashAlgorithm = MD5.Create();
			byte[] saltBytes = new byte[5];

			StringBuilder builder = new StringBuilder();

			foreach (string key in passwords.Keys)
			{
				string password = passwords[key];

				rng.GetBytes(saltBytes);
				string salt = System.Convert.ToBase64String(saltBytes);
				string hash = System.Convert.ToBase64String(hashAlgorithm.ComputeHash(Encoding.Unicode.GetBytes(string.Concat(password, "$", salt))));

				builder.Append("UPDATE [");
				builder.Append(tableName);
				builder.Append("] SET [salt]='");
				builder.Append(salt);
				builder.Append("',[hash]='");
				builder.Append(hash);
				builder.Append("' WHERE [");
				builder.Append(keyColumnName);
				builder.Append("]='");
				builder.Append(key);
				builder.Append("'");
				builder.AppendLine();
			}

			string updateCommand = builder.ToString();
			dbHelper.RunText(updateCommand);
		}
		#endregion
	}
}
