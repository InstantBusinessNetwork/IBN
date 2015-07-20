
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class BusinessScoreRow
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
        
        int _BusinessScoreId;
        string _Key;
        string _Name;
        
        
        public BusinessScoreRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public BusinessScoreRow(int BusinessScoreId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_BusinessScoreSelect", DbHelper2.mp("@BusinessScoreId", SqlDbType.Int, BusinessScoreId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "BusinessScore", "BusinessScoreId", BusinessScoreId));
			}
		}
		
		public BusinessScoreRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_BusinessScoreId = (int)DBNull2Null(reader["BusinessScoreId"]);
			
			_Key = (string)DBNull2Null(reader["Key"]);
			
			_Name = (string)DBNull2Null(reader["Name"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _BusinessScoreId; }		     
		}
		
        
	    public virtual int BusinessScoreId
	    
		{
			get
			{
				return _BusinessScoreId;
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
		
		#endregion
	
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_BusinessScoreUpdate"
			    
			        , DbHelper2.mp("@BusinessScoreId", SqlDbType.Int, _BusinessScoreId)
			        
			        , DbHelper2.mp("@Key", SqlDbType.NVarChar, 15, _Key)
			        
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@BusinessScoreId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_BusinessScoreInsert", outPutKey
		            
			        , DbHelper2.mp("@Key", SqlDbType.NVarChar, 15, _Key)
			        
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)
			         
		        );
		        
		        _BusinessScoreId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_BusinessScoreId);
		}
		
		public static void Delete(int BusinessScoreId)
		{
		    DbHelper2.RunSp("mc_BusinessScoreDelete", DbHelper2.mp("@BusinessScoreId", SqlDbType.Int, BusinessScoreId));
		}

		public static BusinessScoreRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_BusinessScoreList"))
			{
				while(reader.Read())
				{
					retVal.Add(new BusinessScoreRow(reader));
				}
			}
			return (BusinessScoreRow[])retVal.ToArray(typeof(BusinessScoreRow));
		}
    }
}
