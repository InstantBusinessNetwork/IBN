
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class IncidentBoxRuleFunctionRow
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
        
        int _IncidentBoxRuleFunctionId;
        string _Name;
        string _Description;
        string _Type;
        string _MethodName;
        
        
        public IncidentBoxRuleFunctionRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public IncidentBoxRuleFunctionRow(int IncidentBoxRuleFunctionId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxRuleFunctionSelect", DbHelper2.mp("@IncidentBoxRuleFunctionId", SqlDbType.Int, IncidentBoxRuleFunctionId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "IncidentBoxRuleFunction", "IncidentBoxRuleFunctionId", IncidentBoxRuleFunctionId));
			}
		}
		
		public IncidentBoxRuleFunctionRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_IncidentBoxRuleFunctionId = (int)DBNull2Null(reader["IncidentBoxRuleFunctionId"]);
			
			_Name = (string)DBNull2Null(reader["Name"]);
			
			_Description = (string)DBNull2Null(reader["Description"]);
			
			_Type = (string)DBNull2Null(reader["Type"]);
			
			_MethodName = (string)DBNull2Null(reader["MethodName"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _IncidentBoxRuleFunctionId; }		     
		}
		
        
	    public virtual int IncidentBoxRuleFunctionId
	    
		{
			get
			{
				return _IncidentBoxRuleFunctionId;
			}
			
		}
		
	    public virtual string Name
	    
		{
			get
			{
				return _Name;
			}
			
			set
			{
				_Name = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Description
	    
		{
			get
			{
				return _Description;
			}
			
			set
			{
				_Description = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string Type
	    
		{
			get
			{
				return _Type;
			}
			
			set
			{
				_Type = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string MethodName
	    
		{
			get
			{
				return _MethodName;
			}
			
			set
			{
				_MethodName = value;
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
				DbHelper2.RunSp("mc_IncidentBoxRuleFunctionUpdate"
			    
			        , DbHelper2.mp("@IncidentBoxRuleFunctionId", SqlDbType.Int, _IncidentBoxRuleFunctionId)
			        
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)
			        
			        , DbHelper2.mp("@Description", SqlDbType.NText, _Description)
			        
			        , DbHelper2.mp("@Type", SqlDbType.NVarChar, 255, _Type)
			        
			        , DbHelper2.mp("@MethodName", SqlDbType.NVarChar, 255, _MethodName)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@IncidentBoxRuleFunctionId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_IncidentBoxRuleFunctionInsert", outPutKey
		            
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)
			        
		            , DbHelper2.mp("@Description", SqlDbType.NText, _Description)
		            
			        , DbHelper2.mp("@Type", SqlDbType.NVarChar, 255, _Type)
			        
			        , DbHelper2.mp("@MethodName", SqlDbType.NVarChar, 255, _MethodName)
			         
		        );
		        
		        _IncidentBoxRuleFunctionId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_IncidentBoxRuleFunctionId);
		}
		
		public static void Delete(int IncidentBoxRuleFunctionId)
		{
		    DbHelper2.RunSp("mc_IncidentBoxRuleFunctionDelete", DbHelper2.mp("@IncidentBoxRuleFunctionId", SqlDbType.Int, IncidentBoxRuleFunctionId));
		}

		public static IncidentBoxRuleFunctionRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_IncidentBoxRuleFunctionList"))
			{
				while(reader.Read())
				{
					retVal.Add(new IncidentBoxRuleFunctionRow(reader));
				}
			}
			return (IncidentBoxRuleFunctionRow[])retVal.ToArray(typeof(IncidentBoxRuleFunctionRow));
		}
    }
}
