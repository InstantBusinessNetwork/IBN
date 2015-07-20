using System;
using System.Data;
using System.Data.SqlClient;

namespace Mediachase.IBN.Database
{
	/// <summary>
	/// Summary description for DbPop3MailRequests.
	/// </summary>
	public class DbPop3MailRequest
	{
		private DbPop3MailRequest()
		{
		}

		public static int Create(string Sender, int SenderIbnUserId, string FirstName, string LastName,
			string Subject, string InnerText, int Priority, 
			int Pop3BoxId)
		{
			return DbHelper2.RunSpInteger("Pop3MailRequestCreate",
				DbHelper2.mp("@Sender", SqlDbType.NVarChar, 50, Sender),
				DbHelper2.mp("@SenderIbnUserId", SqlDbType.Int, SenderIbnUserId==-1?DBNull.Value:(object)SenderIbnUserId),
				DbHelper2.mp("@FirstName", SqlDbType.NVarChar, 50, FirstName),
				DbHelper2.mp("@LastName", SqlDbType.NVarChar, 50, LastName),
				DbHelper2.mp("@Subject", SqlDbType.NVarChar, 1024, Subject),
				DbHelper2.mp("@InnerText", SqlDbType.NText, InnerText),
				DbHelper2.mp("@Received", SqlDbType.DateTime, DateTime.UtcNow),
				DbHelper2.mp("@Priority", SqlDbType.Int, Priority),
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}

		public static void UpdateMhtFileId(int Pop3MailRequestId, int MhtFileId)
		{
			DbHelper2.RunSp("Pop3MailRequestSetMhtFileId",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, Pop3MailRequestId),
				DbHelper2.mp("@MhtFileId", SqlDbType.Int, MhtFileId));
		}

		static public void Update(int Pop3MailRequestId, string Subject, string InnerText, int Priority)
		{
			DbHelper2.RunSp("Pop3MailRequestUpdate",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, Pop3MailRequestId),
				DbHelper2.mp("@Subject", SqlDbType.NVarChar, 1024, Subject),
				DbHelper2.mp("@InnerText", SqlDbType.NText, InnerText),
				DbHelper2.mp("@Priority", SqlDbType.Int, Priority));
		}

		public static void Delete(int Pop3MailRequestId)
		{
			DbHelper2.RunSp("Pop3MailRequestDelete",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, Pop3MailRequestId));
		}

		#region GetListPop3MailRequests
		/// <summary>
		///  Pop3MailRequestId, Sender, SenderIbnUserId, FirstName, LastName, Subject, InnerText, 
		///  Priority, PriorityName, Pop3BoxId, Received, MhtFileId, SenderType, Pop3BoxName
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetListPop3MailRequests(int Pop3MailRequestId, int Pop3BoxId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataReader(
				TimeZoneId, new string[]{"Received"},
				"Pop3MailRequestsGet",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, Pop3MailRequestId),
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}

		public static DataTable GetListPop3MailRequestsDataTable(int Pop3MailRequestId, int Pop3BoxId, int TimeZoneId, int LanguageId)
		{
			return DbHelper2.RunSpDataTable(
				TimeZoneId, new string[]{"Received"},
				"Pop3MailRequestsGet",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, Pop3MailRequestId),
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, LanguageId));
		}
		#endregion

	}
}
