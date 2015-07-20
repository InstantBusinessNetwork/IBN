using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Mediachase.Ibn.WebAsp
{
	public class DBTrialRequest
	{
		#region public static DataTable GetDataTable()
		/// <summary>
		/// DataTable returns fields:
		/// RequestID, CompanyName, Company_uid, SizeOfGroup, Description, Domain, FirstName, LastName, Email, 
		/// Phone, Country, Login, Password, ResellerId, ResellerTitle, XML, GUID, IsActive, 
		/// IsDeleted, CreationDate, Locale, Referrer
		/// </summary>
		public static DataTable GetDataTable()
		{
			return DBHelper.RunSPReturnDataTable("ASP_TRIAL_REQUEST_GET"
				, DBHelper.mp("@RequestID", SqlDbType.Int, 0)
				, DBHelper.mp("@IncludeInactive", SqlDbType.Bit, 1)
				, DBHelper.mp("@IncludeDeleted", SqlDbType.Bit, 0)
				);
		}
		#endregion

		#region public static IDataReader Get(int requestId)
		/// <summary>
		/// Reader returns fields:
		/// RequestID, CompanyName, [Description], Domain, FirstName,
		/// LastName, Email, Phone, Login, Password, IsActive, IsDeleted, XML, GUID, Locale
		/// </summary>
		public static IDataReader Get(int requestId)
		{
			return Get(requestId, true, false);
		}
		#endregion

		#region public static IDataReader Get(int requestId, bool includeInactive, bool includeDeleted)
		/// <summary>
		/// Reader returns fields:
		/// RequestID, CompanyName, Company_uid, SizeOfGroup, Description, Domain, FirstName, LastName, Email, 
		/// Phone, Country, Login, Password, ResellerId, ResellerTitle, XML, GUID, IsActive, 
		/// IsDeleted, CreationDate, Locale, Referrer
		/// </summary>
		public static IDataReader Get(int requestId, bool includeInactive, bool includeDeleted)
		{
			return DBHelper.RunSPReturnDataReader("ASP_TRIAL_REQUEST_GET"
				, DBHelper.mp("@RequestId", SqlDbType.Int, requestId)
				, DBHelper.mp("@IncludeInactive", SqlDbType.Bit, includeInactive)
				, DBHelper.mp("@IncludeDeleted", SqlDbType.Bit, includeDeleted)
				);
		}
		#endregion

		#region public static IDataReader GetByUser(string email)
		public static IDataReader GetByUser(string email)
		{
			return DBHelper.RunSPReturnDataReader("ASP_TRIAL_REQUEST_GET_BY_USER",
				DBHelper.mp("@EMail", SqlDbType.NVarChar, 100, email));
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
			, Guid resellerGuid
			, string xml
			, string locale
			, string referrer
			)
		{
			password = password.Trim();
			if (String.IsNullOrEmpty(password))
				throw new ArgumentException("Password can not be empty");

			object oXML = xml != null ? (object)xml : DBNull.Value;
			object oPhone = phone != null ? (object)phone : DBNull.Value;
			object oReferrer = referrer != null ? (object)referrer : DBNull.Value;

			return DBHelper.RunSPReturnInteger("ASP_TRIAL_REQUEST_ADD",
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
				DBHelper.mp("@ResellerGuid", SqlDbType.UniqueIdentifier, resellerGuid),
				DBHelper.mp("@XML", SqlDbType.NVarChar, 4000, oXML),
				DBHelper.mp("@Locale", SqlDbType.NVarChar, 10, locale)
				, DBHelper.mp("@Referrer", SqlDbType.NVarChar, 2048, oReferrer)
				);
		}
		#endregion

		#region public static bool CheckGuid(int requestId, Guid guid)
		public static bool CheckGuid(int requestId, Guid guid)
		{
			bool ret = false;
			if (requestId > 0)
			{
				int id = DBHelper.RunSPReturnInteger("ASP_TRIAL_REQUEST_GET_BY_GUID",
					DBHelper.mp("@guid", SqlDbType.UniqueIdentifier, guid));

				if (requestId == id)
					ret = true;
			}
			return ret;
		}
		#endregion

		#region public static bool IsActive(int requestId)
		public static bool IsActive(int requestId)
		{
			bool ret = false;
			using (IDataReader r = Get(requestId, true, false))
			{
				if (r.Read())
					ret = (bool)r["IsActive"];
			}
			return ret;
		}
		#endregion

		#region public static void SetActive(int requestId)
		public static void SetActive(int requestId, Guid company_uid)
		{
			DBHelper.RunSP("ASP_TRIAL_REQUEST_SET_ACTIVE",
				DBHelper.mp("@RequestId", SqlDbType.Int, requestId),
				DBHelper.mp("@Company_uid", SqlDbType.UniqueIdentifier, company_uid),
				DBHelper.mp("@IsActive", SqlDbType.Bit, true));
		}
		#endregion

		#region public static void Update(...)
		public static void Update(
			int requestId
			, string companyName
			, string description
			, string domain
			, string firstName
			, string lastName
			, string email
			, string phone
			, string country
			, string login
			, string password
			, string locale
			)
		{
			password = password.Trim();
			if (String.IsNullOrEmpty(password))
				throw new ArgumentException("Password can not be empty");

			object oPhone = phone != null ? (object)phone : DBNull.Value;

			DBHelper.RunSP("ASP_TRIAL_REQUEST_UPDATE",
				DBHelper.mp("@RequestID", SqlDbType.Int, requestId),
				DBHelper.mp("@CompanyName", SqlDbType.NVarChar, 100, companyName),
				DBHelper.mp("@Description", SqlDbType.NVarChar, 100, description),
				DBHelper.mp("@Domain", SqlDbType.NVarChar, 255, domain),
				DBHelper.mp("@FirstName", SqlDbType.NVarChar, 100, firstName),
				DBHelper.mp("@LastName", SqlDbType.NVarChar, 100, lastName),
				DBHelper.mp("@EMail", SqlDbType.NVarChar, 100, email),
				DBHelper.mp("@Phone", SqlDbType.NVarChar, 50, oPhone),
				DBHelper.mp("@Country", SqlDbType.NVarChar, 100, country),
				DBHelper.mp("@Login", SqlDbType.NVarChar, 50, login),
				DBHelper.mp("@Password", SqlDbType.NVarChar, 50, password),
				DBHelper.mp("@Locale", SqlDbType.NVarChar, 50, locale)
				);
		}
		#endregion

		#region public static void Delete(int requestId)
		public static void Delete(int requestId)
		{
			DBHelper.RunSP("ASP_TRIAL_REQUEST_DELETE",
				DBHelper.mp("@RequestID", SqlDbType.Int, requestId));
		}
		#endregion

		#region public static TemplateVariables GetVariables(int requestId)
		public static TemplateVariables GetVariables(int requestId)
		{
			TemplateVariables vars = new TemplateVariables();

			using (IDataReader reader = Get(requestId, true, true))
			{
				if (reader != null)
				{
					if (reader.Read())
					{
						vars["RequestID"] = requestId.ToString();
						vars["CompanyName"] = reader["CompanyName"].ToString();
						vars["SizeOfGroup"] = reader["SizeOfGroup"].ToString();
						vars["Description"] = reader["Description"].ToString();
						vars["Domain"] = reader["Domain"].ToString();
						vars["FirstName"] = reader["FirstName"].ToString();
						vars["LastName"] = reader["LastName"].ToString();
						vars["Email"] = reader["Email"].ToString();
						vars["Phone"] = reader["Phone"].ToString();
						vars["Country"] = reader["Country"].ToString();
						vars["Login"] = reader["Login"].ToString();
						vars["Password"] = reader["Password"].ToString();
						vars["ResellerTitle"] = reader["ResellerTitle"].ToString();
						vars["XML"] = reader["XML"].ToString();
						vars["RequestGUID"] = reader["GUID"].ToString();
						vars["Locale"] = reader["Locale"].ToString();
						vars["Referrer"] = reader["Referrer"].ToString();
					}
				}
			}
			return vars;
		}
		#endregion
	}
}
