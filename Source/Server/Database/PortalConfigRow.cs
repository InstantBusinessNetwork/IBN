
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;


namespace Mediachase.IBN.Database
{
    public class PortalConfigRow
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
        
        int _SettingId;
        string _Key;
        string _Value;
        
        
        public PortalConfigRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public PortalConfigRow(int SettingId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_PortalConfigSelect", DbHelper2.mp("@SettingId", SqlDbType.Int, SettingId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "PortalConfig", "SettingId", SettingId));
			}
		}
		
		public PortalConfigRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_SettingId = (int)DBNull2Null(reader["SettingId"]);
			
			_Key = (string)DBNull2Null(reader["Key"]);
			
			_Value = (string)DBNull2Null(reader["Value"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _SettingId; }		     
		}
		
        
	    public virtual int SettingId
	    
		{
			get
			{
				return _SettingId;
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
				DbHelper2.RunSp("mc_PortalConfigUpdate"
			    
			        , DbHelper2.mp("@SettingId", SqlDbType.Int, _SettingId)

                    , DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, _Key)
			        
			        , DbHelper2.mp("@Value", SqlDbType.NText, _Value)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@SettingId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_PortalConfigInsert", outPutKey
		            
		            , DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, _Key)
		            
		            , DbHelper2.mp("@Value", SqlDbType.NText, _Value)
		             
		        );
		        
		        _SettingId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_SettingId);
		}
		
		public static void Delete(int SettingId)
		{
		    DbHelper2.RunSp("mc_PortalConfigDelete", DbHelper2.mp("@SettingId", SqlDbType.Int, SettingId));
		}

        public static PortalConfigRow Get(string Key)
        {
            using (IDataReader reader = DbHelper2.RunSpDataReader("mc_PortalConfigSelectByKey",
                DbHelper2.mp("@Key", SqlDbType.NVarChar, 100, Key)))
			{
				if(reader.Read())
				{
                    return new PortalConfigRow(reader);
				}
			}

			return null;
        }

        public static PortalConfigRow[] List()
        {
            List<PortalConfigRow> retVal = new List<PortalConfigRow>(); 

            using (IDataReader reader = DbHelper2.RunSpDataReader("mc_PortalConfigList"))
            {
                while (reader.Read())
                {
                    retVal.Add(new PortalConfigRow(reader));
                }
            }

            return retVal.ToArray();
        }
    }
}
