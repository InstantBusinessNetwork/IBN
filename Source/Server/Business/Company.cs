using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;

using Mediachase.Ibn;
using Mediachase.IBN.Database;


namespace Mediachase.IBN.Business
{
	/// <summary>
	/// Summary description for Company.
	/// </summary>
	public class Company
	{
		#region UpdateCompanyInfo
		public static void UpdateCompanyInfo(string supportName, string supportEmail
			, string title1, string title2, string text1, string text2
			, bool resetCompanyLogo, byte[] companyLogo
			, bool resetLogonPageLogo, byte[] logonPageLogo
			)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				PortalConfig.PortalSupportName = supportName;
				PortalConfig.PortalSupportEmail = supportEmail;
				PortalConfig.PortalHomepageTitle1 = title1;
				PortalConfig.PortalHomepageTitle2 = title2;
				PortalConfig.PortalHomepageText1 = text1;
				PortalConfig.PortalHomepageText2 = text2;

				if (resetCompanyLogo || companyLogo != null)
					PortalConfig.PortalCompanyLogo = companyLogo;

				if (resetLogonPageLogo || logonPageLogo != null)
					PortalConfig.PortalHomepageImage = logonPageLogo;

				tran.Commit();
			}
		}

		public static void UpdateCompanyInfo(
			string Title1, string Title2, string Text1, string Text2, 
			byte[] PortalLogo, byte[] HomePageLogo)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				PortalConfig.PortalHomepageTitle1 = Title1;
				PortalConfig.PortalHomepageTitle2 = Title2;
				PortalConfig.PortalHomepageText1 = Text1;
				PortalConfig.PortalHomepageText2 = Text2;

				if (PortalLogo != null)
					PortalConfig.PortalCompanyLogo = PortalLogo;

				if (HomePageLogo != null)
					PortalConfig.PortalHomepageImage = HomePageLogo;

				tran.Commit();
			}
		}
		#endregion

		#region CheckDiskSpace
		public static bool CheckDiskSpace()
		{
			bool RetVal = false;
			long MaxDiskSpace = PortalConfig.DatabaseSize;
			if (MaxDiskSpace == -1)
				RetVal = true;
			else
				RetVal = (DBCompany.GetDatabaseSize() < (MaxDiskSpace * 1024 * 1024)) ? true : false;

			return RetVal;
		}
		#endregion

		#region GetAlertUserInfo
		public static IDataReader GetAlertUserInfo()
		{
			return DBUser.GetUserInfoByLogin("alert");
		}
		#endregion

		#region UpdateAlertUserInfo
		public static void UpdateAlertInfo(bool EnableAlerts, string FirstName, string LastName, string Email)
		{
			int UserId = -1;
			using(IDataReader reader = DBUser.GetUserInfoByLogin("alert"))
			{
				reader.Read();
				UserId = (int)reader["UserId"];
			}

			try
			{
				using(DbTransaction tran = DbTransaction.Begin())
				{
					DBUser.UpdateUserInfo(UserId, "", FirstName, LastName, Email, "", "");
					PortalConfig.EnableAlerts = EnableAlerts;
					tran.Commit();
				}
			}
			catch(Exception exception)
			{
				if(exception is SqlException)
				{
					SqlException sqlException = exception as SqlException;
					if (sqlException.Number == 2627)
						throw new EmailDuplicationException();
				}
				throw;
			}
			Alerts2.Init();
		}
		#endregion

		#region UpdateAlertNotificationInfo
		public static void UpdateAlertNotificationInfo(int AlertUserId, string FirstName, string LastName, string Email)
		{
			User.UpdateUserInfo(AlertUserId, FirstName, LastName, Email, String.Empty, String.Empty);

			if(HttpRuntime.AppDomainAppId != null)
				Configuration.Init2();
		}
		#endregion

		#region TrialEndDate
		public static DateTime TrialEndDate
		{
			get
			{
				DateTime ret = License.ExpirationDate;
				DateTime endDb = Configuration.EndDate;
				if(endDb < ret)
					ret = endDb;

				return ret;
			}
		}
		#endregion

		#region public static string[] GetApplicationIdsForScheduleService()
		public static string[] GetApplicationIdsForScheduleService()
		{
			List<string> list = new List<string>();

			// TODO: Get companies from ibn.config

			//foreach (DataRow row in DBCompany.GetCompanyInfo(0).Rows)
			//{
			//    object oAppId = row["app_id"];
			//    object oIsActive = row["is_active"];
			//    object oDisableScheduleService = row["DisableScheduleService"];

			//    System.Diagnostics.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "DBCompany.GetCompanyInfo(app_id = {0}, is_active = {1}, DisableScheduleService = {2})", oAppId, oIsActive, oDisableScheduleService));

			//    if ((bool)oIsActive && !(bool)oDisableScheduleService)
			//    {
			//        string appId = oAppId as string;
			//        if (!list.Contains(appId))
			//            list.Add(appId);
			//    }
			//}

			return list.ToArray();
		}
		#endregion

		#region GetDatabaseSize
		public static long GetDatabaseSize()
		{
			return DBCompany.GetDatabaseSize();
		}
		#endregion
	}
}
