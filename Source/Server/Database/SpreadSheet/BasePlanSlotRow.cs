
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class BasePlanSlotRow
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
        
        int _BasePlanSlotId;
        string _Name;
        bool _IsDefault;
        
        
        public BasePlanSlotRow()
        {
            _state = DataRowState.Added;
            
            
            _IsDefault = false;  
            
        }
        
		public BasePlanSlotRow(int BasePlanSlotId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_BasePlanSlotSelect", DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, BasePlanSlotId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "BasePlanSlot", "BasePlanSlotId", BasePlanSlotId));
			}
		}
		
		public BasePlanSlotRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_BasePlanSlotId = (int)DBNull2Null(reader["BasePlanSlotId"]);
			
			_Name = (string)DBNull2Null(reader["Name"]);
			
			_IsDefault = (bool)DBNull2Null(reader["IsDefault"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _BasePlanSlotId; }		     
		}
		
        
	    public virtual int BasePlanSlotId
	    
		{
			get
			{
				return _BasePlanSlotId;
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
		
	    public virtual bool IsDefault
	    
		{
			get
			{
				return _IsDefault;
			}
			
			set
			{
				_IsDefault = value;
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
				DbHelper2.RunSp("mc_BasePlanSlotUpdate"
			    
			        , DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, _BasePlanSlotId)
			        
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 50, _Name)
			        
			        , DbHelper2.mp("@IsDefault", SqlDbType.Bit, _IsDefault)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@BasePlanSlotId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_BasePlanSlotInsert", outPutKey
		            
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 50, _Name)
			        
		            , DbHelper2.mp("@IsDefault", SqlDbType.Bit, _IsDefault)
		             
		        );
		        
		        _BasePlanSlotId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_BasePlanSlotId);
		}
		
		public static void Delete(int BasePlanSlotId)
		{
		    DbHelper2.RunSp("mc_BasePlanSlotDelete", DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, BasePlanSlotId));
		}

		public static BasePlanSlotRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_BasePlanSlotList"))
			{
				while(reader.Read())
				{
					retVal.Add(new BasePlanSlotRow(reader));
				}
			}
			return (BasePlanSlotRow[])retVal.ToArray(typeof(BasePlanSlotRow));
		}
    }
}
