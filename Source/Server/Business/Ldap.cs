using System;
using System.Collections;
using System.Data;
using System.DirectoryServices;
using System.Text;

using Mediachase.Ibn;
using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	#region class LdapField
	public class LdapField
	{
		#region * Fields *

		public int FieldId;
		public bool BitField;
		public string IbnName;
		public string LdapName;
		public int BitMask;
		public bool Equal;
		public int CompareTo;

		#endregion

		// Public
		#region CreateUpdate()
		public static int CreateUpdate(int FieldId
			, int LdapId
			, bool BitField
			, string IbnName
			, string LdapName
			, int BitMask
			, bool Equal
			, int CompareTo
			)
		{
			Ldap.CheckAccess();

			return DbLdap.FieldCreateUpdate(FieldId
				, LdapId
				, BitField
				, IbnName
				, LdapName
				, BitMask
				, Equal
				, CompareTo
				);
		}
		#endregion

		#region Delete()
		public static void Delete(int FieldId)
		{
			Ldap.CheckAccess();

			DbLdap.FieldDelete(FieldId);
		}
		#endregion

		#region Load(int fieldId)
		public static LdapField Load(int fieldId)
		{
			Ldap.CheckAccess();

			LdapField field = null;
			using(IDataReader reader = DbLdap.FieldGet(fieldId))
			{
				if(reader.Read())
				{
					field = new LdapField();
					field.Load(reader);
				}
			}
			return field;
		}
		#endregion

		// Internal
		#region Load(IDataReader reader)
		internal void Load(IDataReader reader)
		{
			FieldId = (int)reader["FieldId"];
			BitField = (bool)reader["BitField"];
			IbnName = reader["IbnName"].ToString();
			LdapName = reader["LdapName"].ToString();
			BitMask = DBCommon.NullToInt32(reader["BitMask"]);
			Equal = (bool)DBCommon.NullToObject(reader["Equal"], true);
			CompareTo = DBCommon.NullToInt32(reader["CompareTo"]);
		}
		#endregion
	}
	#endregion

	#region class LdapSettings
	public class LdapSettings
	{
		#region * Fields *
		
		public int LdapId        = -1;
		public string Title      = "Active Directory";
		public string Domain     = "yourdomain";
		public string Username;
		public string Password;
		public string Filter     = "(&(objectClass=user)(objectCategory=person))";
		public string IbnKey     = UserInfo.IbnProperty.LdapUid.ToString();
		public string LdapKey    = UserInfo.AdProperty.ObjectSid.ToString();
		public bool Activate = true;
		public bool Deactivate = true;
		public bool Autosync     = false;
		public DateTime AutosyncStart;
		public int AutosyncInterval = 24;
		public DateTime LastSynchronization;

		public ArrayList Fields = new ArrayList();

		#endregion

		public LdapSettings()
		{
		}

		#region Load()
		public static LdapSettings Load(int ldapId)
		{
			Ldap.CheckAccess();

			LdapSettings ldap = null;

			using(IDataReader reader = DbLdap.SettingsGet(ldapId))
			{
				if(reader.Read())
				{
					ldap = new LdapSettings();

					ldap.LdapId = ldapId;
					ldap.Title = reader["Title"].ToString();
					ldap.Domain = reader["Domain"].ToString();
					ldap.Username = reader["Username"].ToString();
					ldap.Password = reader["Password"].ToString();
					ldap.Filter = reader["Filter"].ToString();
					ldap.IbnKey = reader["IbnKey"].ToString();
					ldap.LdapKey = reader["LdapKey"].ToString();
					ldap.Activate = (bool)reader["Activate"];
					ldap.Deactivate = (bool)reader["Deactivate"];
					ldap.Autosync = (bool)reader["Autosync"];
					ldap.AutosyncStart = (DateTime)reader["AutosyncStart"];
					ldap.AutosyncInterval = (int)reader["AutosyncInterval"];
					ldap.LastSynchronization = DBCommon.NullToDateTime(reader["LastSynchronization"]);
				}
			}
			
			if(ldap != null)
			{
				using(IDataReader reader = DbLdap.FieldsGet(ldapId))
				{
					while(reader.Read())
					{
						LdapField field = new LdapField();
						field.Load(reader);
						ldap.Fields.Add(field);
					}
				}
			}
			
			return ldap;
		}
		#endregion


		#region CreateUpdate()
		public static int CreateUpdate(int LdapId
			, string Title
			, string Domain
			, string Username
			, string Password
			, string Filter
			, string IbnKey
			, string LdapKey
			, bool Activate
			, bool Deactivate
			, bool Autosync
			, DateTime AutosyncStart
			, int AutosyncInterval
			)
		{
			Ldap.CheckAccess();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				bool create = LdapId < 0;

				LdapId = DbLdap.SettingsCreateUpdate(LdapId
					, Title
					, Domain
					, Username
					, Password
					, Filter
					, IbnKey
					, LdapKey
					, Activate
					, Deactivate
					, Autosync
					, AutosyncStart
					, AutosyncInterval
					);

				if(create)
				{
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Login.ToString(), UserInfo.AdProperty.SamAccountName.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.FirstName.ToString(), UserInfo.AdProperty.GivenName.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.LastName.ToString(), UserInfo.AdProperty.Sn.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Email.ToString(), UserInfo.AdProperty.Mail.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.LdapUid.ToString(), UserInfo.AdProperty.ObjectSid.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.WindowsLogin.ToString(), UserInfo.AdProperty.SamAccountName.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Phone.ToString(), UserInfo.AdProperty.TelephoneNumber.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Mobile.ToString(), UserInfo.AdProperty.Mobile.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Fax.ToString(), UserInfo.AdProperty.FacsimileTelephoneNumber.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Location.ToString(), UserInfo.AdProperty.L.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Company.ToString(), UserInfo.AdProperty.Company.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.Department.ToString(), UserInfo.AdProperty.Department.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, false, UserInfo.IbnProperty.JobTitle.ToString(), UserInfo.AdProperty.Title.ToString(), 0, false, 0);
					DbLdap.FieldCreateUpdate(-1, LdapId, true, UserInfo.IbnProperty.IsActive.ToString(), UserInfo.AdProperty.UserAccountControl.ToString(), 2, true, 0);
				}

				Ldap.UpdateNextSyncTime(LdapId);

				tran.Commit();
			}

			return LdapId;
		}
		#endregion

		#region Delete()
		public static void Delete(int LdapId)
		{
			Ldap.CheckAccess();

			DbLdap.SettingsDelete(LdapId);
		}
		#endregion

		#region Get()
		/// <summary>
		/// Returns fields: LdapId, Title, Domain, Username, Password, Filter, IbnKey, LdapKey, Activate, Deactivate, Autosync, AutosyncStart, AutosyncInterval, LastSynchronization.
		/// </summary>
		public static IDataReader Get(int LdapId)
		{
			Ldap.CheckAccess();

			return DbLdap.SettingsGet(LdapId);
		}
		#endregion
	}
	#endregion

	#region class Ldap
	public class Ldap
	{
		// Public
		#region CheckAccess()
		public static void CheckAccess()
		{
			if(!Security.IsUserInGroup(InternalSecureGroups.Administrator))
				throw new AccessDeniedException();
		}
		#endregion

		#region Synchronize(int LdapId)
		public static int Synchronize(int LdapId)
		{
			Ldap.CheckAccess();

			LdapSettings ldap = LdapSettings.Load(LdapId);

			Hashtable users = new Hashtable();
			ArrayList updatedUsers = new ArrayList();

			using(IDataReader reader = User.GetLdap())
			{
				while(reader.Read())
				{
					UserInfo ui = new UserInfo();
					ui.Load(reader);
					users[ui[ldap.IbnKey]] = ui;
				}
			}

			if(users.Count > 0)
			{
				using (DirectoryEntry root = new DirectoryEntry(string.Format("LDAP://{0}", ldap.Domain), ldap.Username, ldap.Password))
				{
					root.RefreshCache();
					DirectorySearcher searcher = new DirectorySearcher(root, ldap.Filter);

					foreach (SearchResult result in searcher.FindAll())
					{
						UserInfo ui = users[GetPropertyValue(result, ldap.LdapKey)] as UserInfo;
						if (ui != null)
						{
							foreach (LdapField field in ldap.Fields)
							{
								string sVal = GetPropertyValue(result, field.LdapName);

								if (field.BitField)
								{
									if (sVal.Length < 1)
										continue;

									int iVal = int.Parse(sVal) & field.BitMask;
									if (field.Equal)
										sVal = (iVal == field.CompareTo).ToString();
									else
										sVal = (iVal != field.CompareTo).ToString();
								}

								if (field.IbnName == UserInfo.IbnProperty.WindowsLogin.ToString())
									sVal = string.Format("{0}\\{1}", ldap.Domain, sVal).ToLower();

								if (ui[field.IbnName] != sVal && (sVal.Length > 0 || field.IbnName != UserInfo.IbnProperty.Email.ToString()))
								{
									ui.UpdatedValues[field.IbnName] = sVal;

									if (!updatedUsers.Contains(ui))
										updatedUsers.Add(ui);
								}
							}
						}
					}
				}
			}

			ldap.LastSynchronization = DateTime.UtcNow;
			DbLdap.SettingsUpdateLastSynchronization(ldap.LdapId, ldap.LastSynchronization);

			int logId;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				logId = DbLdap.SyncLogCreate(ldap.LdapId, ldap.LastSynchronization, updatedUsers.Count);
				foreach(UserInfo ui in updatedUsers)
				{
					bool wasActive = bool.Parse(ui.IsActive);

					// Save changes to log
					foreach(string name in UserInfo.PropertyNamesIbnAll)
					{
						string oldVal = ui[name];
						string newVal = ui.UpdatedValues[name];
						if(newVal != null || name == UserInfo.IbnProperty.FirstName.ToString() || name == UserInfo.IbnProperty.LastName.ToString())
						{
							if(newVal == null)
								newVal = oldVal;

							DbLdap.SyncFieldCreate(logId, ui.UserId, name, oldVal, newVal);
						}

						// Replace old values with new ones
						if(newVal != null)
							ui[name] = newVal;
					}

					try
					{
						// Update main database
						DBUser.UpdateMain2(ui.OriginalId, ui.Login, ui.FirstName, ui.LastName, ui.Email);

						// Update portal database
						DBUser.Update(ui.UserId, ui.Login, ui.FirstName, ui.LastName, ui.Email, ui.WindowsLogin, ui.LdapUid);
						DBUser.UpdateProfile(ui.UserId, ui.Phone, ui.Fax, ui.Mobile, ui.JobTitle, ui.Department, ui.Company, ui.Location);

						// Update activity
						bool isActive = bool.Parse(ui.IsActive);
						if(isActive != wasActive && (ldap.Activate && isActive || ldap.Deactivate && !isActive))
							User.UpdateActivity(ui.UserId, isActive);
					}
					catch(Exception ex)
					{
						Log.WriteError(ex.ToString());
					}
				}

				tran.Commit();
			}
			return logId;
		}
		#endregion

		#region SyncLogGetList()
		/// <summary>
		/// Returns fields: LogId, LdapId, Dt, UserCount, Title.
		/// </summary>
		public static DataTable SyncLogGetList(int LogId)
		{
			Ldap.CheckAccess();

			return DbLdap.SyncLogGetList(LogId, Security.CurrentUser.TimeZoneId);
		}
		#endregion

		#region SyncLogGetFields(...)
		public static DataTable SyncLogGetFields(int logId)
		{
			Ldap.CheckAccess();

			DataTable table = new DataTable();

			table.Columns.Add("UserId", typeof(int));
			table.Columns.Add("UserName", typeof(string));
			table.Columns.Add("FieldName", typeof(string));
			table.Columns.Add("NewValue", typeof(string));
			table.Columns.Add("OldValue", typeof(string));

			using(IDataReader reader = DbLdap.SyncLogGetFields(logId))
			{
				DataRow group = null;
				int lastUserId = -1;
				while(reader.Read())
				{
					int userId = (int)reader["UserId"];
					if(userId != lastUserId)
					{
						group = table.NewRow();
						group["UserId"] = userId;
						table.Rows.Add(group);
						lastUserId = userId;
					}

					string fieldName = reader["FieldName"].ToString();
					string newValue = reader["NewValue"].ToString();
					string oldValue = reader["OldValue"].ToString();

					if(newValue != oldValue)
					{
						DataRow row = table.NewRow();
						row["FieldName"] = fieldName;
						row["NewValue"] = newValue;
						row["OldValue"] = oldValue;
						table.Rows.Add(row);
					}

					string userName = group["UserName"].ToString();
					if(fieldName == "FirstName")
						group["UserName"] = string.Format("{0}{1}{2}", newValue, (userName != null && userName.Length > 0) ? " " : "", userName);
					else if(fieldName == "LastName")
						group["UserName"] = string.Format("{2}{1}{0}", newValue, (userName != null && userName.Length > 0) ? " " : "", userName);
				}
			}

			return table;
		}
		#endregion

		#region SyncLogDelete(int logId)
		public static void SyncLogDelete(int logId)
		{
			Ldap.CheckAccess();

			DbLdap.SyncLogDelete(logId);
		}
		#endregion

		#region SyncLogDelete(ArrayList list)
		public static void SyncLogDelete(ArrayList list)
		{
			Ldap.CheckAccess();

			using(DbTransaction tran = DbTransaction.Begin())
			{
				foreach(int logId in list)
					DbLdap.SyncLogDelete(logId);
				
				tran.Commit();
			}
		}
		#endregion

		#region SyncLogDeleteWithoutChanges()
		public static void SyncLogDeleteWithoutChanges()
		{
			Ldap.CheckAccess();

			DbLdap.SyncLogDeleteWithoutChanges();
		}
		#endregion

		// Internal
		#region Synchronize(int ldapId, DateTime dt)
		internal static void Synchronize(int ldapId, DateTime dt)
		{
			try
			{
				Synchronize(ldapId);
				UpdateNextSyncTime(ldapId);
			}
			catch(Exception ex)
			{
				Log.WriteError(ex.ToString());
			}
		}
		#endregion

		#region UpdateNextSyncTime(...)
		internal static void UpdateNextSyncTime(int ldapId)
		{
			try
			{
				LdapSettings ldap = LdapSettings.Load(ldapId);
				if(ldap.Autosync)
				{
					// Calculate next sync time
					DateTime now = ldap.LastSynchronization;
					if(now == DateTime.MinValue)
						now = DateTime.UtcNow;

					DateTime next = ldap.AutosyncStart;
					while(next < now)
						next = next.AddHours(ldap.AutosyncInterval);

					// Add next sync time to schedule
					Schedule.AddDateTypeValue(DateTypes.LdapSynchronization, ldapId, next);
				}
				else
					Schedule.DeleteDateTypeValue(DateTypes.LdapSynchronization, ldapId);
			}
			catch(Exception ex)
			{
				Log.WriteError(ex.ToString());
			}
		}
		#endregion

		#region GetPropertyValue
		internal static string GetPropertyValue(SearchResult result, string name)
		{
			string ret = string.Empty;

			if(name != null)
			{
				ResultPropertyValueCollection values = result.Properties[name];
				if(values != null && values.Count > 0)
				{
					object oVal = values[0];
					if(oVal != null)
					{
						if(oVal is byte[])
						{
							StringBuilder sb = new StringBuilder();
							foreach(byte b in (byte[])oVal)
								sb.Append(b.ToString("X2"));
							ret = sb.ToString();
						}
						else
							ret = oVal.ToString();
					}
				}
			}

			return ret;
		}
		#endregion

		// Private
		private Ldap(){}
	}
	#endregion
}
