
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IBN.Business.ReportSystem
{
internal partial class mcweb_ReportAceRow
	{
		private DataRowState _state = DataRowState.Unchanged;
		
		#region Columns class
		public static class Columns
		{
		
			public const string ReportAceId = "ReportAceId";

			public const string ReportId = "ReportId";

			public const string Role = "Role";

			public const string PrincipalId = "PrincipalId";

			public const string Allow = "Allow";

		}
		#endregion
		

		Nullable<int> _ReportAceId;
		Guid _ReportId;
		string _Role;

		Nullable<int> _PrincipalId;
		bool _Allow;


		public mcweb_ReportAceRow()
		{
			_state = DataRowState.Added;


			_Role = null;

			_PrincipalId = null;

			_Allow = false;

		}

		public mcweb_ReportAceRow(int ReportAceId)
		{
			using (IDataReader reader = DataHelper.Select("mcweb_ReportAce", 
							"ReportAceId", 
							SqlDbType.Int,
							ReportAceId))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "mcweb_ReportAce", "ReportAceId", ReportAceId));
			}
		}
		
		public mcweb_ReportAceRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ReportAceId = (int)SqlHelper.DBNull2Null(reader["ReportAceId"]);
			
			_ReportId = (Guid)SqlHelper.DBNull2Null(reader["ReportId"]);
			
			_Role = (string)SqlHelper.DBNull2Null(reader["Role"]);
			
		    _PrincipalId = (Nullable< int >)SqlHelper.DBNull2Null(reader["PrincipalId"]);
		    
			_Allow = (bool)SqlHelper.DBNull2Null(reader["Allow"]);
			
		}

		#region Public Properties

		public virtual Nullable<int> PrimaryKeyId
		{
			get { return _ReportAceId; }
			set { _ReportAceId = value; }
		}
		
        
		public virtual int ReportAceId
	    
		{
			get
			{
				
				return (int)_ReportAceId;
				
			}
			
		}
		
		public virtual Guid ReportId
	    
		{
			get
			{
				
				return _ReportId;
				
			}
			
			set
			{
				_ReportId = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Role
	    
		{
			get
			{
				
				return _Role;
				
			}
			
			set
			{
				_Role = value;
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
		
		public virtual bool Allow
	    
		{
			get
			{
				
				return _Allow;
				
			}
			
			set
			{
				_Allow = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		#endregion
	
		public static mcweb_ReportAceRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("mcweb_ReportAce"))
			{
				while (reader.Read())
				{
					list.Add(new mcweb_ReportAceRow(reader));
				}
			}
			return (mcweb_ReportAceRow[])list.ToArray(typeof(mcweb_ReportAceRow));
		}
	
		public static mcweb_ReportAceRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("mcweb_ReportAce", sorting))
			{
				while (reader.Read())
				{
					list.Add(new mcweb_ReportAceRow(reader));
				}
			}
			return (mcweb_ReportAceRow[])list.ToArray(typeof(mcweb_ReportAceRow));
		}

		public static mcweb_ReportAceRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("mcweb_ReportAce", filters))
			{
				while (reader.Read())
				{
					list.Add(new mcweb_ReportAceRow(reader));
				}
			}
			return (mcweb_ReportAceRow[])list.ToArray(typeof(mcweb_ReportAceRow));
		}

		public static mcweb_ReportAceRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("mcweb_ReportAce", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new mcweb_ReportAceRow(reader));
			  } 
	       }
	       return (mcweb_ReportAceRow[])list.ToArray(typeof(mcweb_ReportAceRow));
	    }

	    public static mcweb_ReportAceRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("mcweb_ReportAce", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new mcweb_ReportAceRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (mcweb_ReportAceRow[])list.ToArray(typeof(mcweb_ReportAceRow));
	    }
	
	    public static mcweb_ReportAceRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("mcweb_ReportAce", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new mcweb_ReportAceRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (mcweb_ReportAceRow[])list.ToArray(typeof(mcweb_ReportAceRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("mcweb_ReportAce", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("mcweb_ReportAce");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("mcweb_ReportAce", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("mcweb_ReportAce", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("mcweb_ReportAce");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("mcweb_ReportAce", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("mcweb_ReportAce"

					, SqlHelper.SqlParameter("@ReportAceId", SqlDbType.Int, _ReportAceId)

					, SqlHelper.SqlParameter("@ReportId", SqlDbType.UniqueIdentifier, _ReportId)

					, SqlHelper.SqlParameter("@Role", SqlDbType.NVarChar, 50, _Role)

					, SqlHelper.SqlParameter("@PrincipalId", SqlDbType.Int, _PrincipalId)

					, SqlHelper.SqlParameter("@Allow", SqlDbType.Bit, _Allow)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				_ReportAceId = DataHelper.Insert("mcweb_ReportAce", "ReportAceId" 

					, SqlHelper.SqlParameter("@ReportAceId", SqlDbType.Int, _ReportAceId)

					, SqlHelper.SqlParameter("@ReportId", SqlDbType.UniqueIdentifier, _ReportId)

					, SqlHelper.SqlParameter("@Role", SqlDbType.NVarChar, 50, _Role)

					, SqlHelper.SqlParameter("@PrincipalId", SqlDbType.Int, _PrincipalId)

					, SqlHelper.SqlParameter("@Allow", SqlDbType.Bit, _Allow)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.ReportAceId);
		}

		public static void Delete(int ReportAceId)
		{
			DataHelper.Delete("mcweb_ReportAce",
				"ReportAceId",
				SqlDbType.Int,
				ReportAceId);
		}
	}
}
