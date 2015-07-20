
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Globalization;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.Sync.EntityObjectProvider
{
public partial class SynchronizationMetadataRow
	{
		private DataRowState _state = DataRowState.Unchanged;
		
		#region Column's name const
		
			public const string ColumnId = "Id";

			public const string ColumnUniqueId = "UniqueId";

			public const string ColumnPrefix = "Prefix";

			public const string ColumnReplicaId = "ReplicaId";

			public const string ColumnCurrentVersion = "CurrentVersion";

			public const string ColumnCreationVersion = "CreationVersion";

			public const string ColumnUri = "Uri";

			public const string ColumnIsDeleted = "IsDeleted";

			public const string ColumnTimestamp = "Timestamp";

		#endregion
		

		Nullable<Guid> _Id;
		Guid _UniqueId;
		long _Prefix;
		Guid _ReplicaId;
		string _CurrentVersion;
		string _CreationVersion;

		Nullable<Guid> _Uri;
		bool _IsDeleted;

		Nullable<long> _Timestamp;


		public SynchronizationMetadataRow()
		{
			_state = DataRowState.Added;


			_Uri = null;

			_Timestamp = null;

		}

		public SynchronizationMetadataRow(Guid primaryKeyId)
		{
			using (IDataReader reader = DataHelper.Select("SYNCHRONIZATION_METADATA", 
							"Id", 
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
		
		public SynchronizationMetadataRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_Id = (Guid)SqlHelper.DBNull2Null(reader["Id"]);
			
			_UniqueId = (Guid)SqlHelper.DBNull2Null(reader["UniqueId"]);
			
			_Prefix = (long)SqlHelper.DBNull2Null(reader["Prefix"]);
			
			_ReplicaId = (Guid)SqlHelper.DBNull2Null(reader["ReplicaId"]);
			
			_CurrentVersion = (string)SqlHelper.DBNull2Null(reader["CurrentVersion"]);
			
			_CreationVersion = (string)SqlHelper.DBNull2Null(reader["CreationVersion"]);
			
		    _Uri = (Nullable< Guid >)SqlHelper.DBNull2Null(reader["Uri"]);
		    
			_IsDeleted = (bool)SqlHelper.DBNull2Null(reader["IsDeleted"]);
			
		    _Timestamp = (Nullable< long >)SqlHelper.DBNull2Null(reader["Timestamp"]);
		    
		
			OnLoad(reader);
		}
		
		protected virtual void OnLoad(IDataReader reader)
		{
		}

		#region Public Properties

		public virtual Nullable<Guid> PrimaryKeyId
		{
			get { return _Id; }
			set { _Id = value; }
		}
		
        
		public virtual Guid Id
	    
		{
			get
			{
				
				return (Guid)_Id;
				
			}
			
			set
			{
				_Id = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Guid UniqueId
	    
		{
			get
			{
				
				return _UniqueId;
				
			}
			
			set
			{
				_UniqueId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual long Prefix
	    
		{
			get
			{
				
				return _Prefix;
				
			}
			
			set
			{
				_Prefix = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Guid ReplicaId
	    
		{
			get
			{
				
				return _ReplicaId;
				
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
		
		public virtual string CurrentVersion
	    
		{
			get
			{
				
				return _CurrentVersion;
				
			}
			
			set
			{
				_CurrentVersion = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string CreationVersion
	    
		{
			get
			{
				
				return _CreationVersion;
				
			}
			
			set
			{
				_CreationVersion = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Nullable< Guid > Uri
	    
		{
			get
			{
				
				return _Uri;
				
			}
			
			set
			{
				_Uri = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual bool IsDeleted
	    
		{
			get
			{
				
				return _IsDeleted;
				
			}
			
			set
			{
				_IsDeleted = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual Nullable< long > Timestamp
	    
		{
			get
			{
				
				return _Timestamp;
				
			}
			
			set
			{
				_Timestamp = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public static SynchronizationMetadataRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_METADATA"))
			{
				while (reader.Read())
				{
					list.Add(new SynchronizationMetadataRow(reader));
				}
			}
			return (SynchronizationMetadataRow[])list.ToArray(typeof(SynchronizationMetadataRow));
		}
	
		public static SynchronizationMetadataRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_METADATA", sorting))
			{
				while (reader.Read())
				{
					list.Add(new SynchronizationMetadataRow(reader));
				}
			}
			return (SynchronizationMetadataRow[])list.ToArray(typeof(SynchronizationMetadataRow));
		}

		public static SynchronizationMetadataRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_METADATA", filters))
			{
				while (reader.Read())
				{
					list.Add(new SynchronizationMetadataRow(reader));
				}
			}
			return (SynchronizationMetadataRow[])list.ToArray(typeof(SynchronizationMetadataRow));
		}

		public static SynchronizationMetadataRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_METADATA", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new SynchronizationMetadataRow(reader));
			  } 
	       }
	       return (SynchronizationMetadataRow[])list.ToArray(typeof(SynchronizationMetadataRow));
	    }

	    public static SynchronizationMetadataRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_METADATA", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new SynchronizationMetadataRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (SynchronizationMetadataRow[])list.ToArray(typeof(SynchronizationMetadataRow));
	    }
	
	    public static SynchronizationMetadataRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("SYNCHRONIZATION_METADATA", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new SynchronizationMetadataRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (SynchronizationMetadataRow[])list.ToArray(typeof(SynchronizationMetadataRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("SYNCHRONIZATION_METADATA", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("SYNCHRONIZATION_METADATA");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("SYNCHRONIZATION_METADATA", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("SYNCHRONIZATION_METADATA", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("SYNCHRONIZATION_METADATA");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("SYNCHRONIZATION_METADATA", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("SYNCHRONIZATION_METADATA"

					, SqlHelper.SqlParameter("@Id", SqlDbType.UniqueIdentifier, _Id)

					, SqlHelper.SqlParameter("@UniqueId", SqlDbType.UniqueIdentifier, _UniqueId)

					, SqlHelper.SqlParameter("@Prefix", SqlDbType.BigInt, _Prefix)

					, SqlHelper.SqlParameter("@ReplicaId", SqlDbType.UniqueIdentifier, _ReplicaId)

					, SqlHelper.SqlParameter("@CurrentVersion", SqlDbType.NVarChar, 50, _CurrentVersion)

					, SqlHelper.SqlParameter("@CreationVersion", SqlDbType.NVarChar, 50, _CreationVersion)

					, SqlHelper.SqlParameter("@Uri", SqlDbType.UniqueIdentifier, _Uri)

					, SqlHelper.SqlParameter("@IsDeleted", SqlDbType.Bit, _IsDeleted)

					, SqlHelper.SqlParameter("@Timestamp", SqlDbType.BigInt, _Timestamp)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				DataHelper.Insert("SYNCHRONIZATION_METADATA"

					, SqlHelper.SqlParameter("@Id", SqlDbType.UniqueIdentifier, _Id)

					, SqlHelper.SqlParameter("@UniqueId", SqlDbType.UniqueIdentifier, _UniqueId)

					, SqlHelper.SqlParameter("@Prefix", SqlDbType.BigInt, _Prefix)

					, SqlHelper.SqlParameter("@ReplicaId", SqlDbType.UniqueIdentifier, _ReplicaId)

					, SqlHelper.SqlParameter("@CurrentVersion", SqlDbType.NVarChar, 50, _CurrentVersion)

					, SqlHelper.SqlParameter("@CreationVersion", SqlDbType.NVarChar, 50, _CreationVersion)

					, SqlHelper.SqlParameter("@Uri", SqlDbType.UniqueIdentifier, _Uri)

					, SqlHelper.SqlParameter("@IsDeleted", SqlDbType.Bit, _IsDeleted)

					, SqlHelper.SqlParameter("@Timestamp", SqlDbType.BigInt, _Timestamp)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.Id);
		}

		public static void Delete(Guid primaryKeyId)
		{
			DataHelper.Delete("SYNCHRONIZATION_METADATA",
				"Id",
				SqlDbType.UniqueIdentifier,
				primaryKeyId);
		}
	}
}
