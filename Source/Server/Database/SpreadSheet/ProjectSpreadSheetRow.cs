
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

using Mediachase.IBN.Database;

namespace Mediachase.IBN.Database.SpreadSheet
{
    public class ProjectSpreadSheetRow
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
        
        int _ProjectSpreadSheetId;
        int _ProjectId;
		int _DocumentType;
        string _BaseTemplateName = string.Empty;
        string _UserRows = string.Empty;
        
        
        public ProjectSpreadSheetRow()
        {
            _state = DataRowState.Added;
            
            
        }
        
		public ProjectSpreadSheetRow(int ProjectSpreadSheetId)
		{
			using (IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectSpreadSheetSelect", DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, ProjectSpreadSheetId)))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "ProjectSpreadSheet", "ProjectSpreadSheetId", ProjectSpreadSheetId));
			}
		}
		
		public ProjectSpreadSheetRow(IDataReader reader)
		{
		   Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ProjectSpreadSheetId = (int)DBNull2Null(reader["ProjectSpreadSheetId"]);
			
			_ProjectId = (int)DBNull2Null(reader["ProjectId"]);

			_DocumentType = (int)DBNull2Null(reader["DocumentType"]);
			
			_BaseTemplateName = (string)DBNull2Null(reader["BaseTemplateName"]);
			
			_UserRows = (string)DBNull2Null(reader["UserRows"]);
			
		}
	
		#region Public Properties
		
		public virtual int PrimaryKeyId
		{
            get { return _ProjectSpreadSheetId; }		     
		}
		
        
	    public virtual int ProjectSpreadSheetId
	    
		{
			get
			{
				return _ProjectSpreadSheetId;
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

		public virtual int DocumentType
	    
		{
			get
			{
				return _DocumentType;
			}
			
			set
			{
				_DocumentType = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string BaseTemplateName
	    
		{
			get
			{
				return _BaseTemplateName;
			}
			
			set
			{
				_BaseTemplateName = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
	    public virtual string UserRows
	    
		{
			get
			{
				return _UserRows;
			}
			
			set
			{
				_UserRows = value;
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
				DbHelper2.RunSp("mc_ProjectSpreadSheetUpdate"
			    
			        , DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, _ProjectSpreadSheetId)
			        
			        , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)

					, DbHelper2.mp("@DocumentType", SqlDbType.Int, _DocumentType)
			        
			        , DbHelper2.mp("@BaseTemplateName", SqlDbType.NVarChar, 255, _BaseTemplateName)
			        
			        , DbHelper2.mp("@UserRows", SqlDbType.NText, _UserRows)
			         
		        );
			}
			else if (_state == DataRowState.Added) 
			{
				SqlParameter outPutKey = new SqlParameter("@ProjectSpreadSheetId", SqlDbType.Int);
				outPutKey.Direction = ParameterDirection.Output;

				DbHelper2.RunSp("mc_ProjectSpreadSheetInsert", outPutKey
		            
		            , DbHelper2.mp("@ProjectId", SqlDbType.Int, _ProjectId)

					, DbHelper2.mp("@DocumentType", SqlDbType.Int, _DocumentType)
		            
			        , DbHelper2.mp("@BaseTemplateName", SqlDbType.NVarChar, 255, _BaseTemplateName)
			        
		            , DbHelper2.mp("@UserRows", SqlDbType.NText, _UserRows)
		             
		        );
		        
		        _ProjectSpreadSheetId = (int)outPutKey.Value;
			}
		}
		
		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(_ProjectSpreadSheetId);
		}
		
		public static void Delete(int ProjectSpreadSheetId)
		{
		    DbHelper2.RunSp("mc_ProjectSpreadSheetDelete", DbHelper2.mp("@ProjectSpreadSheetId", SqlDbType.Int, ProjectSpreadSheetId));
		}

		public static ProjectSpreadSheetRow[] List(int ProjectId)
		{
			ArrayList retVal = new ArrayList();
			using(IDataReader reader = DbHelper2.RunSpDataReader("mc_ProjectSpreadSheetListByProjectId", 
					  DbHelper2.mp("@ProjectId", SqlDbType.Int, ProjectId)))
			{
				while(reader.Read())
				{
					retVal.Add(new ProjectSpreadSheetRow(reader));
				}
			}
			return (ProjectSpreadSheetRow[])retVal.ToArray(typeof(ProjectSpreadSheetRow));
		}
    }
}
