using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Mediachase.Ibn.Data;

namespace Mediachase.IBN.Database
{
	#region class TimeZoneInformation
	internal class TimeZoneInformation
	{
		public int TimeZoneId;
		public int	Bias;
		public int StandardBias;
		public int DaylightBias;
		public int DaylightMonth;
		public int DaylightDayOfWeek;
		public int DaylightWeek;
		public int DaylightHour;
		public int StandardMonth;
		public int StandardDayOfWeek;
		public int StandardWeek;
		public int StandardHour;
	};
	#endregion

	/// <summary>
	/// Summary description for DBCommon.
	/// </summary>
	public class DBCommon
	{
		#region GetListCategories
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategories()
		{
			return DbHelper2.RunSpDataReader("CategoriesGet",
				DbHelper2.mp("@CategoryId", SqlDbType.Int, 0));
		}
		#endregion

		#region GetListCategoriesForDictionaries
		/// <summary>
		///		ItemId, ItemName, CanDelete
		/// </summary>
		public static DataTable GetListCategoriesForDictionaries()
		{
			return DbHelper2.RunSpDataTable("CategoriesGetForDictionaries");
		}
		#endregion

		#region GetListCategoriesByObject
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, CategoryName 
		/// </summary>
		public static IDataReader GetListCategoriesByObject(int ObjectTypeId, int ObjectId)
		{
			return DbHelper2.RunSpDataReader("CategoriesGetByObject",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region RemoveCategoryFromObject
		public static void RemoveCategoryFromObject(int ObjectTypeId, int ObjectId, int CategoryId)
		{
			DbHelper2.RunSp("ObjectCategoryDelete", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region AssignCategoryToObject
		public static void AssignCategoryToObject(int ObjectTypeId, int ObjectId, int CategoryId)
		{
			DbHelper2.RunSp("ObjectCategoryAdd", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region GetListPriorities
		/// <summary>
		/// Reader returns fields:
		///		PriorityId, PriorityName 
		/// </summary>
		public static IDataReader GetListPriorities(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("PrioritiesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListTimeZone
		/// <summary>
		///	TimeZoneId, Bias, StandardBias, DaylightBias, DaylightMonth, DaylightDayOfWeek, 
		///	DaylightWeek, DaylightHour, StandardMonth, StandardDayOfWeek, StandardWeek, 
		///	StandardHour, DisplayName, LanguageId
		/// </summary>
		public static IDataReader GetListTimeZone(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("TimeZonesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetTimeZone
		/// <summary>
		///	TimeZoneId, Bias, StandardBias, DaylightBias, DaylightMonth, DaylightDayOfWeek, 
		///	DaylightWeek, DaylightHour, StandardMonth, StandardDayOfWeek, StandardWeek, 
		///	StandardHour, DisplayName, LanguageId
		/// </summary>
		public static IDataReader GetTimeZone(int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("TimeZoneGet",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetTimeZoneName
		public static string GetTimeZoneName(int TimeZoneId, int LanguageId)
		{
			return (string)DbHelper2.RunSpScalar("TimeZoneGetName",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetTimeZoneByBias
		public static int GetTimeZoneByBias(int Bias)
		{
			return DbHelper2.RunSpInteger("TimeZoneGetByBias",
				DbHelper2.mp("@Bias", SqlDbType.Int, Bias));
		}
		#endregion

		#region GetTimeZoneBias
		public static int GetTimeZoneBias(int TimeZoneId)
		{
			return DbHelper2.RunSpInteger("TimeZoneGetBias",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId));
		}
		#endregion

		#region GetListLanguages
		/// <summary>
		/// Reader returns fields:
		///		LanguageId, Locale, FriendlyName 
		/// </summary>
		public static IDataReader GetListLanguages()
		{
			return DbHelper2.RunSpDataReader("LanguagesGet");
		}
		#endregion


		#region GetDiscussion
		/// <summary>
		/// Reader returns fields:
		///		DiscussionId, ObjectTypeId, ObjectId, CreatorId, CreationDate, Text, CreatorName
		/// </summary>
		public static IDataReader GetDiscussion(int DiscussionId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"CreationDate"},
				"DiscussionGet",
				DbHelper2.mp("@DiscussionId", SqlDbType.Int, DiscussionId));
		}
		#endregion

		#region AddDiscussion
		public static int AddDiscussion(
			int ObjectTypeId, int ObjectId, 
			int CreatorId, DateTime CreationDate,
			string Text)
		{
			return DbHelper2.RunSpInteger("DiscussionAdd", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@CreatorId", SqlDbType.Int, CreatorId),
				DbHelper2.mp("@CreationDate", SqlDbType.DateTime, CreationDate),
				DbHelper2.mp("@Text", SqlDbType.NText, Text));
		}
		#endregion

		#region UpdateDiscussion
		public static void UpdateDiscussion(
			int DiscussionId, string Text)
		{
			DbHelper2.RunSp("DiscussionUpdate", 
				DbHelper2.mp("@DiscussionId", SqlDbType.Int, DiscussionId),
				DbHelper2.mp("@Text", SqlDbType.NText, Text));
		}
		#endregion

		#region DeleteDiscussion
		public static void DeleteDiscussion(int DiscussionId)
		{
			DbHelper2.RunSp("DiscussionDelete", 
				DbHelper2.mp("@DiscussionId", SqlDbType.Int, DiscussionId));
		}
		#endregion

		#region GetListSharingByProUser
		/// <summary>
		/// Reader returns fields:
		///  UserId, FirstName, LastName, Email, Login, Level
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListSharingByProUser(int ProUserId)
		{
			return DbHelper2.RunSpDataReader("SharingGetByProUser",
				DbHelper2.mp("@ProUserId", SqlDbType.Int, ProUserId));
		}
		#endregion

		#region AddRecurrence
		public static void AddRecurrence(
			int ObjectTypeId, int ObjectId, 
			int StartTime, int EndTime,
			byte Pattern, byte SubPattern,
			byte Frequency, byte Weekdays,
			byte DayOfMonth, byte WeekNumber,
			byte MonthNumber, int EndAfter,
			int TimeZoneId)
		{
			DbHelper2.RunSp("RecurrenceAdd", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@StartTime", SqlDbType.Int, StartTime),
				DbHelper2.mp("@EndTime", SqlDbType.Int, EndTime),
				DbHelper2.mp("@Pattern", SqlDbType.TinyInt, Pattern),
				DbHelper2.mp("@SubPattern", SqlDbType.TinyInt, SubPattern),
				DbHelper2.mp("@Frequency", SqlDbType.TinyInt, Frequency),
				DbHelper2.mp("@Weekdays", SqlDbType.TinyInt, Weekdays),
				DbHelper2.mp("@DayOfMonth", SqlDbType.TinyInt, DayOfMonth),
				DbHelper2.mp("@WeekNumber", SqlDbType.TinyInt, WeekNumber),
				DbHelper2.mp("@MonthNumber", SqlDbType.TinyInt, MonthNumber),
				DbHelper2.mp("@EndAfter", SqlDbType.Int, EndAfter),
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId));
		}
		#endregion

		#region DeleteRecurrence
		public static void DeleteRecurrence(int ObjectTypeId, int ObjectId)
		{
			DbHelper2.RunSp("RecurrenceDelete", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region GetRecurrence
		/// <summary>
		/// Reader returns fields:
		///		RecurrenceId, ObjectTypeId, ObjectId, StartTime, EndTime, Pattern, SubPattern, 
		///		Frequency, Weekdays, DayOfMonth, WeekNumber, MonthNumber, EndAfter, TimeZoneId
		/// </summary>
		public static IDataReader GetRecurrence(int ObjectTypeId, int ObjectId)
		{
			int iObjectTypeId = (int)ObjectTypeId;
			return DbHelper2.RunSpDataReader("RecurrenceGet",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, iObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region AddCategory
		public static void AddCategory(string CategoryName)
		{
			DbHelper2.RunSp("CategoryAdd", 
				DbHelper2.mp("@CategoryName", SqlDbType.NVarChar, 50, CategoryName));
		}
		#endregion

		#region UpdateCategory
		public static void UpdateCategory(int CategoryId, string CategoryName)
		{
			DbHelper2.RunSp("CategoryUpdate", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@CategoryName", SqlDbType.NVarChar, 50, CategoryName));
		}
		#endregion

		#region DeleteCategory
		public static void DeleteCategory(int CategoryId)
		{
			DbHelper2.RunSp("CategoryDelete", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId));
		}
		#endregion

		#region GetDefaultLanguage
		public static int GetDefaultLanguage()
		{
			return DbHelper2.RunSpInteger("LanguageGetDefault");
		}
		#endregion

		#region CheckDiscussionsForUnchangeableRoles
		public static int CheckDiscussionsForUnchangeableRoles(int UserId)
		{
			return DbHelper2.RunSpInteger("DiscussionsCheckForUnchangeableRoles",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceDiscussionsUnchangeableUserToManager
		public static void ReplaceDiscussionsUnchangeableUserToManager(int UserId)
		{
			DbHelper2.RunSp("DiscussionsReplaceUnchangeableUserToManager", 
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region ReplaceDiscussionsUnchangeableUser
		public static void ReplaceDiscussionsUnchangeableUser(int FromUserId, int ToUserId)
		{
			DbHelper2.RunSp("DiscussionsReplaceUnchangeableUser", 
				DbHelper2.mp("@FromUserId", SqlDbType.Int, FromUserId),
				DbHelper2.mp("@ToUserId", SqlDbType.Int, ToUserId));
		}
		#endregion

		#region AddGate
		public static Guid AddGate(int objectTypeId, int objectId, int userId)
		{
			return AddGate(objectTypeId, objectId, userId, null);
		}
		public static Guid AddGate(int objectTypeId, int objectId, string email)
		{
			return AddGate(objectTypeId, objectId, -1, email);
		}
		private static Guid AddGate(int objectTypeId, int objectId, int userId, string email)
		{
			return DbHelper2.RunSpUniqueIdentifier("ExternalGateAdd", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId == -1 ? DBNull.Value : (object)userId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, email));
		}
		#endregion
		#region DeleteGate
		public static void DeleteGate(int objectTypeId, int objectId, int userId)
		{
			DeleteGate(objectTypeId, objectId, userId, null);
		}
		public static void DeleteGate(int objectTypeId, int objectId, string email)
		{
			DeleteGate(objectTypeId, objectId, -1, email);
		}
		private static void DeleteGate(int objectTypeId, int objectId, int userId, string email)
		{
			DbHelper2.RunSp("ExternalGateDelete", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId == -1 ? DBNull.Value : (object)userId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, email));
		}
		#endregion
		#region GetGateGuid
		public static string GetGateGuid(int objectTypeId, int objectId, int userId)
		{
			return GetGateGuid(objectTypeId, objectId, userId, null);
		}
		public static string GetGateGuid(int objectTypeId, int objectId, string email)
		{
			return GetGateGuid(objectTypeId, objectId, -1, email);
		}
		private static string GetGateGuid(int objectTypeId, int objectId, int userId, string email)
		{
			string ret = "";
			object o = DbHelper2.RunSpScalar("ExternalGateGetGuid",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, objectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId == -1 ? DBNull.Value : (object)userId),
				DbHelper2.mp("@Email", SqlDbType.NVarChar, 250, email));
			if(o != null)
				ret = o.ToString();
			return ret;
		}
		#endregion
		#region GetGateByGuid
		public static IDataReader GetGateByGuid(string guid)
		{
			return DbHelper2.RunSpDataReader("ExternalGateGetByGuid", 
				DbHelper2.mp("@Guid", SqlDbType.UniqueIdentifier, new Guid(guid)));
		}
		#endregion
        #region RemoveGateExpired
        public static void RemoveGateExpired(int objectTypeId, int expirateValue)
        {
            DbHelper2.RunSp("ExternalGateRemoveExpired",
                DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, objectTypeId),
                DbHelper2.mp("@ExpirateValue", SqlDbType.Int, expirateValue));
        }
        #endregion
        #region RefreshGate
        public static void RefreshGate(Guid guid)
        {
            DbHelper2.RunSp("ExternalGateRefresh",
               DbHelper2.mp("@guid", SqlDbType.UniqueIdentifier, guid));
               
        }
        #endregion

		#region GetListCurrency
		/// <summary>
		/// Reader returns fields:
		///		CurrencyId, CurrencySymbol
		/// </summary>
		public static IDataReader GetListCurrency(int CurrencyId)
		{
			return DbHelper2.RunSpDataReader("CurrencyGet",
				DbHelper2.mp("@CurrencyId", SqlDbType.Int, CurrencyId));
		}
		#endregion

		#region GetListCurrencyForDictionaries
		/// <summary>
		///		ItemId, ItemName, CanDelete
		/// </summary>
		public static DataTable GetListCurrencyForDictionaries()
		{
			return DbHelper2.RunSpDataTable("CurrencyGetForDictionaries");
		}
		#endregion

		#region AddCurrency
		public static void AddCurrency(string CurrencySymbol)
		{
			DbHelper2.RunSp("CurrencyAdd", 
				DbHelper2.mp("@CurrencySymbol", SqlDbType.NVarChar, 10, CurrencySymbol));
		}
		#endregion

		#region UpdateCurrency
		public static void UpdateCurrency(int CurrencyId, string CurrencySymbol)
		{
			DbHelper2.RunSp("CurrencyUpdate", 
				DbHelper2.mp("@CurrencyId", SqlDbType.Int, CurrencyId),
				DbHelper2.mp("@CurrencySymbol", SqlDbType.NVarChar, 10, CurrencySymbol));
		}
		#endregion

		#region DeleteCurrency
		public static void DeleteCurrency(int CurrencyId)
		{
			DbHelper2.RunSp("CurrencyDelete", 
				DbHelper2.mp("@CurrencyId", SqlDbType.Int, CurrencyId));
		}
		#endregion

		#region AddCategoryUser
		public static void AddCategoryUser(int CategoryId, int UserId)
		{
			DbHelper2.RunSp("CategoryUserAdd", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteCategoryUser
		public static void DeleteCategoryUser(int CategoryId, int UserId)
		{
			DbHelper2.RunSp("CategoryUserDelete", 
				DbHelper2.mp("@CategoryId", SqlDbType.Int, CategoryId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListCategoriesByUser
		/// <summary>
		/// Reader returns fields:
		///		CategoryId, UserId
		/// </summary>
		public static IDataReader GetListCategoriesByUser(int UserId)
		{
			return DbHelper2.RunSpDataReader("CategoryUserGetList",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetSqlServerVersion
		public static string GetSqlServerVersion()
		{
			return (string)DbHelper2.RunTextScalar("SELECT SERVERPROPERTY('productversion')");
		}
		#endregion
		#region GetSqlServerEdition
		public static int GetSqlServerEdition()
		{
			int retVal = 0;
			object oRetVal = DbHelper2.RunTextScalar("SELECT SERVERPROPERTY('EngineEdition')");
			if (oRetVal != null && oRetVal != DBNull.Value)
				retVal = (int)oRetVal;
			return retVal;
		}
		#endregion

		#region GetLocalDate
		public static DateTime GetLocalDate(int TimeZoneId, DateTime UTCDate)
		{
			return DbHelper2.RunSpDateTime("LocalDateGet",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@UTCDate", SqlDbType.DateTime, UTCDate));
		}
		#endregion

		#region GetUTCDate
		public static DateTime GetUTCDate(int TimeZoneId, DateTime LocalDate)
		{
			return DbHelper2.RunSpDateTime("UTCDateGet",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@LocalDate", SqlDbType.DateTime, LocalDate));
		}
		#endregion

		#region AddTimeZoneToLocalUtc
		public static int AddTimeToLocalUtc(int TimeZoneId, DateTime Start, DateTime End, int TimeOffset, bool IsDayLight)
		{
			return DbHelper2.RunSpInteger("TimeZoneAddLocalUtc",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Start", SqlDbType.DateTime, Start),
				DbHelper2.mp("@End", SqlDbType.DateTime, End),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, TimeOffset),
				DbHelper2.mp("@IsDayLight", SqlDbType.Bit, IsDayLight)
				);
		}
		#endregion

		#region AddTimeZoneToLocalUtc
		public static int AddTimeToUtcLocal(int TimeZoneId, DateTime Start, DateTime End, int TimeOffset, bool IsDayLight)
		{
			return DbHelper2.RunSpInteger("TimeZoneAddUtcLocal",
				DbHelper2.mp("@TimeZoneId", SqlDbType.Int, TimeZoneId),
				DbHelper2.mp("@Start", SqlDbType.DateTime, Start),
				DbHelper2.mp("@End", SqlDbType.DateTime, End),
				DbHelper2.mp("@TimeOffset", SqlDbType.Int, TimeOffset),
				DbHelper2.mp("@IsDayLight", SqlDbType.Bit, IsDayLight)
				);
		}
		#endregion

		#region TransformDate
		internal static DateTime	TransformDate(DateTime date, int Week, DayOfWeek DayOfWeek)
		{
			DateTime retVal = date;
			int tmpMonth = date.Month;
			while(Week>0&&date.Month==tmpMonth)
			{
				if(date.DayOfWeek==DayOfWeek)
				{
					retVal = date;
					Week--;
				}
				date = date.AddDays(1);
			}
			return retVal;
		}
		#endregion

		#region FillTimeZonesLocalUtcTable
		public static void FillTimeZonesLocalUtcTable(int FromYear, int Durations)
		{
			ArrayList	TimeZones	=	new ArrayList();

			using(IDataReader reader = DBCommon.GetListTimeZone(DBCommon.GetDefaultLanguage()))
			{
				
				while(reader.Read())
				{
					TimeZoneInformation	tmz	=	new TimeZoneInformation();

					tmz.TimeZoneId	= (int)reader["TimeZoneId"];
					tmz.Bias		=	(int)reader["Bias"];
					tmz.StandardBias=	(int)reader["StandardBias"];
					tmz.DaylightBias=	(int)reader["DaylightBias"];
					tmz.DaylightMonth=	(int)reader["DaylightMonth"];
					tmz.DaylightDayOfWeek=	(int)reader["DaylightDayOfWeek"];
					tmz.DaylightWeek=	(int)reader["DaylightWeek"];
					tmz.DaylightHour=	(int)reader["DaylightHour"];
					tmz.StandardMonth =	(int)reader["StandardMonth"];
					tmz.StandardDayOfWeek =	(int)reader["StandardDayOfWeek"];
					tmz.StandardWeek =	(int)reader["StandardWeek"];
					tmz.StandardHour =	(int)reader["StandardHour"];

					TimeZones.Add(tmz);
				}
			}

			foreach(TimeZoneInformation tmz in TimeZones)
			{
				if(tmz.DaylightMonth==0||tmz.StandardMonth==0)
				{
					// Add Standard interval [6/18/2004]
					DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,new DateTime(FromYear,1,1,0,0,0,0),new DateTime(FromYear+Durations,1,1,0,0,0,0),tmz.Bias,false);
				}
				else
				{
					DateTime lastEnd = new DateTime(FromYear,1,1,0,0,0,0);

					for(int YearItem = FromYear; YearItem<(FromYear+Durations);YearItem++)
					{
						if(tmz.DaylightMonth<tmz.StandardMonth)
						{
							DateTime startDaylight = new DateTime(YearItem,tmz.DaylightMonth,1,tmz.DaylightHour-(tmz.StandardBias+tmz.DaylightBias)/60,0,0,0);
							DateTime endDaylight = new DateTime(YearItem,tmz.StandardMonth,1,tmz.StandardHour,0,0,0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight,tmz.DaylightWeek,(DayOfWeek)tmz.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight,tmz.StandardWeek,(DayOfWeek)tmz.StandardDayOfWeek);

							// Add Standard interval [6/18/2004]
							DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,lastEnd,startDaylight,(tmz.Bias+tmz.StandardBias),false);

							// Add Daylight interval [6/18/2004]
							DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,startDaylight,endDaylight,(tmz.Bias+tmz.DaylightBias),true);

							lastEnd = endDaylight;
						}
						else
						{
							DateTime startStandard = new DateTime(YearItem,tmz.StandardMonth,1,tmz.StandardHour,0,0,0);
							DateTime endStandard = new DateTime(YearItem,tmz.DaylightMonth,1,tmz.DaylightHour-(tmz.StandardBias+tmz.DaylightBias)/60,0,0,0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard,tmz.StandardWeek,(DayOfWeek)tmz.StandardDayOfWeek);
							endStandard = TransformDate(endStandard,tmz.DaylightWeek,(DayOfWeek)tmz.DaylightDayOfWeek);

							// Add Standard interval [6/18/2004]
							DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,lastEnd,startStandard,(tmz.Bias+tmz.DaylightBias),true);

							// Add Daylight interval [6/18/2004]
							DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,startStandard,endStandard,(tmz.Bias+tmz.StandardBias),false);

							lastEnd = endStandard;
						}
					}
					// Add Final Interval [6/18/2004]
					// Add Standard interval [6/18/2004]
					if(tmz.DaylightMonth<tmz.StandardMonth)
						DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,lastEnd,new DateTime(FromYear+Durations,1,1,0,0,0,0),(tmz.Bias+tmz.StandardBias),false);
					else
						DBCommon.AddTimeToLocalUtc(tmz.TimeZoneId,lastEnd,new DateTime(FromYear+Durations,1,1,0,0,0,0),(tmz.Bias+tmz.DaylightBias),false);
				}
			}
		}
		#endregion

		#region FillTimeZonesUtcLocalTable
		public static void FillTimeZonesUtcLocalTable(int FromYear, int Durations)
		{
			ArrayList	TimeZones	=	new ArrayList();

			using(IDataReader reader = DBCommon.GetListTimeZone(DBCommon.GetDefaultLanguage()))
			{
				while(reader.Read())
				{
					TimeZoneInformation	tmz	=	new TimeZoneInformation();

					tmz.TimeZoneId	= (int)reader["TimeZoneId"];
					tmz.Bias		=	(int)reader["Bias"];
					tmz.StandardBias=	(int)reader["StandardBias"];
					tmz.DaylightBias=	(int)reader["DaylightBias"];
					tmz.DaylightMonth=	(int)reader["DaylightMonth"];
					tmz.DaylightDayOfWeek=	(int)reader["DaylightDayOfWeek"];
					tmz.DaylightWeek=	(int)reader["DaylightWeek"];
					tmz.DaylightHour=	(int)reader["DaylightHour"];
					tmz.StandardMonth =	(int)reader["StandardMonth"];
					tmz.StandardDayOfWeek =	(int)reader["StandardDayOfWeek"];
					tmz.StandardWeek =	(int)reader["StandardWeek"];
					tmz.StandardHour =	(int)reader["StandardHour"];

					TimeZones.Add(tmz);
				}
			}

			foreach(TimeZoneInformation tmz in TimeZones)
			{
				if(tmz.DaylightMonth==0||tmz.StandardMonth==0)
				{
					// Add Standard interval [6/18/2004]
					DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,new DateTime(FromYear,1,1,0,0,0,0),new DateTime(FromYear+Durations,1,1,0,0,0,0),-tmz.Bias,false);
				}
				else
				{
					DateTime lastEnd = new DateTime(FromYear,1,1,0,0,0,0);

					for(int YearItem = FromYear; YearItem<(FromYear+Durations);YearItem++)
					{
						if(tmz.DaylightMonth<tmz.StandardMonth)
						{
							DateTime startDaylight = new DateTime(YearItem,tmz.DaylightMonth,1,tmz.DaylightHour/*-(tmz.StandardBias+tmz.DaylightBias)/60*/,0,0,0);
							DateTime endDaylight = new DateTime(YearItem,tmz.StandardMonth,1,tmz.StandardHour,0,0,0);

							// Calculate Real Day  [6/18/2004]
							startDaylight = TransformDate(startDaylight,tmz.DaylightWeek,(DayOfWeek)tmz.DaylightDayOfWeek);
							endDaylight = TransformDate(endDaylight,tmz.StandardWeek,(DayOfWeek)tmz.StandardDayOfWeek);

							startDaylight = startDaylight.AddMinutes((tmz.Bias+tmz.StandardBias));
							endDaylight = endDaylight.AddMinutes((tmz.Bias+tmz.StandardBias));

							// Add Standard interval [6/18/2004]
							DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,lastEnd,startDaylight,-(tmz.Bias+tmz.StandardBias),false);

							// Add Daylight interval [6/18/2004]
							DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,startDaylight,endDaylight,-(tmz.Bias+tmz.DaylightBias),true);

							lastEnd = endDaylight;
						}
						else
						{
							DateTime startStandard = new DateTime(YearItem,tmz.StandardMonth,1,tmz.StandardHour,0,0,0);
							DateTime endStandard = new DateTime(YearItem,tmz.DaylightMonth,1,tmz.DaylightHour/*-(tmz.StandardBias+tmz.DaylightBias)/60*/,0,0,0);

							// Calculate Real Day  [6/18/2004]
							startStandard = TransformDate(startStandard,tmz.StandardWeek,(DayOfWeek)tmz.StandardDayOfWeek);
							endStandard = TransformDate(endStandard,tmz.DaylightWeek,(DayOfWeek)tmz.DaylightDayOfWeek);

							startStandard = startStandard.AddMinutes((tmz.Bias+tmz.StandardBias));
							endStandard = endStandard.AddMinutes((tmz.Bias+tmz.StandardBias));

							// Add Standard interval [6/18/2004]
							DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,lastEnd,startStandard,-(tmz.Bias+tmz.DaylightBias),true);

							// Add Daylight interval [6/18/2004]
							DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,startStandard,endStandard,-(tmz.Bias+tmz.StandardBias),false);

							lastEnd = endStandard;
						}
					}
					// Add Final Interval [6/18/2004]
					// Add Standard interval [6/18/2004]
					if(tmz.DaylightMonth<tmz.StandardMonth)
						DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,lastEnd,new DateTime(FromYear+Durations,1,1,0,0,0,0),-(tmz.Bias+tmz.StandardBias),false);
					else
						DBCommon.AddTimeToUtcLocal(tmz.TimeZoneId,lastEnd,new DateTime(FromYear+Durations,1,1,0,0,0,0),-(tmz.Bias+tmz.DaylightBias),false);
				}
			}
		}
		#endregion

		#region GetLanguageByLocale
		public static string GetLanguageByLocale(string Locale)
		{
			string ret = "";
			object o = DbHelper2.RunSpScalar("LanguageGetByLocale",
				DbHelper2.mp("@Locale", SqlDbType.VarChar, 50, Locale));
			if(o != null)
				ret = o.ToString();
			return ret;
		}
		#endregion

		#region GetDefaultLanguageName
		public static string GetDefaultLanguageName()
		{
			using(IDataReader rdr = DbHelper2.RunSpDataReader("LanguageLocaleNameDefaultGet"))
			{
				if (rdr.Read())
					return (string)rdr["Locale"];
				else return "en-US";
			}
		}
		#endregion

		#region AddFavorites
		public static void AddFavorites(int ObjectTypeId, int ObjectId, int UserId)
		{
			DbHelper2.RunSp("FavoritesAdd", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region AddFavoritesByUid
		public static void AddFavoritesByUid(int ObjectTypeId, Guid ObjectUid, int UserId)
		{
			DbHelper2.RunSp("FavoritesAddByUid",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, ObjectUid),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteFavorites
		public static void DeleteFavorites(int ObjectTypeId, int ObjectId, int UserId)
		{
			DbHelper2.RunSp("FavoritesDelete", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteFavoritesByUid
		public static void DeleteFavoritesByUid(int ObjectTypeId, Guid objectUid, int UserId)
		{
			DbHelper2.RunSp("FavoritesDeleteByUid",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, objectUid),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListFavorites
		/// <summary>
		///		FavoriteId, ObjectTypeId, ObjectId, ObjectUid, UserId, Title 
		/// </summary>
		public static IDataReader GetListFavorites(int ObjectTypeId, int UserId)
		{
			return DbHelper2.RunSpDataReader("FavoritesGet",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}

		public static DataTable GetListFavoritesDT(int ObjectTypeId, int UserId)
		{
			return DbHelper2.RunSpDataTable("FavoritesGet",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region CheckFavorites
		public static bool CheckFavorites(int ObjectTypeId, int ObjectId, int UserId)
		{
			int RetVal = DbHelper2.RunSpInteger("FavoritesCheck", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
			return (RetVal == 1) ? true : false;
		}
		#endregion

		#region CheckFavoritesByUid
		public static bool CheckFavoritesByUid(int ObjectTypeId, Guid ObjectUid, int UserId)
		{
			int RetVal = DbHelper2.RunSpInteger("FavoritesCheckByUid",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectUid", SqlDbType.UniqueIdentifier, ObjectUid),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
			return (RetVal == 1) ? true : false;
		}
		#endregion

		#region AddHistory
		public static void AddHistory(int ObjectTypeId, int ObjectId, string ObjectTitle, int UserId)
		{
			DbHelper2.RunSp("HistoryAdd", 
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectTitle", SqlDbType.NVarChar, 250, ObjectTitle),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IsView", SqlDbType.Bit, 1));
		}
		#endregion

		#region DeleteHistory
		public static void DeleteHistory(int ObjectTypeId, int ObjectId)
		{
			DbHelper2.RunSp("HistoryDeleteByObject",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region AddUsedHistory
		public static void AddUsedHistory(int ObjectTypeId, int ObjectId, string ObjectTitle, int UserId)
		{
			DbHelper2.RunSp("HistoryAdd",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, (int)ObjectTypeId),
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectTitle", SqlDbType.NVarChar, 250, ObjectTitle),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@IsView", SqlDbType.Bit, 0));
		}
		#endregion

		#region GetListHistoryDT
		/// <summary>
		///		HistoryId, ObjectTypeId, ObjectId, ObjectTitle, Dt, ClassName, ObjectUid, ObjectCode
		/// </summary>
		public static DataTable GetListHistoryDT(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"Dt"},
				"HistoryGet",
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region GetListHistoryByObjectDT
		/// <summary>
		///		UserId, Dt
		/// </summary>
		public static DataTable GetListHistoryByObjectDT(int ObjectId, int ObjectTypeId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"Dt"},
				"HistoryGetByObject",
				DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId));
		}
		#endregion

		#region GetListHistoryFull
    /// <summary>
    /// HistoryId, ObjectTypeId, ObjectId, ObjectTitle, Dt, IsView
    /// </summary>
    /// <param name="UserId">The user id.</param>
    /// <param name="TimeZoneId">The timezone id.</param>
    /// <returns>DataTable</returns>
    public static DataTable GetListHistoryFull(int UserId, int TimeZoneId)
    {
      return DbHelper2.RunSpDataTable(
        TimeZoneId, new string[] { "Dt" },
        "HistoryGetFull",
        DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
    }
    #endregion

		#region GetTabsByObjectType
		/// <summary>
		///	TabId, TemplateId, IsDefault, RowNum, PosNum, Title
		/// </summary>
		public static IDataReader GetTabsByObjectType(int ObjectTypeId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("ObjectViewTabsGetByObjectTypeId",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetPageViewByObjectType
		/// <summary>
		///	ViewPageId, MenuId, ShortInfoPath, Title, MenuXML
		/// </summary>
		public static IDataReader GetPageViewByObjectType(int ObjectTypeId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader("PageViewGetByObjectTypeId",
				DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListRiskLevels
		/// <summary>
		///		RiskLevelId, RiskLevelName, Weight
		/// </summary>
		public static IDataReader GetListRiskLevels()
		{
			return DbHelper2.RunSpDataReader("RiskLevelsGet");
		}
		#endregion

		#region GetListDeclinedRequests
		/// <summary>
		///		ObjectId, ObjectTypeId, Title, PriorityId, PriorityName, LastSavedDate, UserId, FirstName, LastName
		/// </summary>
		public static IDataReader GetListDeclinedRequests(int ProjectId, int UserId, int LanguageId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"LastSavedDate"},
				"DeclinedRequestsGet",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@UserId", SqlDbType.Int, UserId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region IsPop3Folder
		public static bool IsPop3Folder(int DirectoryId)
		{
			int iRetval =  DbHelper2.RunSpInteger("DirectoryIsAssignedByPop3MessageHandler",
				DbHelper2.mp("@DirectoryId", SqlDbType.Int, DirectoryId));
			return (iRetval>0)? true : false;
		}
		#endregion

		#region NullToObject()
		public static object NullToObject(object val, object defaultVal)
		{
			if(val != null && val != DBNull.Value)
				return val;
			else
				return defaultVal;
		}
		#endregion
		#region NullToDateTime()
		public static DateTime NullToDateTime(object val)
		{
			return (DateTime)NullToObject(val, DateTime.MinValue);
		}
		#endregion
		#region NullToInt32()
		public static Int32 NullToInt32(object val)
		{
			return (Int32)NullToObject(val, -1);
		}
		#endregion

		#region GetListObjectStates
		/// <summary>
		/// Reader returns fields:
		///		StateId, StateName 
		/// </summary>
		public static IDataReader GetListObjectStates(int LanguageId)
		{
			return DbHelper2.RunSpDataReader("ObjectStatesGet",
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

		#region GetListArticlesUsedByUser
		/// <summary>
    ///		ArticleId, Question, Answer, AnswerHTML
    /// </summary>
    public static DataTable GetListArticlesUsedByUser(int UserId)
    {
      return DbHelper2.RunSpDataTable("ArticlesGetByUser",
        DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
    }
    #endregion

		#region GetListTagsForCloud
    /// <summary>
    /// Gets the list of tags for cloud.
    /// </summary>
    /// <param name="ObjectTypeId">The object type id. Typically 20 - Articles</param>
    /// <param name="TagSizeCount">Quantity steps for tag size</param>
    /// <param name="TagCount">Max record count to return</param>
    /// <returns>DataTable with columns: Tag, TagSize</returns>
    public static DataTable GetListTagsForCloud(int ObjectTypeId, int TagSizeCount, int TagCount)
    {
      return DbHelper2.RunSpDataTable("TagsGetForCloud",
        DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
        DbHelper2.mp("@TagSizeCount", SqlDbType.Int, TagSizeCount),
        DbHelper2.mp("@TagCount", SqlDbType.Int, TagCount));
    }
    #endregion

		#region GetListTagsForCloudByTag
    /// <summary>
    /// Gets the list of tags for cloud by tag.
    /// </summary>
    /// <param name="ObjectTypeId">The object type id. Typically 20 - Articles</param>
    /// <param name="TagSizeCount">Quantity steps for tag size</param>
    /// <param name="TagCount">Max record count to return</param>
    /// <param name="Tag">Related tag</param>
    /// <returns>DataTable with columns: Tag, TagSize</returns>
    public static DataTable GetListTagsForCloudByTag(int ObjectTypeId, int TagSizeCount, int TagCount, string Tag)
    {
      return DbHelper2.RunSpDataTable("TagsGetForCloudByTag",
        DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
        DbHelper2.mp("@TagSizeCount", SqlDbType.Int, TagSizeCount),
        DbHelper2.mp("@TagCount", SqlDbType.Int, TagCount),
        DbHelper2.mp("@Tag", SqlDbType.NVarChar, 50, Tag));
    }
    #endregion

		#region GetArticle
    /// <summary>
    ///		ArticleId, Question, Answer, AnswerHTML, Tags, Created, Counter, Delimiter
    /// </summary>
    public static IDataReader GetArticle(int ArticleId, int TimeZoneId)
    {
      return DbHelper2.RunSpDataReader(
        TimeZoneId, new string[] { "Created" },
        "ArticlesGet",
        DbHelper2.mp("@ArticleId", SqlDbType.Int, ArticleId));
    }

    public static DataTable GetArticleDT(int ArticleId, int TimeZoneId)
    {
      return DbHelper2.RunSpDataTable(
        TimeZoneId, new string[] { "Created" },
        "ArticlesGet",
        DbHelper2.mp("@ArticleId", SqlDbType.Int, ArticleId));
    }
    #endregion

		#region AddArticle
		public static int AddArticle(string Question, string Answer, string AnswerHTML, string Tags, string Delimiter)
		{
		  return DbHelper2.RunSpInteger("ArticlesAdd",
			DbHelper2.mp("@Question", SqlDbType.NVarChar, 1000, Question),
			DbHelper2.mp("@Answer", SqlDbType.NText, Answer),
			DbHelper2.mp("@AnswerHTML", SqlDbType.NText, AnswerHTML),
			DbHelper2.mp("@Tags", SqlDbType.NVarChar, 1000, Tags),
			DbHelper2.mp("@Delimiter", SqlDbType.NChar, 1, Delimiter));
		}
		#endregion

		#region UpdateArticle
		public static void UpdateArticle(int ArticleId, string Question, string Answer, string AnswerHTML, string Tags, string Delimiter)
		{
		  DbHelper2.RunSp("ArticlesUpdate",
			DbHelper2.mp("@ArticleId", SqlDbType.Int, ArticleId),
			DbHelper2.mp("@Question", SqlDbType.NVarChar, 1000, Question),
			DbHelper2.mp("@Answer", SqlDbType.NText, Answer),
			DbHelper2.mp("@AnswerHTML", SqlDbType.NText, AnswerHTML),
			DbHelper2.mp("@Tags", SqlDbType.NVarChar, 1000, Tags),
			DbHelper2.mp("@Delimiter", SqlDbType.NChar, 1, Delimiter));
		}
		#endregion

		#region DeleteArticle
		public static void DeleteArticle(int ArticleId)
		{
		  DbHelper2.RunSp("ArticlesDelete",
			DbHelper2.mp("@ArticleId", SqlDbType.Int, ArticleId));
		}
		#endregion

		#region IncreaseArticleCounter
		public static void IncreaseArticleCounter(int ArticleId)
		{
		  DbHelper2.RunSp("ArticlesIncreaseCounter",
			DbHelper2.mp("@ArticleId", SqlDbType.Int, ArticleId));
		}
		#endregion

		#region DeleteTagsByObject
		public static void DeleteTagsByObject(int ObjectTypeId, int ObjectId)
		{
		  DbHelper2.RunSp("TagsDeleteByObject",
			DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
			DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId));
		}
		#endregion

		#region AddTag
		public static int AddTag(int ObjectTypeId, int ObjectId, string Tag)
		{
		  return DbHelper2.RunSpInteger("TagsAdd",
			DbHelper2.mp("@ObjectTypeId", SqlDbType.Int, ObjectTypeId),
			DbHelper2.mp("@ObjectId", SqlDbType.Int, ObjectId),
			DbHelper2.mp("@Tag", SqlDbType.NVarChar, 50, Tag));
		}
		#endregion

		#region ReplaceSqlWildcard
		/// <summary>
		/// Replaces the SQL wildcard chars.
		/// </summary>
		/// <param name="sourceString">The source string to replace.</param>
		/// <returns></returns>
		public static string ReplaceSqlWildcard(string sourceString)
		{
			return Regex.Replace(sourceString, @"(\[|%|_)", "[$0]", RegexOptions.IgnoreCase);
		}
		#endregion

		#region AddEntityHistory
		public static void AddEntityHistory(string className, PrimaryKeyId objectId, string objectTitle, int userId, bool isView)
		{
			DbHelper2.RunSp("HistoryEntityAdd",
				DbHelper2.mp("@ClassName", SqlDbType.NVarChar, 250, className),
				DbHelper2.mp("@ObjectId", SqlDbType.VarChar, 36, objectId.ToString()),
				DbHelper2.mp("@ObjectTitle", SqlDbType.NVarChar, 250, objectTitle),
				DbHelper2.mp("@UserId", SqlDbType.Int, userId),
				DbHelper2.mp("@IsView", SqlDbType.Bit, isView));
		}
		#endregion

		#region GetListEntityHistoryFull
		/// <summary>
		/// HistoryId, ClassName, ObjectId, ObjectTitle, Dt, IsView
		/// </summary>
		/// <param name="UserId">The user id.</param>
		/// <param name="TimeZoneId">The timezone id.</param>
		/// <returns>DataTable</returns>
		public static DataTable GetListEntityHistoryFull(int UserId, int TimeZoneId)
		{
			return DbHelper2.RunSpDataTable(
			  TimeZoneId, new string[] { "Dt" },
			  "HistoryEntityGetFull",
			  DbHelper2.mp("@UserId", SqlDbType.Int, UserId));
		}
		#endregion

		#region DeleteEntityHistory
		public static void DeleteEntityHistory(string className, PrimaryKeyId objectId)
		{
			DbHelper2.RunSp("HistoryEntityDelete",
				DbHelper2.mp("@ClassName", SqlDbType.NVarChar, 250, className),
				DbHelper2.mp("@ObjectId", SqlDbType.VarChar, 36, objectId.ToString()));
		}
		#endregion

		#region public static int GetDatabaseState()
		public static int GetDatabaseState()
		{
			int result = 0;

			object value = DbHelper2.RunTextScalar("SELECT [State] FROM [DatabaseVersion] WITH (NOLOCK)");
			if (value != null && value != DBNull.Value)
				result = (int)value;

			return result;
		}
		#endregion
	}
}
