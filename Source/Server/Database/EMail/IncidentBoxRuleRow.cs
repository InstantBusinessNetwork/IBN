
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class IncidentBoxRuleRow
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
        
        int _IncidentBoxRuleId;
        int _IncidentBoxId;
        int _RuleType;
        int _OutlineIndex;
        string _OutlineLevel;
        string _Key;
        string _Value;
        
        
        public IncidentBoxRuleRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public IncidentBoxRuleRow(int IncidentBoxRuleId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxRuleSelect", DbHelper2.mp("@IncidentBoxRuleId", SqlDbType.Int, IncidentBoxRuleId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "IncidentBoxRule", "IncidentBoxRuleId", IncidentBoxRuleId));
			}
		}
		
		public IncidentBoxRuleRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_IncidentBoxRuleId = (int)DBNull2Null(reader["IncidentBoxRuleId"]);
			
			_IncidentBoxId = (int)DBNull2Null(reader["IncidentBoxId"]);
			
			_RuleType = (int)DBNull2Null(reader["RuleType"]);
			
			_OutlineIndex = (int)DBNull2Null(reader["OutlineIndex"]);
			
			_OutlineLevel = (string)DBNull2Null(reader["OutlineLevel"]);
			
			_Key = (string)DBNull2Null(reader["Key"]);
			
			_Value = (string)DBNull2Null(reader["Value"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _IncidentBoxRuleId; }		     
		}
		
        
	    public virtual int IncidentBoxRuleId
	    
		{
			get
			{
				return _IncidentBoxRuleId;
			}
			
		}
		
	    public virtual int IncidentBoxId
	    
		{
			get
			{
				return _IncidentBoxId;
			}
			
			set
			{
				_IncidentBoxId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int RuleType
	    
		{
			get
			{
				return _RuleType;
			}
			
			set
			{
				_RuleType = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int OutlineIndex
	    
		{
			get
			{
				return _OutlineIndex;
			}
			
			set
			{
				_OutlineIndex = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string OutlineLevel
	    
		{
			get
			{
				return _OutlineLevel;
			}
			
			set
			{
				_OutlineLevel = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Key
	    
		{
			get
			{
				return _Key;
			}
			
			set
			{
				_Key = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Value
	    
		{
			get
			{
				return _Value;
			}
			
			set
			{
				_Value = value;
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
				DbHelper2.RunSp("mc_IncidentBoxRuleUpdate"
			    
			        , DbHelper2.mp("@IncidentBoxRuleId", SqlDbType.Int, _IncidentBoxRuleId)
			        
			        , DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, _IncidentBoxId)
			        
			        , DbHelper2.mp("@RuleType", SqlDbType.Int, _RuleType)
			        
			        , DbHelper2.mp("@OutlineIndex", SqlDbType.Int, _OutlineIndex)
			        
			        , DbHelper2.mp("@OutlineLevel", SqlDbType.NVarChar, 255, _OutlineLevel)
			        
			        , DbHelper2.mp("@Key", SqlDbType.NVarChar, 50, _Key)
			        
			        , DbHelper2.mp("@Value", SqlDbType.NText, _Value)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@IncidentBoxRuleId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_IncidentBoxRuleInsert", outPutKey
		            
		            , DbHelper2.mp("@IncidentBoxId", SqlDbType.Int, _IncidentBoxId)
		            
		            , DbHelper2.mp("@RuleType", SqlDbType.Int, _RuleType)
		            
		            , DbHelper2.mp("@OutlineIndex", SqlDbType.Int, _OutlineIndex)
		            
			        , DbHelper2.mp("@OutlineLevel", SqlDbType.NVarChar, 255, _OutlineLevel)
			        
			        , DbHelper2.mp("@Key", SqlDbType.NVarChar, 50, _Key)
			        
		            , DbHelper2.mp("@Value", SqlDbType.NText, _Value)
		             
		        );
		        
		        _IncidentBoxRuleId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_IncidentBoxRuleId);
		}
		
		public static void Delete(int IncidentBoxRuleId)
		{
		    DbHelper2.RunSp("mc_IncidentBoxRuleDelete", DbHelper2.mp("@IncidentBoxRuleId", SqlDbType.Int, IncidentBoxRuleId));
		}

		public static IncidentBoxRuleRow[] List(int IncidentBoxId)
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxRuleList",
					   DbHelper2.mp("@IncidentBoxId",SqlDbType.Int, IncidentBoxId)))
			{
				while(reader.Read())
				{
					retVal.Add(new IncidentBoxRuleRow(reader));
				}
			}
			return (IncidentBoxRuleRow[])retVal.ToArray(typeof(IncidentBoxRuleRow));
		}
    }
}
