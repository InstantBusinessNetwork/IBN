
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailRouterPop3BoxActivityRow
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
        
        int _EMailRouterPop3BoxActivityId;
        int _EMailRouterPop3BoxId;
        bool _IsActive = false;
        DateTime _LastRequest = DateTime.MinValue;
        DateTime _LastSuccessfulRequest = DateTime.MinValue;
        string _ErrorText = string.Empty;
        int _TotalMessageCount = 0;
        
        
        public EMailRouterPop3BoxActivityRow()
        {
            _state = DataRowState.Added;
            
            _IsActive = false;  
            
            _TotalMessageCount = 0;  
            
        }
        
		public EMailRouterPop3BoxActivityRow(int EMailRouterPop3BoxActivityId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailRouterPop3BoxActivitySelect", DbHelper2.mp("@EMailRouterPop3BoxActivityId", SqlDbType.Int, EMailRouterPop3BoxActivityId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailRouterPop3BoxActivity", "EMailRouterPop3BoxActivityId", EMailRouterPop3BoxActivityId));
			}
		}
		
		public EMailRouterPop3BoxActivityRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailRouterPop3BoxActivityId = (int)DBNull2Null(reader["EMailRouterPop3BoxActivityId"]);
			
			_EMailRouterPop3BoxId = (int)DBNull2Null(reader["EMailRouterPop3BoxId"]);
			
			_IsActive = (bool)DBNull2Null(reader["IsActive"]);
			
			_LastRequest = (DateTime)DBNull2Null(reader["LastRequest"], DateTime.MinValue);
			
			_LastSuccessfulRequest = (DateTime)DBNull2Null(reader["LastSuccessfulRequest"], DateTime.MinValue);
			
			_ErrorText = (string)DBNull2Null(reader["ErrorText"]);
			
			_TotalMessageCount = (int)DBNull2Null(reader["TotalMessageCount"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _EMailRouterPop3BoxActivityId; }		     
		}
		
        
	    public virtual int EMailRouterPop3BoxActivityId
	    
		{
			get
			{
				return _EMailRouterPop3BoxActivityId;
			}
			
		}
		
	    public virtual int EMailRouterPop3BoxId
	    
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
		
	    public virtual bool IsActive
	    
		{
			get
			{
				return _IsActive;
			}
			
			set
			{
				_IsActive = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual DateTime LastRequest
	    
		{
			get
			{
				return _LastRequest;
			}
			
			set
			{
				_LastRequest = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual DateTime LastSuccessfulRequest
	    
		{
			get
			{
				return _LastSuccessfulRequest;
			}
			
			set
			{
				_LastSuccessfulRequest = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string ErrorText
	    
		{
			get
			{
				return _ErrorText;
			}
			
			set
			{
				_ErrorText = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int TotalMessageCount
	    
		{
			get
			{
				return _TotalMessageCount;
			}
			
			set
			{
				_TotalMessageCount = value;
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
				DbHelper2.RunSp("mc_EMailRouterPop3BoxActivityUpdate"
			    
			        , DbHelper2.mp("@EMailRouterPop3BoxActivityId", SqlDbType.Int, _EMailRouterPop3BoxActivityId)
			        
			        , DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, _EMailRouterPop3BoxId)
			        
			        , DbHelper2.mp("@IsActive", SqlDbType.Bit, _IsActive)
			        
			        , DbHelper2.mp("@LastRequest", SqlDbType.DateTime, _LastRequest==DateTime.MinValue?DBNull.Value:(object)_LastRequest)
			        
			        , DbHelper2.mp("@LastSuccessfulRequest", SqlDbType.DateTime, _LastSuccessfulRequest==DateTime.MinValue?DBNull.Value:(object)_LastSuccessfulRequest)
			        
			        , DbHelper2.mp("@ErrorText", SqlDbType.NText, _ErrorText)
			        
			        , DbHelper2.mp("@TotalMessageCount", SqlDbType.Int, _TotalMessageCount)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailRouterPop3BoxActivityId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailRouterPop3BoxActivityInsert", outPutKey
		            
		            , DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, _EMailRouterPop3BoxId)
		            
		            , DbHelper2.mp("@IsActive", SqlDbType.Bit, _IsActive)
		            
					, DbHelper2.mp("@LastRequest", SqlDbType.DateTime, _LastRequest==DateTime.MinValue?DBNull.Value:(object)_LastRequest)
			        
					, DbHelper2.mp("@LastSuccessfulRequest", SqlDbType.DateTime, _LastSuccessfulRequest==DateTime.MinValue?DBNull.Value:(object)_LastSuccessfulRequest)
		            
		            , DbHelper2.mp("@ErrorText", SqlDbType.NText, _ErrorText)
		            
		            , DbHelper2.mp("@TotalMessageCount", SqlDbType.Int, _TotalMessageCount)
		             
		        );
		        
		        _EMailRouterPop3BoxActivityId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailRouterPop3BoxActivityId);
		}
		
		public static void Delete(int EMailRouterPop3BoxActivityId)
		{
		    DbHelper2.RunSp("mc_EMailRouterPop3BoxActivityDelete", DbHelper2.mp("@EMailRouterPop3BoxActivityId", SqlDbType.Int, EMailRouterPop3BoxActivityId));
		}

		public static EMailRouterPop3BoxActivityRow[] List(int EMailRouterPop3BoxId)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailRouterPop3BoxActivityList", 
					   DbHelper2.mp("@EMailRouterPop3BoxId", SqlDbType.Int, EMailRouterPop3BoxId)))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailRouterPop3BoxActivityRow(reader));
				}
			}
			return (EMailRouterPop3BoxActivityRow[])retVal.ToArray(typeof(EMailRouterPop3BoxActivityRow));
		}
    }
}
