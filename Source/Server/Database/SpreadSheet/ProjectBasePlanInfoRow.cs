
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class ProjectBasePlanInfoRow
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
        
        int _ProjectBasePlanInfoId;
        int _ProjectId;
        int _BasePlanSlotId;
        DateTime _Created = DateTime.UtcNow;
        
        
        public ProjectBasePlanInfoRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public ProjectBasePlanInfoRow(int ProjectBasePlanInfoId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectBasePlanInfoSelect", DbHelper2.mp("@ProjectBasePlanInfoId", SqlDbType.Int, ProjectBasePlanInfoId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "ProjectBasePlanInfo", "ProjectBasePlanInfoId", ProjectBasePlanInfoId));
			}
		}
		
		public ProjectBasePlanInfoRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ProjectBasePlanInfoId = (int)DBNull2Null(reader["ProjectBasePlanInfoId"]);
			
			_ProjectId = (int)DBNull2Null(reader["ProjectId"]);
			
			_BasePlanSlotId = (int)DBNull2Null(reader["BasePlanSlotId"]);
			
			_Created = (DateTime)DBNull2Null(reader["Created"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _ProjectBasePlanInfoId; }		     
		}
		
        
	    public virtual int ProjectBasePlanInfoId
	    
		{
			get
			{
				return _ProjectBasePlanInfoId;
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
		
	    public virtual int BasePlanSlotId
	    
		{
			get
			{
				return _BasePlanSlotId;
			}
			
			set
			{
				_BasePlanSlotId = value;
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
				DbHelper2.RunSp("mc_ProjectBasePlanInfoUpdate"
			    
			        , DbHelper2.mp("@ProjectBasePlanInfoId", SqlDbType.Int, _ProjectBasePlanInfoId)
			        
			        , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)
			        
			        , DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, _BasePlanSlotId)
			        
			        , DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@ProjectBasePlanInfoId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_ProjectBasePlanInfoInsert", outPutKey
		            
		            , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)
		            
		            , DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, _BasePlanSlotId)
		            
		            , DbHelper2.mp("@Created", SqlDbType.DateTime, _Created)
		             
		        );
		        
		        _ProjectBasePlanInfoId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_ProjectBasePlanInfoId);
		}
		
		public static void Delete(int ProjectBasePlanInfoId)
		{
		    DbHelper2.RunSp("mc_ProjectBasePlanInfoDelete", DbHelper2.mp("@ProjectBasePlanInfoId", SqlDbType.Int, ProjectBasePlanInfoId));
		}

		public static ProjectBasePlanInfoRow[] List(int ProjectId)
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectBasePlanInfoListByProjectId",
					  DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId)))
			{
				while(reader.Read())
				{
					retVal.Add(new ProjectBasePlanInfoRow(reader));
				}
			}
			return (ProjectBasePlanInfoRow[])retVal.ToArray(typeof(ProjectBasePlanInfoRow));
		}
    }
}
