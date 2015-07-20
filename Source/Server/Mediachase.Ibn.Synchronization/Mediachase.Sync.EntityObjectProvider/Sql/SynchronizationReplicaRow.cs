
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Sync.EntityObjectProvider
{
public partial class SynchronizationReplicaRow
	{
		private DataRowState _state = DataRowState.Unchanged;
		
		#region Column's name const
		
			public const string ColumnReplicaId = "ReplicaId";

			public const string ColumnPrincipalId = "PrincipalId";

			public const string ColumnTickCount = "TickCount";

			public const string ColumnReplicaKeyMap = "ReplicaKeyMap";

			public const string ColumnCurrentKnowledge = "CurrentKnowledge";

			public const string ColumnForgottenKnowledge = "ForgottenKnowledge";

			public const string ColumnConflictLog = "ConflictLog";

			public const string ColumnTombstoneLog = "TombstoneLog";

			public const string ColumnLastDeletedItemsCleanupTime = "LastDeletedItemsCleanupTime";

		#endregion
		

		Nullable<Guid> _ReplicaId;

		Nullable<int> _PrincipalId;
		int _TickCount;
		string _ReplicaKeyMap;
		string _CurrentKnowledge;
		string _ForgottenKnowledge;
		string _ConflictLog;
		string _TombstoneLog;

		Nullable<DateTime> _LastDeletedItemsCleanupTime;


		public SynchronizationReplicaRow()
		{
			_state = DataRowState.Added;


			_PrincipalId = null;

			_ReplicaKeyMap = null;

			_CurrentKnowledge = null;

			_ForgottenKnowledge = null;

			_ConflictLog = null;

			_TombstoneLog = null;

			_LastDeletedItemsCleanupTime = null;

		}

		public SynchronizationReplicaRow(Guid primaryKeyId)
		{
			using (IDataReader reader = DataHelper.Select("SYNCHRONIZATION_REPLICA", 
							"ReplicaId", 
							SqlDbType.UniqueIdentifier,
							primaryKeyId))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new ArgumentException();
			}
		}
		
		public SynchronizationReplicaRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ReplicaId = (Guid)SqlHelper.DBNull2Null(reader["ReplicaId"]);
			
		    _PrincipalId = (Nullable< int >)SqlHelper.DBNull2Null(reader["PrincipalId"]);
		    
			_TickCount = (int)SqlHelper.DBNull2Null(reader["TickCount"]);
			
			_ReplicaKeyMap = (string)SqlHelper.DBNull2Null(reader["ReplicaKeyMap"]);
			
			_CurrentKnowledge = (string)SqlHelper.DBNull2Null(reader["CurrentKnowledge"]);
			
			_ForgottenKnowledge = (string)SqlHelper.DBNull2Null(reader["ForgottenKnowledge"]);
			
			_ConflictLog = (string)SqlHelper.DBNull2Null(reader["ConflictLog"]);
			
			_TombstoneLog = (string)SqlHelper.DBNull2Null(reader["TombstoneLog"]);
			
		    _LastDeletedItemsCleanupTime = (Nullable< DateTime >)SqlHelper.DBNull2Null(reader["LastDeletedItemsCleanupTime"]);
		    
		
			OnLoad(reader);
		}
		
		protected virtual void OnLoad(IDataReader reader)
		{
		}

		#region Public Properties

		public virtual Nullable<Guid> PrimaryKeyId
		{
			get { return _ReplicaId; }
			set { _ReplicaId = value; }
		}
		
        
		public virtual Guid ReplicaId
	    
		{
			get
			{
				
				return (Guid)_ReplicaId;
				
			}
			
			set
			{
				_ReplicaId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Nullable< int > PrincipalId
	    
		{
			get
			{
				
				return _PrincipalId;
				
			}
			
			set
			{
				_PrincipalId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual int TickCount
	    
		{
			get
			{
				
				return _TickCount;
				
			}
			
			set
			{
				_TickCount = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string ReplicaKeyMap
	    
		{
			get
			{
				
				return _ReplicaKeyMap;
				
			}
			
			set
			{
				_ReplicaKeyMap = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string CurrentKnowledge
	    
		{
			get
			{
				
				return _CurrentKnowledge;
				
			}
			
			set
			{
				_CurrentKnowledge = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string ForgottenKnowledge
	    
		{
			get
			{
				
				return _ForgottenKnowledge;
				
			}
			
			set
			{
				_ForgottenKnowledge = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string ConflictLog
	    
		{
			get
			{
				
				return _ConflictLog;
				
			}
			
			set
			{
				_ConflictLog = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string TombstoneLog
	    
		{
			get
			{
				
				return _TombstoneLog;
				
			}
			
			set
			{
				_TombstoneLog = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Nullable< DateTime > LastDeletedItemsCleanupTime
	    
		{
			get
			{
				
				return _LastDeletedItemsCleanupTime;
				
			}
			
			set
			{
				_LastDeletedItemsCleanupTime = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public static SynchronizationReplicaRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_REPLICA"))
			{
				while (reader.Read())
				{
					list.Add(new SynchronizationReplicaRow(reader));
				}
			}
			return (SynchronizationReplicaRow[])list.ToArray(typeof(SynchronizationReplicaRow));
		}
	
		public static SynchronizationReplicaRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_REPLICA", sorting))
			{
				while (reader.Read())
				{
					list.Add(new SynchronizationReplicaRow(reader));
				}
			}
			return (SynchronizationReplicaRow[])list.ToArray(typeof(SynchronizationReplicaRow));
		}

		public static SynchronizationReplicaRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_REPLICA", filters))
			{
				while (reader.Read())
				{
					list.Add(new SynchronizationReplicaRow(reader));
				}
			}
			return (SynchronizationReplicaRow[])list.ToArray(typeof(SynchronizationReplicaRow));
		}

		public static SynchronizationReplicaRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_REPLICA", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new SynchronizationReplicaRow(reader));
			  } 
	       }
	       return (SynchronizationReplicaRow[])list.ToArray(typeof(SynchronizationReplicaRow));
	    }

	    public static SynchronizationReplicaRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_REPLICA", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new SynchronizationReplicaRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (SynchronizationReplicaRow[])list.ToArray(typeof(SynchronizationReplicaRow));
	    }
	
	    public static SynchronizationReplicaRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_REPLICA", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new SynchronizationReplicaRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (SynchronizationReplicaRow[])list.ToArray(typeof(SynchronizationReplicaRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("SYNCHRONIZATION_REPLICA", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("SYNCHRONIZATION_REPLICA");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("SYNCHRONIZATION_REPLICA", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("SYNCHRONIZATION_REPLICA", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("SYNCHRONIZATION_REPLICA");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("SYNCHRONIZATION_REPLICA", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("SYNCHRONIZATION_REPLICA"

					, SqlHelper.SqlParameter("@ReplicaId", SqlDbType.UniqueIdentifier, _ReplicaId)

					, SqlHelper.SqlParameter("@PrincipalId", SqlDbType.Int, _PrincipalId)

					, SqlHelper.SqlParameter("@TickCount", SqlDbType.Int, _TickCount)

					, SqlHelper.SqlParameter("@ReplicaKeyMap", SqlDbType.NText, _ReplicaKeyMap)

					, SqlHelper.SqlParameter("@CurrentKnowledge", SqlDbType.NText, _CurrentKnowledge)

					, SqlHelper.SqlParameter("@ForgottenKnowledge", SqlDbType.NText, _ForgottenKnowledge)

					, SqlHelper.SqlParameter("@ConflictLog", SqlDbType.NText, _ConflictLog)

					, SqlHelper.SqlParameter("@TombstoneLog", SqlDbType.NText, _TombstoneLog)

					, SqlHelper.SqlParameter("@LastDeletedItemsCleanupTime", SqlDbType.DateTime, _LastDeletedItemsCleanupTime)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				DataHelper.Insert("SYNCHRONIZATION_REPLICA"

					, SqlHelper.SqlParameter("@ReplicaId", SqlDbType.UniqueIdentifier, _ReplicaId)

					, SqlHelper.SqlParameter("@PrincipalId", SqlDbType.Int, _PrincipalId)

					, SqlHelper.SqlParameter("@TickCount", SqlDbType.Int, _TickCount)

					, SqlHelper.SqlParameter("@ReplicaKeyMap", SqlDbType.NText, _ReplicaKeyMap)

					, SqlHelper.SqlParameter("@CurrentKnowledge", SqlDbType.NText, _CurrentKnowledge)

					, SqlHelper.SqlParameter("@ForgottenKnowledge", SqlDbType.NText, _ForgottenKnowledge)

					, SqlHelper.SqlParameter("@ConflictLog", SqlDbType.NText, _ConflictLog)

					, SqlHelper.SqlParameter("@TombstoneLog", SqlDbType.NText, _TombstoneLog)

					, SqlHelper.SqlParameter("@LastDeletedItemsCleanupTime", SqlDbType.DateTime, _LastDeletedItemsCleanupTime)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.ReplicaId);
		}

		public static void Delete(Guid primaryKeyId)
		{
			DataHelper.Delete("SYNCHRONIZATION_REPLICA",
				"ReplicaId",
				SqlDbType.UniqueIdentifier,
				primaryKeyId);
		}
	}
}
