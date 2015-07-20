using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
	public class EMailMessageRow
	{
		private enum DataRowState
		{
			Unchanged,
			Added,
			Modified,
			Deleted
		}

		private DataRowState _state = DataRowState.Unchanged;
        
		int _EMailMessageId;
		DateTime _Created = DateTime.MinValue;
		string _From;
		string _To;
		string _Subject;
		byte[] _EmlMessage;
		int _EMailRouterPop3BoxId;

		#region Util
		static object DBNull2Null(object Value)
		{
			if(Value==DBNull.Value)
				return null;
			return Value;
		}
		#endregion
        
        
		public EMailMessageRow()
		{
			_state = DataRowState.Added;

			_Created = DateTime.UtcNow;
		}
        
		public EMailMessageRow(int EMailMessageId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageSelect", DbHelper2.mp("@EMailMessageId", SqlDbType.Int, EMailMessageId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailMessage", "EMailMessageId", EMailMessageId));
			}
		}
		
		public EMailMessageRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailMessageId = (int)DBNull2Null(reader["EMailMessageId"]);
			
			_Created = (DateTime)DBNull2Null(reader["Created"]);
			
			_From = (string)DBNull2Null(reader["From"]);
			
			_To = (string)DBNull2Null(reader["To"]);
			
			_Subject = (string)DBNull2Null(reader["Subject"]);

			_EmlMessage = (byte[])DBNull2Null(reader["EmlMessage"]);
			
			_EMailRouterPop3BoxId = (int)DBNull2Null(reader["EMailRouterPop3BoxId"]);
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
			get { return _EMailMessageId; }		     
		}

		public int EMailRouterPop3BoxId
		{
			get 
			{
				return _EMailRouterPop3BoxId;
			}
			set 
			{
				_EMailRouterPop3BoxId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}
		}
		
        
		public virtual int EMailMessageId
		{
			get
			{
				return _EMailMessageId;
			}
			
		}
		
		public virtual DateTime Created
	    
		{
			get
			{
				return _Created;
			}
			
			set
			{
				_Created = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string From
	    
		{
			get
			{
				return _From;
			}
			
			set
			{
				_From = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string To
	    
		{
			get
			{
				return _To;
			}
			
			set
			{
				_To = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Subject
	    
		{
			get
			{
				return _Subject;
			}
			
			set
			{
				_Subject = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}

		public virtual byte[] EmlMessage
		{
			get
			{
				return _EmlMessage;
			}
			set
			{
				_EmlMessage = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}

			}
		}
		
		#endregion
	

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_EMailMessageUpdate"
			    
					, DbHelper2.mp("@EMailMessageId", SqlDbType.Int, _EMailMessageId)
			        
					, DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
			        
					, DbHelper2.mp("@From", SqlDbType.NVarChar, 255, _From)
			        
					, DbHelper2.mp("@To", SqlDbType.NVarChar, 255, _To)
			        
					, DbHelper2.mp("@Subject", SqlDbType.NText, _Subject)
			         
					, DbHelper2.mp("@EmlMessage", SqlDbType.Image, _EmlMessage)

					, DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, _EMailRouterPop3BoxId)
					);
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter EMailMessageIdPrm = new SqlParameter("@EMailMessageId", SqlDbType.Int);
				EMailMessageIdPrm.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailMessageInsert"
			    
					, EMailMessageIdPrm
			        
					, DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
			        
					, DbHelper2.mp("@From", SqlDbType.NVarChar, 255, _From)
			        
					, DbHelper2.mp("@To", SqlDbType.NVarChar, 255, _To)
			        
					, DbHelper2.mp("@Subject", SqlDbType.NText, _Subject)
			         
					, DbHelper2.mp("@EmlMessage", SqlDbType.Image, _EmlMessage)

					, DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, _EMailRouterPop3BoxId)
					);

				_EMailMessageId = (int)EMailMessageIdPrm.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
			Delete(_EMailMessageId);
		}
		
		public static void Delete(int EMailMessageId)
		{
			DbHelper2.RunSp("mc_EMailMessageDelete", DbHelper2.mp("@EMailMessageId", SqlDbType.Int, EMailMessageId));
		}

		public static int GetOwnerIncidentId(int EMailMessageId)
		{
			return DbHelper2.RunSpInteger("mc_EMailMessageGetOwnerIssueId", DbHelper2.mp("@EMailMessageId", SqlDbType.Int, EMailMessageId));
		}

		public static int GetEMailRouterPop3BoxIdByIssueId(int IncidentId)
		{
			return DbHelper2.RunSpInteger("mc_EMailMessageGetEMailRouterPop3BoxIdByIssueId", DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId));
		}

//		public static string[] GetExternalSendersByIncidentId(int IncidentId)
//		{
//			ArrayList retVal = new ArrayList();
//			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageGetExternalSendersByIssueId", DbHelper2.mp("@IncidentId", SqlDbType.Int, IncidentId)))
//			{
//				while(reader.Read())
//				{
//					retVal.Add(reader["From"]);
//				}
//			}
//			return (string[])retVal.ToArray(typeof(string));
//		}

		#region GetPendingMessages
		// [2008-01-10] OR: Pending Incident List Optimization
		public static DataTable GetPendingMessages(int timeZoneId)
		{
			return DbHelper2.RunSpDataTable(timeZoneId,
				new string[] { "Created" }, "PendingEmailMessagesGet");
		}
		#endregion

		#region GetSenderEmail
		// [2008-01-10] OR: Pending Incident List Optimization
		public static string GetSenderEmail(int emailMessageId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("EmailMessageSenderGet",
				DbHelper2.mp("@EMailMessageId", SqlDbType.Int, emailMessageId)))
			{
				if (reader.Read())
				{
					return (string)reader["From"];
				}
			}

			return string.Empty;
		}
		#endregion
	}
}
