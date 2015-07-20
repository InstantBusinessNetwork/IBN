using System;
using System.Data;

namespace Mediachase.Ibn.WebAsp
{
	public class DBTrialRequestFailed
	{
		#region public static DataTable GetDataTable()
		/// <summary>
		/// DataTable returns fields:
		/// RequestID,ErrorCode,CompanyName,SizeOfGroup,[Description],Domain,
		/// FirstName,LastName,Email,Phone,Country,Login,Password,Locale,
		/// Referrer,ResellerId,ResellerTitle,CreationDate
		/// </summary>
		public static DataTable GetDataTable()
		{
			return DBHelper.RunSPReturnDataTable("ASP_FAILED_TRIAL_REQUEST_GET",
				DBHelper.mp("@RequestID", SqlDbType.Int, 0)
				);
		}
		#endregion

		#region public static int Create(...)
		public static int Create(
			string companyName
			, string sizeOfGroup
			, string description
			, string domain
			, string firstName
			, string lastName
			, string email
			, string phone
			, string country
			, string login
			, string password
			, object resellerGuid
			, string locale
			, string referrer
			, int errorCode
			)
		{
			object oPhone = phone != null ? (object)phone : DBNull.Value;
			object oResellerGuid = resellerGuid != null ? (object)resellerGuid : DBNull.Value;
			object oReferrer = referrer != null ? (object)referrer : DBNull.Value;

			return DBHelper.RunSPReturnInteger("ASP_FAILED_TRIAL_REQUEST_CREATE",
				DBHelper.mp("@CompanyName", SqlDbType.NVarChar, 100, companyName),
				DBHelper.mp("@SizeOfGroup", SqlDbType.NVarChar, 50, sizeOfGroup),
				DBHelper.mp("@Description", SqlDbType.NVarChar, 100, description),
				DBHelper.mp("@Domain", SqlDbType.NVarChar, 255, domain),
				DBHelper.mp("@FirstName", SqlDbType.NVarChar, 100, firstName),
				DBHelper.mp("@LastName", SqlDbType.NVarChar, 100, lastName),
				DBHelper.mp("@EMail", SqlDbType.NVarChar, 100, email),
				DBHelper.mp("@Phone", SqlDbType.NVarChar, 50, oPhone),
				DBHelper.mp("@Country", SqlDbType.NVarChar, 100, country),
				DBHelper.mp("@Login", SqlDbType.NVarChar, 50, login),
				DBHelper.mp("@Password", SqlDbType.NVarChar, 50, password),
				DBHelper.mp("@ResellerGuid", SqlDbType.UniqueIdentifier, oResellerGuid),
				DBHelper.mp("@Locale", SqlDbType.NVarChar, 10, locale)
				, DBHelper.mp("@ErrorCode", SqlDbType.Int, errorCode)
				, DBHelper.mp("@Referrer", SqlDbType.NVarChar, 2048, oReferrer)
				);
		}
		#endregion

		#region public static void Delete(int requestId)
		public static void Delete(int requestId)
		{
			DBHelper.RunSP("ASP_FAILED_TRIAL_REQUEST_DELETE",
				DBHelper.mp("@RequestID", SqlDbType.Int, requestId));
		}
		#endregion
	}
}
