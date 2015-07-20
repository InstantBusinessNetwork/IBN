
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class IncidentUserTicketRow
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
        
        int _IncidentUserTicketId;
        Guid _UID;
        int _UserId;
        int _IncidentId;
        DateTime _Created;
        
        
        public IncidentUserTicketRow()
        {
            _state = DataRowState.Added;
            _UID = Guid.NewGuid();
			_Created = DateTime.UtcNow;   
        }
        
		public IncidentUserTicketRow(int IncidentUserTicketId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentUserTicketSelect", DbHelper2.mp("@IncidentUserTicketId", SqlDbType.Int, IncidentUserTicketId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "IncidentUserTicket", "IncidentUserTicketId", IncidentUserTicketId));
			}
		}
		
		public IncidentUserTicketRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_IncidentUserTicketId = (int)DBNull2Null(reader["IncidentUserTicketId"]);
			
			_UID = (Guid)DBNull2Null(reader["UID"]);
			
			_UserId = (int)DBNull2Null(reader["UserId"]);
			
			_IncidentId = (int)DBNull2Null(reader["IncidentId"]);
			
			_Created = (DateTime)DBNull2Null(reader["Created"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _IncidentUserTicketId; }		     
		}
		
        
	    public virtual int IncidentUserTicketId
	    
		{
			get
			{
				return _IncidentUserTicketId;
			}
			
		}
		
	    public virtual Guid UID
	    
		{
			get
			{
				return _UID;
			}
			
			set
			{
				_UID = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int UserId
	    
		{
			get
			{
				return _UserId;
			}
			
			set
			{
				_UserId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int IncidentId
	    
		{
			get
			{
				return _IncidentId;
			}
			
			set
			{
				_IncidentId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
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
		
		#endregion
	
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_IncidentUserTicketUpdate"
			    
			        , DbHelper2.mp("@IncidentUserTicketId", SqlDbType.Int, _IncidentUserTicketId)
			        
			        , DbHelper2.mp("@UID", SqlDbType.UniqueIdentifier, _UID)
			        
			        , DbHelper2.mp("@UserId", SqlDbType.Int, _UserId)
			        
			        , DbHelper2.mp("@IncidentId", SqlDbType.Int, _IncidentId)
			        
			        , DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@IncidentUserTicketId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_IncidentUserTicketInsert", outPutKey
		            
		            , DbHelper2.mp("@UID", SqlDbType.UniqueIdentifier, _UID)
		            
		            , DbHelper2.mp("@UserId", SqlDbType.Int, _UserId)
		            
		            , DbHelper2.mp("@IncidentId", SqlDbType.Int, _IncidentId)
		            
		            , DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
		             
		        );
		        
		        _IncidentUserTicketId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_IncidentUserTicketId);
		}
		
		public static void Delete(int IncidentUserTicketId)
		{
		    DbHelper2.RunSp("mc_IncidentUserTicketDelete", DbHelper2.mp("@IncidentUserTicketId", SqlDbType.Int, IncidentUserTicketId));
		}

		public static IncidentUserTicketRow[] List(Guid UID)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentUserTicketListByUID",
					   DbHelper2.mp("@UID",SqlDbType.UniqueIdentifier, UID)))
			{
				while(reader.Read())
				{
					retVal.Add(new IncidentUserTicketRow(reader));
				}
			}
			return (IncidentUserTicketRow[])retVal.ToArray(typeof(IncidentUserTicketRow));
		}

		public static IncidentUserTicketRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentUserTicketList"))
			{
				while(reader.Read())
				{
					retVal.Add(new IncidentUserTicketRow(reader));
				}
			}
			return (IncidentUserTicketRow[])retVal.ToArray(typeof(IncidentUserTicketRow));
		}
    }
}
