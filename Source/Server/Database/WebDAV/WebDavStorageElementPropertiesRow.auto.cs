
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using Mediachase.Ibn.Data;
using Mediachase.Ibn.Data.Sql;

namespace Mediachase.IBN.Database.WebDAV
{

	public partial class WebDavStorageElementPropertiesRow
	{
		private DataRowState _state = DataRowState.Unchanged;


		Nullable<int> _ProperyId;
		int _ObjectTypeId;
		int _ObjectId;
		string _Key;
		string _Value;


		public WebDavStorageElementPropertiesRow()
		{
			_state = DataRowState.Added;


		}

		public WebDavStorageElementPropertiesRow(int ProperyId)
		{
			using (IDataReader reader = DataHelper.Select("WebDavStorageElementProperties", 
							"ProperyId", 
							SqlDbType.Int,
							ProperyId))
			{
				if (reader.Read()) 
				{
					Load(reader);
				}
				else throw new Exception(String.Format("Record with {0}.{1}={2} not found", "WebDavStorageElementProperties", "ProperyId", ProperyId));
			}
		}
		
		public WebDavStorageElementPropertiesRow(IDataReader reader)
		{
			Load(reader);
		}
		
		protected virtual void Load(IDataReader reader)
		{
			_state = DataRowState.Unchanged;
			
		
			_ProperyId = (int)SqlHelper.DBNull2Null(reader["ProperyId"]);
			
			_ObjectTypeId = (int)SqlHelper.DBNull2Null(reader["ObjectTypeId"]);
			
			_ObjectId = (int)SqlHelper.DBNull2Null(reader["ObjectId"]);
			
			_Key = (string)SqlHelper.DBNull2Null(reader["Key"]);
			
			_Value = (string)SqlHelper.DBNull2Null(reader["Value"]);
			
		}

		#region Public Properties

		public virtual Nullable<int> PrimaryKeyId
		{
			get { return _ProperyId; }
			set { _ProperyId = value; }
		}
		
        
		public virtual int ProperyId
	    
		{
			get
			{
				
				return (int)_ProperyId;
				
			}
			
		}
		
		public virtual int ObjectTypeId
	    
		{
			get
			{
				
				return _ObjectTypeId;
				
			}
			
			set
			{
				_ObjectTypeId = value;
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
		
		public virtual string Key
	    
		{
			get
			{
				
				return _Key;
				
			}
			
			set
			{
				_Key = value;
				if (_state == DataRowState.Unchanged)
				{
					_state = DataRowState.Modified;
				}
			}	
			
		}
		
		public virtual string Value
	    
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
	
		public static WebDavStorageElementPropertiesRow[] List()
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("WebDavStorageElementProperties"))
			{
				while (reader.Read())
				{
					list.Add(new WebDavStorageElementPropertiesRow(reader));
				}
			}
			return (WebDavStorageElementPropertiesRow[])list.ToArray(typeof(WebDavStorageElementPropertiesRow));
		}
	
		public static WebDavStorageElementPropertiesRow[] List(params SortingElement[] sorting)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("WebDavStorageElementProperties", sorting))
			{
				while (reader.Read())
				{
					list.Add(new WebDavStorageElementPropertiesRow(reader));
				}
			}
			return (WebDavStorageElementPropertiesRow[])list.ToArray(typeof(WebDavStorageElementPropertiesRow));
		}

		public static WebDavStorageElementPropertiesRow[] List(params FilterElement[] filters)
		{
			ArrayList list = new ArrayList();

			using (IDataReader reader = DataHelper.List("WebDavStorageElementProperties", filters))
			{
				while (reader.Read())
				{
					list.Add(new WebDavStorageElementPropertiesRow(reader));
				}
			}
			return (WebDavStorageElementPropertiesRow[])list.ToArray(typeof(WebDavStorageElementPropertiesRow));
		}

		public static WebDavStorageElementPropertiesRow[] List(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("WebDavStorageElementProperties", filters, sorting))
	       {
	          while (reader.Read()) 
	          {
			      list.Add(new WebDavStorageElementPropertiesRow(reader));
			  } 
	       }
	       return (WebDavStorageElementPropertiesRow[])list.ToArray(typeof(WebDavStorageElementPropertiesRow));
	    }

	    public static WebDavStorageElementPropertiesRow[] List(FilterElementCollection filters, SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("WebDavStorageElementProperties", filters, sorting))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new WebDavStorageElementPropertiesRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (WebDavStorageElementPropertiesRow[])list.ToArray(typeof(WebDavStorageElementPropertiesRow));
	    }
	
	    public static WebDavStorageElementPropertiesRow[] List(SortingElementCollection sorting, int start, int count)
	    {
	       ArrayList list = new ArrayList(); 
	    
	       using (IDataReader reader = DataHelper.List("WebDavStorageElementProperties", sorting.ToArray()))
	       {
	          while (reader.Read() && count > 0) 
	          {
	              if (start == 0)
	              {  
			          list.Add(new WebDavStorageElementPropertiesRow(reader));
			          
			          count--;
			      }
			      else start--;
			  } 
	       }
	       return (WebDavStorageElementPropertiesRow[])list.ToArray(typeof(WebDavStorageElementPropertiesRow));
	    }
	
	    public static int GetTotalCount(params FilterElement[] filters)
	    {
	        return DataHelper.GetTotalCount("WebDavStorageElementProperties", filters);
	    }
	
	    public static IDataReader GetReader()
	    {
	       return DataHelper.List("WebDavStorageElementProperties");
	    }
	
	    public static IDataReader GetReader(params FilterElement[] filters)
	    {
	       return DataHelper.List("WebDavStorageElementProperties", filters);
	    }
	
	    public static IDataReader GetReader(FilterElementCollection filters, SortingElementCollection sorting)
	    {
	       return DataHelper.List("WebDavStorageElementProperties", filters, sorting);
	    }
	
	    public static DataTable GetTable()
	    {
	       return DataHelper.GetDataTable("WebDavStorageElementProperties");
	    }
	
	    public static DataTable GetTable(params FilterElement[] filters)
	    {
	       return DataHelper.GetDataTable("WebDavStorageElementProperties", filters);
	    }

		public virtual void Update()
		{
			if (_state == DataRowState.Modified) 
			{
				DataHelper.Update("WebDavStorageElementProperties"

					, SqlHelper.SqlParameter("@ProperyId", SqlDbType.Int, _ProperyId)

					, SqlHelper.SqlParameter("@ObjectTypeId", SqlDbType.Int, _ObjectTypeId)

					, SqlHelper.SqlParameter("@ObjectId", SqlDbType.Int, _ObjectId)

					, SqlHelper.SqlParameter("@Key", SqlDbType.NChar, 100, _Key)

					, SqlHelper.SqlParameter("@Value", SqlDbType.NText, _Value)

				);
			}
			else if (_state == DataRowState.Added) 
			{

				_ProperyId = DataHelper.Insert("WebDavStorageElementProperties", "ProperyId" 

					, SqlHelper.SqlParameter("@ProperyId", SqlDbType.Int, _ProperyId)

					, SqlHelper.SqlParameter("@ObjectTypeId", SqlDbType.Int, _ObjectTypeId)

					, SqlHelper.SqlParameter("@ObjectId", SqlDbType.Int, _ObjectId)

					, SqlHelper.SqlParameter("@Key", SqlDbType.NChar, 100, _Key)

					, SqlHelper.SqlParameter("@Value", SqlDbType.NText, _Value)

				);

			}
		}

		public virtual void Delete()
		{
			_state = DataRowState.Deleted;
		
			Delete(this.ProperyId);
		}

		public static void Delete(int ProperyId)
		{
			DataHelper.Delete("WebDavStorageElementProperties",
				"ProperyId",
				SqlDbType.Int,
				ProperyId);
		}
	}
}
