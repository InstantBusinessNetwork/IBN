
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IBN.Database.Assignments
{
public partial class WorkflowParticipantRow
	{
		private DataRowState _state = DataRowState.Unchanged;
		
		#region Column's name const
		
			public const string ColumnWorkflowParticipantId = "WorkflowParticipantId";

			public const string ColumnUserId = "UserId";

			public const string ColumnWorkflowInstanceId = "WorkflowInstanceId";

			public const string ColumnObjectId = "ObjectId";

			public const string ColumnObjectType = "ObjectType";

		#endregion
		

		Nullable<int> _WorkflowParticipantId;
		int _UserId;
		Guid _WorkflowInstanceId;
		int _ObjectId;
		int _ObjectType;


		public WorkflowParticipantRow()
		{
			_state = DataRowState.Added;


		}

		public WorkflowParticipantRow(int primaryKeyId)
		{
			using (IDataReader reader = DataHelper.Select("WorkflowParticipant", 
							"WorkflowParticipantId", 
							SqlDbType.Int,
							primaryKeyId))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Record with {0}.{1}={2} not found", "WorkflowParticipant", "WorkflowParticipantId", primaryKeyId), "primaryKeyId");
			}
		}
		
		public WorkflowParticipantRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_WorkflowParticipantId = (int)SqlHelper.DBNull2Null(reader["WorkflowParticipantId"]);
			
			_UserId = (int)SqlHelper.DBNull2Null(reader["UserId"]);
			
			_WorkflowInstanceId = (Guid)SqlHelper.DBNull2Null(reader["WorkflowInstanceId"]);
			
			_ObjectId = (int)SqlHelper.DBNull2Null(reader["ObjectId"]);
			
			_ObjectType = (int)SqlHelper.DBNull2Null(reader["ObjectType"]);
			
		
			OnLoad(reader);
		}
		
		protected virtual void OnLoad(IDataReader reader)
		{
		}

		#region Public Properties

		public virtual Nullable<int> PrimaryKeyId
		{
			get { return _WorkflowParticipantId; }
			set { _WorkflowParticipantId = value; }
		}
		
        
		public virtual int WorkflowParticipantId
	    
		{
			get
			{
				
				return (int)_WorkflowParticipantId;
				
			}
			
		}
		
		public virtual int UserId
	    
		{
			get
			{
				
				return _UserId;
				
			}
			
			set
			{
				_UserId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Guid WorkflowInstanceId
	    
		{
			get
			{
				
				return _WorkflowInstanceId;
				
			}
			
			set
			{
				_WorkflowInstanceId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual int ObjectId
	    
		{
			get
			{
				
				return _ObjectId;
				
			}
			
			set
			{
				_ObjectId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual int ObjectType
	    
		{
			get
			{
				
				return _ObjectType;
				
			}
			
			set
			{
				_ObjectType = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public static WorkflowParticipantRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("WorkflowParticipant"))
			{
				while (reader.Read())
				{
					list.Add(new WorkflowParticipantRow(reader));
				}
			}
			return (WorkflowParticipantRow[])list.ToArray(typeof(WorkflowParticipantRow));
		}
	
		public static WorkflowParticipantRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("WorkflowParticipant", sorting))
			{
				while (reader.Read())
				{
					list.Add(new WorkflowParticipantRow(reader));
				}
			}
			return (WorkflowParticipantRow[])list.ToArray(typeof(WorkflowParticipantRow));
		}

		public static WorkflowParticipantRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("WorkflowParticipant", filters))
			{
				while (reader.Read())
				{
					list.Add(new WorkflowParticipantRow(reader));
				}
			}
			return (WorkflowParticipantRow[])list.ToArray(typeof(WorkflowParticipantRow));
		}

		public static WorkflowParticipantRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("WorkflowParticipant", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new WorkflowParticipantRow(reader));
			  } 
	       }
	       return (WorkflowParticipantRow[])list.ToArray(typeof(WorkflowParticipantRow));
	    }

	    public static WorkflowParticipantRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("WorkflowParticipant", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new WorkflowParticipantRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (WorkflowParticipantRow[])list.ToArray(typeof(WorkflowParticipantRow));
	    }
	
	    public static WorkflowParticipantRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("WorkflowParticipant", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new WorkflowParticipantRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (WorkflowParticipantRow[])list.ToArray(typeof(WorkflowParticipantRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("WorkflowParticipant", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("WorkflowParticipant");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("WorkflowParticipant", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("WorkflowParticipant", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("WorkflowParticipant");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("WorkflowParticipant", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("WorkflowParticipant"

					, SqlHelper.SqlParameter("@WorkflowParticipantId", SqlDbType.Int, _WorkflowParticipantId)

					, SqlHelper.SqlParameter("@UserId", SqlDbType.Int, _UserId)

					, SqlHelper.SqlParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, _WorkflowInstanceId)

					, SqlHelper.SqlParameter("@ObjectId", SqlDbType.Int, _ObjectId)

					, SqlHelper.SqlParameter("@ObjectType", SqlDbType.Int, _ObjectType)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				_WorkflowParticipantId = DataHelper.Insert("WorkflowParticipant", "WorkflowParticipantId" 

					, SqlHelper.SqlParameter("@WorkflowParticipantId", SqlDbType.Int, _WorkflowParticipantId)

					, SqlHelper.SqlParameter("@UserId", SqlDbType.Int, _UserId)

					, SqlHelper.SqlParameter("@WorkflowInstanceId", SqlDbType.UniqueIdentifier, _WorkflowInstanceId)

					, SqlHelper.SqlParameter("@ObjectId", SqlDbType.Int, _ObjectId)

					, SqlHelper.SqlParameter("@ObjectType", SqlDbType.Int, _ObjectType)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.WorkflowParticipantId);
		}

		public static void Delete(int primaryKeyId)
		{
			DataHelper.Delete("WorkflowParticipant",
				"WorkflowParticipantId",
				SqlDbType.Int,
				primaryKeyId);
		}
	}
}
