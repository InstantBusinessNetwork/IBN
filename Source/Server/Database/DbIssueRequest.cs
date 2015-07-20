using System;
using System.Data;

namespace Mediachase.IBN.Database
{
	public class DbIssueRequest
	{
		#region Get()
		/// <summary>
		///  Pop3MailRequestId, Sender, SenderIbnUserId, FirstName, LastName, Subject, InnerText, 
		///  Priority, PriorityName, Pop3BoxId, Received, MhtFileId, SenderType, Pop3BoxName
		/// </summary>
		/// <returns></returns>
		public static IDataReader Get(int issueRequestId, int pop3BoxId, int timeZoneId, int languageId)
		{
			return DbHelper2.RunSpDataReader(
				timeZoneId, new string[]{"Received"},
				"Pop3MailRequestsGet",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, issueRequestId),
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, pop3BoxId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId));
		}
		#endregion

		#region GetDataTable()
		/// <summary>
		///  Pop3MailRequestId, Sender, SenderIbnUserId, FirstName, LastName, Subject, InnerText, 
		///  Priority, PriorityName, Pop3BoxId, Received, MhtFileId, SenderType, Pop3BoxName
		/// </summary>
		/// <returns></returns>
		public static DataTable GetDataTable(int issueRequestId, int pop3BoxId, int timeZoneId, int languageId)
		{
			return DbHelper2.RunSpDataTable(
				timeZoneId, new string[]{"Received"},
				"Pop3MailRequestsGet",
				DbHelper2.mp("@Pop3MailRequestId", SqlDbType.Int, issueRequestId),
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, pop3BoxId),
				DbHelper2.mp("@LanguageId", SqlDbType.Int, languageId));
		}
		#endregion

		#region GetMailBoxById()
		/// <summary>
		///	 Pop3BoxId, [Name], Server, Port, Login, [Password], [Interval], LastRequest, LastSuccessfulRequest, Active, AutoKillForRead, LastErrorText
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMailBoxById(int Pop3BoxId)
		{
			return DbHelper2.RunSpDataReader("Pop3BoxGetById", 
				DbHelper2.mp("@Pop3BoxId", SqlDbType.Int, Pop3BoxId));
		}
		#endregion
	}
}
