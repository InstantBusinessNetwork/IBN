
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.EMail
{
    public class EMailIncidentMappingRow
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
        
        int _EMailIncidentMappingId;
        string _Name;
        string _Type;
        string _UserControl;
        
        
        public EMailIncidentMappingRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public EMailIncidentMappingRow(int EMailIncidentMappingId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailIncidentMappingSelect", DbHelper2.mp("@EMailIncidentMappingId", SqlDbType.Int, EMailIncidentMappingId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "EMailIncidentMapping", "EMailIncidentMappingId", EMailIncidentMappingId));
			}
		}
		
		public EMailIncidentMappingRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_EMailIncidentMappingId = (int)DBNull2Null(reader["EMailIncidentMappingId"]);
			
			_Name = (string)DBNull2Null(reader["Name"]);
			
			_Type = (string)DBNull2Null(reader["Type"]);
			
			_UserControl = (string)DBNull2Null(reader["UserControl"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _EMailIncidentMappingId; }		     
		}
		
        
	    public virtual int EMailIncidentMappingId
	    
		{
			get
			{
				return _EMailIncidentMappingId;
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
		
	    public virtual string UserControl
	    
		{
			get
			{
				return _UserControl;
			}
			
			set
			{
				_UserControl = value;
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
				DbHelper2.RunSp("mc_EMailIncidentMappingUpdate"
			    
			        , DbHelper2.mp("@EMailIncidentMappingId", SqlDbType.Int, _EMailIncidentMappingId)
			        
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)
			        
			        , DbHelper2.mp("@Type", SqlDbType.NVarChar, 1024, _Type)
			        
			        , DbHelper2.mp("@UserControl", SqlDbType.NVarChar, 1024, _UserControl)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@EMailIncidentMappingId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_EMailIncidentMappingInsert", outPutKey
		            
			        , DbHelper2.mp("@Name", SqlDbType.NVarChar, 255, _Name)
			        
			        , DbHelper2.mp("@Type", SqlDbType.NVarChar, 1024, _Type)
			        
			        , DbHelper2.mp("@UserControl", SqlDbType.NVarChar, 1024, _UserControl)
			         
		        );
		        
		        _EMailIncidentMappingId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_EMailIncidentMappingId);
		}
		
		public static void Delete(int EMailIncidentMappingId)
		{
		    DbHelper2.RunSp("mc_EMailIncidentMappingDelete", DbHelper2.mp("@EMailIncidentMappingId", SqlDbType.Int, EMailIncidentMappingId));
		}

		public static EMailIncidentMappingRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_EMailIncidentMappingList"))
			{
				while(reader.Read())
				{
					retVal.Add(new EMailIncidentMappingRow(reader));
				}
			}
			return (EMailIncidentMappingRow[])retVal.ToArray(typeof(EMailIncidentMappingRow));
		}
    }
}
