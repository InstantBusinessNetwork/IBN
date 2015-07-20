
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailMessageLogSettingsRow
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
        
        int _EMailMessageLogSettingId;
        bool _IsActive;
        int _Period;
        
        
        public EMailMessageLogSettingsRow()
        {
            _state = DataRowState.Added;
            
            
            _IsActive = false;  
            
            _Period = 7;  
            
        }
        
		public EMailMessageLogSettingsRow(int EMailMessageLogSettingId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageLogSettingsSelect", DbHelper2.mp("@EMailMessageLogSettingId", SqlDbType.Int, EMailMessageLogSettingId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailMessageLogSettings", "EMailMessageLogSettingId", EMailMessageLogSettingId));
			}
		}
		
		public EMailMessageLogSettingsRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailMessageLogSettingId = (int)DBNull2Null(reader["EMailMessageLogSettingId"]);
			
			_IsActive = (bool)DBNull2Null(reader["IsActive"]);
			
			_Period = (int)DBNull2Null(reader["Period"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _EMailMessageLogSettingId; }		     
		}
		
        
	    public virtual int EMailMessageLogSettingId
	    
		{
			get
			{
				return _EMailMessageLogSettingId;
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
		
	    public virtual int Period
	    
		{
			get
			{
				return _Period;
			}
			
			set
			{
				_Period = value;
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
				DbHelper2.RunSp("mc_EMailMessageLogSettingsUpdate"
			    
			        , DbHelper2.mp("@EMailMessageLogSettingId", SqlDbType.Int, _EMailMessageLogSettingId)
			        
			        , DbHelper2.mp("@IsActive", SqlDbType.Bit, _IsActive)
			        
			        , DbHelper2.mp("@Period", SqlDbType.Int, _Period)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailMessageLogSettingId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailMessageLogSettingsInsert", outPutKey
		            
		            , DbHelper2.mp("@IsActive", SqlDbType.Bit, _IsActive)
		            
		            , DbHelper2.mp("@Period", SqlDbType.Int, _Period)
		             
		        );
		        
		        _EMailMessageLogSettingId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailMessageLogSettingId);
		}
		
		public static void Delete(int EMailMessageLogSettingId)
		{
		    DbHelper2.RunSp("mc_EMailMessageLogSettingsDelete", DbHelper2.mp("@EMailMessageLogSettingId", SqlDbType.Int, EMailMessageLogSettingId));
		}

		public static EMailMessageLogSettingsRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailMessageLogSettingsList"))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailMessageLogSettingsRow(reader));
				}
			}
			return (EMailMessageLogSettingsRow[])retVal.ToArray(typeof(EMailMessageLogSettingsRow));
		}
    }
}
