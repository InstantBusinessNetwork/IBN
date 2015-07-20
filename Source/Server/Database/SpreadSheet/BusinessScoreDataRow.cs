
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class BusinessScoreDataRow
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
        
        int _BusinessScoreDataId;
        DateTime _Date;
        int _BusinessScoreId;
        int _ProjectId;
		int _Index;
        double _Value;
        
        
        public BusinessScoreDataRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public BusinessScoreDataRow(int BusinessScoreDataId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_BusinessScoreDataSelect", DbHelper2.mp("@BusinessScoreDataId", SqlDbType.Int, BusinessScoreDataId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "BusinessScoreData", "BusinessScoreDataId", BusinessScoreDataId));
			}
		}
		
		public BusinessScoreDataRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_BusinessScoreDataId = (int)DBNull2Null(reader["BusinessScoreDataId"]);
			
			_Date = (DateTime)DBNull2Null(reader["Date"]);
			
			_BusinessScoreId = (int)DBNull2Null(reader["BusinessScoreId"]);
			
			_ProjectId = (int)DBNull2Null(reader["ProjectId"]);

			_Index = (int)DBNull2Null(reader["ProjectId"]);
			
			_Value = (double)DBNull2Null(reader["Value"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _BusinessScoreDataId; }		     
		}
		
        
	    public virtual int BusinessScoreDataId
	    
		{
			get
			{
				return _BusinessScoreDataId;
			}
			
		}
		
	    public virtual DateTime Date
	    
		{
			get
			{
				return _Date;
			}
			
			set
			{
				_Date = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual int BusinessScoreId
	    
		{
			get
			{
				return _BusinessScoreId;
			}
			
			set
			{
				_BusinessScoreId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
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

		public virtual int Index
	    
		{
			get
			{
				return _Index;
			}
			
			set
			{
				_Index = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}

	    public virtual double Value
	    
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
				DbHelper2.RunSp("mc_BusinessScoreDataUpdate"
			    
			        , DbHelper2.mp("@BusinessScoreDataId", SqlDbType.Int, _BusinessScoreDataId)
			        
			        , DbHelper2.mp("@Date", SqlDbType.DateTime, _Date)
			        
			        , DbHelper2.mp("@BusinessScoreId", SqlDbType.Int, _BusinessScoreId)
			        
			        , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)

					, DbHelper2.mp("@Index", SqlDbType.Int, _Index)
			        
			        , DbHelper2.mp("@Value", SqlDbType.Float, _Value)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@BusinessScoreDataId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_BusinessScoreDataInsert", outPutKey
		            
		            , DbHelper2.mp("@Date", SqlDbType.DateTime, _Date)
		            
		            , DbHelper2.mp("@BusinessScoreId", SqlDbType.Int, _BusinessScoreId)
		            
		            , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)

					, DbHelper2.mp("@Index", SqlDbType.Int, _Index)
		            
		            , DbHelper2.mp("@Value", SqlDbType.Float, _Value)
		             
		        );
		        
		        _BusinessScoreDataId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_BusinessScoreDataId);
		}
		
		public static void Delete(int BusinessScoreDataId)
		{
		    DbHelper2.RunSp("mc_BusinessScoreDataDelete", DbHelper2.mp("@BusinessScoreDataId", SqlDbType.Int, BusinessScoreDataId));
		}

		public static void DeleteByProject(int ProjectId)
		{
			DbHelper2.RunSp("mc_BusinessScoreDataDeleteByProjectId", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId));
		}

		public static void Delete(int ProjectId, int Index)
		{
			DbHelper2.RunSp("mc_BusinessScoreDataDeleteByProjectIdAndIndex", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@Index", SqlDbType.Int, Index));
		}

		public static BusinessScoreDataRow[] List()
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_BusinessScoreDataList"))
			{
				while(reader.Read())
				{
					retVal.Add(new BusinessScoreDataRow(reader));
				}
			}
			return (BusinessScoreDataRow[])retVal.ToArray(typeof(BusinessScoreDataRow));
		}

		public static BusinessScoreDataRow[] List(int Index)
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_BusinessScoreDataListByIndex",
					  DbHelper2.mp("@Index", SqlDbType.Int, Index)))
			{
				while(reader.Read())
				{
					retVal.Add(new BusinessScoreDataRow(reader));
				}
			}
			return (BusinessScoreDataRow[])retVal.ToArray(typeof(BusinessScoreDataRow));
		}
    }
}
