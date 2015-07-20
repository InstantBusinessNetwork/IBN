
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class DefaultIncidentFieldRow
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

		static object DBNull2Null(object Value, object NullValue)
		{
			if(Value==DBNull.Value)
				return NullValue;
			return Value;
		}
		#endregion
        
        int _DefaultIncidentFieldId;
        
        int  _ProjectId;
        int _CreatorId;
        int _ManagerId;
        int _TypeId;
        int _PriorityId;
        int _SeverityId;
        int _TaskTime;
        
        
        public DefaultIncidentFieldRow()
        {
            _state = DataRowState.Added;
            
            _ProjectId = -1;
        }
        
		public DefaultIncidentFieldRow(int DefaultIncidentFieldId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_DefaultIncidentFieldSelect", DbHelper2.mp("@DefaultIncidentFieldId", SqlDbType.Int, DefaultIncidentFieldId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "DefaultIncidentField", "DefaultIncidentFieldId", DefaultIncidentFieldId));
			}
		}
		
		public DefaultIncidentFieldRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_DefaultIncidentFieldId = (int)DBNull2Null(reader["DefaultIncidentFieldId"]);
			
		    _ProjectId = (int)DBNull2Null(reader["ProjectId"],-1);
		    
			_CreatorId = (int)DBNull2Null(reader["CreatorId"]);
			
			_ManagerId = (int)DBNull2Null(reader["ManagerId"]);
			
			_TypeId = (int)DBNull2Null(reader["TypeId"]);
			
			_PriorityId = (int)DBNull2Null(reader["PriorityId"]);
			
			_SeverityId = (int)DBNull2Null(reader["SeverityId"]);
			
			_TaskTime = (int)DBNull2Null(reader["TaskTime"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _DefaultIncidentFieldId; }		     
		}
		
        
	    public virtual int DefaultIncidentFieldId
	    
		{
			get
			{
				return _DefaultIncidentFieldId;
			}
			
		}
		
		public virtual int ProjectId
	    
		{
			get
			{
				return _ProjectId;
			}
			
			set
			{
				_ProjectId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int CreatorId
	    
		{
			get
			{
				return _CreatorId;
			}
			
			set
			{
				_CreatorId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int ManagerId
	    
		{
			get
			{
				return _ManagerId;
			}
			
			set
			{
				_ManagerId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int TypeId
	    
		{
			get
			{
				return _TypeId;
			}
			
			set
			{
				_TypeId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int PriorityId
	    
		{
			get
			{
				return _PriorityId;
			}
			
			set
			{
				_PriorityId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int SeverityId
	    
		{
			get
			{
				return _SeverityId;
			}
			
			set
			{
				_SeverityId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int TaskTime
	    
		{
			get
			{
				return _TaskTime;
			}
			
			set
			{
				_TaskTime = value;
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
				DbHelper2.RunSp("mc_DefaultIncidentFieldUpdate"
			    
			        , DbHelper2.mp("@DefaultIncidentFieldId", SqlDbType.Int, _DefaultIncidentFieldId)
			        
			        , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId<=0?DBNull.Value:(object)_ProjectId)
			        
			        , DbHelper2.mp("@CreatorId", SqlDbType.Int, _CreatorId)
			        
			        , DbHelper2.mp("@ManagerId", SqlDbType.Int, _ManagerId)
			        
			        , DbHelper2.mp("@TypeId", SqlDbType.Int, _TypeId)
			        
			        , DbHelper2.mp("@PriorityId", SqlDbType.Int, _PriorityId)
			        
			        , DbHelper2.mp("@SeverityId", SqlDbType.Int, _SeverityId)
			        
			        , DbHelper2.mp("@TaskTime", SqlDbType.Int, _TaskTime)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@DefaultIncidentFieldId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_DefaultIncidentFieldInsert", outPutKey
		            
		            , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId<=0?DBNull.Value:(object)_ProjectId)
		            
		            , DbHelper2.mp("@CreatorId", SqlDbType.Int, _CreatorId)
		            
		            , DbHelper2.mp("@ManagerId", SqlDbType.Int, _ManagerId)
		            
		            , DbHelper2.mp("@TypeId", SqlDbType.Int, _TypeId)
		            
		            , DbHelper2.mp("@PriorityId", SqlDbType.Int, _PriorityId)
		            
		            , DbHelper2.mp("@SeverityId", SqlDbType.Int, _SeverityId)
		            
		            , DbHelper2.mp("@TaskTime", SqlDbType.Int, _TaskTime)
		             
		        );
		        
		        _DefaultIncidentFieldId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_DefaultIncidentFieldId);
		}
		
		public static void Delete(int DefaultIncidentFieldId)
		{
		    DbHelper2.RunSp("mc_DefaultIncidentFieldDelete", DbHelper2.mp("@DefaultIncidentFieldId", SqlDbType.Int, DefaultIncidentFieldId));
		}

		public static DefaultIncidentFieldRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_DefaultIncidentFieldList"))
			{
				while(reader.Read())
				{
					retVal.Add(new DefaultIncidentFieldRow(reader));
				}
			}
			return (DefaultIncidentFieldRow[])retVal.ToArray(typeof(DefaultIncidentFieldRow));
		}
    }
}
