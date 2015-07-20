
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class ManualAssignEMailMessageRow
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
		static object DBNull2Null(object Value)
		{
			if(Value==DBNull.Value)
				return null;
			return Value;
		}
		#endregion
        
        int _ManualAssignEMailMessageId;
        int _EMailMessageId;
        
        
        public ManualAssignEMailMessageRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public ManualAssignEMailMessageRow(int ManualAssignEMailMessageId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ManualAssignEMailMessageSelect", DbHelper2.mp("@ManualAssignEMailMessageId", SqlDbType.Int, ManualAssignEMailMessageId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "ManualAssignEMailMessage", "ManualAssignEMailMessageId", ManualAssignEMailMessageId));
			}
		}
		
		public ManualAssignEMailMessageRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ManualAssignEMailMessageId = (int)DBNull2Null(reader["ManualAssignEMailMessageId"]);
			
			_EMailMessageId = (int)DBNull2Null(reader["EMailMessageId"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _ManualAssignEMailMessageId; }		     
		}
		
        
	    public virtual int ManualAssignEMailMessageId
	    
		{
			get
			{
				return _ManualAssignEMailMessageId;
			}
			
		}
		
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
	
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_ManualAssignEMailMessageUpdate"
			    
			        , DbHelper2.mp("@ManualAssignEMailMessageId", SqlDbType.Int, _ManualAssignEMailMessageId)
			        
			        , DbHelper2.mp("@EMailMessageId", SqlDbType.Int, _EMailMessageId)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@ManualAssignEMailMessageId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_ManualAssignEMailMessageInsert", outPutKey
		            
		            , DbHelper2.mp("@EMailMessageId", SqlDbType.Int, _EMailMessageId)
		             
		        );
		        
		        _ManualAssignEMailMessageId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_ManualAssignEMailMessageId);
		}
		
		public static void Delete(int ManualAssignEMailMessageId)
		{
		    DbHelper2.RunSp("mc_ManualAssignEMailMessageDelete", DbHelper2.mp("@ManualAssignEMailMessageId", SqlDbType.Int, ManualAssignEMailMessageId));
		}

		public static void DeleteByEMailMessageId(int EMailMessageId)
		{
			DbHelper2.RunSp("mc_ManualAssignEMailMessageDeleteByEMailMessageId", DbHelper2.mp("@EMailMessageId", SqlDbType.Int, EMailMessageId));
		}


		public static ManualAssignEMailMessageRow[] List()
		{
			ArrayList retVal = new ArrayList();

			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_ManualAssignEMailMessageList"))
			{
				while(reader.Read())
				{
					retVal.Add(new ManualAssignEMailMessageRow(reader));
				}
			}

			return (ManualAssignEMailMessageRow[])retVal.ToArray(typeof(ManualAssignEMailMessageRow));
		}
    }
}
