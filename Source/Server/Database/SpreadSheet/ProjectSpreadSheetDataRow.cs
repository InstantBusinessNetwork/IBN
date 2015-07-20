
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class ProjectSpreadSheetDataRow
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
        
        int _ProjectSpreadSheetDataId;
        int _ProjectSpreadSheetId;
        int _Index;
        string _ColumnId;
        string _RowId;
        double _Value;
        int _CellType;
        
        
        public ProjectSpreadSheetDataRow()
        {
            _state = DataRowState.Added;
            
            
            _Index = 0;  
            
            _CellType = 1;  
            
        }
        
		public ProjectSpreadSheetDataRow(int ProjectSpreadSheetDataId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectSpreadSheetDataSelect", DbHelper2.mp("@ProjectSpreadSheetDataId", SqlDbType.Int, ProjectSpreadSheetDataId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "ProjectSpreadSheetData", "ProjectSpreadSheetDataId", ProjectSpreadSheetDataId));
			}
		}
		
		public ProjectSpreadSheetDataRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		/// <summary>
		/// Loads the specified reader.
		/// </summary>
		/// <param name="reader">The reader.</param>
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ProjectSpreadSheetDataId = (int)DBNull2Null(reader["ProjectSpreadSheetDataId"]);
			
			_ProjectSpreadSheetId = (int)DBNull2Null(reader["ProjectSpreadSheetId"]);
			
			_Index = (int)DBNull2Null(reader["Index"]);
			
			_ColumnId = (string)DBNull2Null(reader["ColumnId"]);
			
			_RowId = (string)DBNull2Null(reader["RowId"]);
			
			_Value = (double)DBNull2Null(reader["Value"]);
			
			_CellType = (int)DBNull2Null(reader["CellType"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _ProjectSpreadSheetDataId; }		     
		}
		
        
	    public virtual int ProjectSpreadSheetDataId
	    
		{
			get
			{
				return _ProjectSpreadSheetDataId;
			}
			
		}
		
	    public virtual int ProjectSpreadSheetId
	    
		{
			get
			{
				return _ProjectSpreadSheetId;
			}
			
			set
			{
				_ProjectSpreadSheetId = value;
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
		
	    public virtual string ColumnId
	    
		{
			get
			{
				return _ColumnId;
			}
			
			set
			{
				_ColumnId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string RowId
	    
		{
			get
			{
				return _RowId;
			}
			
			set
			{
				_RowId = value;
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
		
	    public virtual int CellType
	    
		{
			get
			{
				return _CellType;
			}
			
			set
			{
				_CellType = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		/// <summary>
		/// Updates this instance.
		/// </summary>
		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DbHelper2.RunSp("mc_ProjectSpreadSheetDataUpdate"
			    
			        , DbHelper2.mp("@ProjectSpreadSheetDataId", SqlDbType.Int, _ProjectSpreadSheetDataId)
			        
			        , DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, _ProjectSpreadSheetId)
			        
			        , DbHelper2.mp("@Index", SqlDbType.Int, _Index)
			        
			        , DbHelper2.mp("@ColumnId", SqlDbType.NVarChar, 50, _ColumnId)
			        
			        , DbHelper2.mp("@RowId", SqlDbType.NVarChar, 250, _RowId)
			        
			        , DbHelper2.mp("@Value", SqlDbType.Float, _Value)
			        
			        , DbHelper2.mp("@CellType", SqlDbType.Int, _CellType)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@ProjectSpreadSheetDataId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_ProjectSpreadSheetDataInsert", outPutKey
		            
		            , DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, _ProjectSpreadSheetId)
		            
		            , DbHelper2.mp("@Index", SqlDbType.Int, _Index)
		            
			        , DbHelper2.mp("@ColumnId", SqlDbType.NVarChar, 50, _ColumnId)
			        
			        , DbHelper2.mp("@RowId", SqlDbType.NVarChar, 250, _RowId)
			        
		            , DbHelper2.mp("@Value", SqlDbType.Float, _Value)
		            
		            , DbHelper2.mp("@CellType", SqlDbType.Int, _CellType)
		             
		        );
		        
		        _ProjectSpreadSheetDataId = (int)outPutKey.Value;
			}
		}
		
		/// <summary>
		/// Deletes this instance.
		/// </summary>
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_ProjectSpreadSheetDataId);
		}
		
		/// <summary>
		/// Deletes the specified project spread sheet data id.
		/// </summary>
		/// <param name="ProjectSpreadSheetDataId">The project spread sheet data id.</param>
		public static void Delete(int ProjectSpreadSheetDataId)
		{
		    DbHelper2.RunSp("mc_ProjectSpreadSheetDataDelete", DbHelper2.mp("@ProjectSpreadSheetDataId", SqlDbType.Int, ProjectSpreadSheetDataId));
		}

		public static void Delete(int ProjectId, int Index)
		{
			DbHelper2.RunSp("mc_ProjectSpreadSheetDataDeleteByProjectIdAndIndex", 
				DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId),
				DbHelper2.mp("@Index", SqlDbType.Int, Index));
		}

		/// <summary>
		/// Updates the specified project spread sheet id.
		/// </summary>
		/// <param name="ProjectSpreadSheetId">The project spread sheet id.</param>
		/// <param name="Index">The index.</param>
		/// <param name="ColumnId">The column id.</param>
		/// <param name="RowId">The row id.</param>
		/// <param name="Value">The value.</param>
		/// <param name="CellType">Type of the cell.</param>
		public static void Update(int ProjectSpreadSheetId,
			int Index,
			string ColumnId,
			string RowId,
			double Value,
			int CellType)
		{
				DbHelper2.RunSp("mc_ProjectSpreadSheetDataUpdate2"
			    
			        , DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, ProjectSpreadSheetId)
			        
			        , DbHelper2.mp("@Index", SqlDbType.Int, Index)
			        
			        , DbHelper2.mp("@ColumnId", SqlDbType.NVarChar, 50, ColumnId)
			        
			        , DbHelper2.mp("@RowId", SqlDbType.NVarChar, 250, RowId)
			        
			        , DbHelper2.mp("@Value", SqlDbType.Float, Value)
			        
			        , DbHelper2.mp("@CellType", SqlDbType.Int, CellType));
		}

		/// <summary>
		/// Lists the specified project spread sheet id.
		/// </summary>
		/// <param name="ProjectSpreadSheetId">The project spread sheet id.</param>
		/// <param name="Index">The index.</param>
		/// <returns></returns>
		public static ProjectSpreadSheetDataRow[] List(int ProjectSpreadSheetId, int Index)
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectSpreadSheetDataListByProjectSpreadSheetIdAndIndex", 
					  DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, ProjectSpreadSheetId),
					  DbHelper2.mp("@Index", SqlDbType.Int, Index)))
			{
				while(reader.Read())
				{
					retVal.Add(new ProjectSpreadSheetDataRow(reader));
				}
			}
			return (ProjectSpreadSheetDataRow[])retVal.ToArray(typeof(ProjectSpreadSheetDataRow));
		}

		/// <summary>
		/// Lists the current by project id.
		/// </summary>
		/// <param name="ProjectId">The project id.</param>
		/// <returns></returns>
		public static ProjectSpreadSheetDataRow[] ListCurrentByProjectId(int ProjectId)
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectSpreadSheetDataListCurrentByProjectId", 
					  DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId)))
			{
				while(reader.Read())
				{
					retVal.Add(new ProjectSpreadSheetDataRow(reader));
				}
			}
			return (ProjectSpreadSheetDataRow[])retVal.ToArray(typeof(ProjectSpreadSheetDataRow));
		}
    }
}
