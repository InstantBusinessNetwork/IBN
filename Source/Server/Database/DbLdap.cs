using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbLdap
	{
		private DbLdap(){}

		#region SettingsCreateUpdate()
		public static int SettingsCreateUpdate(
			int LdapId
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
			return DbHelper2.RunSpInteger("LdapSettingsCreateUpdate"
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				, DbHelper2.mp("@Title", SqlDbType.NVarChar, 255, Title)
				, DbHelper2.mp("@Domain", SqlDbType.NVarChar, 255, Domain)
				, DbHelper2.mp("@Username", SqlDbType.NVarChar, 255, Username)
				, DbHelper2.mp("@Password", SqlDbType.NVarChar, 255, Password)
				, DbHelper2.mp("@Filter", SqlDbType.NVarChar, 255, Filter)
				, DbHelper2.mp("@IbnKey", SqlDbType.NVarChar, 255, IbnKey)
				, DbHelper2.mp("@LdapKey", SqlDbType.NVarChar, 255, LdapKey)
				, DbHelper2.mp("@Activate", SqlDbType.Bit, Activate)
				, DbHelper2.mp("@Deactivate", SqlDbType.Bit, Deactivate)
				, DbHelper2.mp("@Autosync", SqlDbType.Bit, Autosync)
				, DbHelper2.mp("@AutosyncStart", SqlDbType.DateTime, AutosyncStart)
				, DbHelper2.mp("@AutosyncInterval", SqlDbType.Int, AutosyncInterval)
				);
		}
		#endregion
		
		#region SettingsDelete()
		public static void SettingsDelete(int LdapId)
		{
			DbHelper2.RunSp("LdapSettingsDelete"
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				);
		}
		#endregion

		#region SettingsGet()
		/// <summary>
		/// Returns fields: LdapId, Title, Domain, Username, Password, Filter, IbnKey, LdapKey, Activate, Deactivate, Autosync, AutosyncStart, AutosyncInterval, LastSynchronization.
		/// </summary>
		public static IDataReader SettingsGet(int LdapId)
		{
			return DbHelper2.RunSpDataReader("LdapSettingsGet"
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				);
		}
		#endregion

		#region SettingsUpdateLastSynchronization()
		public static void SettingsUpdateLastSynchronization(int LdapId, DateTime LastSynchronization)
		{
			DbHelper2.RunSp("LdapSettingsUpdateLastSynchronization"
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				, DbHelper2.mp("@LastSynchronization", SqlDbType.DateTime, LastSynchronization)
				);
		}
		#endregion

		
		#region FieldCreateUpdate()
		public static int FieldCreateUpdate(int FieldId
			, int LdapId
			, bool BitField
			, string IbnName
			, string LdapName
			, int BitMask
			, bool Equal
			, int CompareTo
			)
		{
			return DbHelper2.RunSpInteger("LdapFieldCreateUpdate"
				, DbHelper2.mp("@FieldId", SqlDbType.Int, FieldId)
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				, DbHelper2.mp("@BitField", SqlDbType.Bit, BitField)
				, DbHelper2.mp("@IbnName", SqlDbType.NVarChar, 255, IbnName)
				, DbHelper2.mp("@LdapName", SqlDbType.NVarChar, 255, LdapName)
				, DbHelper2.mp("@BitMask", SqlDbType.Int, BitMask)
				, DbHelper2.mp("@Equal", SqlDbType.Bit, Equal)
				, DbHelper2.mp("@CompareTo", SqlDbType.Int, CompareTo)
				);
		}
		#endregion

		#region FieldDelete()
		public static void FieldDelete(int FieldId)
		{
			DbHelper2.RunSp("LdapFieldDelete"
				, DbHelper2.mp("@FieldId", SqlDbType.Int, FieldId)
				);
		}
		#endregion

		#region FieldGet()
		public static IDataReader FieldGet(int FieldId)
		{
			return DbHelper2.RunSpDataReader("LdapFieldGet"
				, DbHelper2.mp("@FieldId", SqlDbType.Int, FieldId)
				);
		}
		#endregion

		#region FieldsGet()
		public static IDataReader FieldsGet(int LdapId)
		{
			return DbHelper2.RunSpDataReader("LdapFieldsGet"
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				);
		}
		#endregion

	
		#region SyncFieldCreate()
		public static void SyncFieldCreate(int LogId, int UserId, string FieldName, string OldValue, string NewValue)
		{
			DbHelper2.RunSp("LdapSyncFieldCreate"
				, DbHelper2.mp("@LogId", SqlDbType.Int, LogId)
				, DbHelper2.mp("@UserId", SqlDbType.Int, UserId)
				, DbHelper2.mp("@FieldName", SqlDbType.NVarChar, 255, FieldName)
				, DbHelper2.mp("@OldValue", SqlDbType.NVarChar, 255, OldValue)
				, DbHelper2.mp("@NewValue", SqlDbType.NVarChar, 255, NewValue)
				);
		}
		#endregion

		#region SyncLogCreate()
		public static int SyncLogCreate(int LdapId
			, DateTime Dt
			, int UserCount
			)
		{
			return DbHelper2.RunSpInteger("LdapSyncLogCreate"
				, DbHelper2.mp("@LdapId", SqlDbType.Int, LdapId)
				, DbHelper2.mp("@Dt", SqlDbType.DateTime, Dt)
				, DbHelper2.mp("@UserCount", SqlDbType.Int, UserCount)
				);
		}
		#endregion

		#region SyncLogGetList()
		/// <summary>
		/// Returns fields: LogId, LdapId, Dt, UserCount, Title.
		/// </summary>
		public static DataTable SyncLogGetList(int LogId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(TimeZoneId, new string[]{"Dt"}, "LdapSyncLogGetList"
				, DbHelper2.mp("@LogId", SqlDbType.Int, LogId)
				);
		}
		#endregion

		#region SyncLogDelete()
		public static void SyncLogDelete(int LogId)
		{
			DbHelper2.RunSp("LdapSyncLogDelete"
				, DbHelper2.mp("@LogId", SqlDbType.Int, LogId)
				);
		}
		#endregion

		#region SyncLogDeleteWithoutChanges()
		public static void SyncLogDeleteWithoutChanges()
		{
			DbHelper2.RunSp("LdapSyncLogDeleteWithoutChanges");
		}
		#endregion

		#region SyncLogGetFields()
		public static IDataReader SyncLogGetFields(int LogId)
		{
			return DbHelper2.RunSpDataReader("LdapSyncLogGetFields"
				, DbHelper2.mp("@LogId", SqlDbType.Int, LogId)
				);
		}
		#endregion
	}
}
