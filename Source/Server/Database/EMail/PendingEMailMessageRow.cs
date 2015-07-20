
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class PendingEMailMessageRow
    {
    	private enum DataRowState
		{
			Unchanged,
			Added,
			Modified,
			Deleted
		}
		
        private DataRowState _state = DataRowState.Unchanged;
        
        #region Util
		/// <summary>
		/// DBs the null2 null.
		/// </summary>
		/// <param name="Value">The value.</param>
		/// <returns></returns>
		static object DBNull2Null(object Value)
		{
			if(Value==DBNull.Value)
				return null;
			return Value;
		}
		#endregion
        
        int _PendingEMailMessageId;
        int _EMailMessageId;


		/// <summary>
		/// Initializes a new instance of the <see cref="PendingEMailMessageRow"/> class.
		/// </summary>
        public PendingEMailMessageRow()
        {
            _state = DataRowState.Added;
            
            
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="PendingEMailMessageRow"/> class.
		/// </summary>
		/// <param name="PendingEMailMessageId">The pending E mail message id.</param>
		public PendingEMailMessageRow(int PendingEMailMessageId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_PendingEMailMessageSelect", DbHelper2.mp("@PendingEMailMessageId", SqlDbType.Int, PendingEMailMessageId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "PendingEMailMessage", "PendingEMailMessageId", PendingEMailMessageId));
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PendingEMailMessageRow"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public PendingEMailMessageRow(IDataReader reader)
		{
		   Load(reader);
		}

		/// <summary>
		/// Loads the specified reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_PendingEMailMessageId = (int)DBNull2Null(reader["PendingEMailMessageId"]);
			
			_EMailMessageId = (int)DBNull2Null(reader["EMailMessageId"]);
			
		}
	
		#region Public Properties

		/// <summary>
		/// Gets the primary key id.
		/// </summary>
		/// <value>The primary key id.</value>
		public virtual int PrimaryKeyId
		{
            get { return _PendingEMailMessageId; }		     
		}


		/// <summary>
		/// Gets the pending E mail message id.
		/// </summary>
		/// <value>The pending E mail message id.</value>
	    public virtual int PendingEMailMessageId
	    
		{
			get
			{
				return _PendingEMailMessageId;
			}
			
		}

		/// <summary>
		/// Gets or sets the E mail message id.
		/// </summary>
		/// <value>The E mail message id.</value>
	    public virtual int EMailMessageId
	    
		{
			get
			{
				return _EMailMessageId;
			}
			
			set
			{
				_EMailMessageId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion

		/// <summary>
		/// Updates this instance.
		/// </summary>
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_PendingEMailMessageUpdate"
			    
			        , DbHelper2.mp("@PendingEMailMessageId", SqlDbType.Int, _PendingEMailMessageId)
			        
			        , DbHelper2.mp("@EMailMessageId", SqlDbType.Int, _EMailMessageId)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@PendingEMailMessageId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_PendingEMailMessageInsert", outPutKey
		            
		            , DbHelper2.mp("@EMailMessageId", SqlDbType.Int, _EMailMessageId)
		             
		        );
		        
		        _PendingEMailMessageId = (int)outPutKey.Value;
			}
		}

		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_PendingEMailMessageId);
		}

		/// <summary>
		/// Deletes the specified pending E mail message id.
		/// </summary>
		/// <param name="PendingEMailMessageId">The pending E mail message id.</param>
		public static void Delete(int PendingEMailMessageId)
		{
		    DbHelper2.RunSp("mc_PendingEMailMessageDelete", DbHelper2.mp("@PendingEMailMessageId", SqlDbType.Int, PendingEMailMessageId));
		}

		/// <summary>
		/// Deletes the by E mail message id.
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		public static void DeleteByEMailMessageId(int EMailMessageId)
		{
			DbHelper2.RunSp("mc_PendingEMailMessageDeleteByEMailMessageId", DbHelper2.mp("@EMailMessageId", SqlDbType.Int, EMailMessageId));
		}

		/// <summary>
		/// Lists this instance.
		/// </summary>
		/// <returns></returns>
		public static PendingEMailMessageRow[] List()
		{
			ArrayList retVal = new ArrayList();

			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_PendingEMailMessageList"))
			{
				while(reader.Read())
				{
					retVal.Add(new PendingEMailMessageRow(reader));
				}
			}

			return (PendingEMailMessageRow[])retVal.ToArray(typeof(PendingEMailMessageRow));
		}

		/// <summary>
		/// Determines whether [contains] [the specified E mail message id].
		/// </summary>
		/// <param name="EMailMessageId">The E mail message id.</param>
		/// <returns>
		/// 	<c>true</c> if [contains] [the specified E mail message id]; otherwise, <c>false</c>.
		/// </returns>
		public static bool Contains(int EMailMessageId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_PendingEMailMessageByEMailMessageId",
				DbHelper2.mp("@EMailMessageId", SqlDbType.Int, EMailMessageId)))
			{
				if (reader.Read())
				{
					return true;
				}
			}

			return false;
		}
	}
}
