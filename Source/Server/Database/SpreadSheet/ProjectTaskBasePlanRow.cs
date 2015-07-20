
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class ProjectTaskBasePlanRow
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
        
        int _ProjectTaskBasePlanId;
        int _ProjectId;
        int _BasePlanSlotId;
        int _TaskId;
        DateTime _StartDate;
        
        
        public ProjectTaskBasePlanRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public ProjectTaskBasePlanRow(int ProjectTaskBasePlanId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectTaskBasePlanSelect", DbHelper2.mp("@ProjectTaskBasePlanId", SqlDbType.Int, ProjectTaskBasePlanId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "ProjectTaskBasePlan", "ProjectTaskBasePlanId", ProjectTaskBasePlanId));
			}
		}
		
		public ProjectTaskBasePlanRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ProjectTaskBasePlanId = (int)DBNull2Null(reader["ProjectTaskBasePlanId"]);
			
			_ProjectId = (int)DBNull2Null(reader["ProjectId"]);
			
			_BasePlanSlotId = (int)DBNull2Null(reader["BasePlanSlotId"]);
			
			_TaskId = (int)DBNull2Null(reader["TaskId"]);
			
			_StartDate = (DateTime)DBNull2Null(reader["StartDate"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _ProjectTaskBasePlanId; }		     
		}
		
        
	    public virtual int ProjectTaskBasePlanId
	    
		{
			get
			{
				return _ProjectTaskBasePlanId;
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
		
	    public virtual int TaskId
	    
		{
			get
			{
				return _TaskId;
			}
			
			set
			{
				_TaskId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual DateTime StartDate
	    
		{
			get
			{
				return _StartDate;
			}
			
			set
			{
				_StartDate = value;
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
				DbHelper2.RunSp("mc_ProjectTaskBasePlanUpdate"
			    
			        , DbHelper2.mp("@ProjectTaskBasePlanId", SqlDbType.Int, _ProjectTaskBasePlanId)
			        
			        , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)
			        
			        , DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, _BasePlanSlotId)
			        
			        , DbHelper2.mp("@TaskId", SqlDbType.Int, _TaskId)
			        
			        , DbHelper2.mp("@StartDate", SqlDbType.DateTime, _StartDate)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@ProjectTaskBasePlanId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_ProjectTaskBasePlanInsert", outPutKey
		            
		            , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)
		            
		            , DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, _BasePlanSlotId)
		            
		            , DbHelper2.mp("@TaskId", SqlDbType.Int, _TaskId)
		            
		            , DbHelper2.mp("@StartDate", SqlDbType.DateTime, _StartDate)
		             
		        );
		        
		        _ProjectTaskBasePlanId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_ProjectTaskBasePlanId);
		}
		
		public static void Delete(int ProjectTaskBasePlanId)
		{
		    DbHelper2.RunSp("mc_ProjectTaskBasePlanDelete", DbHelper2.mp("@ProjectTaskBasePlanId", SqlDbType.Int, ProjectTaskBasePlanId));
		}

		public static void DeleteByProject(int ProjectId)
		{
			DbHelper2.RunSp("mc_ProjectTaskBasePlanDeleteByProjectId", DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}

		public static ProjectTaskBasePlanRow[] List(int ProjectId, int BasePlanSlotId)
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectTaskBasePlanListByProjectIdAndBasePlanSlotId",
					  DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
					  DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, BasePlanSlotId)))
			{
				while(reader.Read())
				{
					retVal.Add(new ProjectTaskBasePlanRow(reader));
				}
			}
			return (ProjectTaskBasePlanRow[])retVal.ToArray(typeof(ProjectTaskBasePlanRow));
		}

		public static void Fill(int ProjectId, int BasePlanSlotId)
		{
			DbHelper2.RunSp("mc_ProjectTaskBasePlanFill",
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
					  DbHelper2.mp("@BasePlanSlotId", SqlDbType.Int, BasePlanSlotId));
		}
    }
}
