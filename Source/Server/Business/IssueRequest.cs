using System;
using System.Collections;
using System.Data;

using Mediachase.Ibn;
using Mediachase.IBN.Business.ControlSystem;
using Mediachase.IBN.Database;
using Mediachase.IBN.Database.Pop3;


namespace Mediachase.IBN.Business
{
	public class IssueRequest
	{
		#region MailIssuesEnabled
		public static bool MailIssuesEnabled()
		{
			return License.MailIssuesEnabled;
		}
		#endregion

		#region CanUse()
		public static bool CanUse()
		{
			return Security.CurrentUser.IsAlertService || Security.IsUserInGroup(InternalSecureGroups.HelpDeskManager);
		}

		public static bool CanUse(int userId)
		{
			ArrayList secure_groups = User.GetListSecureGroupAll(userId);
			return secure_groups.Contains((int)InternalSecureGroups.HelpDeskManager);
		}
		#endregion

		#region Get()
		/// <summary>
		///	 Pop3MailRequestId, Sender, SenderIbnUserId, FirstName, LastName, Subject, InnerText, 
		///  Priority, PriorityName, Pop3BoxId, Received, MhtFileId, SenderType, Pop3BoxName
		/// </summary>
		/// <returns></returns>
		public static IDataReader Get(int issueRequestId)
		{
			if(!CanUse())
				throw new AccessDeniedException();

			return DbIssueRequest.Get(issueRequestId, 0, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region GetMailBoxById()
		/// <summary>
		///	 Pop3BoxId, [Name], Server, Port, Login, [Password], [Interval], LastRequest, LastSuccessfulRequest, Active, AutoKillForRead, LastErrorText
		/// </summary>
		/// <returns></returns>
		public static IDataReader GetMailBoxById(int Pop3BoxId)
		{
			return DbIssueRequest.GetMailBoxById(Pop3BoxId);
		}
		#endregion

		#region GetByMailBoxDataTable()
		/// <summary>
		///  Pop3MailRequestId, Sender, SenderIbnUserId, FirstName, LastName, Subject, InnerText, 
		///  InnerHtml, Priority, PriorityName, Pop3BoxId, Received, IsProcessed, Processed, 
		///  ProcessedBy, MhtFileId, SenderType
		/// </summary>
		/// <returns></returns>
		public static DataTable GetByMailBoxDataTable(int Pop3BoxId)
		{
			if(!CanUse())
				throw new AccessDeniedException();

			return DbIssueRequest.GetDataTable(0, Pop3BoxId, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId);
		}
		#endregion

		#region Delete()
		/// <summary>
		/// Deletes the specified request.
		/// </summary>
		/// <param name="Pop3MailRequestId">The POP3 mail request id.</param>
		static public void Delete(int issueRequestId)
		{
			using(DbTransaction tran = DbTransaction.Begin())
			{
				SystemEvents.AddSystemEvents(SystemEventTypes.IssueRequest_Deleted, issueRequestId);
				FileStorage fileStorage = GetFileStorage(issueRequestId);
				fileStorage.DeleteAll();

				DbPop3MailRequest.Delete(issueRequestId);
			
				tran.Commit();
			}
		}
		#endregion

		#region Update()
		/// <summary>
		/// Updates the specified issue request.
		/// </summary>
		/// <param name="issueRequestId">The issue request id.</param>
		/// <param name="subject">The subject.</param>
		/// <param name="innerText">The inner text.</param>
		/// <param name="priority">The priority.</param>
		static public void Update(int issueRequestId, string subject, string innerText, int priority)
		{
			DbPop3MailRequest.Update(issueRequestId, subject, innerText, priority);
		}
		#endregion

		/*#region Approve()
		public static int Approve(int issueRequestId, int projectId, bool createExternalUser)
		{
			int issueId = -1;

			string senderEmail = string.Empty;
			int userId = -1;
			string FirstName = string.Empty;
			string LastName = string.Empty;
			string Subject = string.Empty;
			string InnerText = string.Empty;
			int Priority = 500;
			int Pop3BoxId = -1;
			DateTime Received = DateTime.Now;

			using(IDataReader reader = Get(issueRequestId))
			{
				if(reader.Read())
				{
					senderEmail = reader["Sender"].ToString();
					if(reader["SenderIbnUserId"]!=DBNull.Value)
						userId = (int)reader["SenderIbnUserId"];
					FirstName = reader["FirstName"].ToString();
					LastName = reader["LastName"].ToString();
					Subject = reader["Subject"].ToString();
					InnerText = reader["InnerText"].ToString();
					Priority = (int)reader["Priority"];
					Pop3BoxId = (int)reader["Pop3BoxId"];
					Received = (DateTime)reader["Received"];
				}
			}

			if(senderEmail.Length > 0)
			{
				using(DbTransaction tran = DbTransaction.Begin())
				{
					if(createExternalUser && userId <= 0)
					{
						// Create an external user
						userId = User.CreateExternal(FirstName,LastName, senderEmail, new ArrayList(), true,
							string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
							string.Empty, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId,
							string.Empty, string.Empty, null);
					}

					// Create issue

					// Commented by O.Rylin - весь этот метод не должен вызываться
//					issueId = Incident.Create(Subject, InnerText, Priority, userId, projectId, userId <= 0);
					
					BaseIbnContainer destContainer;
					BaseIbnContainer issueContainer = BaseIbnContainer.Create("FileLibrary", string.Format("IncidentId_{0}", issueId));

					if(userId > 0)
						destContainer = issueContainer;
					else
					{
						Issue2.UpdateMailSenderEmail(issueId, senderEmail);

						// Create Forum Node
						ForumStorage forumStorage = (ForumStorage)issueContainer.LoadControl("ForumStorage");
						ForumThreadNodeInfo node = forumStorage.CreateForumThreadNode(Subject,string.Format("{0} {1}", FirstName, LastName), senderEmail, (int)ForumStorage.NodeTypes.ExternalQuestion);

						destContainer = BaseIbnContainer.Create("FileLibrary", string.Format("ForumNodeId_{0}", node.Id));
					}

					// Upload Files
					FileStorage destFileStorage = (FileStorage)destContainer.LoadControl("FileStorage");
					foreach(FileInfo srcFileInfo in GetFileStorage(issueRequestId).GetFiles())
					{
						destFileStorage.CopyFile(srcFileInfo, destFileStorage.Root);
					}

					// Delete Request
					SystemEvents.AddSystemEvents(SystemEventTypes.IssueRequest_Approved, issueRequestId);
					Delete(issueRequestId);

					tran.Commit();
				}
			}
			return issueId;
		}
		#endregion*/

		#region Create()
		public static int Create(string senderEmail, int senderUserId, string firstName, string lastName,
			string subject, string innerText, int priority, int pop3BoxId)
		{
			int issueRequestId;
			using(DbTransaction tran = DbTransaction.Begin())
			{
				issueRequestId = DbPop3MailRequest.Create(senderEmail, senderUserId, firstName, lastName, subject, innerText, priority, pop3BoxId);
				
				SystemEvents.AddSystemEvents(SystemEventTypes.IssueRequest_Created, issueRequestId);

				tran.Commit();
			}
			return issueRequestId;
		}
		#endregion

		#region GetContainer()
		public static BaseIbnContainer GetContainer(int issueRequestId)
		{
			return BaseIbnContainer.Create("Pop3MailRequest", string.Format("Pop3MailRequestId_{0}", issueRequestId));
		}
		#endregion

		#region GetFileStorage()
		public static FileStorage GetFileStorage(int issueRequestId)
		{
			return (FileStorage)GetContainer(issueRequestId).LoadControl("FileStorage");
		}
		#endregion

    #region GetSubject()
    public static string GetSubject(int issueRequestId)
    {
      string Subject = "";
      using (IDataReader reader = DbIssueRequest.Get(issueRequestId, 0, Security.CurrentUser.TimeZoneId, Security.CurrentUser.LanguageId))
      {
        if (reader.Read())
          Subject = reader["Subject"].ToString();
      }
      return Subject;
    }
    #endregion
	}
}
